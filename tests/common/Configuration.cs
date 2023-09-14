using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using NUnit.Framework;

using Xamarin.Utils;

#nullable disable // until we get around to fixing this file

namespace Xamarin.Tests {
	static partial class Configuration {
		public const string XI_ProductName = "MonoTouch";
		public const string XM_ProductName = "Xamarin.Mac";

		static string mt_root;
		static string ios_destdir;
		static string mac_destdir;
		public static string DotNetBclDir;
		public static string DotNetCscCommand;
		public static string DotNetExecutable;
		public static string DotNetTfm;
		public static string mt_src_root;
		public static string sdk_version;
		public static string watchos_sdk_version;
		public static string tvos_sdk_version;
		public static string macos_sdk_version;
		public static string xcode_root;
		public static string XcodeVersionString;
		public static string xcode83_root;
		public static string xcode94_root;
#if MONOMAC
		public static string mac_xcode_root;
#endif
		public static Dictionary<string, string> make_config = new Dictionary<string, string> ();

		public static bool include_ios;
		public static bool include_mac;
		public static bool include_tvos;
		public static bool include_watchos;
		public static bool include_dotnet_watchos;
		public static bool include_maccatalyst;
		public static bool include_device;
		public static bool include_dotnet;
		public static bool include_legacy_xamarin;
		public static bool iOSSupports32BitArchitectures;

		static Version xcode_version;
		public static Version XcodeVersion {
			get {
				if (xcode_version is null)
					xcode_version = Version.Parse (XcodeVersionString);
				return xcode_version;
			}
		}

		static bool? use_system; // if the system-installed XI/XM should be used instead of the local one.
		public static bool UseSystem {
			get {
				if (!use_system.HasValue)
					use_system = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("TESTS_USE_SYSTEM"));
				return use_system.Value;
			}
			set {
				use_system = value;
			}
		}

