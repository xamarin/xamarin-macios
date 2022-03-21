using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public abstract class FindCscCompilerTaskBase : XamarinTask {
		[Output]
		public string CscCompiler { get; set; }

		public override bool Execute ()
		{
			if (!File.Exists (CscCompiler))
				CscCompiler = ComputeCscCompilerPath ();

			if (!File.Exists (CscCompiler)) {
				Log.LogError (MSBStrings.E7081 /*"The C# compiler '{0}' does not exist." */, CscCompiler);
			} else {
				Log.LogMessage (MessageImportance.Low, "Found the C# compiler: {0}", CscCompiler);
			}

			return !Log.HasLoggedErrors;
		}

		// If we can't find the csc compiler path, evaluate a project file that does know how to find it.
		// This happens when executing remotely from Windows
		string ComputeCscCompilerPath ()
		{
			var projectPath = Path.GetTempFileName ();
			var outputFile = Path.GetTempFileName ();

			File.Delete (projectPath);
			projectPath += ".csproj";
			var csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net6.0-{PlatformName}</TargetFramework>
	</PropertyGroup>
	<Target Name=""ComputeCscCompilerPath"">
		<PropertyGroup>
			<_CscCompiler>$(RoslynTargetsPath)/bincore/csc.dll</_CscCompiler>
		</PropertyGroup>
		<WriteLinesToFile File=""$(OutputFilePath)"" Lines=""$(_CscCompiler)"" />
	</Target>
</Project>
";
			File.WriteAllText (projectPath, csproj);

			var executable = Environment.GetEnvironmentVariable ("DOTNET_HOST_PATH");
			if (string.IsNullOrEmpty (executable))
				executable = "dotnet";

			var arguments = new List<string> ();
			arguments.Add ("build");
			arguments.Add ("/p:OutputFilePath=" + outputFile);
			arguments.Add ("/t:ComputeCscCompilerPath");
			arguments.Add (projectPath);

			try {
				ExecuteAsync (executable, arguments).Wait ();
				return File.ReadAllText (outputFile).Trim ();
			} finally {
				File.Delete (projectPath);
				File.Delete (outputFile);
			}
		}
	}
}

