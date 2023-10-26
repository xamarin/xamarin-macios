using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;

using Microsoft.Build.Framework;
using Xamarin.Localization.MSBuild;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class FindAotCompiler : XamarinBuildTask {
		[Required]
		public ITaskItem [] MonoAotCrossCompiler { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public string AotCompiler { get; set; } = string.Empty;

		protected override bool ExecuteLocally ()
		{
			// If we can't find the AOT compiler path in MonoAotCrossCompiler, evaluate a project file that does know how to find it.
			// This happens when executing remotely from Windows, because the MonoAotCrossCompiler item group will be empty in that case.
			var evaluate = false;

			if (MonoAotCrossCompiler?.Length > 0 && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_FORCE_AOT_COMPILER_PATH_COMPUTATION"))) {
				var aotCompilerItem = MonoAotCrossCompiler.SingleOrDefault (v => v.GetMetadata ("RuntimeIdentifier") == RuntimeIdentifier);

				if (aotCompilerItem is null) {
					Log.LogMessage (MessageImportance.Low, "Unable to find the AOT compiler for the RuntimeIdentifier '{0}' in the MonoAotCrossCompiler item group", RuntimeIdentifier);
					evaluate = true;
				} else {
					AotCompiler = aotCompilerItem.ItemSpec;
				}
			} else {
				evaluate = true;
			}

			if (evaluate) {
				if (!TryGetItem ("MonoAotCrossCompiler", null, out var json))
					return false;
				if (!TryFindAotCompilerInJson (json, out var compiler))
					return false;
				AotCompiler = compiler;
			}

			// Don't check if the aot compiler exists if an error was already reported.
			if (Log.HasLoggedErrors)
				return false;

			if (!File.Exists (AotCompiler))
				Log.LogError (MSBStrings.E7081 /*"The AOT compiler '{0}' does not exist." */, AotCompiler);

			return !Log.HasLoggedErrors;
		}

		bool TryFindAotCompilerInJson (string json, [NotNullWhen (true)] out string? value)
		{
			value = null;

			// The json looks like this:
			//
			//    {
			//      "Items": {
			//        "MonoAotCrossCompiler": [
			//          {
			//            "Identity": "/usr/local/share/dotnet/packs/Microsoft.NETCore.App.Runtime.AOT.osx-arm64.Cross.maccatalyst-arm64/8.0.0/Sdk/../tools/mono-aot-cross",
			//            "RuntimeIdentifier": "maccatalyst-arm64",
			//            [...]
			//          },
			//          {
			//            "Identity": "/usr/local/share/dotnet/packs/Microsoft.NETCore.App.Runtime.AOT.osx-arm64.Cross.maccatalyst-x64/8.0.0/Sdk/../tools/mono-aot-cross",
			//            "RuntimeIdentifier": "maccatalyst-x64",
			//            [...]
			//          }
			//        ]
			//      }
			//    }
			//

			var options = new JsonDocumentOptions () {
				AllowTrailingCommas = true,
			};

			using var doc = JsonDocument.Parse (json, options);

			if (!doc.RootElement.TryGetProperty ("Items", out var items))
				return ShowError ();

			if (!items.TryGetProperty ("MonoAotCrossCompiler", out var compilers))
				return ShowError ();

			foreach (var compiler in compilers.EnumerateArray ()) {
				if (!compiler.TryGetProperty ("RuntimeIdentifier", out var ridElement))
					return ShowError ();

				var rid = ridElement.GetString ();
				if (!string.Equals (rid, RuntimeIdentifier, StringComparison.OrdinalIgnoreCase))
					continue;

				if (!compiler.TryGetProperty ("Identity", out var identity))
					return ShowError ();

				value = identity.GetString ();
				return true;
			}

			return false;

			bool ShowError ()
			{
				Log.LogError (MSBStrings.E7121 /* Unable to parse the json output from the AOT compiler search. */);
				Log.LogMessage (MessageImportance.Low, json);
				return false;
			}
		}
	}
}

