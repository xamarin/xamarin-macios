using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks.Tests {

	[TestFixture]
	public class ResolveNativeReferencesTaskTest : TestBase {

		TaskLoggingHelper log = new TaskLoggingHelper (new TestEngine (), "ResolveNativeReferences");

		[TestCase ("XTest.framework", false)]
		[TestCase ("XTest.Framework", true)]
		[TestCase ("XTest.FRAMEWORK", true)]
		[TestCase ("XTest.fRaMeWoRk", true)]
		[TestCase ("XTest.framework/XTest", false)]
		[TestCase ("XTest.Framework/XTest", true)]
		[TestCase ("XTest.FRAMEWORK/XTest", true)]
		[TestCase ("XTest.Framework/.", true)]
		[TestCase ("Uppercase.Framework.Name.framework", false)]
		[TestCase ("libtest.a", false)]
		[TestCase ("libtest.dylib", false)]
		public void FrameworkExtension (string reference, bool expectWarning)
		{
			var task = CreateTask<ResolveNativeReferences> ();
			task.TargetFrameworkMoniker = TargetFramework.DotNet_macOS_String;
			task.FrameworksDirectory = "Frameworks";
			task.NativeReferences = new ITaskItem [] { new TaskItem (reference) };

			ExecuteTask (task);

			var warnings = Engine.Logger.WarningsEvents.ToArray ();
			Assert.That (warnings.Length, Is.EqualTo (expectWarning ? 1 : 0), "Warning count");
			if (expectWarning) {
				Assert.That (warnings [0].Message, Does.Contain ("lowercase '.framework'"), "Warning message");
				Assert.That (warnings [0].Code, Does.EndWith ("7198"), "Warning code");
			}
			Assert.That (task.NativeFrameworks, Has.Length.EqualTo (1), "Reference is still processed");
		}

		[TestCase (".framework", false)]
		[TestCase (".Framework", true)]
		[TestCase (".FRAMEWORK", true)]
		public void CompressedFrameworkExtension (string extension, bool expectWarning)
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var name = "XTest" + extension;
			var archive = Path.Combine (tmpdir, name + ".zip");
			using (var zip = ZipFile.Open (archive, ZipArchiveMode.Create)) {
				zip.CreateEntry (name + "/");
				using var writer = new StreamWriter (zip.CreateEntry (name + "/XTest").Open ());
				writer.Write ("fake framework");
			}

			var task = CreateTask<ResolveNativeReferences> ();
			task.TargetFrameworkMoniker = TargetFramework.DotNet_macOS_String;
			task.FrameworksDirectory = "Frameworks";
			task.IntermediateOutputPath = Path.Combine (tmpdir, "obj");
			task.NativeReferences = new ITaskItem [] { new TaskItem (archive) };

			ExecuteTask (task);

			Assert.That (Engine.Logger.WarningsEvents.Count, Is.EqualTo (expectWarning ? 1 : 0), "Warning count");
			Assert.That (task.NativeFrameworks?.Select (v => v.ItemSpec), Is.EqualTo (new [] { Path.Combine (tmpdir, "obj", name + ".zip", name, "XTest") }), "Resolved framework");
		}

		[TestCase (".framework", false)]
		[TestCase (".Framework", true)]
		[TestCase (".FRAMEWORK", true)]
		public void XCFrameworkExtension (string extension, bool expectWarning)
		{
			var engine = new TestEngine ();
			var logger = new TaskLoggingHelper (engine, "ResolveNativeReferences");
			var directory = Path.GetDirectoryName (GetType ().Assembly.Location) ?? throw new InvalidOperationException ("The test assembly has no directory.");
			var path = Path.Combine (directory, "Resources", "xcf-xcode12.2.plist");
			var plist = PDictionary.OpenFile (path);
			var libraries = plist.GetArray ("AvailableLibraries") ?? throw new InvalidOperationException ("The test xcframework has no libraries.");
			foreach (PDictionary library in libraries)
				library ["LibraryPath"] = new PString ("Universal" + extension);

			var result = ResolveNativeReferences.TryResolveXCFramework (logger, plist, "Test.xcframework", TargetFramework.DotNet_macOS_String, false, "arm64", null, out var frameworkPath);

			Assert.That (result, Is.True, "Resolved");
			Assert.That (frameworkPath, Is.EqualTo ($"macos-arm64_x86_64/Universal{extension}/Universal"), "Framework path");
			Assert.That (engine.Logger.WarningsEvents.Count, Is.EqualTo (expectWarning ? 1 : 0), "Warning count");
		}

		// single arch request (subset are fine)
		[TestCase (TargetFramework.DotNet_iOS_String, false, "arm64", "ios-arm64/Universal.framework/Universal")]
		[TestCase (TargetFramework.DotNet_iOS_String, true, "x86_64", "ios-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase (TargetFramework.DotNet_MacCatalyst_String, false, "x86_64", "ios-arm64_x86_64-maccatalyst/Universal.framework/Universal")] // subset
		[TestCase (TargetFramework.DotNet_tvOS_String, false, "arm64", "tvos-arm64/Universal.framework/Universal")]
		[TestCase (TargetFramework.DotNet_tvOS_String, true, "x86_64", "tvos-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase (TargetFramework.DotNet_macOS_String, false, "x86_64", "macos-arm64_x86_64/Universal.framework/Universal")] // subset

		// multiple arch request (all must be present)
		[TestCase (TargetFramework.DotNet_macOS_String, false, "x86_64, arm64", "macos-arm64_x86_64/Universal.framework/Universal")]

		// failure to resolve requested architecture
		[TestCase (TargetFramework.DotNet_iOS_String, true, "i386, x86_64", null)] // i386 not available

		// failure to resolve mismatched variant
		[TestCase (TargetFramework.DotNet_macOS_String, true, "x86_64", null)] // simulator not available on macOS
		public void Xcode12_x (string targetFrameworkMoniker, bool isSimulator, string architecture, string expected)
		{
			// on Xcode 12.2+ you get arm64 for all (iOS, tvOS) simulators
			var path = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location)!, "Resources", "xcf-xcode12.2.plist");
			var plist = PDictionary.OpenFile (path);
			var result = ResolveNativeReferences.TryResolveXCFramework (log, plist, "N/A", targetFrameworkMoniker, isSimulator, architecture, null, out var frameworkPath);
			Assert.That (!string.IsNullOrEmpty (expected), Is.EqualTo (result), "result");
			Assert.That (frameworkPath, Is.EqualTo (expected), "frameworkPath");
		}

		[TestCase (TargetFramework.DotNet_iOS_String, false, "ARMv7", "ios-arm64_armv7_armv7s/XTest.framework/XTest")]
		public void PreXcode12 (string targetFrameworkMoniker, bool isSimulator, string architecture, string expected)
		{
			var path = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location)!, "Resources", "xcf-prexcode12.plist");
			var plist = PDictionary.OpenFile (path);
			var result = ResolveNativeReferences.TryResolveXCFramework (log, plist, "N/A", targetFrameworkMoniker, isSimulator, architecture, null, out var frameworkPath);
			Assert.That (!string.IsNullOrEmpty (expected), Is.EqualTo (result), "result");
			Assert.That (frameworkPath, Is.EqualTo (expected), "frameworkPath");
		}

		[Test]
		public void BadInfoPlist ()
		{
			var plist = new PDictionary ();
			var result = ResolveNativeReferences.TryResolveXCFramework (log, plist, "N/A", TargetFramework.DotNet_iOS_String, false, "x86_64", null, out var frameworkPath);
			Assert.That (result, Is.False, "Invalid Info.plist");
		}
	}
}
