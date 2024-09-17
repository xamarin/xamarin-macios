using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public class Sharpie : XamarinTask {

		// Task input parameters
		public string Command { get; set; } = string.Empty;

		public string Namespace { get; set; } = string.Empty;

		public string Sdk { get; set; } = string.Empty;

		public string Scope { get; set; } = string.Empty;

		public string OutputPath { get; set; } = string.Empty;

		public ITaskItem [] Headers { get; set; } = Array.Empty<ITaskItem> ();

		public string SdkDevPath { get; set; } = string.Empty;


		const string ClassicXIAssembly = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/64bits/iOS/Xamarin.iOS.dll";

		protected virtual IList<string> GenerateCommandLineCommands ()
		{
			var args = new List<string> ();

			if (!string.IsNullOrEmpty (Command))
				args.Add (Command);

			if (!string.IsNullOrEmpty (Namespace)) {
				args.Add ("-namespace");
				args.Add (Namespace);
			}

			if (!string.IsNullOrEmpty (Sdk)) {
				args.Add ("-sdk");
				args.Add (Sdk);
			}

			if (!string.IsNullOrEmpty (Scope)) {
				args.Add ("-scope");
				args.Add (Scope);
			}

			if (!string.IsNullOrEmpty (SdkDevPath)) {
				args.Add ("-x");
				args.Add (SdkDevPath);
			}

			if (!string.IsNullOrEmpty (OutputPath)) {
				args.Add ("-output");
				args.Add (OutputPath);
			}

			foreach (var header in Headers) {
				args.Add (header.ItemSpec);
			}

			return args;
		}

		public override bool Execute ()
		{
			var sharpieTool = Path.Combine ("/usr", "local", "bin", "sharpie");
			if (!File.Exists (sharpieTool)) {
				Log.LogError (MSBStrings.XISHRP1000);
				return false;
			}

			if (!File.Exists (ClassicXIAssembly)) {
				Log.LogWarning (MSBStrings.XISHRP1001);
				return false;
			}

			ExecuteAsync (sharpieTool, GenerateCommandLineCommands ()).Wait ();
			return !Log.HasLoggedErrors;
		}

	}
}
