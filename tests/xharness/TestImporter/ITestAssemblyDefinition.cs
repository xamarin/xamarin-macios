namespace Xharness.TestImporter {

	// factory that hides the creation of the ITestAssemblyDefinition to hide the concreate class that is used by the
	// template.
	public interface ITestAssemblyDefinitionFactory {
		ITestAssemblyDefinition Create (string assembly, IAssemblyLocator loader);
	}

	// interface that represents a test assembly to be added in a test project. Each implemenation will hide specific
	// details. For example, on xamarin-macios we build the name of the assembly based on the platform as
	// prefix + assembly name. That was done to simplify the configureaton class.
	public interface ITestAssemblyDefinition {
		string Name { get; set; }
		bool IsXUnit { get; set; }
		IAssemblyLocator AssemblyLocator { get; set; }

		string GetName (Platform platform);
		string GetPath (Platform platform);
	}
}
