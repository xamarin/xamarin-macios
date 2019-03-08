using System;

namespace xharness
{
	public class MacTarget : Target
	{
		public bool ThirtyTwoBit;

		public MonoNativeInfo MonoNativeInfo { get; set; }

		protected override bool FixProjectReference (string name)
		{
			switch (name) {
			case "GuiUnit_NET_4_5":
				return false;
			default:
				return base.FixProjectReference (name);
			}
		}
		public string SimplifiedName {
			get {
				return Name.EndsWith ("-mac", StringComparison.Ordinal) ? Name.Substring (0, Name.Length - 4) : Name;
			}
		}
	}
}

