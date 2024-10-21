---
title: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Properties
description: .NET for iOS, Mac Catalyst, macOS, and tvOS Build Properties
ms.date: 09/19/2024
---

# Build Properties

MSBuild properties control the behavior of the
[targets](build-targets.md).
They're specified within the project file, for example **MyApp.csproj**, within
an MSBuild PropertyGroup.

## MaciOSPrepareForBuildDependsOn

A semi-colon delimited property that can be used to extend the build process.
MSBuild targets added to this property will execute early in the build for both
application and library project types. This property is empty by default.

Example:

```xml
<PropertyGroup>
  <MaciOSPrepareForBuildDependsOn>MyCustomTarget</MaciOSPrepareForBuildDependsOn>
</PropertyGroup>

<Target Name="MyCustomTarget" >
  <Message Text="Running target: 'MyCustomTarget'" Importance="high"  />
</Target>
```

This property was introduced in .NET 9.

## MetalLibPath

The full path to the `metallib` tool (the Metal Linker).

The default behavior is to use `xcrun metallib`.

## MetalPath

The full path to the Metal compiler.

The default behavior is to use `xcrun metal`.

## MobileAggressiveAttributeTrimming

This property determines whether numerous attributes that are very rarely
needed at runtime should be trimmed away.

This is enabled by default.

Note that while the attributes that are removed are very rarely used, it's
technically possible that the removal can change runtime behavior.

For example, System.Xml.Serialization will behave differently if a constructor
has the `[Obsolete]` attribute (which is one of the attributes that are
removed). This is low enough risk to justify removing these attributes by
default because of the size savings.

The list of attributes that are removed may change in the future, but at the
time of this writing (for .NET 10), these are the attributes:

* Microsoft.CodeAnalysis.EmbeddedAttribute
* System.CLSCompliantAttribute
* System.CodeDom.Compiler.GeneratedCodeAttribute
* System.ComponentModel.EditorBrowsableAttribute
* System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute
* System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute
* System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute
* System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute
* System.Diagnostics.CodeAnalysis.ExperimentalAttribute
* System.Diagnostics.CodeAnalysis.FeatureGuardAttribute
* System.Diagnostics.CodeAnalysis.FeatureSwitchDefinitionAttribute
* System.Diagnostics.CodeAnalysis.MemberNotNullAttribute
* System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute
* System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute
* System.Diagnostics.CodeAnalysis.NotNullWhenAttribute
* System.Diagnostics.CodeAnalysis.RequiresAssemblyFilesAttribute
* System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute
* System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute
* System.Diagnostics.CodeAnalysis.StringSyntaxAttribute
* System.Diagnostics.CodeAnalysis.SuppressMessageAttribute
* System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessageAttribute
* System.Diagnostics.CodeAnalysis.UnscopedRefAttribute
* System.ObsoleteAttribute
* System.Reflection.AssemblyCompanyAttribute
* System.Reflection.AssemblyConfigurationAttribute
* System.Reflection.AssemblyCopyrightAttribute
* System.Reflection.AssemblyDefaultAliasAttribute
* System.Reflection.AssemblyDescriptionAttribute
* System.Reflection.AssemblyMetadataAttribute
* System.Reflection.AssemblyProductAttribute
* System.Reflection.AssemblyTitleAttribute
* System.Runtime.CompilerServices.AsyncMethodBuilderAttribute
* System.Runtime.CompilerServices.CallerArgumentExpressionAttribute
* System.Runtime.CompilerServices.CallerFilePathAttribute
* System.Runtime.CompilerServices.CallerLineNumberAttribute
* System.Runtime.CompilerServices.CallerMemberNameAttribute
* System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute
* System.Runtime.CompilerServices.CompilerGlobalScopeAttribute
* System.Runtime.CompilerServices.EnumeratorCancellationAttribute
* System.Runtime.CompilerServices.ExtensionAttribute
* System.Runtime.CompilerServices.InterpolatedStringHandlerArgumentAttribute
* System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute
* System.Runtime.CompilerServices.IntrinsicAttribute
* System.Runtime.CompilerServices.IsReadOnlyAttribute
* System.Runtime.CompilerServices.IsUnmanagedAttribute
* System.Runtime.CompilerServices.NativeIntegerAttribute
* System.Runtime.CompilerServices.RefSafetyRulesAttribute
* System.Runtime.CompilerServices.ScopedRefAttribute
* System.Runtime.CompilerServices.SkipLocalsInitAttribute
* System.Runtime.CompilerServices.TupleElementNamesAttribute
* System.Runtime.InteropServices.LibraryImportAttribute
* System.Runtime.InteropServices.Marshalling.ContiguousCollectionMarshallerAttribute
* System.Runtime.InteropServices.Marshalling.CustomMarshallerAttribute
* System.Runtime.InteropServices.Marshalling.MarshalUsingAttribute
* System.Runtime.InteropServices.Marshalling.NativeMarshallingAttribute
* System.Runtime.Versioning.NonVersionableAttribute
* System.Runtime.Versioning.ObsoletedOSPlatformAttribute
* System.Runtime.Versioning.RequiresPreviewFeaturesAttribute
* System.Runtime.Versioning.SupportedOSPlatformAttribute
* System.Runtime.Versioning.SupportedOSPlatformGuardAttribute
* System.Runtime.Versioning.TargetPlatformAttribute
* System.Runtime.Versioning.UnsupportedOSPlatformAttribute
* System.Runtime.Versioning.UnsupportedOSPlatformGuardAttribute
