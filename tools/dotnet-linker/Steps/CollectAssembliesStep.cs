using Mono.Cecil;

namespace Xamarin.Linker {
	public class CollectAssembliesStep : ConfigurationAwareStep {
		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			Configuration.Assemblies.Add (assembly);
		}
	}
}

