using System;
using Xharness.BCLTestImporter.Templates;

namespace Xharness.BCLTestImporter.Xamarin {
	public class AssemblyDefinitionFactory : ITestAssemblyDefinitionFactory {
		public ITestAssemblyDefinition Create (string assembly, IAssemblyLocator loader) => new TestAssemblyDefinition (assembly, loader);
	}
}
