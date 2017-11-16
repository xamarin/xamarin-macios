using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;

using Xamarin.Tests;
using Xamarin.Utils;

using NUnit.Framework;

namespace Xamarin
{
	public enum MTouchAction
	{
		None,
		BuildDev,
		BuildSim,
		LaunchSim,
	}

	public enum MTouchLinker
	{
		Unspecified,
		LinkAll,
		LinkSdk,
		DontLink,
	}

	public enum MTouchSymbolMode
	{
		Unspecified,
		Default,
		Linker,
		Code,
		Ignore,
	}

	public enum MTouchRegistrar
	{
		Unspecified,
		Dynamic,
		Static,
	}

	public enum MTouchBitcode
	{
		Unspecified,
		ASMOnly,
		Full, // LLVMOnly
		Marker,
	}

	[Flags]
	enum I18N
	{
		None = 0,

		CJK = 1,
		MidEast = 2,
		Other = 4,
		Rare = 8,
		West = 16,

		All = CJK | MidEast | Other | Rare | West,
		Base
	}

	class MTouchTool : Tool, IDisposable
	{
		public const string None = "None";

#pragma warning disable 649
		// These map directly to mtouch options
		public int Verbosity;
		public string SdkRoot;
		public bool? NoSign;
		public bool? Debug;
		public bool? FastDev;
		public bool? Dlsym;
		public string Sdk;
		public string TargetVer;
		public string [] References;
		public string Executable;
		public string RootAssembly;
		public string TargetFramework;
		public string Abi;
		public string AppPath;
		public string Cache;
		public string Device; // --device
		public MTouchLinker Linker;
		public MTouchSymbolMode SymbolMode;
		public bool? NoFastSim;
		public MTouchRegistrar Registrar;
		public I18N I18N;
		public bool? Extension;
		public List<MTouchTool> AppExtensions = new List<MTouchTool> ();
		public List<string> Frameworks = new List<string> ();
		public string HttpMessageHandler;
		public bool? PackageMdb;
		public bool? MSym;
		public bool? DSym;
		public bool? NoStrip;
		public string NoSymbolStrip;
		public string Mono;
		public string GccFlags;

		// These are a bit smarter
		public Profile Profile = Profile.iOS;
		public bool NoPlatformAssemblyReference;
		public List<string> AssemblyBuildTargets = new List<string> ();
		static XmlDocument device_list_cache;
		public string LLVMOptimizations;
		public string [] CustomArguments; // Sometimes you want to pass invalid arguments to mtouch, in this case this array is used. No processing will be done, if quotes are required, they must be added to the arguments in the array.
		public int [] NoWarn; // null array: no passed to mtouch. empty array: pass --nowarn (which means disable all warnings).
		public int [] WarnAsError; // null array: no passed to mtouch. empty array: pass --warnaserror (which means makes all warnings errors).
		public MTouchBitcode Bitcode;
		public string AotArguments;
		public string AotOtherArguments;
		public string [] LinkSkip;
		public string [] XmlDefinitions;
		public bool? Profiling;
		public string SymbolList;
		public string ResponseFile;

#pragma warning restore 649

		public class DeviceInfo
		{
			public string UDID;
			public string Name;
			public string CompanionIdentifier;
			public string DeviceClass;

			public DeviceInfo Companion;
		}

		string GetVerbosity ()
		{
			if (Verbosity == 0) {
				return string.Empty;
			} else if (Verbosity > 0) {
				return new string ('-', Verbosity).Replace ("-", "-v ");
			} else {
				return new string ('-', -Verbosity).Replace ("-", "-q ");
			}
		}

		public int LaunchOnDevice (DeviceInfo device, string appPath, bool waitForUnlock, bool waitForExit)
		{
			return Execute ("--devname \"{0}\" --launchdev \"{1}\" --sdkroot \"{2}\" --wait-for-unlock:{3} --wait-for-exit:{4} {5}", device.Name, appPath, Configuration.xcode_root, waitForUnlock ? "yes" : "no", waitForExit ? "yes" : "no", GetVerbosity ());
		}

		public int InstallOnDevice (DeviceInfo device, string appPath, string devicetype = null)
		{
			return Execute ("--devname \"{0}\" --installdev \"{1}\" --sdkroot \"{2}\" {3} {4}", device.Name, appPath, Configuration.xcode_root, GetVerbosity (), devicetype == null ? string.Empty : "--device " + devicetype);
		}

