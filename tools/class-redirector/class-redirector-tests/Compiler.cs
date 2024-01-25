using System;
using System.Diagnostics;
using System.Text;

using Xamarin;
using Xamarin.Utils;

namespace ClassRedirectorTests;

public class Compiler {
	public Compiler ()
	{
	}

	public static CompileResult Compile (string code)
	{
		var directory = Cache.CreateTemporaryDirectory ("class-redirector");
		var file = "Code.cs";
		var inputpath = Path.Combine (directory, file);
		var outputpath = Path.Combine (directory, "Code.exe");
		File.WriteAllText (inputpath, code);

		//var args = "-unsafe -out:Code.exe " + inputpath;
		string result = "";
		try {
			var args = new List<string> () {
				"-unsafe",
				"-out:Code.exe",
				inputpath
			};
			var execTask = Execution.RunAsync ("mcs", args, environment: null, mergeOutput: false,
				workingDirectory: directory, log: null, timeout: null, cancellationToken: null);
			execTask.Wait ();
			var execResult = execTask.Result;
			if (execResult.ExitCode != 0) {
				result = execResult.StandardError?.ToString () ?? "no error?!";
			}
		} catch (Exception e) {
			result = e.Message;
		}

		return new CompileResult (directory, outputpath, result);
	}

	public static string Run (string path, List<string> args, string? workingDirectory = null, bool verbose = false)
	{
		var execTask = Execution.RunAsync (path, args, environment: null, mergeOutput: false);
		execTask.Wait ();
		var execResult = execTask.Result;
		if (execResult.ExitCode != 0)
			throw new Exception ($"Failed to execute (exit code {execResult.ExitCode}): {path} {string.Join (" ", args)}\n{execResult.StandardError?.ToString () ?? "".ToString ()}");
		return execResult.StandardOutput?.ToString () ?? "";
	}
}


