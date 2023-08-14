using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public class LinkerOptions {
		public CommandLineArgumentBuilder Arguments { get; private set; }
		public HashSet<string> WeakFrameworks { get; private set; }
		public HashSet<string> Frameworks { get; private set; }
		public bool Cxx { get; set; }

		public LinkerOptions ()
		{
			Arguments = new CommandLineArgumentBuilder ();
			WeakFrameworks = new HashSet<string> ();
			Frameworks = new HashSet<string> ();
		}

		public void BuildNativeReferenceFlags (TaskLoggingHelper Log, ITaskItem [] NativeReferences)
		{
			if (NativeReferences is null)
				return;

			var libraryPaths = new HashSet<string> ();
			foreach (var item in NativeReferences) {
				var value = item.GetMetadata ("Kind");
				NativeReferenceKind kind;
				bool boolean;

				if (string.IsNullOrEmpty (value) || !Enum.TryParse (value, out kind)) {
					Log.LogWarning (MSBStrings.W0051, item.ItemSpec);
					continue;
				}

				if (kind == NativeReferenceKind.Static) {
					value = item.GetMetadata ("ForceLoad");

					if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean) {
						Arguments.Add ("-force_load");
					}

					Arguments.AddQuoted (item.ItemSpec);

					value = item.GetMetadata ("IsCxx");

					if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean)
						Cxx = true;
				} else if (kind == NativeReferenceKind.Framework) {
					var path = item.ItemSpec;
					// in case the full path to the library is given (msbuild)
					if (Path.GetExtension (path) != ".framework")
						path = Path.GetDirectoryName (path);
					Frameworks.Add (path);
				} else if (kind == NativeReferenceKind.Dynamic) {
					var path = item.ItemSpec;
					var directory = Path.GetDirectoryName (path);
					var lib = Path.GetFileName (path);
					if (lib.StartsWith ("lib", StringComparison.Ordinal)) {
						if (!string.IsNullOrEmpty (directory) && !libraryPaths.Contains (directory)) {
							Arguments.AddQuoted ("-L" + directory);
							libraryPaths.Add (directory);
						}
						// remove extension + "lib" prefix
						if (lib.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase))
							lib = Path.GetFileNameWithoutExtension (lib);
						Arguments.AddQuoted ("-l" + lib.Substring (3));
					} else {
						Arguments.AddQuoted (path);
					}
				} else {
					Log.LogWarning (MSBStrings.W0052, item.ItemSpec);
					continue;
				}

				value = item.GetMetadata ("NeedsGccExceptionHandling");
				if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean) {
					if (!Arguments.Contains ("-lgcc_eh"))
						Arguments.Add ("-lgcc_eh");
				}

				value = item.GetMetadata ("WeakFrameworks");
				if (!string.IsNullOrEmpty (value)) {
					foreach (var framework in value.Split (' ', '\t'))
						WeakFrameworks.Add (framework);
				}

				value = item.GetMetadata ("Frameworks");
				if (!string.IsNullOrEmpty (value)) {
					foreach (var framework in value.Split (' ', '\t'))
						Frameworks.Add (framework);
				}

				// Note: these get merged into gccArgs by our caller
				value = item.GetMetadata ("LinkerFlags");
				if (!string.IsNullOrEmpty (value)) {
					var linkerFlags = CommandLineArgumentBuilder.Parse (value);

					foreach (var flag in linkerFlags)
						Arguments.AddQuoted (flag);
				}
			}
		}
	}
}
