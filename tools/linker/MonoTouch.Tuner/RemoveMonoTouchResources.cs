using System;

using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Mono.Cecil;

namespace MonoTouch.Tuner {

	public class RemoveMonoTouchResources : BaseStep {

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			if (Annotations.GetAction (assembly) != AssemblyAction.Link)
				return;

			RemoveResources (assembly);
		}

		public static bool RemoveResources (AssemblyDefinition assembly)
		{
			// if called from mtouch it can be null if the reference was not found
			// but it's not the place to complain about this
			if (assembly == null)
				return false;

			// only user code has such resources
			if (Profile.IsProductAssembly (assembly) || Profile.IsSdkAssembly (assembly))
				return false;

			var module = assembly.MainModule;
			if (!module.HasResources)
				return false;

			bool found = false;
			var resources = module.Resources;

			for (int i = 0; i < resources.Count; i++) {
				if (!IsMonoTouchResource (resources [i]))
					continue;

				resources.RemoveAt (i--);
				found = true;
			}
			return found;
		}

		static bool IsMonoTouchResource (Resource resource)
		{
			if (!(resource is EmbeddedResource))
				return false;

			var name = resource.Name;

			if (name.StartsWith ("__monotouch_content_", StringComparison.OrdinalIgnoreCase))
				return true;

			if (name.StartsWith ("__monotouch_page_", StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		}
	}
}
