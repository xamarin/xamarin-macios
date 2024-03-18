using System;
using System.IO;
using System.Linq;
using System.Collections;

using Microsoft.Build.Evaluation;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class Bug60536 : ProjectTest {
		public Bug60536 ()
			: base ("iPhoneSimulator", "Debug")
		{
		}

		[Test]
		public void TestACToolTaskCatchesJsonException ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var project = SetupProjectPaths ("Bug60536");
			var csproj = project.ProjectCSProjPath;

			MonoTouchProject = project;
			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", Config);

			RunTarget (project, "Clean");
			Assert.IsFalse (Directory.Exists (AppBundlePath), "App bundle exists after cleanup: {0} ", AppBundlePath);
			Assert.IsFalse (Directory.Exists (AppBundlePath + ".dSYM"), "App bundle .dSYM exists after cleanup: {0} ", AppBundlePath + ".dSYM");
			Assert.IsFalse (Directory.Exists (AppBundlePath + ".mSYM"), "App bundle .mSYM exists after cleanup: {0} ", AppBundlePath + ".mSYM");

			var baseDir = Path.GetDirectoryName (csproj);
			var objDir = Path.Combine (baseDir, "obj", Platform, Config);
			var binDir = Path.Combine (baseDir, "bin", Platform, Config);

			if (Directory.Exists (objDir)) {
				var path = Directory.EnumerateFiles (objDir, "*.*", SearchOption.AllDirectories).FirstOrDefault ();
				Assert.IsNull (path, "File not cleaned: {0}", path);
			}

			if (Directory.Exists (binDir)) {
				var path = Directory.EnumerateFiles (binDir, "*.*", SearchOption.AllDirectories).FirstOrDefault ();
				Assert.IsNull (path, "File not cleaned: {0}", path);
			}

			RunTarget (MonoTouchProject, "Build", expectedErrorCount: 1);

			var expectedFile = Path.Combine ("Assets.xcassets", "AppIcon.appiconset", "Contents.json");
			Assert.AreEqual (expectedFile, Engine.Logger.ErrorEvents [0].File, "File");
			Assert.AreEqual (197, Engine.Logger.ErrorEvents [0].LineNumber, "LineNumber");
			Assert.AreEqual (4, Engine.Logger.ErrorEvents [0].ColumnNumber, "ColumnNumber");
			Assert.AreEqual (197, Engine.Logger.ErrorEvents [0].EndLineNumber, "EndLineNumber");
			Assert.AreEqual (4, Engine.Logger.ErrorEvents [0].EndColumnNumber, "EndColumnNumber");
			Assert.AreEqual ("']' is invalid without a matching open. LineNumber: 196 | BytePositionInLine: 3.", Engine.Logger.ErrorEvents [0].Message, "Message");
		}
	}
}
