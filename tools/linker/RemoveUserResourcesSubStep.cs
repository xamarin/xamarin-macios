using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
#if NET
using Mono.Linker.Steps;
#endif
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker.Steps;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.Linker {

#if ASSEMBLY_PREPARER
	public class RemoveUserResourcesSubStep : AssemblyModifierStep {
#else
	public class RemoveUserResourcesSubStep : ExceptionalSubStep {
#endif
		static string [] monotouch_prefixes = new string [] {
			"__monotouch_content_",
			"__monotouch_page_",
			"__monotouch_item_",
		};

		static string [] xammac_prefixes = new string [] {
			"__xammac_content_",
			"__xammac_page_",
			"__xammac_item_",
		};

#if !ASSEMBLY_PREPARER
		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly; }
		}
#endif

		ApplePlatform Platform {
			get { return Configuration.Platform; }
		}

#if ASSEMBLY_PREPARER
		public bool Simulator { get { return Configuration.IsSimulatorBuild; } }
#else
		public bool Simulator { get { return LinkContext.App.IsSimulatorBuild; } }
		public Application App { get { return LinkContext.App; } }
#endif

		protected override string Name { get; } = "Removing User Resources";
		protected override int ErrorCode { get; } = 2030;

		public string [] GetPrefixes (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.MacCatalyst:
				return monotouch_prefixes;
			case ApplePlatform.MacOSX:
				return xammac_prefixes;
			default:
				Report (ErrorHelper.CreateError (71, Errors.MX0071, platform, App.ProductName));
				break;
			}
			return Array.Empty<string> ();
		}

#if ASSEMBLY_PREPARER
		protected override bool ModifyAssembly (AssemblyDefinition assembly)
#else
		protected override void Process (AssemblyDefinition assembly)
		{
			// In Hot Reload compatible builds, don't strip resources from reloadable (non-linked) assemblies,
			// because doing so would upgrade the assembly from Copy to Save below, re-serializing it and
			// breaking Hot Reload. Linked assemblies are re-saved regardless, so stripping them is fine.
			if (Configuration.HotReloadCompatibleBuild && Annotations.GetAction (assembly) != AssemblyAction.Link)
				return;

			var modified = ModifyAssembly (assembly);

			// we'll need to save (if we're not linking) this assembly
			if (modified && Annotations.GetAction (assembly) != AssemblyAction.Link)
				Annotations.SetAction (assembly, AssemblyAction.Save);
		}

		bool ModifyAssembly (AssemblyDefinition assembly)
#endif
		{
			// NativeAOT excludes native library resources using ILC substitutions instead, leaving
			// them available for extraction even when post-processing runs after ILC.
			if (App.XamarinRuntime == XamarinRuntime.NativeAOT)
				return false;

#if ASSEMBLY_PREPARER
			// In Hot Reload compatible builds, don't strip resources from reloadable (non-linked) assemblies,
			// because modifying them re-serializes (saves) the assembly, breaking Hot Reload. Linked assemblies
			// are re-serialized regardless, so stripping them is fine.
			if (Configuration.HotReloadCompatibleBuild && Annotations.GetAction (assembly) != AssemblyAction.Link)
				return false;
#endif

			if (App.Profile.IsProductAssembly (assembly) || App.Profile.IsSdkAssembly (assembly))
				return false;

			var module = assembly.MainModule;
			if (!module.HasResources)
				return false;

			var libraries = Xamarin.Bundler.Assembly.GetNativeLibraryResourceNames (assembly);

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

			return found;
		}

		bool IsMonoTouchResource (string resourceName)
		{
			// Don't bother removing the resources if we're building for the simulator
			if (Simulator)
				return false;

			foreach (var prefix in GetPrefixes (Platform)) {
				if (resourceName.StartsWith (prefix, StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return false;
		}

		static bool IsNativeLibrary (string resourceName, HashSet<string>? libraries)
		{
			if (libraries is null)
				return false;

			return libraries.Contains (resourceName);
		}
	}
}
