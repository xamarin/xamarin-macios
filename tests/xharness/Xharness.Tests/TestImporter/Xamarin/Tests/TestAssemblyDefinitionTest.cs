using System;

using Moq;

using NUnit.Framework;

using Xharness.TestImporter;
using Xharness.TestImporter.Xamarin;

namespace Xharness.Tests.TestImporter.Tests {
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
			var testAssemblyDefinition = new TestAssemblyDefinition ("monotouch_System.Json.Microsoft_test.dll", assemblyLocator.Object);
			Assert.Throws<ArgumentNullException> (() => testAssemblyDefinition.GetPath (Platform.iOS));
		}

		[Test]
		public void IsNotXUnit ()
		{
			var testAssemblyDefinition = new TestAssemblyDefinition ("monotouch_System.Json.Microsoft_test.dll", assemblyLocator.Object);
			Assert.False (testAssemblyDefinition.IsXUnit);
		}

		[Test]
		public void IsXUnit ()
		{
			var testAssemblyDefinition = new TestAssemblyDefinition ("monotouch_System.Json.Microsoft_xunit-test.dll", assemblyLocator.Object);
			Assert.True (testAssemblyDefinition.IsXUnit);
		}

	}
}
