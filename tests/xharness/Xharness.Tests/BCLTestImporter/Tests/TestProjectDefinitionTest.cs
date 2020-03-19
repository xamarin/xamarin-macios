using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Xharness.BCLTestImporter;
using Xharness.BCLTestImporter.Templates;

namespace Xharness.Tests.BCLTestImporter.Tests {
	public class TestProjectDefinitionTest {

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
		public void GetTypeForAssembliesNullMonoPath ()
		{
			var projectDefinition = new BCLTestProjectDefinition ("MyProject", assemblyLocator.Object, new List<BCLTestAssemblyDefinition> (), "");	
			Assert.Throws<ArgumentNullException> (() => projectDefinition.GetTypeForAssemblies (null, Platform.iOS));
		}
	}
}
