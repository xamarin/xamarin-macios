Debug vs. Release Configuration Property Documentation
========================================================

There are a number of properties that are contingent upon the configuration setting chosen. The properties and their corresponding values are documented here to provide transparency and enable users to better create custom configuration modes.

### Release Configuration

| **Property**                                            | **Default value**                 | **Condition?**                                                                         |
|---------------------------------------------------------|-----------------------------------|----------------------------------------------------------------------------------------|
| DebuggerSupport                                         | false                             |                                                                                        |
| EnableAssemblyILStripping                               | true                              |                                                                                        |
| RuntimeIdentifiers                                      | maccatalyst-x64;maccatalyst-arm64 | TargetFramework == netx.x-maccatalyst and effective SupportedOSPlatformVersion < 27.0  |
| RuntimeIdentifier                                       | Host architecture                 | TargetFramework == netx.x-maccatalyst and effective SupportedOSPlatformVersion >= 27.0 |
| RuntimeIdentifiers                                      | osx-x64;osx-arm64                 | TargetFramework == netx.x-macos and effective SupportedOSPlatformVersion < 27.0        |
| RuntimeIdentifier                                       | Host architecture                 | TargetFramework == netx.x-macos and effective SupportedOSPlatformVersion >= 27.0       |
| UseSystemResourceKeys                                   | true                              |                                                                                        |
| VerifyDependencyInjectionOpenGenericServiceTrimmability | false                             |                                                                                        |

When `SupportedOSPlatformVersion` is not available during SDK props evaluation, its effective value is the SDK's default `TargetPlatformVersion`. Set `SupportedOSPlatformVersion` in `Directory.Build.props` or on the command line when it must affect the default runtime identifier.

For iOS and tvOS projects, only arm64 device and simulator runtime identifiers are supported; `iossimulator-x64` and `tvossimulator-x64` are no longer supported. Setting `RuntimeIdentifiers` produces a build warning when targeting .NET 12 or earlier, and a build error when targeting .NET 13 or later. Use `RuntimeIdentifier` to build for a single architecture instead. Builds with multiple runtime identifiers are still supported on macOS and Mac Catalyst.

### Debug Configuration

| **Property**             	| **Default value** 	| **Condition?**                        	|
|--------------------------	|-------------------	|---------------------------------------	|
| CodesignDisableTimestamp 	| true              	|                                       	|
| DebuggerSupport          	| true              	|                                       	|
| NoSymbolStrip          	| true              	|                                       	|
| RuntimeIdentifiers       	| maccatalyst-x64   	| TargetFramework == netx.x-maccatalyst 	|
| RuntimeIdentifiers       	| osx-x64           	| TargetFramework == netx.x-macos       	|
| UseSystemResourceKeys    	| false             	|                                       	|

### Optimize=true

These are options that are set when the `Optimize` property is `true` (which happens by default if `Configuration=Release`).

| **Property**                   | **Value**  |
|--------------------------------|------------|
| EventSourceSupport             | false      |
| HttpActivityPropagationSupport | false      |
| MetricsSupport                 | false      |