		public int Execute (MTouchAction action)
		{
			return Execute (BuildArguments (action), always_show_output: Verbosity > 0);
		}

		public void AssertExecute (MTouchAction action, string message = null)
		{
			var rv = Execute (action);
			if (rv == 0)
				return;
			var errors = Messages.Where ((v) => v.IsError).ToList ();
			Assert.Fail ($"Expected execution to succeed, but exit code was {rv}, and there were {errors.Count} error(s): {message}\n\t" +
			             string.Join ("\n\t", errors.Select ((v) => v.ToString ())));
		}

		public void AssertExecuteFailure (MTouchAction action, string message = null)
		{
			NUnit.Framework.Assert.AreEqual (1, Execute (action), message);
		}

		// Assert that none of the files in the app has changed (except 'except' files)
		public void AssertNoneModified (DateTime timestamp, string message, params string [] except)
		{
			var failed = new List<string> ();
			var files = Directory.EnumerateFiles (AppPath, "*", SearchOption.AllDirectories);
			foreach (var file in files) {
				var info = new FileInfo (file);
				if (info.LastWriteTime > timestamp) {
					if (except != null && except.Contains (Path.GetFileName (file))) {
						Console.WriteLine ("SKIP: {0} modified: {1} > {2}", file, info.LastWriteTime, timestamp);
					} else {
						failed.Add (string.Format ("{0} is modified, timestamp: {1} > {2}", file, info.LastWriteTime, timestamp));
						Console.WriteLine ("FAIL: {0} modified: {1} > {2}", file, info.LastWriteTime, timestamp);
					}
				} else {
					Console.WriteLine ("{0} not modified ted: {1} <= {2}", file, info.LastWriteTime, timestamp);
				}
			}
			Assert.IsEmpty (failed, message);
		}

		// Assert that all of the files in the app has changed (except 'except' files)
		public void AssertAllModified (DateTime timestamp, string message, params string [] except)
		{
			var failed = new List<string> ();
			var files = Directory.EnumerateFiles (AppPath, "*", SearchOption.AllDirectories);
			foreach (var file in files) {
				var info = new FileInfo (file);
				if (info.LastWriteTime <= timestamp) {
					if (except != null && except.Contains (Path.GetFileName (file))) {
						Console.WriteLine ("SKIP: {0} not modified: {1} <= {2}", file, info.LastWriteTime, timestamp);
					} else {
						failed.Add (string.Format ("{0} is not modified, timestamp: {1} <= {2}", file, info.LastWriteTime, timestamp));
						Console.WriteLine ("FAIL: {0} not modified: {1} <= {2}", file, info.LastWriteTime, timestamp);
					}
				} else {
					Console.WriteLine ("{0} modified (as expected): {1} > {2}", file, info.LastWriteTime, timestamp);
				}
			}
			Assert.IsEmpty (failed, message);
		}

		// Asserts that the given files were modified.
		public void AssertModified (DateTime timestamp, string message, params string [] files)
		{
			Assert.IsNotEmpty (files);

			var failed = new List<string> ();
			var fs = Directory.EnumerateFiles (AppPath, "*", SearchOption.AllDirectories);
			foreach (var file in fs) {
				if (!files.Contains (Path.GetFileName (file)))
					continue;
				var info = new FileInfo (file);
				if (info.LastWriteTime < timestamp) {
					failed.Add (string.Format ("{0} is not modified, timestamp: {1} < {2}", file, info.LastWriteTime, timestamp));
					Console.WriteLine ("FAIL: {0} not modified: {1} < {2}", file, info.LastWriteTime, timestamp);
				} else {
					Console.WriteLine ("{0} modified (as expected): {1} >= {2}", file, info.LastWriteTime, timestamp);
				}
			}
			Assert.IsEmpty (failed, message);
		}

