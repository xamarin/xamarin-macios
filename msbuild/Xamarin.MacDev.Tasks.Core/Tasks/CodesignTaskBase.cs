using System;
using System.IO;
using System.Linq;

using Parallel = System.Threading.Tasks.Parallel;
using ParallelOptions = System.Threading.Tasks.ParallelOptions;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CodesignTaskBase : XamarinTask
	{
		const string ToolName = "codesign";
		const string MacOSDirName = "MacOS";
		const string CodeSignatureDirName = "_CodeSignature";
		string toolExe;

		#region Inputs

		// Can also be specified per resource using the 'CodesignStampPath' metadata
		public string StampPath { get; set; }

		// Can also be specified per resource using the 'CodesignAllocate' metadata
		public string CodesignAllocate { get; set; }

		// Can also be specified per resource using the 'CodesignDisableTimestamp' metadata
		public bool DisableTimestamp { get; set; }

		// Can also be specified per resource using the 'CodesignEntitlements' metadata
		public string Entitlements { get; set; }

		// Can also be specified per resource using the 'CodesignKeychain' metadata
		public string Keychain { get; set; }

		[Required]
		public ITaskItem[] Resources { get; set; }

		// Can also be specified per resource using the 'CodesignResourceRules' metadata
		public string ResourceRules { get; set; }

		// Can also be specified per resource using the 'CodesignSigningKey' metadata
		public string SigningKey { get; set; }

		// Can also be specified per resource using the 'CodesignExtraArgs' metadata
		public string ExtraArgs { get; set; }

		// Can also be specified per resource using the 'CodesignDeep' metadata (yes, the naming difference is correct and due to historical reasons)
		public bool IsAppExtension { get; set; }

		// Can also be specified per resource using the 'CodesignUseHardenedRuntime' metadata
		public bool UseHardenedRuntime { get; set; }

		// Can also be specified per resource using the 'CodesignUseSecureTimestamp' metadata
		public bool UseSecureTimestamp { get; set; }

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; }

		#endregion

		#region Outputs

		// This output value is not observed anywhere in our targets, but it's required for building on Windows
		// to make sure any codesigned files other tasks depend on are copied back to the windows machine.
		[Output]
		public ITaskItem[] CodesignedFiles { get; set; }

		#endregion

		string GetFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		string GetOutputPath (ITaskItem item)
		{
			var path = item.ItemSpec;
			var app = path.LastIndexOf (".app/");
			return Path.Combine (GetCodesignStampPath (item), path.Substring (app + ".app/".Length));
		}

		string GetCodesignStampPath (ITaskItem item)
		{
			return GetNonEmptyStringOrFallback (item, "CodesignStampPath", StampPath);
		}

		string GetCodesignAllocate (ITaskItem item)
		{
			return GetNonEmptyStringOrFallback (item, "CodesignAllocate", CodesignAllocate, "CodesignAllocate", required: true);
		}

		bool NeedsCodesign (ITaskItem item)
		{
			var stampPath = GetCodesignStampPath (item);
			if (string.IsNullOrEmpty (stampPath))
				return true;

			var output = GetOutputPath (item);

			if (!File.Exists (output))
				return true;

			if (File.GetLastWriteTimeUtc (item.ItemSpec) >= File.GetLastWriteTimeUtc (output))
				return true;

			return false;
		}

		bool ParseBoolean (ITaskItem item, string metadataName, bool fallbackValue)
		{
			var metadataValue = item.GetMetadata (metadataName);
			if (string.IsNullOrEmpty (metadataValue))
				return fallbackValue;
			return string.Equals (metadataValue, "true", StringComparison.OrdinalIgnoreCase);
		}

		string GetNonEmptyStringOrFallback (ITaskItem item, string metadataName, string fallbackValue, string fallbackName = null, bool required = false)
		{
			var metadataValue = item.GetMetadata (metadataName);
			if (!string.IsNullOrEmpty (metadataValue))
				return metadataValue;
			if (required && string.IsNullOrEmpty (fallbackValue))
				Log.LogError (MSBStrings.E7085 /* The "{0}" task was not given a value for the required parameter "{1}", nor was there a "{2}" metadata on the resource {3}. */, "Codesign", fallbackName, metadataName, item.ItemSpec);
			return fallbackValue;
		}

		IList<string> GenerateCommandLineArguments (ITaskItem item)
		{
			var args = new List<string> ();
			var isDeep = ParseBoolean (item, "CodesignDeep", IsAppExtension);
			var useHardenedRuntime = ParseBoolean (item, "CodesignUseHardenedRuntime", UseHardenedRuntime);
			var useSecureTimestamp = ParseBoolean (item, "CodesignUseSecureTimestamp", UseSecureTimestamp);
			var disableTimestamp = ParseBoolean (item, "CodesignDisableTimestamp", DisableTimestamp);
			var signingKey = GetNonEmptyStringOrFallback (item, "CodesignSigningKey", SigningKey, "SigningKey", required: true);
			var keychain = GetNonEmptyStringOrFallback (item, "CodesignKeychain", Keychain);
			var resourceRules = GetNonEmptyStringOrFallback (item, "CodesignResourceRules", ResourceRules);
			var entitlements = GetNonEmptyStringOrFallback (item, "CodesignEntitlements", Entitlements);
			var extraArgs = GetNonEmptyStringOrFallback (item, "CodesignExtraArgs", ExtraArgs);

			args.Add ("-v");
			args.Add ("--force");

			if (isDeep)
				args.Add ("--deep");

			if (useHardenedRuntime) {
				args.Add ("-o");
				args.Add ("runtime");
			}

			if (useSecureTimestamp) {
				if (disableTimestamp) {
					// Conflicting '{0}' and '{1}' options. '{1}' will be ignored.
					Log.LogWarning (MSBStrings.W0176, "UseSecureTimestamp", "DisableTimestamp");
				}
				args.Add ("--timestamp");
			} else
				args.Add ("--timestamp=none");

			args.Add ("--sign");
			args.Add (signingKey);

			if (!string.IsNullOrEmpty (keychain)) {
				args.Add ("--keychain");
				args.Add (Path.GetFullPath (keychain));
			}

			if (!string.IsNullOrEmpty (resourceRules)) {
				args.Add ("--resource-rules");
				args.Add (Path.GetFullPath (resourceRules));
			}

			if (!string.IsNullOrEmpty (entitlements)) {
				args.Add ("--entitlements");
				args.Add (Path.GetFullPath (entitlements));
			}

			if (!string.IsNullOrEmpty (extraArgs))
				args.Add (extraArgs);

			// signing a framework and a file inside a framework is not *always* identical
			// on macOS apps {item.ItemSpec} can be a symlink to `Versions/Current/{item.ItemSpec}`
			// and `Current` also a symlink to `A`... and `_CodeSignature` will be found there
			var path = item.ItemSpec;
			var parent = Path.GetDirectoryName (path);
      
			// so do not don't sign `A.framework/A`, sign `A.framework` which will always sign the *bundle*
			if ((Path.GetExtension (parent) == ".framework") && (Path.GetFileName (path) == Path.GetFileNameWithoutExtension (parent)))
				path = parent;

			path = PathUtils.ResolveSymbolicLinks (path);
			args.Add (Path.GetFullPath (path));

			return args;
		}

		void Codesign (ITaskItem item)
		{
			var fileName = GetFullPathToTool ();
			var arguments = GenerateCommandLineArguments (item);
			var environment = new Dictionary<string, string> () {
				{ "CODESIGN_ALLOCATE", GetCodesignAllocate (item) },
			};
			var rv = ExecuteAsync (fileName, arguments, null, environment, mergeOutput: false).Result;
			var exitCode = rv.ExitCode;
			var messages = rv.StandardOutput.ToString ();
			
			if (messages.Length > 0)
				Log.LogMessage (MessageImportance.Normal, "{0}", messages.ToString ());

			if (exitCode != 0) {
				var errors = rv.StandardError.ToString ();
				if (errors.Length > 0)
					Log.LogError (MSBStrings.E0004, item.ItemSpec, errors);
				else
					Log.LogError (MSBStrings.E0005, item.ItemSpec);
			} else if (!string.IsNullOrEmpty (GetCodesignStampPath (item))) {
				var outputPath = GetOutputPath (item);
				Directory.CreateDirectory (Path.GetDirectoryName (outputPath));
				File.WriteAllText (outputPath, string.Empty);
			}
		}

		public override bool Execute ()
		{
			if (Resources.Length == 0)
				return true;

			var codesignedFiles = new List<ITaskItem> ();
			var resourcesToSign = Resources.Where (v => NeedsCodesign (v));

			Parallel.ForEach (resourcesToSign, new ParallelOptions { MaxDegreeOfParallelism = Math.Max (Environment.ProcessorCount / 2, 1) }, (item) => {
				Codesign (item);

				var files = GetCodesignedFiles (item);
				lock (codesignedFiles)
					codesignedFiles.AddRange (files);
			});

			CodesignedFiles = codesignedFiles.ToArray ();

			return !Log.HasLoggedErrors;
		}

		IEnumerable<ITaskItem> GetCodesignedFiles (ITaskItem item)
		{
			var codesignedFiles = new List<ITaskItem> ();

			if (Directory.Exists (item.ItemSpec)) {
				var codeSignaturePath = Path.Combine (item.ItemSpec, CodeSignatureDirName);

				if (!Directory.Exists (codeSignaturePath))
					return codesignedFiles;

				codesignedFiles.AddRange (Directory.EnumerateFiles (codeSignaturePath).Select (x => new TaskItem (x)));

				var extension = Path.GetExtension (item.ItemSpec);

				if (extension == ".app" || extension == ".appex") {
					var executableName = Path.GetFileName (item.ItemSpec);
					var manifestPath = Path.Combine (item.ItemSpec, "Info.plist");

					if (File.Exists(manifestPath)) {
						var bundleExecutable = PDictionary.FromFile (manifestPath).GetCFBundleExecutable ();

						if (!string.IsNullOrEmpty(bundleExecutable))
							executableName = bundleExecutable;
					}

					var basePath = item.ItemSpec;

					if (Directory.Exists (Path.Combine (basePath, MacOSDirName)))
						basePath = Path.Combine (basePath, MacOSDirName);

					var executablePath = Path.Combine (basePath, executableName);

					if (File.Exists (executablePath))
						codesignedFiles.Add (new TaskItem (executablePath));
				}
			} else if (File.Exists (item.ItemSpec)) {
				codesignedFiles.Add (item);
			}

			return codesignedFiles;
		}
	}
}
