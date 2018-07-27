using System;
using System.IO;
using NUnit.Framework;
using Xamarin.MMP.Tests;
using Xamarin.Bundler;
using System.Text;
using Xamarin.Tests;

namespace MonoTouchFixtures.Net45 {
	[TestFixture]
	public class Unified45 {
		[Test]
		public void ProtobufShouldSerializeAndDeserialize ()
		{
			var testFolder = Path.Combine (TI.FindRootDirectory (), "../tests/common/mac/TestProjects/Protobuf_Test/Protobuf_Test");
			var testResults = testFolder + "/TestResult.txt";
			if (File.Exists (testResults))
				File.Delete (testResults);

			StringBuilder restoreOutput = new StringBuilder ();
			int code = Driver.RunCommand ("mono", String.Format ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore {0}/packages.config -PackagesDirectory {1}", testFolder, Configuration.NuGetPackagesDirectory), output: restoreOutput);

			if (code != 0)
				Assert.Fail ("ProtobufShouldSerializeAndDeserialize failed to restore nuget packages");

			TI.BuildProject (testFolder + "/Protobuf_Test.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/Protobuf_Test.app/Contents/MacOS/Protobuf_Test", (string)null, "Run");
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
			var testFolder = Path.Combine (TI.FindRootDirectory (), "../tests/common/mac/TestProjects/ImmutableCollection_Test/ImmutableCollection_Test");

			StringBuilder restoreOutput = new StringBuilder ();

			int code = Driver.RunCommand ("mono", String.Format ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore {0}/packages.config -PackagesDirectory {1}", testFolder, Configuration.NuGetPackagesDirectory), output: restoreOutput);

			if (code != 0)
				Assert.Fail ("Net45ShouldUseImmutableCollection failed to restore nuget packages");

			TI.BuildProject (testFolder + "/ImmutableCollection_Test.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/ImmutableCollection_Test.app/Contents/MacOS/ImmutableCollection_Test", (string)null, "Run");
		}

		[Test]
		public void BasicPCLTest ()
		{
			var testFolder = Path.Combine (TI.FindRootDirectory (), "../tests/common/mac/TestProjects/BasicPCLTest/BasicPCLTest");
			var testResults = testFolder + "/TestResult.txt";
			if (File.Exists (testResults))
				File.Delete (testResults);

			StringBuilder restoreOutput = new StringBuilder ();

			int code = Driver.RunCommand ("mono", String.Format ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore {0}/../MyLibrary/packages.config -PackagesDirectory {1}", testFolder, Configuration.NuGetPackagesDirectory), output: restoreOutput);

			if (code != 0)
				Assert.Fail ("Net45ShouldUseImmutableCollection failed to restore nuget packages");

			TI.BuildProject (testFolder + "/BasicPCLTest.csproj", true);

			TI.RunAndAssert (testFolder + "/bin/Debug/BasicPCLTest.app/Contents/MacOS/BasicPCLTest", (string)null, "Run");
			Assert.True (File.Exists (testResults));

			using (TextReader reader = File.OpenText (testResults)) {
				var output = reader.ReadToEnd ();

				Assert.AreEqual ("{\n  \"MyArray\": [\n    \"Manual text\",\n    \"2000-05-23T00:00:00\"\n  ]\n}\n", output);
			}

			File.Delete (testResults);
		}
	}
}
