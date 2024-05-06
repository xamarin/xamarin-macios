using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public class LinkNativeCode : XamarinTask, ITaskCallback {
		string outputPath = string.Empty;

		#region Inputs
		public ITaskItem [] LinkerFlags { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] LinkWithLibraries { get; set; } = Array.Empty<ITaskItem> ();

		// A path to entitlements to be embedded into the executable
		public string EntitlementsInExecutable { get; set; } = string.Empty;

		[Required]
		public string SdkDevPath { get; set; } = string.Empty;

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkRoot { get; set; } = string.Empty;

		[Required]
		public string OutputFile { get; set; } = string.Empty;

		[Required]
		public ITaskItem [] ObjectFiles { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string MinimumOSVersion { get; set; } = string.Empty;

		public ITaskItem [] NativeReferences { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] Frameworks { get; set; } = Array.Empty<ITaskItem> ();

		public string DylibRPath { get; set; } = string.Empty;

		public string FrameworkRPath { get; set; } = string.Empty;

		[Required]
		public string TargetArchitectures { get; set; } = string.Empty;

		TargetArchitecture architectures;
		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				outputPath = PathUtils.ConvertToMacPath (Path.GetDirectoryName (OutputFile));

				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
			}

			try {
				return ExecuteUnsafe ();
			} catch (Exception e) {
				return Log.LogErrorsFromException (e);
			}
		}

		bool ExecuteUnsafe ()
		{
			if (!Enum.TryParse (TargetArchitectures, out architectures)) {
				Log.LogError (12, null, MSBStrings.E0012, TargetArchitectures);
				return false;
			}

			var abis = architectures.ToArray ();
			if (abis.Count != 1) {
				Log.LogError (7070, null, MSBStrings.E7070, /* Invalid architecture ({0}): can't link more than one architecture at a time. */ TargetArchitectures);
				return false;
			}
			var abi = abis [0].ToNativeArchitecture ();
			var linkerExecutable = "clang++";
			var arguments = new List<string> ();
			arguments.Add (linkerExecutable);

			var hasEmbeddedFrameworks = false;

			if (NativeReferences.Length > 0) {
				var linkerArguments = new LinkerOptions ();
				linkerArguments.BuildNativeReferenceFlags (Log, NativeReferences);
				foreach (var framework in linkerArguments.Frameworks) {
					var fullPath = Path.GetFullPath (framework);
					arguments.Add ("-F");
					arguments.Add (Path.GetDirectoryName (fullPath));
					arguments.Add ("-framework");
					arguments.Add (Path.GetFileNameWithoutExtension (fullPath));
				}
				foreach (var framework in linkerArguments.WeakFrameworks) {
					var fullPath = Path.GetFullPath (framework);
					arguments.Add ("-F");
					arguments.Add (Path.GetDirectoryName (fullPath));
					arguments.Add ("-weak_framework");
					arguments.Add (Path.GetFileNameWithoutExtension (fullPath));
				}
				arguments.AddRange (linkerArguments.Arguments.ToList ());

				hasEmbeddedFrameworks |= linkerArguments.Frameworks.Count > 0 || linkerArguments.WeakFrameworks.Count > 0;
			}

			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.MacOSX:
				arguments.Add (PlatformFrameworkHelper.GetMinimumVersionArgument (TargetFrameworkMoniker, SdkIsSimulator, MinimumOSVersion));
				arguments.Add ("-isysroot");
				arguments.Add (SdkRoot);

				arguments.Add ("-arch");
				arguments.Add (abi);

				break;
			case ApplePlatform.MacCatalyst:
				arguments.Add ($"-target");
				arguments.Add ($"{abi}-apple-ios{MinimumOSVersion}-macabi");
				arguments.Add ("-isysroot");
				arguments.Add (SdkRoot);
				arguments.Add ("-iframework");
				arguments.Add (Path.Combine (SdkRoot, "System", "iOSSupport", "System", "Library", "Frameworks"));
				arguments.Add ($"-L{Path.Combine (SdkRoot, "System", "iOSSupport", "usr", "lib")}");
				break;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}

			bool hasDylibs = false;
			if (LinkWithLibraries is not null) {
				foreach (var libSpec in LinkWithLibraries) {
					var lib = Path.GetFullPath (libSpec.ItemSpec);
					var libExtension = Path.GetExtension (lib).ToLowerInvariant ();
					switch (libExtension) {
					case "": // we can get static libraries without any extensions at all
					case ".a":
					case ".o":
						var forceLoad = string.Equals (libSpec.GetMetadata ("ForceLoad"), "true", StringComparison.OrdinalIgnoreCase);
						if (forceLoad)
							arguments.Add ("-force_load");
						arguments.Add (lib);
						break;
					case ".dylib":
						var libName = Path.GetFileNameWithoutExtension (lib);
						if (libName.StartsWith ("lib", StringComparison.Ordinal)) {
							arguments.Add ("-L" + Path.GetDirectoryName (lib));
							arguments.Add ("-l" + libName.Substring (3));
						} else {
							arguments.Add (libSpec.ItemSpec);
						}
						hasDylibs = true;
						break;
					case ".framework":
						arguments.Add ("-F" + Path.GetDirectoryName (lib));
						arguments.Add ("-framework");
						arguments.Add (Path.GetFileNameWithoutExtension (lib));
						break;
					default:
						Log.LogError ($"Unknown library extension '{libExtension}' to link with for {lib}.");
						return false;
					}
				}
			}

			hasDylibs |= NativeReferences.Any (v => string.Equals (".dylib", Path.GetExtension (v.ItemSpec), StringComparison.OrdinalIgnoreCase));

			if (hasDylibs) {
				arguments.Add ("-rpath");
				arguments.Add (DylibRPath ?? "@executable_path");
			}

			if (Frameworks is not null) {
				foreach (var fw in Frameworks) {
					var is_weak = fw.GetMetadata ("IsWeak") == "true";
					var framework = fw.ItemSpec;
					if (framework.EndsWith (".framework", StringComparison.Ordinal)) {
						// user framework, we need to pass -F to the linker so that the linker finds the user framework.
						arguments.Add ("-F");
						arguments.Add (Path.GetDirectoryName (Path.GetFullPath (framework)));
						framework = Path.GetFileNameWithoutExtension (framework);
						hasEmbeddedFrameworks = true;
					}
					arguments.Add (is_weak ? "-weak_framework" : "-framework");
					arguments.Add (framework);
				}
			}

			if (hasEmbeddedFrameworks) {
				arguments.Add ("-rpath");
				arguments.Add (FrameworkRPath ?? "@executable_path/Frameworks");
			}

			if (ObjectFiles is not null)
				foreach (var obj in ObjectFiles)
					arguments.Add (Path.GetFullPath (obj.ItemSpec));

			arguments.AddRange (GetEmbedEntitlementsInExecutableLinkerFlags (EntitlementsInExecutable));

			arguments.Add ("-o");
			arguments.Add (Path.GetFullPath (OutputFile));

			if (LinkerFlags is not null) {
				foreach (var flag in LinkerFlags)
					arguments.Add (flag.ItemSpec);
			}

			var rv = ExecuteAsync ("xcrun", arguments, sdkDevPath: SdkDevPath, showErrorIfFailure: false).Result;
			if (rv.ExitCode != 0) {
				var stderr = rv.StandardError?.ToString ()?.Trim ();
#if NET
				if (string.IsNullOrEmpty (stderr)) {
#else
				if (stderr is null || string.IsNullOrEmpty (stderr)) {
#endif
					Log.LogError (MSBStrings.E0117, /* {0} exited with code {1} */ linkerExecutable, rv.ExitCode);
				} else {
					// Don't show any lines with "ld: warning: " in the error message, they're typically confusing.
					// Also show a max of 25 lines just to not overload the error output, the native linker can
					// sometimes give a truly staggering amount of error output.
					var stderrLines = stderr
										.Split (new char [] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
										.Where (v => !v.StartsWith ("ld: warning: ", StringComparison.Ordinal))
										.Take (25);
					var errorOutput = string.Join (Environment.NewLine, stderrLines);
					if (string.IsNullOrEmpty (errorOutput)) {
						// Show all lines after all if there's nothing left after removing the "ld: warning: " lines
						errorOutput = stderr;
					}

					Log.LogError (MSBStrings.E0118, /* {0} exited with code {1}:\n{2} */ linkerExecutable, rv.ExitCode, errorOutput);
				}
			}

			return !Log.HasLoggedErrors;
		}

		public static string [] GetEmbedEntitlementsInExecutableLinkerFlags (string entitlements)
		{
			if (string.IsNullOrEmpty (entitlements))
				return Array.Empty<string> ();

			if (!EntitlementsRequireLinkerFlags (entitlements))
				return Array.Empty<string> ();

			return new string [] {
				"-Xlinker", "-sectcreate",
				"-Xlinker", "__TEXT",
				"-Xlinker", "__entitlements",
				"-Xlinker", Path.GetFullPath (entitlements),
			};
		}

		static bool EntitlementsRequireLinkerFlags (string path)
		{
			try {
				var plist = PDictionary.FromFile (path)!;

				// FIXME: most keys do not require linking in the entitlements file, so we
				// could probably add some smarter logic here to iterate over all of the
				// keys in order to determine whether or not we really need to link with
				// the entitlements or not.
				return plist.Count != 0;
			} catch {
				return false;
			}
		}

		// We should avoid copying files from the output path because those already exist on the Mac
		// and the ones on Windows are empty, so we will break the build
		public bool ShouldCopyToBuildServer (ITaskItem item) => !PathUtils.ConvertToMacPath (item.ItemSpec).StartsWith (outputPath);

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
