// arguments are: <platform> <version> <xcodeVersion> <sdkVersion> <outputPath>

using System.IO;
using System.Xml;

var expectedArgumentCount = 5;
if (args.Length != expectedArgumentCount) {
	Console.WriteLine ($"Need {expectedArgumentCount} arguments, got {args.Length}");
	return 1;
}

var argumentIndex = 0;
var platform = args [argumentIndex++];
var version = args [argumentIndex++];
var xcodeVersion = args [argumentIndex++];
var sdkVersion = args [argumentIndex++];
var outputPath = args [argumentIndex++];

var platformLowerCase = platform.ToLowerInvariant ();

using (var writer = new StreamWriter (outputPath)) {
	writer.WriteLine ($"{{");
	writer.WriteLine ($"  \"microsoft.net.sdk.{platformLowerCase}\": {{");
	writer.WriteLine ($"    \"workload\": {{");
	writer.WriteLine ($"      \"alias\": [ \"{platformLowerCase}\" ],");
	writer.WriteLine ($"      \"version\": \"{version}\"");
	writer.WriteLine ($"    }},");
	writer.WriteLine ($"    \"xcode\": {{");
	writer.WriteLine ($"      \"version\": \"[{xcodeVersion},)\",");
	writer.WriteLine ($"      \"recommendedVersion\": \"{xcodeVersion}\"");
	writer.WriteLine ($"    }},");
	writer.WriteLine ($"    \"sdk\": {{");
	writer.WriteLine ($"      \"version\": \"{sdkVersion}\"");
	writer.WriteLine ($"    }}");
	writer.WriteLine ($"  }}");
	writer.WriteLine ($"}}");
}

return 0;
