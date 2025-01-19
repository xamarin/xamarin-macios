using System.CommandLine;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Transformer;

public class Program {
	public static int Main (string [] args)
	{
		// Create options
		var rspOption = new Option<string> (
			new [] { "--response-file", "--rsp" },
			"Path to the RSP file"
		);

		var destinationOption = new Option<string> (
			new [] { "--destination", "-d" },
			"Path to the destination directory"
		);

		var forceOption = new Option<bool> (
			new [] { "--force", "-f" },
			"Erase the destination directory if it exists"
		);

		var workingDirectoryOption = new Option<string> (
			new [] { "--working-directory", "-w" },
			"Absolute path to an existing working directory"
		);

		var sdkPathOption = new Option<string> (
			new [] { "--sdk", "-s" },
			"Absolute path to the sdk directory"
		);

		// Create root command and add options
		var rootCmd = new RootCommand ("command to convert outdated bindings to be rgen compatible") {
			rspOption, destinationOption, forceOption, workingDirectoryOption, sdkPathOption
		};

		// If no arguments, show help and exit
		if (args.Length == 0) {
			rootCmd.InvokeAsync (new string [] { "--help" }).Wait ();
			return 0;
		}

		// Set handler for parsing and executing
		rootCmd.SetHandler (async (rspPath, destPath, workingDirectory, sdkPath, force) => {
			// Convert local to absolute, expand ~
			rspPath = ToAbsolutePath (rspPath);
			workingDirectory = ToAbsolutePath (workingDirectory);
			destPath = ToAbsolutePath (destPath);
			sdkPath = ToAbsolutePath (sdkPath);

			ValidateRsp (rspPath);
			ValidateSdk (sdkPath);
			ValidateWorkingDirectory (workingDirectory);
			PrepareDestination (destPath, force);

			// Parse the .rsp file with Roslyn's CSharpCommandLineParser
			var args = new string [] { $"@{rspPath}" };
			var parseResult = CSharpCommandLineParser.Default.Parse (args, workingDirectory, null);
			Console.WriteLine ($"RSP parsed. Errors: {parseResult.Errors.Length}");
			foreach (var resultError in parseResult.Errors) {
				Console.WriteLine (resultError);
			}

			await Transformer.Execute (
				destinationDirectory: destPath,
				rspFile: rspPath,
				workingDirectory: workingDirectory,
				sdkDirectory: sdkPath
			);
		},
			rspOption, destinationOption, workingDirectoryOption, sdkPathOption, forceOption
		);

		// Invoke command
		return rootCmd.InvokeAsync (args).Result;
	}

	static string ToAbsolutePath (string path)
	{
		var absolutePath = path.Replace ("~", Environment.GetFolderPath (Environment.SpecialFolder.UserProfile));
		if (!Path.IsPathRooted (absolutePath))
			absolutePath = Path.GetFullPath (absolutePath);
		return absolutePath;
	}
	static void ValidateRsp (string path)
	{
		if (string.IsNullOrWhiteSpace (path) || !File.Exists (path))
			throw new FileNotFoundException ("RSP file not found.");
		if ((File.GetAttributes (path) & FileAttributes.Directory) == FileAttributes.Directory)
			throw new Exception ("RSP path is a directory.");
		using var stream = File.OpenRead (path); // validate readability
	}

	static void ValidateWorkingDirectory (string path)
	{
		if (string.IsNullOrWhiteSpace (path) || !Directory.Exists (path))
			throw new DirectoryNotFoundException ("Working directory does not exist.");
	}

	static void ValidateSdk (string path)
	{
		if (string.IsNullOrWhiteSpace (path) || !Directory.Exists (path))
			throw new DirectoryNotFoundException ("Working directory does not exist.");
	}

	static void PrepareDestination (string path, bool force)
	{
		if (Directory.Exists (path)) {
			if (force)
				Directory.Delete (path, true);
		}

		Directory.CreateDirectory (path);
	}
}
