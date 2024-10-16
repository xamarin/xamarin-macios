# Roslyn Analyzers Sample

Roslyn analyzer to be installed along side the Roslyn Conde generator that will help developers work on Microsoft.Macios bindings.

## Content
### Microsoft.Macios.Bindings.Analyzer

A .NET Standard project with implementations of sample analyzers and code fix providers.
**You must build this project to see the results (warnings) in the IDE.**

### Microsoft.Macios.Bindings.Analyzer.Sample
A project that references the sample analyzers. Note the parameters of `ProjectReference` in [Microsoft.Macios.Bindings.Analyzer.Sample.csproj](../Microsoft.Macios.Bindings.Analyzer.Sample/Microsoft.Macios.Bindings.Analyzer.Sample.csproj), they make sure that the project is referenced as a set of analyzers. 

### Microsoft.Macios.Bindings.Analyzer.Tests
Unit tests for the sample analyzers and code fix provider. The easiest way to develop language-related features is to start with unit tests.

## How To?
### How to debug?
- Use the [launchSettings.json](Properties/launchSettings.json) profile.
- Debug tests (in VSCode).

### How can I determine which syntax nodes I should expect?
Consider installing the Roslyn syntax tree viewer plugin [Rossynt](https://plugins.jetbrains.com/plugin/16902-rossynt/).

### Learn more about wiring analyzers
The complete set of information is available at [roslyn github repo wiki](https://github.com/dotnet/roslyn/blob/main/docs/wiki/README.md).