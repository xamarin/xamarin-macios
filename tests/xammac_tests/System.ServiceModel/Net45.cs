using System;
using NUnit.Framework;
using Xamarin.Bundler;
using System.Text;
using System.IO;
using Xamarin.MMP.Tests;

namespace MonoTouchFixtures.ServiceModel {
	//https://testrail.xamarin.com/index.php?/cases/view/236768&group_by=cases:section_id&group_order=asc&group_id=72254
	[TestFixture]
	public class Net45 {
		void RunMSBuildTest (Action<string> test)
		{
			string tmpDir = Path.Combine (Path.GetTempPath (), "msbuild-tests");
			try {
				Directory.CreateDirectory (tmpDir);
				test (tmpDir);
			} finally {
				Directory.Delete (tmpDir, true);
			}
		}

		[Test]
		public void ShouldIncludeSystemServiceModel ()
		{
			StringBuilder output = new StringBuilder ();
			int result = Driver.RunCommand ("/Library/Frameworks/Mono.framework/Versions/Current/Commands/monop", String.Format ("--refs -r:{0}/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/4.5/System.ServiceModel.dll", TI.FindRootDirectory ()), null, output);
			Assert.That (result, Is.EqualTo (0));
			Assert.That (output.ToString (), Contains.Substring ("System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
		}

		[Test]
		public void ShouldNotIncludeSystemDrawing ()
		{
			StringBuilder output = new StringBuilder ();
			int result = Driver.RunCommand ("/Library/Frameworks/Mono.framework/Versions/Current/Commands/monop", String.Format ("--refs -r:{0}/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/4.5/System.ServiceModel.dll", TI.FindRootDirectory ()), null, output);
			Assert.That (result, Is.EqualTo (0));
			Assert.That (output.ToString (), !Contains.Substring ("System.Drawing"));
		}

		[Test]
		public void ServiceModelShouldCreateCommunicationException ()
		{
			var testFolder = Path.Combine (TI.FindRootDirectory (), "../tests/common/mac/TestProjects/ServiceModel_Test/ServiceModel_Test");
			var testResults = testFolder + "/TestResult.txt";
			if (File.Exists (testResults))
				File.Delete (testResults);

			TI.BuildProject (testFolder + "/ServiceModel_Test.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/ServiceModel_Test.app/Contents/MacOS/ServiceModel_Test", (string)null, "Run");
			Assert.True (File.Exists (testResults));

			using (TextReader reader = File.OpenText (testResults)) {
				var output = reader.ReadLine ();

				Assert.AreEqual ("Test Passed: System.ServiceModel.CommunicationException: System error.", output);
			}

			File.Delete (testResults);
		}
	}
}
