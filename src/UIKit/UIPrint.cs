#if !XAMCORE_3_0

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace UIKit {
	public static class UIPrint {

		static NSString _ErrorDomain;
		public static NSString ErrorDomain {
			get {
				if (_ErrorDomain == null)
					_ErrorDomain = Dlfcn.GetStringConstant (Libraries.UIKit.Handle, "UIPrintErrorDomain");
				return _ErrorDomain;
			}
		}
	
	}
}

#endif
