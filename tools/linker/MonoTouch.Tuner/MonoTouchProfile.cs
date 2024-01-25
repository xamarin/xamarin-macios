using System;

using Xamarin.Linker;

namespace MonoTouch.Tuner {

	public class MonoTouchProfile : MobileProfile {
		string product_assembly;

		protected override bool IsProduct (string assemblyName)
		{
			return assemblyName == product_assembly;
		}

		public override string ProductAssembly {
			get { return product_assembly; }
		}

		public void SetProductAssembly (string assembly)
		{
			product_assembly = assembly;
		}
	}
}
