using Microsoft.DotNet.XHarness.iOS.Shared.TestImporter;

namespace Xharness.TestImporter.Xamarin {
	public class AssemblyDefinitionFactory : ITestAssemblyDefinitionFactory {
		public ITestAssemblyDefinition Create (string assembly, IAssemblyLocator loader) => new TestAssemblyDefinition (assembly, loader);
	}
}
