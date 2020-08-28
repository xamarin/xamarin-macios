using System;
using Xamarin.Linker;

namespace MonoTouch.Tuner {

	public class MonoTouchProfile : MobileProfile {
		string product_assembly;

		protected override bool IsProduct (string assemblyName)
		{
			switch (assemblyName) {
			case "MonoTouch.Dialog-1":
			case "MonoTouch.NUnitLite":
				return true;
			default:
				return assemblyName == product_assembly;
			}
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
