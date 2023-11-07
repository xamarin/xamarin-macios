using System;

using Mono.Cecil;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Linker {
	public class DotNetResolver : CoreResolver {
		public override AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
		{
			throw new NotImplementedException ($"Unable to resolve the assembly reference {name}");
		}
	}
}
