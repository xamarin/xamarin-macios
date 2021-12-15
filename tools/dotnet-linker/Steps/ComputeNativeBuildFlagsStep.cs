using System.Collections.Generic;

using Xamarin.Utils;

namespace Xamarin.Linker {
	// The static registrar may need access to information that has been linked away,
	// in particular types and interfaces, so we need to store those somewhere
	// so that the static registrar can access them.
	public class ComputeNativeBuildFlagsStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Compute Native Build Flags";
		protected override int ErrorCode { get; } = 2340;

		protected override void TryEndProcess ()
		{
			base.TryEndProcess ();

			var linkerFrameworks = new List<MSBuildItem> ();

			switch (Configuration.Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.MacCatalyst:
				linkerFrameworks.Add (new MSBuildItem {
					Include = "GSS",
					Metadata = { { "IsWeak", "false" } },
				});
				break;
			}

			Configuration.WriteOutputForMSBuild ("_LinkerFrameworks", linkerFrameworks);

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
			Configuration.WriteOutputForMSBuild ("_AssemblyLinkerFlags", linkerFlags);

			if (Configuration.Application.CustomLinkFlags?.Count > 0)
				Configuration.Application.DeadStrip = false;

			var mainLinkerFlags = new List<MSBuildItem> ();
			if (Configuration.Application.DeadStrip) {
				mainLinkerFlags.Add (new MSBuildItem { Include = "-dead_strip" });
			}
			Configuration.WriteOutputForMSBuild ("_AssemblyLinkerFlags", mainLinkerFlags);

		}
	}
}
