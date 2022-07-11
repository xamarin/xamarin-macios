using System;
namespace Xharness {

	[Flags]
	public enum MacFlavors {
		Modern = 1, // Xamarin.Mac/Modern app
		Full = 2, // Xamarin.Mac/Full app
		System = 4, // Xamarin.Mac/System app
		Console = 8, // Console executable
		DotNet = 16,
		MacCatalyst = 32,
	}

	public class MacTestProject : TestProject {
		public MacFlavors TargetFrameworkFlavors;

		public bool GenerateFull => GenerateVariations && (TargetFrameworkFlavors & MacFlavors.Full) == MacFlavors.Full;
		public bool GenerateSystem => GenerateVariations && (TargetFrameworkFlavors & MacFlavors.System) == MacFlavors.System;

		public override bool GenerateVariations {
			get {
				if (IsDotNetProject)
					return false;

				// If a bitwise combination of flavors, then we're generating variations
				return TargetFrameworkFlavors != MacFlavors.Modern && TargetFrameworkFlavors != MacFlavors.Full && TargetFrameworkFlavors != MacFlavors.System && TargetFrameworkFlavors != MacFlavors.Console;
			}
			set {
				throw new Exception ("This value is read-only");
			}
		}

		public string Platform = "x86";

		public MacTestProject (TestLabel label, string path, bool isExecutableProject = true, MacFlavors targetFrameworkFlavor = MacFlavors.Full | MacFlavors.Modern) : base (label, path, isExecutableProject)
		{
			TargetFrameworkFlavors = targetFrameworkFlavor;
		}

		public override TestProject Clone ()
		{
			return CompleteClone (new MacTestProject (Label, Path, IsExecutableProject, TargetFrameworkFlavors));
		}

		protected override TestProject CompleteClone (TestProject project)
		{
			var rv = (MacTestProject) project;
			rv.Platform = Platform;
			return base.CompleteClone (rv);
		}

		public override string ToString ()
		{
			return base.ToString () + " (" + TargetFrameworkFlavors.ToString () + ")";
		}
	}
}
