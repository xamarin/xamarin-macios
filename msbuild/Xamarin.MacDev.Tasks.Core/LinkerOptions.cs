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

		public void BuildNativeReferenceFlags (TaskLoggingHelper Log, ITaskItem[] NativeReferences)
		{
			if (NativeReferences == null)
				return;

			foreach (var item in NativeReferences) {
				var value = item.GetMetadata ("Kind");
				NativeReferenceKind kind;
				bool boolean;

				if (string.IsNullOrEmpty (value) || !Enum.TryParse (value, out kind)) {
					Log.LogWarning (MSBStrings.W0051, item.ItemSpec);
					continue;
				}

				if (kind == NativeReferenceKind.Static) {
					var libName = Path.GetFileName (item.ItemSpec);

					if (libName.EndsWith (".a", StringComparison.Ordinal))
						libName = libName.Substring (0, libName.Length - 2);

					if (libName.StartsWith ("lib", StringComparison.Ordinal))
						libName = libName.Substring (3);

					if (!string.IsNullOrEmpty (Path.GetDirectoryName (item.ItemSpec)))
						Arguments.AddQuoted ("-L" + Path.GetDirectoryName (item.ItemSpec));

					Arguments.AddQuoted ("-l" + libName);

					value = item.GetMetadata ("ForceLoad");

					if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean) {
						Arguments.Add ("-force_load");
						Arguments.AddQuoted (item.ItemSpec);
					}

					value = item.GetMetadata ("IsCxx");

					if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean)
						Cxx = true;
				} else if (kind == NativeReferenceKind.Framework) {
					var path = item.ItemSpec;
					// in case the full path to the library is given (msbuild)
					if (Path.GetExtension (path) != ".framework")
						path = Path.GetDirectoryName (path);
					Frameworks.Add (path);
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

