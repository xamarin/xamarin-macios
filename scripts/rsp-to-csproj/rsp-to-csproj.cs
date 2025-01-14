using System.IO;
using System.Text;

using Mono.Options;

var output = string.Empty;
var rspFiles = new List<string> ();
var workingDirectory = string.Empty;
var targetFramework = string.Empty;
var verbose = false;
var fullPaths = true;

var options = new OptionSet {
	{ "output=", (v) => output = v },
	{ "rsp=", (v) => rspFiles.Add (v) },
	{ "working-directory=", (v) => workingDirectory = v },
	{ "target-framework=", (v) => targetFramework = v },
	{ "v|verbose", (v) => verbose = true },
};

int exitCode = 0;

void ReportError (string message)
{
	Console.Error.WriteLine ($"rsp-to-csproj: {message}");
	exitCode = 1;
}

var unhandled = options.Parse (args);
if (unhandled.Any ()) {
	ReportError ($"Didn't understand these arguments:");
	foreach (var u in unhandled)
		ReportError ($"    {u}");
	return exitCode;
}

if (string.IsNullOrEmpty (output)) {
	ReportError ($"No output file specified");
	return exitCode;
}

var sourceFiles = new List<string> ();
var arguments = new List<string> ();

foreach (var file in rspFiles)
	ProcessFile (file);

string GetFullPath (string path)
{
	if (!fullPaths)
		return path;
	return Path.GetFullPath (path);
}

void ProcessFile (string file)
{
	if (verbose)
		Console.WriteLine ($"Reading {file}...");
	var lines = File.ReadAllLines (file);
	foreach (var line in lines) {
		var elements = line.Split (' '); // at this moment we don't need to handle arguments or files with spaces, so make it simple
		foreach (var element in elements) {
			if (string.IsNullOrEmpty (element))
				continue;
			if (element [0] == '@') {
				ProcessFile (element [1..]);
				continue;
			} else if (element [0] == '/') {
				arguments.Add (element [1..]);
			} else if (element [0] == '-') {
				if (element.Length > 1 && element [1] == '-') {
					arguments.Add (element [2..]);
				} else {
					arguments.Add (element [1..]);
				}
			} else {
				sourceFiles.Add (element);
			}
		}
	}
}

var properties = new List<(string Name, string Value)> ();
var items = new List<(string Name, string Include)> ();

if (verbose)
	Console.WriteLine ($"Found {arguments.Count} arguments and {sourceFiles.Count} files.");

foreach (var a in arguments) {
	var splitIndex = a.IndexOfAny (new char [] { ':', '=' });
	var name = a;
	var value = string.Empty;
	if (splitIndex >= 0) {
		name = a [..splitIndex];
		value = a [(splitIndex + 1)..];
	}
	switch (name) {
	case "D":
	case "d":
	case "define":
		properties.Add (new ("DefineConstants", $"$(DefineConstants);{value}"));
		break;
	case "unsafe":
		properties.Add (new ("AllowUnsafeBlocks", "true"));
		break;
	case "deterministic":
		properties.Add (new ("Deterministic", "true"));
		break;
	case "nologo":
		properties.Add (new ("NoLogo", "true"));
		break;
	case "nostdlib+":
		properties.Add (new ("NoCompilerStandardLib", "true"));
		break;
	case "embed":
		foreach (var e in value.Split (','))
			items.Add (new ("EmbeddedFiles", GetFullPath (e)));
		break;
	case "features":
		properties.Add (new ("Features", "strict"));
		break;
	case "target":
		properties.Add (new ("OutputType", "Library"));
		break;
	case "optimize":
		properties.Add (new ("Optimize", "true"));
		break;
	case "keyfile":
		properties.Add (new ("KeyOriginatorFile", GetFullPath (value)));
		break;
	case "publicsign":
		properties.Add (new ("PublicSign", "true"));
		break;
	case "refout":
		properties.Add (new ("IntermediateRefAssembly", GetFullPath (value)));
		break;
	case "out":
		properties.Add (new ("IntermediateAssembly", GetFullPath (value)));
		break;
	case "debug":
		properties.Add (new ("DebugSymbols", "true"));
		break;
	case "nowarn":
		properties.Add (new ("NoWarn", $"$(NoWarn);{value}"));
		break;
	case "res":
		items.Add (new ("EmbeddedResource", GetFullPath (value)));
		break;
	case "warnaserror":
		properties.Add (new ("WarningsAsErrors", value));
		break;
	case "warnaserror+":
		properties.Add (new ("TreatWarningsAsErrors", "true"));
		break;
	case "doc":
		properties.Add (new ("DocumentationFile", GetFullPath (value)));
		break;
	case "sourcelink":
		properties.Add (new ("SourceLink", GetFullPath (value)));
		break;
	case "nullable+":
		properties.Add (new ("Nullable", "enable"));
		break;
	case "r":
		items.Add (new ("ReferencePathWithRefAssemblies", GetFullPath (value)));
		break;
	case "nostdlib":
		items.Add (new ("NoStdLib", "true"));
		break;
	case "analyzer":
		items.Add (new ("Analyzer", GetFullPath (value)));
		break;
	case "generatedfilesout":
		properties.Add (new ("GeneratedFilesOutputPath", GetFullPath (value)));
		break;
	case "noconfig": // this is already passed to csc by default
		break;
	default:
		ReportError ($"Didn't understand argument '{a}'");
		break;
	}
}

foreach (var file in sourceFiles) {
	items.Add (new ("Compile", GetFullPath (file)));
}

var sb = new StringBuilder ();
sb.AppendLine ($"<Project Sdk='Microsoft.Net.Sdk'>");
sb.AppendLine ($"    <PropertyGroup>");
sb.AppendLine ($"        <TargetFramework>{targetFramework}</TargetFramework>");
sb.AppendLine ($"        <EnableDefaultItems>false</EnableDefaultItems>");
sb.AppendLine ($"        <GenerateAssemblyInfo>false</GenerateAssemblyInfo>");
sb.AppendLine ($"        <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>");
sb.AppendLine ($"        <ProduceReferenceAssembly>false</ProduceReferenceAssembly>");
foreach (var prop in properties) {
	sb.AppendLine ($"        <{prop.Name}>{prop.Value}</{prop.Name}>");
}
sb.AppendLine ($"    </PropertyGroup>");
sb.AppendLine ($"    <ItemGroup>");
foreach (var item in items) {
	sb.AppendLine ($"        <{item.Name} Include=\"{item.Include}\" />");
}
sb.AppendLine ($"    </ItemGroup>");
sb.AppendLine ($"</Project>");
if (string.IsNullOrEmpty (output)) {
	Console.WriteLine (sb);
} else {
	File.WriteAllText (output, sb.ToString ());
}

return exitCode;
