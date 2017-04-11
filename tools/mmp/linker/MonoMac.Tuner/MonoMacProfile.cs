using System;

namespace MonoMac.Tuner {

	class MonoMacProfile : MacBaseProfile {

		public override string ProductAssembly {
			get { return "XamMac"; }
		}

		public override string GetNamespace (string nspace)
		{
			return "MonoMac." + base.GetNamespace (nspace);
		}
	}
}
