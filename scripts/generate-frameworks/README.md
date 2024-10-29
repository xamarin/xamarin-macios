# generate-frameworks

This script takes the list of frameworks that are supported for all platforms,
and generates a C# file with a hashtable of all the frameworks for each
platform.

Example output file for iOS:

```cs
using System.Collections.Generic;

partial class Frameworks {
	internal readonly HashSet<string> iosframeworks = new HashSet<string> {
		"Accelerate",
		"Accessibility",
		"AccessorySetupKit",
		/// ...
		"WebKit",
		"XKit",
	};
	internal readonly HashSet<string> macosframeworks = new HashSet<string> {
		"Accelerate",
		"Accessibility",
		/// ...
		"WebKit",
		"XKit",
	};
}
```
