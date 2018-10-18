using System;
using System.Collections.Generic;
using System.IO;

using Xunit;
using Xunit.Sdk;

using BCLTestImporter;

namespace BCLTestImporterTests {
	public class TestProjectDefinitionTest {

		[Fact]
		public void GetTypeForAssembliesNullMonoPath ()
		{
			var projectDefinition = new BCLTestProjectDefinition ("MyProject", new List<BCLTestAssemblyDefinition> ());	
			Assert.Throws<ArgumentNullException> (() => projectDefinition.GetTypeForAssemblies (null, "iOS"));
		}

		[Fact]
		public void GetTypeForAssembliesNullPlatform ()
		{
			var projectDefinition = new BCLTestProjectDefinition ("MyProject", new List<BCLTestAssemblyDefinition> ());	
			Assert.Throws<ArgumentNullException> (() => projectDefinition.GetTypeForAssemblies (Environment.GetEnvironmentVariable("HOME"), null));
		}
	}
}
