// Copyright 2013 Xamarin Inc. All rights reserved.

using System;

using Xamarin.Linker;

namespace MonoMac.Tuner {

	class MacMobileProfile : MobileProfile {
		public override string ProductAssembly {
			get { return "Xamarin.Mac"; }
		}

		protected override bool IsProduct (string assemblyName)
		{
			return assemblyName == ProductAssembly;
		}
	}
}
