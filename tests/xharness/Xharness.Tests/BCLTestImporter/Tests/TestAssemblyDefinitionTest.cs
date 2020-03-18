using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Xharness.BCLTestImporter;
using Xharness.BCLTestImporter.Templates;

namespace Xharness.Tests.BCLTestImporter.Tests {
	public class TestAssemblyDefinitionTest {

		Mock<IAssemblyLocator> assemblyLocator;

		[SetUp]
		public void SetUp ()
		{
			assemblyLocator = new Mock<IAssemblyLocator> ();
		}

		[TearDown]
		public void TearDown ()
		{
			assemblyLocator = null;
		}

		[Test]
		public void GetPathNullMonoRoot ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_test.dll", assemblyLocator.Object);
			Assert.Throws<ArgumentNullException> (() => testAssemblyDefinition.GetPath (Platform.iOS));
		}

		[Test]
		public void IsNotXUnit ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_test.dll", assemblyLocator.Object);
			Assert.False (testAssemblyDefinition.IsXUnit);
		}

		[Test]
		public void IsXUnit ()
		{
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll", assemblyLocator.Object);
			Assert.True( testAssemblyDefinition.IsXUnit);
		}

		[Test]
		public void GetPathiOS ()
		{
			assemblyLocator.Setup (a => a.GetAssembliesRootLocation (It.IsAny<Platform> ())).Returns (Environment.GetEnvironmentVariable ("HOME"));
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll", assemblyLocator.Object);
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "monotouch", "tests", testAssemblyDefinition.Name);
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Platform.iOS));
		}

		[Test]
		public void GetPathTvOS ()
		{
			assemblyLocator.Setup (a => a.GetAssembliesRootLocation (It.IsAny<Platform> ())).Returns (Environment.GetEnvironmentVariable ("HOME"));
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll", assemblyLocator.Object);
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "monotouch_tv", "tests", testAssemblyDefinition.Name.Replace ("monotouch", "monotouch_tv"));
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Platform.TvOS));
		}

		[Test]
		public void GetPathWatchOS ()
		{
			assemblyLocator.Setup (a => a.GetAssembliesRootLocation (It.IsAny<Platform> ())).Returns (Environment.GetEnvironmentVariable ("HOME"));
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll", assemblyLocator.Object);
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "monotouch_watch", "tests", testAssemblyDefinition.Name.Replace ("monotouch", "monotouch_watch"));
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Platform.WatchOS));
		}

		[Test]
		public void GetPathMacOS ()
		{
			assemblyLocator.Setup (a => a.GetAssembliesRootLocation (It.IsAny<Platform> ())).Returns (Environment.GetEnvironmentVariable ("HOME"));
			var testAssemblyDefinition = new BCLTestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll", assemblyLocator.Object);
			var home = Environment.GetEnvironmentVariable ("HOME");
			var expectedPath = Path.Combine (home, "xammac_net_4_5", "tests", testAssemblyDefinition.Name);
			Assert.AreEqual (expectedPath, testAssemblyDefinition.GetPath (Platform.MacOSFull));
		}
	}
}
