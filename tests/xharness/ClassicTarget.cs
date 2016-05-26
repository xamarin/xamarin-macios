using System;

namespace xharness
{
	public class ClassicTarget : Target
	{
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
				return "ios";
			}
		}

		public override string MakefileWhereSuffix {
			get {
				return "classic";
			}
		}

		public override string ProjectFileSuffix {
			get {
				return string.Empty;
			}
		}
	}
}

