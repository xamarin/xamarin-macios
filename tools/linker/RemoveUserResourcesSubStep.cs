using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
#if NET
using Mono.Linker.Steps;
#endif
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Utils;

namespace Xamarin.Linker {

	public class RemoveUserResourcesSubStep : ExceptionalSubStep {

		const string iOS_Content = "__monotouch_content_";
		const string iOS_Page = "__monotouch_page_";
		const string Mac_Content = "__xammac_content_";
		const string Mac_Page = "__xammac_page_";

		string Content;
		string Page;

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly; }
		}

		public bool Simulator { get { return LinkContext.App.IsSimulatorBuild; } }

		protected override string Name { get; } = "Removing User Resources";
		protected override int ErrorCode { get; } = 2030;

		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);

			switch (LinkContext.App.Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.MacCatalyst:
				Content = iOS_Content;
				Page = iOS_Page;
				break;
			case ApplePlatform.MacOSX:
				Content = Mac_Content;
				Page = Mac_Page;
				break;
			default:
				Report (ErrorHelper.CreateError (71, Errors.MX0071, LinkContext.App.Platform, LinkContext.App.ProductName));
				break;
			}
		}

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
					if (lwa.LibraryName is not null) {
						if (libraries is null)
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
			// Don't bother removing the resources if we're building for the simulator
			if (Simulator)
				return false;

			if (resourceName.StartsWith (Content, StringComparison.OrdinalIgnoreCase))
				return true;

			if (resourceName.StartsWith (Page, StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		}

		static bool IsNativeLibrary (string resourceName, HashSet<string> libraries)
		{
			if (libraries is null)
				return false;

			return libraries.Contains (resourceName);
		}
	}
}
