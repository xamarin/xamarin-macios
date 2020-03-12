using System;
using System.IO;

using NUnit.Framework;
using Xharness.BCLTestImporter;

namespace Xharness.Tests.BCLTestImporter.Tests {
	public class TestAssemblyDefinitionTest {
		
		[Test]
		public void GetPathNullMonoRoot ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_test.dll");
			Assert.Throws<ArgumentNullException> (() => testAssemblyDefinition.GetPath (null, Platform.iOS, true));
		}

		[Test]
		public void IsNotXUnit ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_test.dll");
			Assert.False (testAssemblyDefinition.IsXUnit);
		}

		[Test]
		public void IsXUnit ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll");
			Assert.True( testAssemblyDefinition.IsXUnit);
		}

		[Test]
		public void GetPathiOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "monotouch", "tests", testAssemblyDefinition.Name);
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.iOS, false));
		}

		[Test]
		public void GetPathTvOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "monotouch_tv", "tests", testAssemblyDefinition.Name.Replace ("monotouch", "monotouch_tv"));
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.TvOS, false));
		}

		[Test]
		public void GetPathWatchOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "monotouch_watch", "tests", testAssemblyDefinition.Name.Replace ("monotouch", "monotouch_watch"));
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.WatchOS, false));
		}

		[Test]
		public void GetPathMacOS ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll");
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "mcs/class/lib", "xammac_net_4_5", "tests", testAssemblyDefinition.Name);
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Environment.GetEnvironmentVariable("HOME"), Platform.MacOSFull, false));
		}
	}
}
