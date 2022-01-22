using System;
using System.Collections.Generic;
using System.IO;

namespace Xamarin.Linker {

	public class ExtractBindingLibrariesStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Extract Binding Libraries";
		protected override int ErrorCode { get; } = 2340;

		protected override void TryEndProcess ()
		{
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

			var frameworksToPublish = new List<MSBuildItem> ();
			foreach (var asm in Configuration.Target.Assemblies) {
				var fwks = new HashSet<string> ();
				fwks.UnionWith (asm.Frameworks);
				fwks.UnionWith (asm.WeakFrameworks);

				// Only keep frameworks that point to a location on disk
				fwks.RemoveWhere (v => !v.EndsWith (".framework", StringComparison.Ordinal));

				foreach (var fwk in fwks) {
					if (!Configuration.Application.VerifyDynamicFramework (fwk))
						continue;

					var executable = Path.Combine (fwk, Path.GetFileNameWithoutExtension (fwk));

					var item = new MSBuildItem {
						Include = executable,
					};
					frameworksToPublish.Add (item);
				}
			}
			Configuration.WriteOutputForMSBuild ("_FrameworkToPublish", frameworksToPublish);

			var dynamicLibraryToPublish = new List<MSBuildItem> ();
			foreach (var asm in Configuration.Target.Assemblies) {
				foreach (var arg in asm.LinkWith) {
					if (!arg.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase))
						continue;

					var item = new MSBuildItem {
						Include = arg,
						Metadata = new Dictionary<string, string> {
							{ "RelativePath", Path.Combine (Configuration.RelativeAppBundlePath, Configuration.Application.RelativeDylibPublishPath, Path.GetFileName (arg)) },
						},
					};
					dynamicLibraryToPublish.Add (item);
				}
			}
			Configuration.WriteOutputForMSBuild ("_DynamicLibraryToPublish", dynamicLibraryToPublish);
		}
	}
}
