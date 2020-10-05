using System;

using Mono.Cecil;

using Xamarin.Bundler;

namespace Xamarin.Linker {
	public class DotNetResolver : CoreResolver {
		public override AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
		{
			throw new NotImplementedException ();
		}
	}
}
