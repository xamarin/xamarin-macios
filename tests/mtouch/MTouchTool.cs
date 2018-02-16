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

	public enum MTouchSymbolMode
	{
		Unspecified,
		Default,
		Linker,
		Code,
		Ignore,
	}

	public enum MTouchBitcode
	{
		Unspecified,
		ASMOnly,
		Full, // LLVMOnly
		Marker,
	}

	class MTouchTool : BundlerTool, IDisposable
	{
#pragma warning disable 649
		// These map directly to mtouch options
		public MTouchAction? Action; // --sim, --dev, --launchsim, etc
		public bool? NoSign;
		public bool? FastDev;
		public bool? Dlsym;
		public string Executable;
		public string AppPath;
		public string Device; // --device
		public MTouchSymbolMode SymbolMode;
		public bool? NoFastSim;
		public List<MTouchTool> AppExtensions = new List<MTouchTool> ();
		public List<string> Frameworks = new List<string> ();
		public bool? PackageMdb;
		public bool? MSym;
		public bool? DSym;
		public bool? NoStrip;
		public string NoSymbolStrip;
		public string Mono;

		// These are a bit smarter
		public List<string> AssemblyBuildTargets = new List<string> ();
		static XmlDocument device_list_cache;
		public string LLVMOptimizations;
		public MTouchBitcode Bitcode;
		public string AotArguments;
		public string AotOtherArguments;
		public string SymbolList;

#pragma warning restore 649

		public MTouchTool ()
		{
			Profile = Profile.iOS;
		}

		public class DeviceInfo
		{
			public string UDID;
			public string Name;
			public string CompanionIdentifier;
			public string DeviceClass;

			public DeviceInfo Companion;
		}

		public int LaunchOnDevice (DeviceInfo device, string appPath, bool waitForUnlock, bool waitForExit)
		{
			return Execute ("--devname \"{0}\" --launchdev \"{1}\" --sdkroot \"{2}\" --wait-for-unlock:{3} --wait-for-exit:{4} {5}", device.Name, appPath, Configuration.xcode_root, waitForUnlock ? "yes" : "no", waitForExit ? "yes" : "no", GetVerbosity ());
		}

		public int InstallOnDevice (DeviceInfo device, string appPath, string devicetype = null)
		{
			return Execute ("--devname \"{0}\" --installdev \"{1}\" --sdkroot \"{2}\" {3} {4}", device.Name, appPath, Configuration.xcode_root, GetVerbosity (), devicetype == null ? string.Empty : "--device " + devicetype);
		}

		// The default is to build for the simulator
		public override int Execute ()
		{
			if (!Action.HasValue)
				Action = MTouchAction.BuildSim;
			return base.Execute ();
		}

		public int Execute (MTouchAction action)
		{
			this.Action = action;
			return base.Execute ();
		}

		// The default is to build for the simulator
		public override void AssertExecute (string message = null)
		{
			if (!Action.HasValue)
				Action = MTouchAction.BuildSim;
			base.AssertExecute (message);
		}

		public void AssertExecute (MTouchAction action, string message = null)
		{
			this.Action = action;
			base.AssertExecute (message);
		}

		public void AssertExecuteFailure (MTouchAction action, string message = null)
		{
			Action = action;
			NUnit.Framework.Assert.AreEqual (1, Execute (), message);
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

		protected override string GetDefaultAbi()
		{
			var isDevice = false;

			switch (Action.Value) {
			case MTouchAction.None:
				break;
			case MTouchAction.BuildDev:
				isDevice = true;
				break;
			case MTouchAction.BuildSim:
				isDevice = false;
				break;
			case MTouchAction.LaunchSim:
				isDevice = false;
				break;
			default:
				throw new NotImplementedException ();
			}

			switch (Profile) {
			case Profile.iOS:
				return null; // not required
			case Profile.tvOS:
				return isDevice ? "arm64" : "x86_64";
			case Profile.watchOS:
				return isDevice ? "armv7k" : "i386";
			default:
				throw new NotImplementedException ();
			}
		}

		protected override void BuildArguments (StringBuilder sb)
		{
			base.BuildArguments (sb);

			switch (Action.Value) {
			case MTouchAction.None:
				break;
			case MTouchAction.BuildDev:
				MTouch.AssertDeviceAvailable ();
				if (AppPath == null)
					throw new Exception ("No AppPath specified.");
				sb.Append (" --dev ").Append (StringUtils.Quote (AppPath));
				break;
			case MTouchAction.BuildSim:
				if (AppPath == null)
					throw new Exception ("No AppPath specified.");
				sb.Append (" --sim ").Append (StringUtils.Quote (AppPath));
				break;
			case MTouchAction.LaunchSim:
				if (AppPath == null)
					throw new Exception ("No AppPath specified.");
				sb.Append (" --launchsim ").Append (StringUtils.Quote (AppPath));
				break;
			default:
				throw new NotImplementedException ();
			}

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

			if (!string.IsNullOrEmpty (SymbolList))
				sb.Append (" --symbollist=").Append (StringUtils.Quote (SymbolList));

			if (MSym.HasValue)
				sb.Append (" --msym:").Append (MSym.Value ? "true" : "false");

			if (DSym.HasValue)
				sb.Append (" --dsym:").Append (DSym.Value ? "true" : "false");

			foreach (var appext in AppExtensions)
				sb.Append (" --app-extension ").Append (StringUtils.Quote (appext.AppPath));

			foreach (var framework in Frameworks)
				sb.Append (" --framework ").Append (StringUtils.Quote (framework));

			if (!string.IsNullOrEmpty (Mono))
				sb.Append (" --mono:").Append (StringUtils.Quote (Mono));
			
			if (Dlsym.HasValue)
				sb.Append (" --dlsym:").Append (Dlsym.Value ? "true" : "false");
			
			if (!string.IsNullOrEmpty (Executable))
				sb.Append (" --executable ").Append (StringUtils.Quote (Executable));

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

			if (!string.IsNullOrEmpty (Device))
				sb.Append (" --device:").Append (StringUtils.Quote (Device));

			if (!string.IsNullOrEmpty (LLVMOptimizations))
				sb.Append (" --llvm-opt=").Append (StringUtils.Quote (LLVMOptimizations));
			
			if (Bitcode != MTouchBitcode.Unspecified)
				sb.Append (" --bitcode:").Append (Bitcode.ToString ().ToLower ());

			foreach (var abt in AssemblyBuildTargets)
				sb.Append (" --assembly-build-target=").Append (StringUtils.Quote (abt));

			if (!string.IsNullOrEmpty (AotArguments))
				sb.Append (" --aot:").Append (StringUtils.Quote (AotArguments));

			if (!string.IsNullOrEmpty (AotOtherArguments))
				sb.Append (" --aot-options:").Append (StringUtils.Quote (AotOtherArguments));
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

		public override void CreateTemporaryApp (Profile profile, string appName = "testApp", string code = null, string extraArg = "", string extraCode = null, string usings = null, bool use_csc = false)
		{
			Profile = profile;
			CreateTemporaryApp (appName: appName, code: code, extraArg: extraArg, extraCode: extraCode, usings: usings, use_csc: use_csc);
		}

		public void CreateTemporaryApp (bool hasPlist = false, string appName = "testApp", string code = null, string extraArg = "", string extraCode = null, string usings = null, bool use_csc = false)
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
			RootAssembly = CompileTestAppExecutable (testDir, code, extraArg, Profile, appName, extraCode, usings, use_csc);

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

		public override string GetAppAssembliesDirectory ()
		{
			return AppPath;
		}
	}
}
