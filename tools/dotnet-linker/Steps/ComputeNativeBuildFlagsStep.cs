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
		}
	}
}
