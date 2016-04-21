using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;

namespace Xamarin.MacDev.Tasks
{
	public class UnpackLibraryResources : UnpackLibraryResourcesTaskBase
	{
		protected override IEnumerable<ManifestResource> GetAssemblyManifestResources (string fileName)
		{
			AssemblyDefinition assembly;

			try {
				assembly = AssemblyDefinition.ReadAssembly (fileName);
			} catch {
				yield break;
			}

			foreach (var _r in assembly.MainModule.Resources.OfType<EmbeddedResource> ()) {
				var r = _r;
				yield return new ManifestResource (r.Name, r.GetResourceStream);
			}
		}
	}
}

