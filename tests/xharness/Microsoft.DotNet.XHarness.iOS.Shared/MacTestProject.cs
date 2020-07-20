using System;
namespace Microsoft.DotNet.XHarness.iOS.Shared {

	[Flags]
	public enum MacFlavors {
		Modern = 1, // Xamarin.Mac/Modern app
		Full = 2, // Xamarin.Mac/Full app
		System = 4, // Xamarin.Mac/System app
		Console = 8, // Console executable
	}

	public class MacTestProject : TestProject
	{
		public MacFlavors TargetFrameworkFlavors;

		public bool GenerateFull => GenerateVariations && (TargetFrameworkFlavors & MacFlavors.Full) == MacFlavors.Full;
		public bool GenerateSystem => GenerateVariations && (TargetFrameworkFlavors & MacFlavors.System) == MacFlavors.System;

		public override bool GenerateVariations {
			get {
				// If a bitwise combination of flavors, then we're generating variations
				return TargetFrameworkFlavors != MacFlavors.Modern && TargetFrameworkFlavors != MacFlavors.Full && TargetFrameworkFlavors != MacFlavors.System && TargetFrameworkFlavors != MacFlavors.Console;
			}
			set {
				throw new Exception ("This value is read-only");
			}
		}

		public string Platform = "x86";

		public MacTestProject () : base ()
		{
		}

		public MacTestProject (string path, bool isExecutableProject = true, MacFlavors targetFrameworkFlavor = MacFlavors.Full | MacFlavors.Modern) : base (path, isExecutableProject)
		{
			TargetFrameworkFlavors = targetFrameworkFlavor;
		}

		public override TestProject Clone()
		{
			var rv = (MacTestProject) base.Clone ();
			rv.TargetFrameworkFlavors = TargetFrameworkFlavors;
			rv.Platform = Platform;
			return rv;
		}

		public override string ToString ()
		{
			return base.ToString () + " (" + TargetFrameworkFlavors.ToString () + ")";
		}
	}
}
