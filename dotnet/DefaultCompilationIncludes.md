# Default complication includes in iOS, tvOS, watchOS and macOS projects

Default compilation includes for .NET Core projects is explained here:
[Default compilation includes in .NET Core projects][1]

This document explains how default compilation includes is implemented for
iOS, tvOS, watchOS and macOS projects.

Default inclusion can be completely disabled by setting
`EnableDefaultItems=false`. Each file type also have its own specific flag.

Specific files can also be excluded by adding them to `DefaultItemExcludes`,
or the specific variable for each file type.

## Property lists

All \*.plist files in the root directory are included by default (as `None`
items).

To completely disable: `EnableDefaultXamarinPlistItems=false`. To exclude
specific files from being included, add them to
`DefaultXamarinPlistItemExcludes`.

[1]: https://docs.microsoft.com/en-us/dotnet/core/tools/csproj#default-compilation-includes-in-net-core-projects
