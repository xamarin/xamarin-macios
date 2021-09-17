namespace Xharness.TestImporter.Xamarin {
	public class AssemblyDefinitionFactory : ITestAssemblyDefinitionFactory {
		public ITestAssemblyDefinition Create (string assembly, IAssemblyLocator loader) => new TestAssemblyDefinition (assembly, loader);
	}
}
