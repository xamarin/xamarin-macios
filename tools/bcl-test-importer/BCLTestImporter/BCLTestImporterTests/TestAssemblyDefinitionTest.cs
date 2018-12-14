using System;
using System.IO;

using Xunit;
using Xunit.Sdk;

using BCLTestImporter;

namespace BCLTestImporterTests {
	public class TestAssemblyDefinitionTest {
		
		[Fact]
		public void GetPathNullMonoRoot ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("MONOTOUCH_System.Json.Microsoft_test.dll");
			Assert.Throws<ArgumentNullException> (() => testAssemblyDefinition.GetPath (null, Platform.iOS, true));
		}

		[Fact]
		public void IsNotXUnit ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("MONOTOUCH_System.Json.Microsoft_test.dll");
			Assert.False (testAssemblyDefinition.IsXUnit);
		}

		[Fact]
		public void IsXUnit ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("MONOTOUCH_System.Json.Microsoft_xunit-test.dll");
			Assert.True( testAssemblyDefinition.IsXUnit);
		}

		[Fact]
		public void GetPathiOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("MONOTOUCH_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "monotouch", "tests", testAssemblyDefinition.Name);
			Assert.Equal (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.iOS, false));
		}

		[Fact]
		public void GetPathTvOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("MONOTOUCH_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "monotouch", "tests", testAssemblyDefinition.Name);
			Assert.Equal (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.TvOS, false));
		}

		[Fact]
		public void GetPathWatchOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("MONOTOUCH_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "monotouch", "tests", testAssemblyDefinition.Name);
			Assert.Equal (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.WatchOS, false));
		}

		[Fact]
		public void GetPathMacOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("MONOTOUCH_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "xammac_net_4_5", "tests", testAssemblyDefinition.Name);
			Assert.Equal (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.MacOSFull, false));
		}
	}
}
