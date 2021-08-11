# File layout

## Microsoft.NET.Sdk._platform_

* WorkloadManifest.json: workload manifest that describes the different
  workloads for _platform_.
* WorkloadManifest.targets: imports Microsoft._platform_.Sdk if we're building
  for _platform_ (based on `TargetPlatformIdentifier`).

### Documentation

* [.NET Optional SDK
  Workloads](https://github.com/dotnet/designs/blob/main/accepted/2020/workloads/workloads.md)
* [Workload
  resolvers](https://github.com/dotnet/designs/blob/main/accepted/2020/workloads/workload-resolvers.md)
* [Workload manifests](https://github.com/dotnet/designs/pull/120)

## Microsoft._platform_.Sdk

These files are imported into every project that references Microsoft.NET.Sdk:

Files are imported in the following order:

1. Sdk/AutoImport.props: automatically imported into _every build_ that
   references Microsoft.NET.Sdk. It has strict restrictions on what it can do,
   to make sure it doesn't break builds for other SDKs. It imports
   Microsoft._platform_.Sdk.DefaultItems.props, where the default inclusion
   itemgroups are defined.

2. The user's `.csproj`.

3. Some early parts of `Microsoft.NET.Sdk` / `Sdk.targets`.

4. WorkloadManifest.targets:

    * Sdk/Sdk.props: imported by WorkloadManifest.targets
    * targets/Microsoft._platform_.Sdk.SupportedTargetPlatforms.props: lists which
      versions of _platform_ are supported
    * targets/Xamarin.Shared.Sdk.TargetFrameworkInference.props: some TargetFramework
      logic.
    * targets/Microsoft._platform_.Sdk.Versions.props: declares various properties related to
      version information.
    * targets/Xamarin.Shared.Sdk.props: imports other props files.

    We try to do as little as possible in this stage; only set properties and
    item groups that the rest of `Microsoft.NET.Sdk` requires.

5. The rest of `Microsoft.NET.Sdk` / `Sdk.targets`.

6. `targets/Microsoft._platform_.Sdk.targets`

    * targets/Microsoft._platform_.Sdk.targets: contains logic specific to _platform_.
    * targets/Xamarin.Shared.Sdk.DefaultItems.targets: contains logic to enable the
      default behavior we want.
    * targets/Xamarin.Shared.Sdk.Publish.targets: contains logic to publish the
      app bundle.
    * targets/Xamarin.Shared.Sdk.targets: all of the build logic shared between all
      platforms.

## Microsoft._platform_.Ref

Contains reference assemblies.

## Microsoft._platform_.Runtime.[runtimeIdentifier]

Contains implementation assemblies and native bits.

## Microsoft._platform.Templates

Contains project & item templates for each platform.
