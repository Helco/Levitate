# Levitate

A small experiment trying to incrementally reimplement the game "Hover!" from Microsoft in C#.

Currently only parts of the used MFC, the "Merlin" file formats are implemented.

## Structure

| Project | Purpose |
|:--------|:--------|
| StartLevitate | Launches Hover with the injected LevitateLoader.dll using Detours |
| LevitateLoader | The only custom native part, a detours-compatible DLL loading the core CLR into the Hover process at startup and run the `Levitate.Injector.Inject` method |
| Levitate | The main library, containing the reimplemented parts of Hover as well as the injection functionality |
| Levitate.SourceGen | A Roslyn sourcegenerator, for generating all boilerplate code related to the injection, controlled by the `AttachAttribute` |