		static bool? is_vsts; // if the system-installed XI/XM should be used instead of the local one.
		public static bool IsVsts {
			get {
				if (!is_vsts.HasValue)
					is_vsts = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("BUILD_BUILDID"));
				return is_vsts.Value;
			}
			set {
				is_vsts = value;
			}
		}

		public static string XcodeLocation {
			get {
				return xcode_root;
			}
		}

		public static string IOS_DESTDIR {
			get { return ios_destdir; }
		}

		public static string MAC_DESTDIR {
			get { return mac_destdir; }
		}

		// This is the location of an Xcode which is older than the recommended one.
		public static string GetOldXcodeRoot (Version min_version = null)
		{
			var xcodes = Directory.GetDirectories ("/Applications", "Xcode*.app", SearchOption.TopDirectoryOnly);
			var with_versions = new List<Tuple<Version, string>> ();

			var max_version = Version.Parse (XcodeVersionString);
			foreach (var xcode in xcodes) {
				var path = Path.Combine (xcode, "Contents", "Developer");
				var xcode_version = GetXcodeVersion (path);
				if (xcode_version is null)
					continue;
				var version = Version.Parse (xcode_version);
				if (version >= max_version)
					continue;
				if (version.Major == max_version.Major)
					continue;
				if (min_version is not null && version < min_version)
					continue;
				with_versions.Add (new Tuple<Version, string> (version, path));
			}

			if (with_versions.Count == 0)
				return null;

			with_versions.Sort ((x, y) => {
				if (x.Item1 > y.Item1)
					return -1;
				else if (x.Item1 < y.Item1)
					return 1;
				else
					return 0;
			});

			return with_versions [0].Item2; // return the most recent Xcode older than the recommended one.
		}

		// This is /Library/Frameworks/Xamarin.iOS.framework/Versions/Current if running
		// against a system XI, otherwise it's the <git checkout>/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/Current directory.
		public static string MonoTouchRootDirectory {
			get {
				return mt_root;
			}
		}

		static IEnumerable<string> FindConfigFiles (string name)
		{
			var dir = TestAssemblyDirectory;
			while (!string.IsNullOrEmpty (dir) && dir != "/") {
				var file = Path.Combine (dir, name);
				if (File.Exists (file))
					yield return file;
				file = Path.Combine (dir, "tests", name); // when running the msbuild tests.
				if (File.Exists (file))
					yield return file;
				dir = Path.GetDirectoryName (dir);
			}
		}

		static void ParseConfigFiles ()
		{
			var test_config = FindConfigFiles (UseSystem ? "test-system.config" : "test.config");
			if (!test_config.Any () && Environment.OSVersion.Platform != PlatformID.Win32NT) {
				// Run 'make test.config' in the tests/ directory
				// First find the tests/ directory
				var dir = TestAssemblyDirectory;
				string tests_dir = null;
				while (dir.Length > 1) {
					var file = Path.Combine (dir, "tests");
					if (Directory.Exists (file)) {
						tests_dir = file;
						break;
					}
					dir = Path.GetDirectoryName (dir);
				}
				if (tests_dir is null)
					throw new Exception ($"Could not find the directory 'tests'. Please run 'make' in the tests/ directory.");
				// Run make
				ExecutionHelper.Execute ("make", new string [] { "-C", tests_dir, "test.config" });
				test_config = FindConfigFiles ("test.config");
			}
			if (test_config.Any ())
				ParseConfigFiles (test_config);
			ParseConfigFiles (FindConfigFiles ("configure.inc"));
			ParseConfigFiles (FindConfigFiles ("Make.config.local"));
			ParseConfigFiles (FindConfigFiles ("Make.config"));
		}

		static void ParseConfigFiles (IEnumerable<string> files)
		{
			foreach (var file in files)
				ParseConfigFile (file);
		}

		static void ParseConfigFile (string file)
		{
			if (string.IsNullOrEmpty (file))
				return;

			foreach (var line in File.ReadAllLines (file).Reverse ()) {
				var eq = line.IndexOf ('=');
				if (eq == -1)
					continue;
				var key = line.Substring (0, eq);
				if (!make_config.ContainsKey (key))
					make_config [key] = line.Substring (eq + 1);
			}
		}

		internal static string GetVariable (string variable, string @default)
		{
			var result = Environment.GetEnvironmentVariable (variable);
			if (string.IsNullOrEmpty (result))
				make_config.TryGetValue (variable, out result);
			if (string.IsNullOrEmpty (result))
				result = @default;
			return result;
		}

		public static IList<string> GetVariableArray (string variable, string @default = "")
		{
			// variables with more than one value are wrapped in ', get the var remove the '' and split
			var value = GetVariable (variable, @default).Trim ('\'');
			return value.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string EvaluateVariable (string variable)
		{
			var result = Environment.GetEnvironmentVariable (variable);
			if (!string.IsNullOrEmpty (result))
				return result;

			var output = new StringBuilder ();
			var rv = ExecutionHelper.Execute ("/usr/bin/make", new string [] { "-C", Path.Combine (SourceRoot, "tools", "devops"), "print-abspath-variable", $"VARIABLE={variable}" }, environmentVariables: null, stdout: output, stderr: output, timeout: TimeSpan.FromSeconds (5));
			if (rv != 0)
				throw new Exception ($"Failed to evaluate variable '{variable}'. Exit code: {rv}. Output:\n{output}");
			result = output.ToString ().Split (new char [] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Where (v => v.StartsWith (variable + "=", StringComparison.Ordinal)).SingleOrDefault ();
			if (result is null)
				throw new Exception ($"Could not find the variable '{variable}' to evaluate.");
			return result.Substring (variable.Length + 1);
		}

		static string GetXcodeVersion (string xcode_path)
		{
			var version_plist = Path.Combine (xcode_path, "..", "version.plist");
			if (!File.Exists (version_plist))
				return null;

			return GetPListStringValue (version_plist, "CFBundleShortVersionString");
		}

		public static string GetPListStringValue (string plist, string key)
		{
			var settings = new System.Xml.XmlReaderSettings ();
			settings.DtdProcessing = System.Xml.DtdProcessing.Ignore;
			var doc = new System.Xml.XmlDocument ();
			using (var fs = new StringReader (ReadPListAsXml (plist))) {
				using (var reader = System.Xml.XmlReader.Create (fs, settings)) {
					doc.Load (reader);
					return doc.DocumentElement.SelectSingleNode ($"//dict/key[text()='{key}']/following-sibling::string[1]/text()").Value;
				}
			}
		}

		public static string ReadPListAsXml (string path)
		{
			string tmpfile = null;
			try {
				tmpfile = Path.GetTempFileName ();
				File.Copy (path, tmpfile, true);
				using (var process = new System.Diagnostics.Process ()) {
					process.StartInfo.FileName = "plutil";
					process.StartInfo.Arguments = StringUtils.FormatArguments ("-convert", "xml1", tmpfile);
					process.Start ();
					process.WaitForExit ();
					return File.ReadAllText (tmpfile);
				}
			} finally {
				if (tmpfile is not null)
					File.Delete (tmpfile);
			}
		}

		static Configuration ()
		{
			ParseConfigFiles ();

			mt_root = GetVariable ("MONOTOUCH_PREFIX", "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current");
			ios_destdir = GetVariable ("IOS_DESTDIR", null);
			mac_destdir = GetVariable ("MAC_DESTDIR", null);
			sdk_version = GetVariable ("IOS_SDK_VERSION", "8.0");
			watchos_sdk_version = GetVariable ("WATCH_SDK_VERSION", "2.0");
			tvos_sdk_version = GetVariable ("TVOS_SDK_VERSION", "9.0");
			macos_sdk_version = GetVariable ("MACOS_SDK_VERSION", "10.12");
			xcode_root = GetVariable ("XCODE_DEVELOPER_ROOT", "/Applications/Xcode.app/Contents/Developer");
			xcode83_root = GetVariable ("XCODE83_DEVELOPER_ROOT", "/Applications/Xcode83.app/Contents/Developer");
			xcode94_root = GetVariable ("XCODE94_DEVELOPER_ROOT", "/Applications/Xcode94.app/Contents/Developer");
			include_ios = !string.IsNullOrEmpty (GetVariable ("INCLUDE_IOS", ""));
			include_mac = !string.IsNullOrEmpty (GetVariable ("INCLUDE_MAC", ""));
			include_tvos = !string.IsNullOrEmpty (GetVariable ("INCLUDE_TVOS", ""));
			include_watchos = !string.IsNullOrEmpty (GetVariable ("INCLUDE_WATCH", ""));
			include_dotnet_watchos = !string.IsNullOrEmpty (GetVariable ("INCLUDE_DOTNET_WATCH", ""));
			include_maccatalyst = !string.IsNullOrEmpty (GetVariable ("INCLUDE_MACCATALYST", ""));
			include_device = !string.IsNullOrEmpty (GetVariable ("INCLUDE_DEVICE", ""));
			include_dotnet = !string.IsNullOrEmpty (GetVariable ("ENABLE_DOTNET", ""));
			include_legacy_xamarin = !string.IsNullOrEmpty (GetVariable ("INCLUDE_XAMARIN_LEGACY", ""));
			DotNetBclDir = GetVariable ("DOTNET_BCL_DIR", null);
			DotNetCscCommand = GetVariable ("DOTNET_CSC_COMMAND", null)?.Trim ('\'');
			DotNetExecutable = GetVariable ("DOTNET", null);
			DotNetTfm = GetVariable ("DOTNET_TFM", null);
			iOSSupports32BitArchitectures = !string.IsNullOrEmpty (GetVariable ("IOS_SUPPORTS_32BIT_ARCHITECTURES", ""));

			XcodeVersionString = GetXcodeVersion (xcode_root);
#if MONOMAC
			mac_xcode_root = xcode_root;
#endif

			if (!string.IsNullOrEmpty (ios_destdir))
				mt_root = Path.Combine (ios_destdir, mt_root.Substring (1));

			Console.WriteLine ("Test configuration:");
			Console.WriteLine ("  MONOTOUCH_PREFIX={0}", mt_root);
			Console.WriteLine ("  IOS_DESTDIR={0}", ios_destdir);
			Console.WriteLine ("  MAC_DESTDIR={0}", mac_destdir);
			Console.WriteLine ("  SDK_VERSION={0}", sdk_version);
			Console.WriteLine ("  XCODE_ROOT={0}", xcode_root);
#if MONOMAC
			Console.WriteLine ("  MAC_XCODE_ROOT={0}", mac_xcode_root);
#endif
			Console.WriteLine ("  INCLUDE_IOS={0}", include_ios);
			Console.WriteLine ("  INCLUDE_MAC={0}", include_mac);
			Console.WriteLine ("  INCLUDE_TVOS={0}", include_tvos);
			Console.WriteLine ("  INCLUDE_WATCHOS={0}", include_watchos);
			Console.WriteLine ("  INCLUDE_MACCATALYST={0}", include_maccatalyst);
			Console.WriteLine ("  ENABLE_DOTNET={0}", include_dotnet);
		}

		public static string RootPath {
			get {
				var dir = TestAssemblyDirectory;
				var path = Path.Combine (dir, ".git");
				while (!Directory.Exists (path) && path.Length > 3) {
					dir = Path.GetDirectoryName (dir);
					if (dir is null)
						throw new Exception ($"Could not find the xamarin-macios repo given the test assembly directory {TestAssemblyDirectory}");
					path = Path.Combine (dir, ".git");
				}
				path = Path.GetDirectoryName (path);
				if (!Directory.Exists (path))
					throw new Exception ($"Could not find the xamarin-macios repo given the test assembly directory {TestAssemblyDirectory}");
				return path;
			}
		}

		public static bool TryGetRootPath (out string rootPath)
		{
			try {
				rootPath = RootPath;
				return true;
			} catch {
				rootPath = null;
				return false;
			}
		}

		static string TestAssemblyDirectory {
			get {
				return TestContext.CurrentContext.WorkDirectory;
			}
		}

		public static string SourceRoot {
			get {
				if (mt_src_root is null)
					mt_src_root = RootPath;
				return mt_src_root;
			}
		}

		public static string XamarinIOSDll {
			get {
				return Path.Combine (mt_root, "lib", "mono", "Xamarin.iOS", "Xamarin.iOS.dll");
			}
		}

		public static string XamarinCatalystDll {
			get {
				return Path.Combine (mt_root, "lib", "mono", "Xamarin.MacCatalyst", "Xamarin.MacCatalyst.dll");
			}
		}

		public static string XamarinWatchOSDll {
			get {
				return Path.Combine (mt_root, "lib", "mono", "Xamarin.WatchOS", "Xamarin.WatchOS.dll");
			}
		}

		public static string XamarinTVOSDll {
			get {
				return Path.Combine (mt_root, "lib", "mono", "Xamarin.TVOS", "Xamarin.TVOS.dll");
			}
		}

		public static string XamarinMacMobileDll {
			get {
				return Path.Combine (SdkRootXM, "lib", "mono", "Xamarin.Mac", "Xamarin.Mac.dll");
			}
		}

		public static string XamarinMacFullDll {
			get {
				return Path.Combine (SdkRootXM, "lib", "mono", "4.5", "Xamarin.Mac.dll");
			}
		}

		public static string SdkBinDir {
			get {
#if MONOMAC
				return BinDirXM;
#else
				return BinDirXI;
#endif
			}
		}

		public static string TargetDirectoryXI {
			get {
				if (UseSystem)
					return "/";
				return make_config ["IOS_DESTDIR"];
			}
		}

		public static string TargetDirectoryXM {
			get {
				if (UseSystem)
					return "/";
				return make_config ["MAC_DESTDIR"];
			}
		}

		public static string TestProjectsDirectory {
			get {
				return Path.Combine (RootPath, "tests", "common", "TestProjects");
			}
		}

		public static string SdkRoot {
			get {
#if MONOMAC
				return SdkRootXM;
#else
				return SdkRootXI;
#endif
			}
		}

		static string GetRefNuGetName (TargetFramework targetFramework) => GetRefNuGetName (targetFramework.Platform);

		static string GetRefNuGetName (ApplePlatform platform)
		{
			var variableName = platform.AsString ().ToUpper () + "_NUGET_REF_NAME";
			return GetVariable (variableName, variableName + " not found");
		}

		static string GetRuntimeNuGetName (ApplePlatform platform, string runtimeIdentifier)
		{
			var variableName = runtimeIdentifier + "_NUGET_RUNTIME_NAME";
			return GetVariable (variableName, variableName + " not found");
		}

		static string GetSdkNuGetName (TargetFramework targetFramework)
		{
			return GetSdkNuGetName (targetFramework.Platform);
		}

		public static string GetSdkNuGetName (ApplePlatform platform)
		{
			var variableName = platform.AsString ().ToUpper () + "_NUGET_SDK_NAME";
			return GetVariable (variableName, variableName + " not found");
		}

		public static string GetNuGetOsVersion (ApplePlatform platform)
		{
			var variableName = platform.AsString ().ToUpper () + "_NUGET_OS_VERSION";
			return GetVariable (variableName, variableName + " not found");
		}

		public static string GetDotNetRoot ()
		{
			if (IsVsts) {
				return Path.Combine (EvaluateVariable ("DOTNET_DIR"), "packs");
			} else {
				return Path.Combine (SourceRoot, "_build");
			}
		}

		public static string GetRefDirectory (ApplePlatform platform)
		{
			var rv = Path.Combine (GetDotNetRoot (), GetRefNuGetName (platform));
			if (UseSystem)
				rv = Path.Combine (rv, GetNuGetVersionNoMetadata (platform));
			rv = Path.Combine (rv, "ref", DotNetTfm);
			return rv;
		}

		public static string GetRefDirectory (TargetFramework targetFramework)
		{
			if (targetFramework.IsDotNet)
				return GetRefDirectory (targetFramework.Platform);

			// This is only applicable for .NET
			throw new InvalidOperationException (targetFramework.ToString ());
		}

		public static string GetNuGetVersionNoMetadata (TargetFramework framework)
		{
			return GetNuGetVersionNoMetadata (framework.Platform);
		}

		public static string GetNuGetVersionNoMetadata (ApplePlatform platform)
		{
			var workloadVersion = Environment.GetEnvironmentVariable ($"{platform.AsString ().ToUpper ()}_WORKLOAD_VERSION");
			return string.IsNullOrEmpty (workloadVersion) ?
				GetVariable ($"{platform.AsString ().ToUpper ()}_NUGET_VERSION_NO_METADATA", string.Empty) : workloadVersion;
		}

		// This is only applicable for .NET
		public static string GetRuntimeDirectory (ApplePlatform platform, string runtimeIdentifier)
		{
			var rv = Path.Combine (GetDotNetRoot (), GetRuntimeNuGetName (platform, runtimeIdentifier));
			if (UseSystem)
				rv = Path.Combine (rv, GetNuGetVersionNoMetadata (platform));
			return Path.Combine (rv, "runtimes", runtimeIdentifier);
		}

		public static string GetTargetDirectory (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return TargetDirectoryXI;
			case ApplePlatform.MacOSX:
				return TargetDirectoryXM;
			default:
				throw new InvalidOperationException (platform.ToString ());
			}
		}

		public static string GetSdkRoot (TargetFramework targetFramework)
		{
			if (targetFramework.IsDotNet)
				return GetSdkRoot (targetFramework.Platform);
			switch (targetFramework.Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return SdkRootXI;
			case ApplePlatform.MacOSX:
				return SdkRootXM;
			default:
				throw new InvalidOperationException ();
			}
		}

		// Only valid for .NET
		public static string GetSdkRoot (ApplePlatform platform)
		{
			var rv = Path.Combine (GetDotNetRoot (), GetSdkNuGetName (platform));
			if (UseSystem)
				rv = Path.Combine (rv, GetNuGetVersionNoMetadata (platform));
			return Path.Combine (rv, "tools");
		}

		public static string SdkRootXI {
			get {
				return Path.Combine (TargetDirectoryXI, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
			}
		}

		public static string SdkRootXM {
			get {
				return Path.Combine (TargetDirectoryXM, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
			}
		}

		public static string BinDirXI {
			get {
				return Path.Combine (SdkRootXI, "bin");
			}
		}

		public static string BinDirXM {
			get {
				return Path.Combine (SdkRootXM, "bin");
			}
		}

		public static string BtouchPath {
			get {
				return Path.Combine (SdkBinDir, "btouch-native");
			}
		}

		public static string BGenPath {
			get {
				return Path.Combine (SdkBinDir, "bgen");
			}
		}

		public static string BGenClassicPath {
			get {
				return Path.Combine (BinDirXM, "bgen-classic");
			}
		}

		public static string GetBindingAttributePath (TargetFramework targetFramework)
		{
			if (targetFramework.IsDotNet)
				return Path.Combine (GetSdkRoot (targetFramework), "lib", "Xamarin.Apple.BindingAttributes.dll");

			switch (targetFramework.Platform) {
			case ApplePlatform.iOS:
				return Path.Combine (GetSdkRoot (targetFramework), "lib", "bgen", "Xamarin.iOS.BindingAttributes.dll");
			case ApplePlatform.TVOS:
				return Path.Combine (GetSdkRoot (targetFramework), "lib", "bgen", "Xamarin.TVOS.BindingAttributes.dll");
			case ApplePlatform.WatchOS:
				return Path.Combine (GetSdkRoot (targetFramework), "lib", "bgen", "Xamarin.WatchOS.BindingAttributes.dll");
			case ApplePlatform.MacOSX:
				if (targetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
					return Path.Combine (GetSdkRoot (targetFramework), "lib", "bgen", "Xamarin.Mac-mobile.BindingAttributes.dll");
				} else if (targetFramework == TargetFramework.Xamarin_Mac_4_5_Full) {
					return Path.Combine (GetSdkRoot (targetFramework), "lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
				}
				goto default;
			default:
				throw new InvalidOperationException ();
			}
		}

		public static string MmpPath {
			get {
				return Path.Combine (BinDirXM, "mmp");
			}
		}

		public static string MtouchPath {
			get {
				return Path.Combine (BinDirXI, "mtouch");
			}
		}

		public static string MlaunchPath {
			get {
				var env = Environment.GetEnvironmentVariable ("MLAUNCH_PATH");
				if (!string.IsNullOrEmpty (env))
					return env;
				return Path.Combine (BinDirXI, "mlaunch");
			}
		}

#if !XAMMAC_TESTS
		public static void AssertRuntimeIdentifierAvailable (ApplePlatform platform, string runtimeIdentifier)
		{
			if (string.IsNullOrEmpty (runtimeIdentifier))
				return;

			if (GetRuntimeIdentifiers (platform).Contains (runtimeIdentifier))
				return;

			Assert.Ignore ($"The runtime identifier {runtimeIdentifier} is not available on {platform}");
		}

		public static void AssertRuntimeIdentifiersAvailable (ApplePlatform platform, string runtimeIdentifiers)
		{
			if (string.IsNullOrEmpty (runtimeIdentifiers))
				return;

			foreach (var rid in runtimeIdentifiers.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				AssertRuntimeIdentifierAvailable (platform, rid);
		}

		public static string GetBaseLibrary (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return XamarinIOSDll;
			case Profile.tvOS:
				return XamarinTVOSDll;
			case Profile.watchOS:
				return XamarinWatchOSDll;
			case Profile.macOSMobile:
				return XamarinMacMobileDll;
			case Profile.macOSFull:
			case Profile.macOSSystem:
				return XamarinMacFullDll;
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetBaseLibrary (ApplePlatform platform, bool isDotNet)
		{
			if (isDotNet)
				return Path.Combine (GetRefDirectory (platform), GetBaseLibraryName (platform, isDotNet));
			return GetBaseLibrary (platform.AsProfile ());
		}

		static string GetBaseLibraryName (TargetFramework targetFramework)
		{
			return GetBaseLibraryName (targetFramework.Platform, targetFramework.IsDotNet);
		}

		public static string GetBaseLibraryName (ApplePlatform platform, bool isDotNet)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				return isDotNet ? "Microsoft.iOS.dll" : "Xamarin.iOS.dll";
			case ApplePlatform.TVOS:
				return isDotNet ? "Microsoft.tvOS.dll" : "Xamarin.TVOS.dll";
			case ApplePlatform.WatchOS:
				return isDotNet ? "Microsoft.watchOS.dll" : "Xamarin.WatchOS.dll";
			case ApplePlatform.MacOSX:
				return isDotNet ? "Microsoft.macOS.dll" : "Xamarin.Mac.dll";
			case ApplePlatform.MacCatalyst:
				return isDotNet ? "Microsoft.MacCatalyst.dll" : "Xamarin.MacCatalyst.dll";
			default:
				throw new InvalidOperationException (platform.ToString ());
			}
		}

		public static ApplePlatform GetPlatform (string assemblyName, bool isDotNet)
		{
			if (isDotNet) {
				switch (Path.GetFileNameWithoutExtension (assemblyName)) {
				case "Microsoft.iOS":
					return ApplePlatform.iOS;
				case "Microsoft.MacCatalyst":
					return ApplePlatform.MacCatalyst;
				case "Microsoft.tvOS":
					return ApplePlatform.TVOS;
				case "Microsoft.macOS":
					return ApplePlatform.MacOSX;
				default:
					throw new NotSupportedException ($"Unknown assembly: {assemblyName}");
				}
			} else {
				switch (Path.GetFileNameWithoutExtension (assemblyName)) {
				case "Xamarin.iOS":
					return ApplePlatform.iOS;
				case "Xamarin.WatchOS":
					return ApplePlatform.WatchOS;
				case "Xamarin.TVOS":
					return ApplePlatform.TVOS;
				case "Xamarin.Mac":
					return ApplePlatform.MacOSX;
				default:
					throw new NotSupportedException ($"Unknown assembly: {assemblyName}");
				}
			}
		}

		public static string GetBaseLibrary (TargetFramework targetFramework)
		{
			if (targetFramework.IsDotNet)
				return Path.Combine (GetRefDirectory (targetFramework), GetBaseLibraryName (targetFramework));

			switch (targetFramework.Platform) {
			case ApplePlatform.iOS:
				return XamarinIOSDll;
			case ApplePlatform.TVOS:
				return XamarinTVOSDll;
			case ApplePlatform.WatchOS:
				return XamarinWatchOSDll;
			}

			if (targetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				return XamarinMacMobileDll;
			} else if (targetFramework == TargetFramework.Xamarin_Mac_4_5_Full) {
				return XamarinMacFullDll;
			}

			throw new InvalidOperationException (targetFramework.ToString ());
		}

		public static IList<string> GetRuntimeIdentifiers (ApplePlatform platform)
		{
			return GetVariableArray ($"DOTNET_{platform.AsString ().ToUpper ()}_RUNTIME_IDENTIFIERS");
		}

		public static IList<string> GetArchitectures (ApplePlatform platform)
		{
			var rv = new List<string> ();
			foreach (var rid in GetRuntimeIdentifiers (platform))
				rv.AddRange (GetArchitectures (rid));
			return rv;
		}

		public static IList<string> GetArchitectures (string runtimeIdentifier)
		{
			return GetVariableArray ($"DOTNET_{runtimeIdentifier}_ARCHITECTURES");
		}

		public static IEnumerable<string> GetBaseLibraryImplementations ()
		{
			foreach (var platform in GetIncludedPlatforms (true))
				foreach (var lib in GetBaseLibraryImplementations (platform))
					yield return lib;
		}

		public static IEnumerable<string> GetBaseLibraryImplementations (ApplePlatform platform)
		{
			var runtimeIdentifiers = GetRuntimeIdentifiers (platform);
			foreach (var rid in runtimeIdentifiers) {
				var libdir = Path.Combine (GetRuntimeDirectory (platform, rid), "lib", DotNetTfm);
				yield return Path.Combine (libdir, GetBaseLibraryName (platform, true));
			}
		}

		public static IEnumerable<string> GetRefLibraries ()
		{
			foreach (var platform in GetIncludedPlatforms (true))
				yield return Path.Combine (GetRefDirectory (platform), GetBaseLibraryName (platform, true));
		}


		public static string GetRefLibrary (ApplePlatform platform)
		{
			return GetBaseLibrary (platform, true);
		}

		public static string GetTargetFramework (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return "Xamarin.iOS,v1.0";
			case Profile.tvOS:
				return "Xamarin.TVOS,v1.0";
			case Profile.watchOS:
				return "Xamarin.WatchOS,v1.0";
			case Profile.macOSMobile:
				return "Xamarin.Mac,Version=v2.0,Profile=Mobile";
			case Profile.macOSFull:
				return "Xamarin.Mac,Version=v4.5,Profile=Full";
			case Profile.macOSSystem:
				return "Xamarin.Mac,Version=v4.5,Profile=System";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetSdkVersion (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return Configuration.sdk_version;
			case Profile.tvOS:
				return Configuration.tvos_sdk_version;
			case Profile.watchOS:
				return Configuration.watchos_sdk_version;
			case Profile.macOSFull:
			case Profile.macOSMobile:
			case Profile.macOSSystem:
				return Configuration.macos_sdk_version;
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetSdkPath (Profile profile, bool is_device)
		{
			switch (profile) {
			case Profile.iOS:
				return Path.Combine (MonoTouchRootDirectory, "SDKs", "MonoTouch." + (is_device ? "iphoneos" : "iphonesimulator") + ".sdk");
			case Profile.tvOS:
				return Path.Combine (MonoTouchRootDirectory, "SDKs", "Xamarin.AppleTV" + (is_device ? "OS" : "Simulator") + ".sdk");
			case Profile.watchOS:
				return Path.Combine (MonoTouchRootDirectory, "SDKs", "Xamarin.Watch" + (is_device ? "OS" : "Simulator") + ".sdk");
			case Profile.macOSFull:
			case Profile.macOSMobile:
			case Profile.macOSSystem:
				return Path.Combine (SdkRootXM, "lib");
			default:
				throw new NotImplementedException (profile.ToString ());
			}
		}

		public static string GetCompiler (Profile profile, IList<string> args)
		{
			args.Add ($"-lib:{Path.GetDirectoryName (GetBaseLibrary (profile))}");
			return "/Library/Frameworks/Mono.framework/Commands/csc";
		}

		public static void AssertiOS32BitAvailable ()
		{
			if (iOSSupports32BitArchitectures)
				return;
			Assert.Ignore ($"32-bit iOS support is not available in the current build.");
		}
#endif // !XAMMAC_TESTS

		public static IEnumerable<ApplePlatform> GetIncludedPlatforms (bool dotnet)
		{
			if (dotnet && !include_dotnet)
				yield break;

			if (include_ios)
				yield return ApplePlatform.iOS;
			if (include_tvos)
				yield return ApplePlatform.TVOS;
			if (include_mac)
				yield return ApplePlatform.MacOSX;
			if (include_maccatalyst)
				yield return ApplePlatform.MacCatalyst;
			if (include_watchos && !dotnet)
				yield return ApplePlatform.WatchOS;
		}

		public static IEnumerable<ApplePlatform> GetAllPlatforms (bool dotnet)
		{
			yield return ApplePlatform.iOS;
			yield return ApplePlatform.TVOS;
			yield return ApplePlatform.MacOSX;
			if (dotnet)
				yield return ApplePlatform.MacCatalyst;
			if (!dotnet)
				yield return ApplePlatform.WatchOS;
		}

		public static string NuGetPackagesDirectory {
			get {
				return Path.Combine (RootPath, "packages");
			}
		}

		public static string XIBuildPath {
			get { return Path.GetFullPath (Path.Combine (RootPath, "tools", "xibuild", "xibuild")); }
		}

		public static void AssertDeviceAvailable ()
		{
			if (include_device)
				return;
			Assert.Ignore ("This build does not include device support.");
		}

		public static void AssertDotNetAvailable ()
		{
			if (include_dotnet)
				return;
			Assert.Ignore (".NET tests not enabled");
		}

		public static void AssertLegacyXamarinAvailable ()
		{
			if (include_legacy_xamarin)
				return;
			Assert.Ignore ("Legacy xamarin build not enabled");
		}

		public static string CloneTestDirectory (string directory)
		{
			// Copy the test projects to a temporary directory so that we can run the tests from there without affecting the working directory.
			// Some tests may modify the test code / projects, and this way the working copy doesn't end up dirty.
			var testsTemporaryDirectory = Cache.CreateTemporaryDirectory ($"{Path.GetFileName (directory)}");

			// Only copy files in git, we want a clean copy
			var rv = ExecutionHelper.Execute ("git", new string [] { "ls-files" }, out var ls_files_output, working_directory: directory, timeout: TimeSpan.FromSeconds (15));
			if (rv != 0)
				throw new Exception ($"Failed to list test files. 'git ls-files' in {directory} failed with exit code {rv}.");

			var files = ls_files_output.ToString ().Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray ();
			foreach (var file in files) {
				var src = Path.Combine (directory, file);
				var tgt = Path.Combine (testsTemporaryDirectory, file);
				var tgtDir = Path.GetDirectoryName (tgt);
				Directory.CreateDirectory (tgtDir);
				File.Copy (src, tgt);
				if (tgt.EndsWith (".csproj", StringComparison.OrdinalIgnoreCase)) {
					var initialContents = File.ReadAllText (tgt);
					var fixedContents = initialContents.Replace ($"$(MSBuildThisFileDirectory)", Path.GetDirectoryName (src) + Path.DirectorySeparatorChar);
					if (initialContents != fixedContents)
						File.WriteAllText (tgt, fixedContents);
				}
			}

			return testsTemporaryDirectory;
		}

		// Replace one file with another
		// Example files:
		//    foo.csproj
		//    foo.mode.csproj
		// when called with mode="mode", will delete foo.csproj and move foo.mode.csproj to foo.csproj
		// Will also replace the string ".mode." in any replaced file with "."
		public static void FixupTestFiles (string directory, string mode)
		{
			var files = Directory.GetFiles (directory, "*", SearchOption.AllDirectories);
			var replace = "." + mode + ".";
			foreach (var file in files) {
				if (!file.Contains (replace))
					continue;
				var tgt = file.Replace (replace, ".");

				File.Delete (tgt);
				var contents = File.ReadAllText (file);
				contents = contents.Replace (replace, ".");
				File.WriteAllText (tgt, contents);
				File.Delete (file);
			}
		}

		public static Dictionary<string, string> GetBuildEnvironment (ApplePlatform platform)
		{
			Dictionary<string, string> environment = new Dictionary<string, string> ();
			SetBuildVariables (platform, ref environment);
			return environment;
		}

		public static void SetBuildVariables (ApplePlatform platform, ref Dictionary<string, string> environment)
		{
			var rootDirectory = GetTargetDirectory (platform);

			if (environment is null)
				environment = new Dictionary<string, string> ();

			environment ["MD_APPLE_SDK_ROOT"] = Path.GetDirectoryName (Path.GetDirectoryName (xcode_root));
			environment ["TargetFrameworkFallbackSearchPaths"] = Path.Combine (rootDirectory, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
			environment ["MSBuildExtensionsPathFallbackPathsOverride"] = Path.Combine (rootDirectory, "Library", "Frameworks", "Mono.framework", "External", "xbuild");

			// This is set by `dotnet test` and can cause building legacy projects to fail to build with:
			// Microsoft.NET.Build.Extensions.ConflictResolution.targets(30,5):
			// error MSB4062: The "ResolvePackageFileConflicts" task could not be loaded from the assembly Microsoft.NET.Build.Extensions.Tasks.dll.
			// Invalid Image Confirm that the <UsingTask> declaration is correct, that the assembly and all its dependencies are available,
			// and that the task contains a public class that implements Microsoft.Build.Framework.ITask.
			environment ["MSBuildExtensionsPath"] = null;

			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				environment ["MD_MTOUCH_SDK_ROOT"] = Path.Combine (rootDirectory, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
				break;
			case ApplePlatform.MacOSX:
				environment ["XAMMAC_FRAMEWORK_PATH"] = Path.Combine (rootDirectory, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				environment ["XamarinMacFrameworkRoot"] = Path.Combine (rootDirectory, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				break;
			default:
				throw new NotImplementedException (platform.ToString ());
			}
		}

		// Calls Assert.Ignore if the given platform isn't included in the current build.
		public static void IgnoreIfIgnoredPlatform (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				if (!include_ios)
					Assert.Ignore ("iOS is not included in this build");
				break;
			case ApplePlatform.TVOS:
				if (!include_tvos)
					Assert.Ignore ("tvOS is not included in this build");
				break;
			case ApplePlatform.WatchOS:
				if (!include_watchos)
					Assert.Ignore ("watchOS is not included in this build");
#if NET
				if (!include_dotnet_watchos)
					Assert.Ignore ("watchOS is not included in this build");
#endif

				break;
			case ApplePlatform.MacOSX:
				if (!include_mac)
					Assert.Ignore ("macOS is not included in this build");
				break;
			case ApplePlatform.MacCatalyst:
				if (!include_maccatalyst)
					Assert.Ignore ("Mac Catalyst is not included in this build");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		// Calls Assert.Ignore if the given platform isn't included in the current build.
		public static void IgnoreIfIgnoredPlatform (string platform)
		{
			switch (platform.ToLower ()) {
			case "ios":
			case "tvos":
			case "watchos":
			case "macosx":
			case "maccatalyst":
				IgnoreIfIgnoredPlatform ((ApplePlatform) Enum.Parse (typeof (ApplePlatform), platform, true));
				break;
			case "macos":
				IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		public static bool AnyIgnoredPlatforms (bool dotnet = true)
		{
			return AnyIgnoredPlatforms (dotnet, out var _);
		}

		public static bool AnyIgnoredPlatforms (bool dotnet, out IEnumerable<ApplePlatform> notIncluded)
		{
			var allPlatforms = GetAllPlatforms (dotnet);
			var includedPlatforms = GetIncludedPlatforms (dotnet);
			notIncluded = allPlatforms.Where (v => !includedPlatforms.Contains (v)).ToArray ();
			return notIncluded.Any ();
		}

		public static void IgnoreIfAnyIgnoredPlatforms (bool dotnet = true)
		{
			if (AnyIgnoredPlatforms (dotnet, out var notIncluded))
				Assert.Ignore ($"This test requires all platforms to be included, but the following platforms aren't included: {string.Join (", ", notIncluded.Select (v => v.AsString ()))}");
		}

		public static string GetTestLibraryDirectory (ApplePlatform platform, bool? simulator = null)
		{
			string dir;

			switch (platform) {
			case ApplePlatform.iOS:
				dir = simulator.Value ? "iphonesimulator" : "iphoneos";
				break;
			case ApplePlatform.MacOSX:
				dir = "macos";
				break;
			case ApplePlatform.WatchOS:
				dir = simulator.Value ? "watchsimulator" : "watchos";
				break;
			case ApplePlatform.TVOS:
				dir = simulator.Value ? "tvsimulator" : "tvos";
				break;
			case ApplePlatform.MacCatalyst:
				dir = "maccatalyst";
				break;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}

			return Path.Combine (SourceRoot, "tests", "test-libraries", ".libs", dir);
		}

		// This implementation of Touch is to update a timestamp (not to make sure a certain file exists).
		public static void Touch (string file)
		{
			if (!File.Exists (file))
				throw new FileNotFoundException ($"Can't touch the file '{file}' because it doesn't exist.");
			EnsureFilestampChange ();
			File.SetLastWriteTimeUtc (file, DateTime.UtcNow);
			EnsureFilestampChange ();
		}

		static bool? is_apfs;
		static bool IsAPFS {
			get {
				if (!is_apfs.HasValue) {
					var exit_code = ExecutionHelper.Execute ("/bin/df", new string [] { "-t", "apfs", "/" }, out var output, TimeSpan.FromSeconds (10));
					is_apfs = exit_code == 0 && output.Trim ().Split ('\n').Length >= 2;
				}
				return is_apfs.Value;
			}
		}

		// Some file systems have a rather low resolution for file timestamps, so make sure enough time passes that
		// touching a file will update the timestamp.
		static void EnsureFilestampChange ()
		{
			if (IsAPFS)
				return; // APFS has high resolution timestamps, so no need to wait at all.
			Thread.Sleep (1000);
		}

		// Return true if the current machine can run ARM64 binaries.
		static bool? canRunArm64;
		public static bool CanRunArm64 {
			get {
				if (!canRunArm64.HasValue) {
					int rv = 0;
					IntPtr size = (IntPtr) sizeof (int);
					if (sysctlbyname ("hw.optional.arm64", ref rv, ref size, IntPtr.Zero, IntPtr.Zero) == 0) {
						canRunArm64 = rv == 1;
					} else {
						canRunArm64 = false;
					}
				}
				return canRunArm64.Value;
			}
		}

		[DllImport ("libc")]
		static extern int sysctlbyname (string name, ref int value, ref IntPtr size, IntPtr zero, IntPtr zeroAgain);
	}
}
