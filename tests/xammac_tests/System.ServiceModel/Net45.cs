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
			int result = Driver.RunCommand ("/Library/Frameworks/Mono.framework/Versions/Current/Commands/monop", "--refs -r:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/4.5/System.ServiceModel.dll", null, output);
			Assert.That (result, Is.EqualTo (0));
			Assert.That (output.ToString (), Contains.Substring ("System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
		}

		[Test]
		public void ShouldNotIncludeSystemDrawing ()
		{
			StringBuilder output = new StringBuilder ();
			int result = Driver.RunCommand ("/Library/Frameworks/Mono.framework/Versions/Current/Commands/monop", "--refs -r:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/4.5/System.ServiceModel.dll", null, output);
			Assert.That (result, Is.EqualTo (0));
			Assert.That (output.ToString (), !Contains.Substring ("System.Drawing"));
		}

		[Test]
		public void ServiceModelShouldCreateCommunicationException ()
		{
			var testFolder = "../../../../../../../common/mac/ServiceModel_Test/ServiceModel_Test";
			var testResults = testFolder + "/TestResult.txt";
			if (File.Exists (testResults))
				File.Delete (testResults);

			TI.BuildProject (testFolder + "/ServiceModel_Test.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/ServiceModel_Test.app/Contents/MacOS/ServiceModel_Test", null, "Run");
			Assert.True (File.Exists (testResults));

			using (TextReader reader = File.OpenText (testResults)) {
				var output = reader.ReadLine ();

				Assert.AreEqual ("Test Passed: System.ServiceModel.CommunicationException: System error.", output);
			}

			File.Delete (testResults);
		}

		[Test]
		public void ProtobufShouldSerializeAndDeserialize ()
		{
			var testFolder = "../../../../../../../common/mac/Protobuf_Test/Protobuf_Test";
			var testResults = testFolder + "/TestResult.txt";
			if (File.Exists (testResults))
				File.Delete (testResults);

			TI.BuildProject (testFolder + "/Protobuf_Test.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/Protobuf_Test.app/Contents/MacOS/Protobuf_Test", null, "Run");
			Assert.True (File.Exists (testResults));

			using (TextReader reader = File.OpenText (testResults)) {
				var output = reader.ReadLine ();

				Assert.AreEqual ("Test Passed", output);
			}

			File.Delete (testResults);
		}

		[Test]
		public void Net45ShouldUseImmutableCollection ()
		{
			var testFolder = "../../../../../../../common/mac/ImmutableCollection_Test/ImmutableCollection_Test";

			TI.BuildProject (testFolder + "/ImmutableCollection_Test.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/ImmutableCollection_Test.app/Contents/MacOS/ImmutableCollection_Test", null, "Run");
		}

		[Test]
		public void BasicPCLTest ()
		{
			var testFolder = "../../../../../../../common/mac/BasicPCLTest/BasicPCLTest";
			var testResults = testFolder + "/TestResult.txt";
			if (File.Exists (testResults))
				File.Delete (testResults);

			TI.BuildProject (testFolder + "/BasicPCLTest.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/BasicPCLTest.app/Contents/MacOS/BasicPCLTest", null, "Run");
			Assert.True (File.Exists (testResults));

			using (TextReader reader = File.OpenText (testResults)) {
				var output = reader.ReadToEnd ();

				Assert.AreEqual ("{\n  \"MyArray\": [\n    \"Manual text\",\n    \"2000-05-23T00:00:00\"\n  ]\n}\n", output);
			}

			File.Delete (testResults);
		}
	}
}
