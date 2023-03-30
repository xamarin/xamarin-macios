Debug vs. Release Configuration Property Documentation
========================================================

There are a number of properties that are contingent upon the configuration setting chosen. The properties and their corresponding values are documented here to provide transparency and enable users to better create custom configuration modes.

### Release Configuration

| **Property**                                            	| **Default value**                 	| **Condition?**                        	|
|---------------------------------------------------------	|-----------------------------------	|---------------------------------------	|
| DebuggerSupport                                         	| false                             	|                                       	|
| EnableAssemblyILStripping                               	| true                              	|                                       	|
| RuntimeIdentifiers                                      	| maccatalyst-x64;maccatalyst-arm64 	| TargetFramework == netx.x-maccatalyst 	|
| RuntimeIdentifiers                                      	| osx-x64;osx-arm64                 	| TargetFramework == netx.x-macos       	|
| UseSystemResourceKeys                                   	| true                              	|                                       	|
| VerifyDependencyInjectionOpenGenericServiceTrimmability 	| false                             	|                                       	|


### Debug Configuration

| **Property**             	| **Default value** 	| **Condition?**                        	|
|--------------------------	|-------------------	|---------------------------------------	|
| CodesignDisableTimestamp 	| true              	|                                       	|
| DebuggerSupport          	| true              	|                                       	|
| RuntimeIdentifiers       	| maccatalyst-x64   	| TargetFramework == netx.x-maccatalyst 	|
| RuntimeIdentifiers       	| osx-x64           	| TargetFramework == netx.x-macos       	|
| UseSystemResourceKeys    	| false             	|                                       	|
