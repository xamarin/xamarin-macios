# Default complication includes in iOS, tvOS, watchOS and macOS projects

Default compilation includes for .NET Core projects is explained here:
[Default compilation includes in .NET Core projects][1]

This document explains how default compilation includes is implemented for
iOS, tvOS, watchOS and macOS projects.

Default inclusion can be completely disabled by setting
`EnableDefaultItems=false`. It can also be disabled per-platform by setting
the platform-specific variables `EnableDefaultiOSItems=false`,
`EnableDefaulttvOSItems=false`, `EnableDefaultwatchOSItems=false`, or
`EnableDefaultmacOSItems=false`.

## Property lists

All \*.plist files in the root directory are included by default (as `None`
items).

[1]: https://docs.microsoft.com/en-us/dotnet/core/tools/csproj#default-compilation-includes-in-net-core-projects
