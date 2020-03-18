using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Xharness.BCLTestImporter;
using Xharness.BCLTestImporter.Templates;

namespace Xharness.Tests.BCLTestImporter.Tests {
	public class TestProjectDefinitionTest {

		Mock<IAssemblyLocator> assemblyLocator;
		Mock<ITestAssemblyDefinitionFactory> factory;

		[SetUp]
		public void SetUp ()
		{
			assemblyLocator = new Mock<IAssemblyLocator> ();
			factory = new Mock<ITestAssemblyDefinitionFactory> ();
		}

		[TearDown]
		public void TearDown ()
		{
			assemblyLocator = null;
		}

		[Test]
		public void GetTypeForAssembliesNullMonoPath ()
		{
			var projectDefinition = new BCLTestProjectDefinition ("MyProject", assemblyLocator.Object, factory.Object,  new List<ITestAssemblyDefinition> (), "");	
			Assert.Throws<ArgumentNullException> (() => projectDefinition.GetTypeForAssemblies (null, Platform.iOS));
		}
	}
}
