using System;

namespace MonoMac.Tuner {

	class XamarinMacProfile : MacBaseProfile {

		readonly int bits;

		public XamarinMacProfile (int arch)
		{
			bits = arch;
		}

		public bool Is32Bits {
			get { return bits == 32; }
		}

		public bool Is64Bits {
			get { return bits == 64; }
		}

		public override string ProductAssembly {
			get { return "Xamarin.Mac"; }
		}
	}
}
