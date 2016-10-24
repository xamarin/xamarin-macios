using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using MonoTouch.Tuner;

namespace Xamarin.Linker {

	public class RemoveUserResourcesSubStep : ExceptionalSubStep {

		public RemoveUserResourcesSubStep (LinkerOptions options)
		{
			Device = options.Device;
		}

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly; }
		}

		public bool Device { get; private set; }

		protected override string Name { get; } = " Removing User Resources";
		protected override int ErrorCode { get; } = 2030;

		protected override void Process (AssemblyDefinition assembly)
		{
			if (Profile.IsProductAssembly (assembly) || Profile.IsSdkAssembly (assembly))
				return;

			var module = assembly.MainModule;
			if (!module.HasResources)
				return;
			
			HashSet<string> libraries = null;
			if (assembly.HasCustomAttributes) {
				foreach (var ca in assembly.CustomAttributes) {
					if (!ca.AttributeType.Is ("ObjCRuntime", "LinkWithAttribute"))
						continue;
					var lwa = Xamarin.Bundler.Assembly.GetLinkWithAttribute (ca);
					if (libraries == null)
						libraries = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
					libraries.Add (lwa.LibraryName);
				}
			}

			if (libraries == null)
				return;

			var found = false;
			var resources = module.Resources;
			for (int i = 0; i < resources.Count; i++) {
				var resource = resources [i];

				if (!(resource is EmbeddedResource))
					continue;

				var name = resource.Name;
				if (!IsMonoTouchResource (name) && !IsNativeLibrary (name, libraries))
					continue;

				resources.RemoveAt (i--);
				found = true;
			}

			// we'll need to save (if we're not linking) this assembly
			if (found && Annotations.GetAction (assembly) != AssemblyAction.Link)
				Annotations.SetAction (assembly, AssemblyAction.Save);
		}

		static bool IsMonoTouchResource (string resourceName)
		{
			if (resourceName.StartsWith ("__monotouch_content_", StringComparison.OrdinalIgnoreCase))
				return true;

			if (resourceName.StartsWith ("__monotouch_page_", StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		}

		static bool IsNativeLibrary (string resourceName, HashSet<string> libraries)
		{
			if (libraries == null)
				return false;

			return libraries.Contains (resourceName);
		}
	}
}
