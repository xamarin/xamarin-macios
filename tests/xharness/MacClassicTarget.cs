using System;

namespace xharness
{
	public class MacClassicTarget : MacTarget
	{
		public MacClassicTarget ()
		{
			ThirtyTwoBit = true;
		}

		protected override void ExecuteInternal ()
		{
			// nothing to do here
		}

		public override string Suffix {
			get {
				return "-classic";
			}
		}

		public override string Platform {
			get {
				return "mac";
			}
		}

		public override string MakefileWhereSuffix {
			get {
				return "mac-classic";
			}
		}

		public override string ProjectFileSuffix {
			get {
				return string.Empty;
			}
		}
	}
}
