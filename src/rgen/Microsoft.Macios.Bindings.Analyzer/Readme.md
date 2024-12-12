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
