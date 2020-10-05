using Mono.Cecil;

namespace Xamarin.Linker {
	public class CollectAssembliesStep : ConfigurationAwareStep {
		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);

			Configuration.Assemblies.Add (assembly);
		}
	}
}

