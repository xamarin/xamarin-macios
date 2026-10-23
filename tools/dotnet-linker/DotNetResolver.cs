using System;

using Mono.Cecil;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Linker {
	public class DotNetResolver : CoreResolver {
		public DotNetResolver (Application app)
		{
		}

		public override AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
		{
			if (cache.TryGetValue (name.Name, out var assembly))
				return assembly;
			throw new NotImplementedException ($"Unable to resolve the assembly reference {name}");
		}
	}
}
