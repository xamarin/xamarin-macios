using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

namespace Xamarin.Linker {

	public class RemoveUserResourcesSubStep : ExceptionalSubStep {

#if MTOUCH
		const string Content = "__monotouch_content_";
		const string Page = "__monotouch_page_";

		public RemoveUserResourcesSubStep (MonoTouch.Tuner.LinkerOptions options)
		{
			Device = options.Device;
		}
#else
		const string Content = "__xammac_content_";
		const string Page = "__xammac_page_";
#endif
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
					if (lwa.LibraryName != null) {
						if (libraries == null)
							libraries = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
						libraries.Add (lwa.LibraryName);
					}
				}
			}

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

		bool IsMonoTouchResource (string resourceName)
		{
#if MTOUCH
			if (!Device)
				return false;
#endif
			if (resourceName.StartsWith (Content, StringComparison.OrdinalIgnoreCase))
				return true;

			if (resourceName.StartsWith (Page, StringComparison.OrdinalIgnoreCase))
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
