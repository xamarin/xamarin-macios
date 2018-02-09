using System;

namespace Foundation {
	
	public partial class NSDimension {

		// this is something that need to be overridden by subclasses, which is not something we _usually_ do in C#
		// it is exposed here so we can throw a managed exception if some subclasses (e.g. user code) fails to override
		// (re-declare with `new`) the static property
		public static NSDimension BaseUnit {

			get {
				throw new NotImplementedException ("All subclasses of NSDimension must re-implement this property");
			}
		}
	}
}
