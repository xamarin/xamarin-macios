// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using Xamarin.Linker;

namespace MonoMac.Tuner {

	class MacMobileProfile : MobileProfile {

		readonly int bits;

		public MacMobileProfile (int arch)
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

		protected override bool IsProduct (string assemblyName)
		{
			return assemblyName == ProductAssembly;
		}
	}
}