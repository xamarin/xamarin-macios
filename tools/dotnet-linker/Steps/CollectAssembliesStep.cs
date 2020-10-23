using Mono.Cecil;

namespace Xamarin.Linker {
	public class CollectAssembliesStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Collect Assemblies";
		protected override int ErrorCode { get; } = 2330;

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			Configuration.Assemblies.Add (assembly);
		}
	}
}

