using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;

#nullable enable

namespace Xamarin.MacDev.Tasks
{
	public class UnpackLibraryResources : UnpackLibraryResourcesTaskBase
	{
		protected override IEnumerable<ManifestResource> GetAssemblyManifestResources (string fileName)
		{
			AssemblyDefinition? assembly = null;
			try {
				try {
					assembly = AssemblyDefinition.ReadAssembly (fileName);
				} catch {
					yield break;
				}

				foreach (var _r in assembly.MainModule.Resources.OfType<EmbeddedResource> ()) {
					var r = _r;
					yield return new ManifestResource (r.Name, r.GetResourceStream);
				}
			} finally {
				assembly?.Dispose ();
			}
		}
	}
}

