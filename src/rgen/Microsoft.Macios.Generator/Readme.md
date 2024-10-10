# Roslyn binding code generator

This directory contains the code generator for binding code. The generator is the implementation of [RFC: Migrate bgen to use roslyn instead of the reflection API](https://github.com/xamarin/xamarin-macios/issues/21308)

## Content

### Microsoft.Macios.Generator
A .NET Standard project with implementations of sample source generators.

**You must build this project to see the result (generated code) in the IDE.**

### Microsoft.Macios.Generator.Sample
A project that references source generators. Note the parameters of `ProjectReference` in [Microsoft.Macios.Generator.Sample.csproj](../Microsoft.Macios.Generator.Sample/Microsoft.Macios.Generator.Sample.csproj), they make sure that the project is referenced as a set of source generators. 

### Microsoft.Macios.Generator.Tests
Unit tests for source generators. The easiest way to develop language-related features is to start with unit tests.

## How To?
### How to debug?
- Use the [launchSettings.json](Properties/launchSettings.json) profile.
- Debug tests.