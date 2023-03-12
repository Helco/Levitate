using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Levitate.SourceGen;

[Generator]
public class AttachGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
            return;

        var methodsByClass = receiver.Methods
            .GroupBy<MethodInfo, INamedTypeSymbol>(t => t.Symbol.ContainingType, SymbolEqualityComparer.Default);
        foreach (var clazz in methodsByClass)
        {
            var classSource = ProcessClass(context, clazz.Key!, clazz.ToArray());
            context.AddSource($"{clazz.Key!.Name}_attach.g.cs", SourceText.From(classSource, Encoding.UTF8));
        }

        var injectorSource = ProcessInjector(context, receiver.Methods.Where(m => m.IsReplacing));
        context.AddSource("Injector_attach.g.cs", SourceText.From(injectorSource, Encoding.UTF8));
    }

    private string ProcessClass(GeneratorExecutionContext context, INamedTypeSymbol classSymbol, MethodInfo[] methods)
    {
        var source = new StringBuilder();

        var namespaces = methods
            .SelectMany(m => m.Symbol.Parameters.Select(p => p.Type))
            .Concat(methods.Select(m => m.Symbol.ReturnType))
            .Select(t => t is IPointerTypeSymbol pointerType
                ? pointerType.PointedAtType
                : t)
            .Select(t => t?.ContainingNamespace?.ToDisplayString())
            .Distinct()
            .Except(new[] { null!, "" });
        foreach (var ns in namespaces)
            source.AppendLine($"using {ns};");

        source.AppendLine($@"
namespace {classSymbol.ContainingNamespace.ToDisplayString()}
{{
    unsafe partial struct {classSymbol.Name}
    {{
");

        foreach (var method in methods)
        {
            ProcessFunctionPointerField(context, method, source);
            if (method.IsReplacing)
                ProcessReplacedMethod(context, method, source);
            else
                ProcessNonReplacedMethod(context, method, source);

            source.AppendLine();
        }

        source.AppendLine("    }");
        source.AppendLine("}");
        return source.ToString();
    }

    private void ProcessFunctionPointerField(GeneratorExecutionContext context, MethodInfo method, StringBuilder source)
    {
        var functionPointerType = method.CreateFunctionPointerTypeSymbol(context).ToDisplayString();
        var functionPointerName = method.FunctionPointerName;
        source.AppendLine($"        internal static readonly {functionPointerType} {functionPointerName} = ({functionPointerType}){method.AddressLiteral};");
    }

    private void ProcessNonReplacedMethod(GeneratorExecutionContext context, MethodInfo method, StringBuilder source)
    {
        source.AppendLine($"        {method.MethodDeclarationSource()}");
        source.AppendLine("        {");

        if (!method.Symbol.IsStatic)
            source.AppendLine($"            fixed({method.Symbol.ContainingType.ToDisplayString()}* pThis = &this)");

        source.Append("            ");
        if (method.Symbol.ReturnType.ToDisplayString() != "void")
            source.Append("return ");
        source.Append(method.FunctionPointerName);
        AppendParameterList(method, source, includeThis: true);

        source.AppendLine("        }");
    }

    private static void AppendParameterList(MethodInfo method, StringBuilder source, bool includeThis)
    {
        source.Append('(');
        var didHaveThis = includeThis && !method.Symbol.IsStatic;
        if (didHaveThis)
            source.Append("pThis");
        foreach (var parameter in method.Symbol.Parameters)
        {
            if (!SymbolEqualityComparer.Default.Equals(parameter, method.Symbol.Parameters.First()) || didHaveThis)
                source.Append(", ");
            if (parameter.RefKind == RefKind.Ref)
                source.Append("ref ");
            source.Append(parameter.Name);
        }
        source.AppendLine(");");
    }

    private void ProcessReplacedMethod(GeneratorExecutionContext context, MethodInfo method, StringBuilder source)
    {
        var callConv = method.Attribute.ConstructorArguments[1].ToCSharpString() switch
        {
            var t when t.Contains("ThisCall") => "System.Runtime.CompilerServices.CallConvThiscall",
            var t when t.Contains("StdCall") => "System.Runtime.CompilerServices.CallConvStdcall",
            var t when t.Contains("Cdecl") => "System.Runtime.CompilerServices.CallConvCdecl",
            _ => "CallConvThiscall" // no need for another diagnostic, we have one already
        };
        source.AppendLine($"        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] {{ typeof({callConv}) }})]");

        var wrapperSyntax = method.MethodDeclarationSource(method.UnmanagedWrapperName);
        if (!method.Symbol.IsStatic)
        {
            wrapperSyntax = wrapperSyntax
                .WithModifiers(
                    TokenList(
                        wrapperSyntax.Modifiers.Append(Token(SyntaxKind.StaticKeyword))))
                .WithParameterList(
                    ParameterList(
                        SeparatedList(
                            wrapperSyntax.ParameterList.Parameters.Prepend(
                                Parameter(
                                    Identifier("pThis"))
                                .WithType(
                                    PointerType(
                                        IdentifierName(method.Symbol.ContainingType.Name)))))))
                .NormalizeWhitespace();
        }
        source.AppendLine($"        {wrapperSyntax} =>");

        source.Append("            ");
        if (!method.Symbol.IsStatic)
            source.Append("pThis->");
        source.Append(method.Symbol.Name);
        AppendParameterList(method, source, includeThis: false);
    }

    private string ProcessInjector(GeneratorExecutionContext context, IEnumerable<MethodInfo> methods)
    {
        // the function pointer variables have to be kept alive
        // so we commit the transaction in AttachAllMethods as well
        // otherwise the pointers used are invalidated after AttachAllMethods returns

        var source = new StringBuilder(@"
using static Levitate.Detours;

namespace Levitate
{
    unsafe partial class Injector
    {
        private static partial void AttachAllMethods()
        {
            CheckWin(DetourTransactionBegin());
");


        int i = 0;
        foreach (var method in methods)
            ProcessMethodAttach(context, method, source, i++);

        source.AppendLine("            CheckWin(DetourTransactionCommit());");
        source.AppendLine("        }");
        source.AppendLine("    }");
        source.AppendLine("}");
        return source.ToString();
    }

    private void ProcessMethodAttach(GeneratorExecutionContext context, MethodInfo method, StringBuilder source, int index)
    {
        var typePath = method.Symbol.ContainingType.ToDisplayString();
        source.AppendLine($"            {method.CreateFunctionPointerTypeSymbol(context)}");
        source.AppendLine($"                orig{index} = {typePath}.{method.FunctionPointerName},");
        source.AppendLine($"                repl{index} = &{typePath}.{method.UnmanagedWrapperName};");
        source.AppendLine($"            CheckWin(DetourAttach(new(&orig{index}), new(repl{index})));");
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    private static readonly DiagnosticDescriptor UnknownCallingConvDiagnostic = new(
        id: "LEV0001",
        title: "Unknown calling convention for attach methods",
        messageFormat: "Unknown calling convention for attach methods",
        category: "Levitate.Attach",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private class MethodInfo
    {
        public MethodInfo(IMethodSymbol methodSymbol, MethodDeclarationSyntax syntax, AttributeData attribute)
        {
            Symbol = methodSymbol;
            Syntax = syntax;
            Attribute = attribute;
        }

        public IMethodSymbol Symbol { get; }
        public MethodDeclarationSyntax Syntax { get; }
        public AttributeData Attribute { get; }
        public bool IsReplacing => !Symbol.IsPartialDefinition;
        public string FunctionPointerName => Symbol.Name + "_fnptr";
        public string UnmanagedWrapperName => Symbol.Name + "_native";
        public string AddressLiteral => Attribute.ConstructorArguments[0].ToCSharpString();

        public IFunctionPointerTypeSymbol CreateFunctionPointerTypeSymbol(GeneratorExecutionContext context)
        {
            var callConv = Attribute.ConstructorArguments[1].ToCSharpString() switch
            {
                var t when t.Contains("ThisCall") => SignatureCallingConvention.ThisCall,
                var t when t.Contains("StdCall") => SignatureCallingConvention.StdCall,
                var t when t.Contains("Cdecl") => SignatureCallingConvention.CDecl,
                _ => SignatureCallingConvention.Default
            };
            if (callConv == SignatureCallingConvention.Default)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    UnknownCallingConvDiagnostic, Symbol.Locations.First()));
            }

            var parameterTypes = Symbol.Parameters.Select(p => p.Type);
            var parameterRefKinds = Symbol.Parameters.Select(p => p.RefKind);
            if (!Symbol.IsStatic)
            {
                parameterTypes = parameterTypes.Prepend(context.Compilation.CreatePointerTypeSymbol(Symbol.ContainingType));
                parameterRefKinds = parameterRefKinds.Prepend(RefKind.None);
            }

            return context.Compilation.CreateFunctionPointerTypeSymbol(
                Symbol.ReturnType,
                RefKind.None,
                parameterTypes.ToImmutableArray(),
                parameterRefKinds.ToImmutableArray(),
                callConv);
        }

        public MethodDeclarationSyntax MethodDeclarationSource(string? overrideName = null) => MethodDeclaration(
            List<AttributeListSyntax>(),
            Syntax.Modifiers,
            Syntax.ReturnType,
            Syntax.ExplicitInterfaceSpecifier,
            Identifier(overrideName ?? Symbol.Name),
            Syntax.TypeParameterList,
            Syntax.ParameterList,
            Syntax.ConstraintClauses,
            null,
            null);
    }

    private class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<MethodInfo> Methods { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not MethodDeclarationSyntax methodSyntax ||
                context.SemanticModel.GetDeclaredSymbol(methodSyntax) is not IMethodSymbol methodSymbol)
                return;

            var attributeData = methodSymbol
                .GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "Levitate.AttachAttribute");
            if (attributeData == null)
                return;

            Methods.Add(new(methodSymbol, methodSyntax, attributeData));
        }
    }
}