		string BuildArguments (MTouchAction action)
		{
			var sb = new StringBuilder ();
			var isDevice = false;

			switch (action) {
			case MTouchAction.None:
				break;
			case MTouchAction.BuildDev:
				MTouch.AssertDeviceAvailable ();
				if (AppPath == null)
					throw new Exception ("No AppPath specified.");
				isDevice = true;
				sb.Append (" --dev ").Append (StringUtils.Quote (AppPath));
				break;
			case MTouchAction.BuildSim:
				isDevice = false;
				if (AppPath == null)
					throw new Exception ("No AppPath specified.");
				sb.Append (" --sim ").Append (StringUtils.Quote (AppPath));
				break;
			case MTouchAction.LaunchSim:
				isDevice = false;
				if (AppPath == null)
					throw new Exception ("No AppPath specified.");
				sb.Append (" --launchsim ").Append (StringUtils.Quote (AppPath));
				break;
			default:
				throw new NotImplementedException ();
			}

			if (SdkRoot == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (SdkRoot)) {
				sb.Append (" --sdkroot ").Append (StringUtils.Quote (SdkRoot));
			} else {
				sb.Append (" --sdkroot ").Append (StringUtils.Quote (Configuration.xcode_root));
			}

			sb.Append (" ").Append (GetVerbosity ());

			if (Sdk == None) {
				// do nothing	
			} else if (!string.IsNullOrEmpty (Sdk)) {
				sb.Append (" --sdk ").Append (Sdk);
			} else {
				sb.Append (" --sdk ").Append (MTouch.GetSdkVersion (Profile));
			}

			if (TargetVer == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (TargetVer)) {
				sb.Append (" --targetver ").Append (TargetVer);
			}

			if (Debug.HasValue && Debug.Value)
				sb.Append (" --debug");

			if (FastDev.HasValue && FastDev.Value)
				sb.Append (" --fastdev");

			if (PackageMdb.HasValue)
				sb.Append (" --package-mdb:").Append (PackageMdb.Value ? "true" : "false");

			if (NoStrip.HasValue && NoStrip.Value)
				sb.Append (" --nostrip");

			if (NoSymbolStrip != null) {
				if (NoSymbolStrip.Length == 0) {
					sb.Append (" --nosymbolstrip");
				} else {
					sb.Append (" --nosymbolstrip:").Append (NoSymbolStrip);
				}
			}

			if (Profiling.HasValue)
				sb.Append (" --profiling:").Append (Profiling.Value ? "true" : "false");

			if (!string.IsNullOrEmpty (SymbolList))
				sb.Append (" --symbollist=").Append (StringUtils.Quote (SymbolList));

			if (MSym.HasValue)
				sb.Append (" --msym:").Append (MSym.Value ? "true" : "false");

			if (DSym.HasValue)
				sb.Append (" --dsym:").Append (DSym.Value ? "true" : "false");

			if (Extension == true)
				sb.Append (" --extension");

			foreach (var appext in AppExtensions)
				sb.Append (" --app-extension ").Append (StringUtils.Quote (appext.AppPath));

			foreach (var framework in Frameworks)
				sb.Append (" --framework ").Append (StringUtils.Quote (framework));

			if (!string.IsNullOrEmpty (Mono))
				sb.Append (" --mono:").Append (StringUtils.Quote (Mono));

			if (!string.IsNullOrEmpty (GccFlags))
				sb.Append (" --gcc_flags ").Append (StringUtils.Quote (GccFlags));

			if (!string.IsNullOrEmpty (HttpMessageHandler))
				sb.Append (" --http-message-handler=").Append (StringUtils.Quote (HttpMessageHandler));

			if (Dlsym.HasValue)
				sb.Append (" --dlsym:").Append (Dlsym.Value ? "true" : "false");

			if (References != null) {
				foreach (var r in References)
					sb.Append (" -r:").Append (StringUtils.Quote (r));
			}

			if (!string.IsNullOrEmpty (Executable))
				sb.Append (" --executable ").Append (StringUtils.Quote (Executable));

			if (!string.IsNullOrEmpty (RootAssembly))
				sb.Append (" ").Append (StringUtils.Quote (RootAssembly));

			if (TargetFramework == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (TargetFramework)) {
				sb.Append (" --target-framework ").Append (TargetFramework);
			} else if (!NoPlatformAssemblyReference) {
				// make the implicit default the way tests have been running until now, and at the same time the very minimum to make apps build.
				switch (Profile) {
				case Profile.iOS:
					sb.Append (" -r:").Append (StringUtils.Quote (Configuration.XamarinIOSDll));
					break;
				case Profile.tvOS:
				case Profile.watchOS:
					sb.Append (" --target-framework ").Append (MTouch.GetTargetFramework (Profile));
					sb.Append (" -r:").Append (StringUtils.Quote (MTouch.GetBaseLibrary (Profile)));
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			if (Abi == None) {
				// add nothing
			} else if (!string.IsNullOrEmpty (Abi)) {
				sb.Append (" --abi ").Append (Abi);
			} else {
				switch (Profile) {
				case Profile.iOS:
					break; // not required
				case Profile.tvOS:
					sb.Append (isDevice ? " --abi arm64" : " --abi x86_64");
					break;
				case Profile.watchOS:
					sb.Append (isDevice ? " --abi armv7k" : " --abi i386");
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			switch (Linker) {
			case MTouchLinker.LinkAll:
			case MTouchLinker.Unspecified:
				break;
			case MTouchLinker.DontLink:
				sb.Append (" --nolink");
				break;
			case MTouchLinker.LinkSdk:
				sb.Append (" --linksdkonly");
				break;
			default:
				throw new NotImplementedException ();
			}

			switch (SymbolMode) {
			case MTouchSymbolMode.Ignore:
				sb.Append (" --dynamic-symbol-mode=ignore");
				break;
			case MTouchSymbolMode.Code:
				sb.Append (" --dynamic-symbol-mode=code");
				break;
			case MTouchSymbolMode.Default:
				sb.Append (" --dynamic-symbol-mode=default");
				break;
			case MTouchSymbolMode.Linker:
				sb.Append (" --dynamic-symbol-mode=linker");
				break;
			case MTouchSymbolMode.Unspecified:
				break;
			default:
				throw new NotImplementedException ();
			}

			if (NoFastSim.HasValue && NoFastSim.Value)
				sb.Append (" --nofastsim");

			switch (Registrar) {
			case MTouchRegistrar.Unspecified:
				break;
			case MTouchRegistrar.Dynamic:
				sb.Append (" --registrar:dynamic");
				break;
			case MTouchRegistrar.Static:
				sb.Append (" --registrar:static");
				break;
			default:
				throw new NotImplementedException ();
			}

			if (I18N != I18N.None) {
				sb.Append (" --i18n ");
				int count = 0;
				if ((I18N & I18N.CJK) == I18N.CJK)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("cjk");
				if ((I18N & I18N.MidEast) == I18N.MidEast)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("mideast");
				if ((I18N & I18N.Other) == I18N.Other)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("other");
				if ((I18N & I18N.Rare) == I18N.Rare)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("rare");
				if ((I18N & I18N.West) == I18N.West)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("west");
			}

			if (!string.IsNullOrEmpty (Cache))
				sb.Append (" --cache ").Append (StringUtils.Quote (Cache));

			if (!string.IsNullOrEmpty (Device))
				sb.Append (" --device:").Append (StringUtils.Quote (Device));

			if (!string.IsNullOrEmpty (LLVMOptimizations))
				sb.Append (" --llvm-opt=").Append (StringUtils.Quote (LLVMOptimizations));

			if (CustomArguments != null) {
				foreach (var arg in CustomArguments) {
					sb.Append (" ").Append (arg);
				}
			}

			if (NoWarn != null) {
				if (NoWarn.Length > 0) {
					sb.Append (" --nowarn:");
					foreach (var code in NoWarn)
						sb.Append (code).Append (',');
					sb.Length--;
				} else {
					sb.Append (" --nowarn");
				}
			}

			if (WarnAsError != null) {
				if (WarnAsError.Length > 0) {
					sb.Append (" --warnaserror:");
					foreach (var code in WarnAsError)
						sb.Append (code).Append (',');
					sb.Length--;
				} else {
					sb.Append (" --warnaserror");
				}
			}

			if (Bitcode != MTouchBitcode.Unspecified)
				sb.Append (" --bitcode:").Append (Bitcode.ToString ().ToLower ());

			foreach (var abt in AssemblyBuildTargets)
				sb.Append (" --assembly-build-target=").Append (StringUtils.Quote (abt));

			if (!string.IsNullOrEmpty (AotArguments))
				sb.Append (" --aot:").Append (StringUtils.Quote (AotArguments));

			if (!string.IsNullOrEmpty (AotOtherArguments))
				sb.Append (" --aot-options:").Append (StringUtils.Quote (AotOtherArguments));

			if (LinkSkip?.Length > 0) {
				foreach (var ls in LinkSkip)
					sb.Append (" --linkskip:").Append (StringUtils.Quote (ls));
			}

			if (XmlDefinitions?.Length > 0) {
				foreach (var xd in XmlDefinitions)
					sb.Append (" --xml:").Append (StringUtils.Quote (xd));
			}

			if (!string.IsNullOrEmpty (ResponseFile))
				sb.Append (" @").Append (StringUtils.Quote (ResponseFile));

			return sb.ToString ();
		}

		XmlDocument FetchDeviceList (bool allowCache = true)
		{
			if (device_list_cache == null || !allowCache) {
				var output_file = Path.GetTempFileName ();
				try {
					if (Execute ("--listdev:{1} --sdkroot {0} --output-format xml", Configuration.xcode_root, output_file) != 0)
						throw new Exception ("Failed to list devices.");
					device_list_cache = new XmlDocument ();
					device_list_cache.Load (output_file);
				} finally {
					File.Delete (output_file);
				}
			}
			return device_list_cache;
		}

		public List<DeviceInfo> ListDevices ()
		{
			var rv = new List<DeviceInfo> ();

			foreach (XmlNode node in FetchDeviceList ().SelectNodes ("//MTouch/Device")) {
				rv.Add (new DeviceInfo () {
					UDID = node.SelectSingleNode ("UDID")?.InnerText,
					Name = node.SelectSingleNode ("Name")?.InnerText,
					CompanionIdentifier = node.SelectSingleNode ("CompanionIdentifier")?.InnerText,
					DeviceClass = node.SelectSingleNode ("DeviceClass")?.InnerText,
				});
			}

			foreach (var device in rv) {
				if (string.IsNullOrEmpty (device.CompanionIdentifier))
					continue;

				device.Companion = rv.FirstOrDefault ((d) => d.UDID == device.CompanionIdentifier);
			}

			return rv;
		}

		public IEnumerable<DeviceInfo> FindAvailableDevices (string [] deviceClasses)
		{
			return ListDevices ().Where ((info) => deviceClasses.Contains (info.DeviceClass));
		}

		public string NativeExecutablePath {
			get {
				return Path.Combine (AppPath, Path.GetFileNameWithoutExtension (RootAssembly));
			}
		}

		public static string CreatePlist (Profile profile, string appName)
		{
			string plist = null;

			switch (profile) {
			case Profile.iOS:
				plist = string.Format (@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>CFBundleDisplayName</key>
	<string>{0}</string>
	<key>CFBundleIdentifier</key>
	<string>com.xamarin.{0}</string>
	<key>CFBundleExecutable</key>
	<string>{0}</string>
	<key>MinimumOSVersion</key>
	<string>{1}</string>
	<key>UIDeviceFamily</key>
	<array>
		<integer>1</integer>
		<integer>2</integer>
	</array>
	<key>UISupportedInterfaceOrientations</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationPortraitUpsideDown</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
</dict>
</plist>
", appName, MTouch.GetSdkVersion (profile));
				break;
			case Profile.tvOS:
				plist = string.Format (@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>CFBundleDisplayName</key>
	<string>Extensiontest</string>
	<key>CFBundleIdentifier</key>
	<string>com.xamarin.{0}</string>
	<key>CFBundleExecutable</key>
	<string>{0}</string>
	<key>MinimumOSVersion</key>
	<string>{1}</string>
	<key>UIDeviceFamily</key>
	<array>
		<integer>3</integer>
	</array>
</dict>
</plist>
", appName, MTouch.GetSdkVersion (profile));
				break;
			default:
				throw new Exception ("Profile not specified.");
			}

			return plist;
		}

		public string CreateTemporarySatelliteAssembly (string culture = "en-AU")
		{
			var asm_dir = Path.Combine (Path.GetDirectoryName (RootAssembly), culture);
			Directory.CreateDirectory (asm_dir);

			var asm_name = Path.GetFileNameWithoutExtension (RootAssembly) + ".resources.dll";
			// Cheat a bit, by compiling a normal assembly with code instead of creating a resource assembly
			return MTouch.CompileTestAppLibrary (asm_dir, "class X {}", appName: Path.GetFileNameWithoutExtension (asm_name));
		}

		public void CreateTemporaryApp (bool hasPlist = false, string appName = "testApp", string code = null, string extraArg = "", string extraCode = null, string usings = null)
		{
			string testDir;
			if (RootAssembly == null) {
				testDir = CreateTemporaryDirectory ();
			} else {
				// We're rebuilding an existing executable, so just reuse that
				testDir = Path.GetDirectoryName (RootAssembly);
			}
			var app = AppPath ?? Path.Combine (testDir, appName + ".app");
			Directory.CreateDirectory (app);
			AppPath = app;
			RootAssembly = MTouch.CompileTestAppExecutable (testDir, code, extraArg, Profile, appName, extraCode, usings);

			if (hasPlist)
				File.WriteAllText (Path.Combine (app, "Info.plist"), CreatePlist (Profile, appName));
		}

		public void CreateTemporaryServiceExtension (string code = null, string extraCode = null, string extraArg = null, string appName = "testServiceExtension")
		{
			string testDir;
			if (RootAssembly == null) {
				testDir = CreateTemporaryDirectory ();
			} else {
				// We're rebuilding an existing executable, so just reuse that
				testDir = Path.GetDirectoryName (RootAssembly);
			}
			var app = AppPath ?? Path.Combine (testDir, $"{appName}.appex");
			Directory.CreateDirectory (app);

			if (code == null) {
				code = @"using UserNotifications;
[Foundation.Register (""NotificationService"")]
public partial class NotificationService : UNNotificationServiceExtension
{
	protected NotificationService (System.IntPtr handle) : base (handle) {}
}";
			}
			if (extraCode != null)
				code += extraCode;

			AppPath = app;
			Extension = true;
			RootAssembly = MTouch.CompileTestAppLibrary (testDir, code: code, profile: Profile, extraArg: extraArg, appName: appName);

			var info_plist = 
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>CFBundleDisplayName</key>
	<string>serviceextension</string>
	<key>CFBundleName</key>
	<string>serviceextension</string>
	<key>CFBundleIdentifier</key>
	<string>com.xamarin.testapp.serviceextension</string>
	<key>CFBundleDevelopmentRegion</key>
	<string>en</string>
	<key>CFBundleInfoDictionaryVersion</key>
	<string>6.0</string>
	<key>CFBundlePackageType</key>
	<string>XPC!</string>
	<key>CFBundleShortVersionString</key>
	<string>1.0</string>
	<key>CFBundleVersion</key>
	<string>1.0</string>
	<key>MinimumOSVersion</key>
	<string>10.0</string>
	<key>NSExtension</key>
	<dict>
		<key>NSExtensionPointIdentifier</key>
		<string>com.apple.usernotifications.service</string>
		<key>NSExtensionPrincipalClass</key>
		<string>NotificationService</string>
	</dict>
</dict>
</plist>
";
			var plist_path = Path.Combine (app, "Info.plist");
			if (!File.Exists (plist_path) || File.ReadAllText (plist_path) != info_plist)
				File.WriteAllText (plist_path, info_plist);
		}

		public void CreateTemporaryTodayExtension (string code = null, string extraCode = null, string extraArg = null, string appName = "testTodayExtension")
		{
			string testDir;
			if (RootAssembly == null) {
				testDir = CreateTemporaryDirectory ();
			} else {
				// We're rebuilding an existing executable, so just reuse that
				testDir = Path.GetDirectoryName (RootAssembly);
			}
			var app = AppPath ?? Path.Combine (testDir, $"{appName}.appex");
			Directory.CreateDirectory (app);

			if (code == null) {
				code = @"using System;
using Foundation;
using NotificationCenter;
using UIKit;

public partial class TodayViewController : UIViewController, INCWidgetProviding
{
	public TodayViewController (IntPtr handle) : base (handle)
	{
	}

	[Export (""widgetPerformUpdateWithCompletionHandler:"")]
	public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
	{
		completionHandler (NCUpdateResult.NewData);
	}
}
";
			}
			if (extraCode != null)
				code += extraCode;

			AppPath = app;
			Extension = true;
			RootAssembly = MTouch.CompileTestAppLibrary (testDir, code: code, profile: Profile, extraArg: extraArg, appName: appName);

			var info_plist = // FIXME: this includes a NSExtensionMainStoryboard key which points to a non-existent storyboard. This won't matter as long as we're only building, and not running the extension.
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>CFBundleDisplayName</key>
	<string>todayextension</string>
	<key>CFBundleName</key>
	<string>todayextension</string>
	<key>CFBundleIdentifier</key>
	<string>com.xamarin.testapp.todayextension</string>
	<key>CFBundleDevelopmentRegion</key>
	<string>en</string>
	<key>CFBundleInfoDictionaryVersion</key>
	<string>6.0</string>
	<key>CFBundlePackageType</key>
	<string>XPC!</string>
	<key>CFBundleShortVersionString</key>
	<string>1.0</string>
	<key>CFBundleVersion</key>
	<string>1.0</string>
	<key>MinimumOSVersion</key>
	<string>10.0</string>
	<key>NSExtension</key>
	<dict>
		<key>NSExtensionPointIdentifier</key>
		<string>widget-extension</string>
		<key>NSExtensionMainStoryboard</key>
		<string>MainInterface</string>
	</dict>
</dict>
</plist>
";
			var plist_path = Path.Combine (app, "Info.plist");
			if (!File.Exists (plist_path) || File.ReadAllText (plist_path) != info_plist)
				File.WriteAllText (plist_path, info_plist);
		}

		public void CreateTemporaryWatchKitExtension (string code = null)
		{
			string testDir;
			if (RootAssembly == null) {
				testDir = CreateTemporaryDirectory ();
			} else {
				// We're rebuilding an existing executable, so just reuse that directory
				testDir = Path.GetDirectoryName (RootAssembly);
			}
			var app = AppPath ?? Path.Combine (testDir, "testApp.appex");
			Directory.CreateDirectory (app);

			if (code == null) {
				code = @"using WatchKit;
public partial class NotificationController : WKUserNotificationInterfaceController
{
	protected NotificationController (System.IntPtr handle) : base (handle) {}
}";
			}

			AppPath = app;
			Extension = true;
			RootAssembly = MTouch.CompileTestAppLibrary (testDir, code: code, profile: Profile);

			File.WriteAllText (Path.Combine (app, "Info.plist"), @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>CFBundleDisplayName</key>
	<string>testapp</string>
	<key>CFBundleName</key>
	<string>testapp</string>
	<key>CFBundleIdentifier</key>
	<string>com.xamarin.testapp</string>
	<key>CFBundleDevelopmentRegion</key>
	<string>en</string>
	<key>CFBundleVersion</key>
	<string>1.0</string>
	<key>MinimumOSVersion</key>
	<string>2.0</string>
	<key>NSExtension</key>
	<dict>
		<key>NSExtensionAttributes</key>
		<dict>
			<key>WKAppBundleIdentifier</key>
			<string>com.xamarin.testapp.watchkitapp</string>
		</dict>
		<key>NSExtensionPointIdentifier</key>
		<string>com.apple.watchkit</string>
	</dict>
	<key>RemoteInterfacePrincipleClass</key>
	<string>InterfaceController</string>
	<key>CFBundleShortVersionString</key>
	<string>1.0</string>
</dict>
</plist>
");
		}

		public string CreateTemporaryDirectory ()
		{
			return Xamarin.Cache.CreateTemporaryDirectory ();
		}

		public void CreateTemporaryApp_LinkWith ()
		{
			AppPath = CreateTemporaryAppDirectory ();
			RootAssembly = MTouch.CompileTestAppExecutableLinkWith (Path.GetDirectoryName (AppPath), profile: Profile);
		}

		public string CreateTemporaryAppDirectory ()
		{
			if (AppPath != null)
				throw new Exception ("There already is an App directory");

			AppPath = Path.Combine (CreateTemporaryDirectory (), "testApp.app");
			Directory.CreateDirectory (AppPath);

			return AppPath;
		}

		public void CreateTemporaryCacheDirectory ()
		{
			Cache = Path.Combine (CreateTemporaryDirectory (), "mtouch-test-cache");
			Directory.CreateDirectory (Cache);
		}

		void IDisposable.Dispose ()
		{
		}

		public IEnumerable<string> NativeSymbolsInExecutable {
			get { 
				return ExecutionHelper.Execute ("nm", $"-gUj {StringUtils.Quote (NativeExecutablePath)}", hide_output: true).Split ('\n');
			}
		}

		protected override string ToolPath {
			get { return Configuration.MtouchPath; }
		}

		protected override string MessagePrefix {
			get { return "MT"; }
		}
	}
}
