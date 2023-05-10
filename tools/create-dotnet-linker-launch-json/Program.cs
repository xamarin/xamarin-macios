using System.IO;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Logging.StructuredLogger;

class Program {
	static int Main (string [] args)
	{

		if (args.Length != 1) {
			Console.Error.WriteLine ($"Expected exactly 1 argument, got {args.Length} arguments.");
			return 1;
		}

		var path = Path.GetFullPath (args [0]);
		var reader = new BinLogReader ();
		var records = reader.ReadRecords (path).ToArray ();
		foreach (var record in records) {
			if (record is null)
				continue;

			if (record.Args is null)
				continue;

			if (record.Args is TaskStartedEventArgs tsea && tsea.TaskName == "ILLink") {
				var relevantRecords = records.Where (v => v?.Args?.BuildEventContext?.TaskId == tsea.BuildEventContext.TaskId).Select (v => v.Args).ToArray ();
				var cla = relevantRecords.Where (v => v is BuildMessageEventArgs).Cast<BuildMessageEventArgs> ().Where (v => v?.ToString ()?.Contains ("CommandLineArguments") == true).ToArray ();
				foreach (var rr in relevantRecords) {
					if (rr is TaskCommandLineEventArgs tclea) {
						if (!Xamarin.Utils.StringUtils.TryParseArguments (tclea.CommandLine.Replace ('\n', ' '), out var arguments, out var ex)) {
							Console.WriteLine ($"Failed to parse command line arguments: {ex.Message}");
							return 1;
						}

						WriteLaunchJson (CreateLaunchJson (Path.GetDirectoryName (path)!, arguments));
						return 0;
					}
				}
			}
		}

		Console.Error.WriteLine ($"Unable to find command line arguments for ILLink in {path}");
		return 1;
	}

	static void WriteLaunchJson (string contents)
	{
		var dir = Environment.CurrentDirectory!;
		while (!Directory.Exists (Path.Combine (dir, "tools", "dotnet-linker")))
			dir = Path.GetDirectoryName (dir)!;
		var path = Path.Combine (dir, "tools", "dotnet-linker", ".vscode", "launch.json");
		File.WriteAllText (path, contents);
		Console.WriteLine ($"Created {path}");
	}

	static string CreateLaunchJson (string workingDirectory, string [] arguments)
	{
		var dotnet = arguments [0];
		var sb = new StringBuilder ();
		sb.AppendLine ("{");
		sb.AppendLine ("    // Use IntelliSense to learn about possible attributes.");
		sb.AppendLine ("    // Hover to view descriptions of existing attributes.");
		sb.AppendLine ("    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387");
		sb.AppendLine ("    \"version\": \"0.2.0\",");
		sb.AppendLine ("    \"configurations\": [");
		sb.AppendLine ("        {");
		sb.AppendLine ("            \"justMyCode\": false,");
		sb.AppendLine ("            \"preLaunchTask\": \"make\",");
		sb.AppendLine ("            \"name\": \".NET Core Launch (console)\",");
		sb.AppendLine ("            \"type\": \"coreclr\",");
		sb.AppendLine ("            \"request\": \"launch\",");
		sb.AppendLine ($"            \"program\": \"{dotnet}\",");
		sb.AppendLine ("            \"args\": [");
		for (var i = 1; i < arguments.Length; i++) {
			sb.AppendLine ($"                \"{arguments [i]}\"{(i < arguments.Length - 1 ? "," : "")}");
		}
		sb.AppendLine ("            ],");
		sb.AppendLine ($"            \"cwd\": \"{Path.GetFullPath (workingDirectory)}\",");
		sb.AppendLine ("            \"console\": \"internalConsole\",");
		sb.AppendLine ("            \"stopAtEntry\": false");
		sb.AppendLine ("        },");
		sb.AppendLine ("        {");
		sb.AppendLine ("            \"name\": \".NET Core Attach\",");
		sb.AppendLine ("            \"type\": \"coreclr\",");
		sb.AppendLine ("            \"request\": \"attach\"");
		sb.AppendLine ("        }");
		sb.AppendLine ("    ]");
		sb.AppendLine ("}");
		return sb.ToString ();
	}
}
