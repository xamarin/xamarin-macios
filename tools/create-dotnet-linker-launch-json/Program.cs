using System.IO;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Logging.StructuredLogger;

using Mono.Options;

class Program {
	static int Main (string [] args)
	{
		var printHelp = false;
		var binlog = string.Empty;
		var rootDirectory = string.Empty;
		var skippedLinkerCommands = 0;
		var options = new OptionSet {
			{ "h|?|help", "Print this help message", (v) => printHelp = true },
			{ "r|root=", "The root directory", (v) => rootDirectory = v },
			{ "bl|binlog=", "The binlog", (v) => binlog = v },
			{ "s|skip", "Task invocations to skip", (v) => skippedLinkerCommands++ },
		};

		if (printHelp) {
			options.WriteOptionDescriptions (Console.Out);
			return 0;
		}

		var others = options.Parse (args);
		if (others.Any ()) {
			Console.WriteLine ("Unexpected arguments:");
			foreach (var arg in others)
				Console.WriteLine ("\t{0}", arg);
			Console.WriteLine ("Expected arguments are:");
			options.WriteOptionDescriptions (Console.Out);
			return 1;
		}

		if (string.IsNullOrEmpty (binlog)) {
			Console.Error.WriteLine ("A binlog is required");
			Console.WriteLine ("Expected arguments are:");
			options.WriteOptionDescriptions (Console.Out);
			return 1;
		}

		var path = Path.GetFullPath (binlog);

		if (string.IsNullOrEmpty (rootDirectory))
			rootDirectory = Path.GetDirectoryName (path)!;

		Console.WriteLine ($"Processing {path} with root directory {rootDirectory}...");


		var reader = new BinLogReader ();
		var records = reader.ReadRecords (path).ToArray ();
		foreach (var record in records) {
			if (record is null)
				continue;

			if (record.Args is null)
				continue;

			if (record.Args is TaskStartedEventArgs tsea && tsea.TaskName == "ILLink") {
				if (skippedLinkerCommands > 0) {
					Console.WriteLine ($"Skipped an ILLink task invocation, {skippedLinkerCommands} left to skip...");
					skippedLinkerCommands--;
					continue;
				}


				var relevantRecords = records.Where (v => v?.Args?.BuildEventContext?.TaskId == tsea.BuildEventContext.TaskId).Select (v => v.Args).ToArray ();
				var cla = relevantRecords.Where (v => v is BuildMessageEventArgs).Cast<BuildMessageEventArgs> ().Where (v => v?.ToString ()?.Contains ("CommandLineArguments") == true).ToArray ();
				foreach (var rr in relevantRecords) {
					if (rr is TaskCommandLineEventArgs tclea) {
						if (!Xamarin.Utils.StringUtils.TryParseArguments (tclea.CommandLine.Replace ('\n', ' '), out var arguments, out var ex)) {
							Console.WriteLine ($"Failed to parse command line arguments: {ex.Message}");
							return 1;
						}

						WriteLaunchJson (CreateLaunchJson (rootDirectory, arguments));
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
