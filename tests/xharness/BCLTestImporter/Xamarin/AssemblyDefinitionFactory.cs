using System;
using Xharness.BCLTestImporter.Templates;

namespace Xharness.BCLTestImporter.Xamarin {
	public class AssemblyDefinitionFactory : ITestAssemblyDefinitionFactory {

		public ITestAssemblyDefinition Create (string assembly, IAssemblyLocator loader)
		{
			return new TestAssemblyDefinition (assembly, loader);
		}
	}
}
