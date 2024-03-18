using System.Collections.Generic;
using Mono.Cecil;

#nullable enable

namespace Xamarin.Linker {
	public class CollectAssembliesStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Collect Assemblies";
		protected override int ErrorCode { get; } = 2330;

		protected override void TryProcess ()
		{
			base.TryProcess ();

			// This is a temporary workaround, we need to mark members and types, and we have to do it before
			// the MarkStep. However, MarkStep is the first step, and if we add another step before it,
			// there won't be any assemblies loaded (since MarkStep will load assemblies as needed).
			// This step now runs at the very beginning, using reflection to call into the linker to load all
			// the referenced assemblies, so that we can then have another step before MarkStep that does the
			// custom marking we need to do.
			var getReferencedAssemblies = Configuration.Context.GetType ().GetMethod ("GetReferencedAssemblies")!;
			var assemblies = (IEnumerable<AssemblyDefinition>) getReferencedAssemblies.Invoke (Configuration.Context, new object [0])!;
			Configuration.Assemblies.AddRange (assemblies);
		}
	}
}
