using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Xharness.TestImporter;

namespace Xharness.Tests.TestImporter.Tests {
	public class ProjectDefinitionTests {

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
			var projectDefinition = new ProjectDefinition ("MyProject", assemblyLocator.Object, factory.Object, new List<ITestAssemblyDefinition> ());
			Assert.Throws<ArgumentNullException> (() => projectDefinition.GetTypeForAssemblies (null, Platform.iOS));
		}
	}
}
