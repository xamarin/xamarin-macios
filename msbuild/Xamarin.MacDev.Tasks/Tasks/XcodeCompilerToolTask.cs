using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

using Xamarin.MacDev;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class XcodeCompilerToolTask : XamarinTask {
		protected bool Link { get; set; }
		IList<string>? prefixes;
		string? toolExe;

		#region Inputs

		public string BundleIdentifier { get; set; } = string.Empty;

		[Required]
		public string MinimumOSVersion { get; set; } = string.Empty;

		[Required]
		public string IntermediateOutputPath { get; set; } = string.Empty;

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		[Required]
		public string ResourcePrefix { get; set; } = string.Empty;

		public string SdkBinPath { get; set; } = string.Empty;

		[Required]
		public string SdkPlatform { get; set; } = string.Empty;

		string? sdkDevPath;
		public string SdkDevPath {
#if NET
			get { return string.IsNullOrEmpty (sdkDevPath) ? "/" : sdkDevPath; }
#else
			get { return (sdkDevPath is null || string.IsNullOrEmpty (sdkDevPath)) ? "/" : sdkDevPath; }
#endif
			set { sdkDevPath = value; }
		}

		public string SdkUsrPath { get; set; } = string.Empty;

		[Required]
		public string SdkVersion { get; set; } = string.Empty;

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; } = string.Empty;

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] BundleResources { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] OutputManifests { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		#region Inputs from the app manifest

		public string CLKComplicationGroup { get; set; } = string.Empty;

		public string NSExtensionPointIdentifier { get; set; } = string.Empty;

		public string UIDeviceFamily { get; set; } = string.Empty;

		public bool WKWatchKitApp { get; set; }

		public string XSAppIconAssets { get; set; } = string.Empty;

		public string XSLaunchImageAssets { get; set; } = string.Empty;

		#endregion

		public IPhoneDeviceType ParsedUIDeviceFamily {
			get {
				if (!string.IsNullOrEmpty (UIDeviceFamily))
					return (IPhoneDeviceType) Enum.Parse (typeof (IPhoneDeviceType), UIDeviceFamily);
				return IPhoneDeviceType.NotSet;
			}
		}

		protected abstract string DefaultBinDir {
			get;
		}

		protected string DeveloperRootBinDir {
			get { return Path.Combine (SdkDevPath, "usr", "bin"); }
		}

		protected IList<string> ResourcePrefixes {
			get {
				if (prefixes is null)
					prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);

				return prefixes;
			}
		}

		protected abstract string ToolName { get; }

		protected virtual bool UseCompilationDirectory {
			get { return false; }
		}

		protected bool IsWatchExtension {
			get {
				return NSExtensionPointIdentifier == "com.apple.watchkit";
			}
		}

		protected IEnumerable<string> GetTargetDevices ()
		{
			return GetTargetDevices (ParsedUIDeviceFamily, WKWatchKitApp, IsWatchExtension);
		}

		IEnumerable<string> GetTargetDevices (IPhoneDeviceType devices, bool watch, bool watchExtension)
		{
			if (Platform == ApplePlatform.MacOSX)
				yield break;

			if (!watch) {
				// the project is either a normal iOS project or an extension
				if (devices == IPhoneDeviceType.NotSet) {
					// library projects and extension projects will not have this key, but
					// we'll want them to work for both iPhones and iPads if the
					// xib or storyboard supports them
					devices = IPhoneDeviceType.IPhoneAndIPad;
				}

				// if the project is a watch extension, we'll also want to include watch support
				watch = watchExtension;
			} else {
				// the project is a WatchApp, only include watch support
			}

			if ((devices & IPhoneDeviceType.IPhone) != 0)
				yield return "iphone";

			if ((devices & IPhoneDeviceType.IPad) != 0)
				yield return "ipad";

			if (watch)
				yield return "watch";

			yield break;
		}

		protected abstract void AppendCommandLineArguments (IDictionary<string, string?> environment, CommandLineArgumentBuilder args, ITaskItem [] items);

		static bool? translated;

		[DllImport ("/usr/lib/libSystem.dylib", SetLastError = true)]
		static extern int sysctlbyname (/* const char */ [MarshalAs (UnmanagedType.LPStr)] string property, ref long oldp, ref long oldlenp, IntPtr newp, /* size_t */ long newlen);

		// https://developer.apple.com/documentation/apple_silicon/about_the_rosetta_translation_environment
		static bool IsTranslated ()
		{
			if (translated is null) {
				long result = 0;
				long size = sizeof (long);
				translated = ((sysctlbyname ("sysctl.proc_translated", ref result, ref size, IntPtr.Zero, 0) != -1) && (result == 1));
			}
			return translated.Value;
		}

		protected int Compile (ITaskItem [] items, string output, ITaskItem manifest)
		{
			var environment = new Dictionary<string, string?> ();
			var args = new CommandLineArgumentBuilder ();

			if (!string.IsNullOrEmpty (SdkBinPath))
				environment.Add ("PATH", SdkBinPath);

			if (!string.IsNullOrEmpty (SdkUsrPath))
				environment.Add ("XCODE_DEVELOPER_USR_PATH", SdkUsrPath);

			if (!string.IsNullOrEmpty (SdkDevPath))
				environment.Add ("DEVELOPER_DIR", SdkDevPath);

			// workaround for ibtool[d] bug / asserts if Intel version is loaded
			string tool;
			if (IsTranslated ()) {
				// we force the Intel (translated) msbuild process to launch ibtool as "Apple"
				tool = "arch";
				args.Add ("-arch", "arm64e");
				args.Add ("/usr/bin/xcrun");
			} else {
				tool = "/usr/bin/xcrun";
			}
			args.Add (ToolName);
			args.Add ("--errors", "--warnings", "--notices");
			args.Add ("--output-format", "xml1");

			AppendCommandLineArguments (environment, args, items);

			if (Link)
				args.Add ("--link");
			else if (UseCompilationDirectory)
				args.Add ("--compilation-directory");
			else
				args.Add ("--compile");

			args.AddQuoted (Path.GetFullPath (output));

			foreach (var item in items)
				args.AddQuoted (item.GetMetadata ("FullPath"));

			var arguments = args.ToList ();
			var rv = ExecuteAsync (tool, arguments, sdkDevPath, environment: environment, mergeOutput: false).Result;
			var exitCode = rv.ExitCode;
			var messages = rv.StandardOutput!.ToString ();
			File.WriteAllText (manifest.ItemSpec, messages);

			if (exitCode != 0) {
				// Note: ibtool or actool exited with an error. Dump everything we can to help the user
				// diagnose the issue and then delete the manifest log file so that rebuilding tries
				// again (in case of ibtool's infamous spurious errors).
				var errors = rv.StandardError!.ToString ();
				if (errors.Length > 0)
					Log.LogError (null, null, null, items [0].ItemSpec, 0, 0, 0, 0, "{0}", errors);

				Log.LogError (MSBStrings.E0117, ToolName, exitCode);

				// Note: If the log file exists and is parseable, log those warnings/errors as well...
				if (File.Exists (manifest.ItemSpec)) {
					try {
						var plist = PDictionary.FromFile (manifest.ItemSpec)!;

						LogWarningsAndErrors (plist, items [0]);
					} catch (Exception ex) {
						Log.LogError (MSBStrings.E0094, ToolName, manifest.ItemSpec, ex.Message);
					}

					File.Delete (manifest.ItemSpec);
				}
			}

			return exitCode;
		}

		protected void LogWarningsAndErrors (PDictionary plist, ITaskItem file)
		{
			PDictionary? dictionary;
			PString? message;
			PArray? array;

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.notices", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("message", out message))
						Log.LogMessage (MessageImportance.Low, "{0} notice : {1}", ToolName, message.Value);
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.warnings", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("message", out message))
						Log.LogWarning (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.errors", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("message", out message))
						Log.LogError (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
				}
			}

			//Trying to parse document warnings and erros using a PDictionary first since it's what ibtool is returning when building a storyboard.
			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.notices", ToolName), out dictionary)) {
				foreach (var valuePair in dictionary) {
					array = valuePair.Value as PArray;
					foreach (var item in array.OfType<PDictionary> ()) {
						if (item.TryGetValue ("message", out message))
							Log.LogMessage (MessageImportance.Low, "{0} notice : {1}", ToolName, message.Value);
					}
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.warnings", ToolName), out dictionary)) {
				foreach (var valuePair in dictionary) {
					array = valuePair.Value as PArray;
					foreach (var item in array.OfType<PDictionary> ()) {
						if (item.TryGetValue ("message", out message))
							Log.LogWarning (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
					}
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.document.errors", ToolName), out dictionary)) {
				foreach (var valuePair in dictionary) {
					array = valuePair.Value as PArray;
					foreach (var item in array.OfType<PDictionary> ()) {
						if (item.TryGetValue ("message", out message))
							Log.LogError (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
					}
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.errors", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("description", out message))
						Log.LogError (ToolName, null, null, file.ItemSpec, 0, 0, 0, 0, "{0}", message.Value);
				}
			}

			if (plist.TryGetValue (string.Format ("com.apple.{0}.notices", ToolName), out array)) {
				foreach (var item in array.OfType<PDictionary> ()) {
					if (item.TryGetValue ("description", out message))
						Log.LogMessage (MessageImportance.Low, "{0} notice : {1}", ToolName, message.Value);
				}
			}
		}
	}
}
