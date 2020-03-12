using System;
using System.Collections.Generic;

using NUnit.Framework;
using Xharness.BCLTestImporter;

namespace Xharness.Tests.BCLTestImporter.Tests {
	public class TestProjectDefinitionTest {

		[Test]
		public void GetTypeForAssembliesNullMonoPath ()
		{
			var projectDefinition = new BCLTestProjectDefinition ("MyProject", new List<BCLTestAssemblyDefinition> (), "");	
			Assert.Throws<ArgumentNullException> (() => projectDefinition.GetTypeForAssemblies (null, Platform.iOS, true));
		}
	}
}
