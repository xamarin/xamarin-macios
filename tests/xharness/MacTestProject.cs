using System;
namespace Xharness {

	[Flags]
	public enum MacFlavors {
		None = 0,
		DotNet = 16,
		MacCatalyst = 32,
	}

	public class MacTestProject : TestProject {
		public MacFlavors TargetFrameworkFlavors;

		public override bool GenerateVariations {
			get {
				return false;
			}
			set {
				throw new Exception ("This value is read-only");
			}
		}

		public MacTestProject (TestLabel label, string path)
			: base (label, path, true)
		{
		}

		public override TestProject Clone ()
		{
			return CompleteClone (new MacTestProject (Label, Path));
		}

		protected override TestProject CompleteClone (TestProject project)
		{
			var rv = (MacTestProject) project;
			rv.TargetFrameworkFlavors = TargetFrameworkFlavors;
			return base.CompleteClone (rv);
		}

		public override string ToString ()
		{
			return base.ToString () + " (" + TargetFrameworkFlavors.ToString () + ")";
		}
	}
}
