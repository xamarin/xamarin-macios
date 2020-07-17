using System;
using System.Collections.Generic;

namespace Xamarin.Linker {

	public class ExtractBindingLibrariesStep : ConfigurationAwareStep {
		protected override void EndProcess ()
		{
			base.EndProcess ();

			// No attributes are currently linked away, which means we don't need to worry about linked away LinkWith attributes.
			// Ref: https://github.com/mono/linker/issues/952 (still open as of this writing).
			var exceptions = new List<Exception> ();
			Configuration.Target.ExtractNativeLinkInfo (exceptions);
			Report (exceptions);

			// Tell MSBuild about the native libraries we found
			var linkWith = new List<MSBuildItem> ();
			foreach (var asm in Configuration.Target.Assemblies) {
				foreach (var arg in asm.LinkWith) {
					var item = new MSBuildItem {
						Include = arg,
						Metadata = new Dictionary<string, string> {
							{ "ForceLoad", "true" },
							{ "Assembly", asm.Identity },
						},
					};
					linkWith.Add (item);
				}
			}
			Configuration.WriteOutputForMSBuild ("_BindingLibraryLinkWith", linkWith);

			// Tell MSBuild about the frameworks libraries we found
			var frameworks = new List<MSBuildItem> ();
			foreach (var asm in Configuration.Target.Assemblies) {
				foreach (var fw in asm.Frameworks) {
					var item = new MSBuildItem {
						Include = fw,
						Metadata = new Dictionary<string, string> {
							{ "Assembly", asm.Identity },
						},
					};
					frameworks.Add (item);
				}
				foreach (var fw in asm.WeakFrameworks) {
					var item = new MSBuildItem {
						Include = fw,
						Metadata = new Dictionary<string, string> {
							{ "IsWeak", "true " },
							{ "Assembly", asm.Identity },
						},
					};
					frameworks.Add (item);
				}
			}
			Configuration.WriteOutputForMSBuild ("_BindingLibraryFrameworks", frameworks);

			// Tell MSBuild about any additional linker flags we found
			var linkerFlags = new List<MSBuildItem> ();
			foreach (var asm in Configuration.Target.Assemblies) {
				if (asm.LinkerFlags == null)
					continue;
				foreach (var arg in asm.LinkerFlags) {
					var item = new MSBuildItem {
						Include = arg,
						Metadata = new Dictionary<string, string> {
							{ "Assembly", asm.Identity },
						},
					};
					linkerFlags.Add (item);
				}
			}
			Configuration.WriteOutputForMSBuild ("_BindingLibraryLinkerFlags", linkerFlags);
		}
	}
}
