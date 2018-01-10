#if !WATCH
using System;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	public partial class UINavigationController {
		static IntPtr LookupClass (Type t)
		{
			return t == null ? IntPtr.Zero : Class.GetHandle (t);
		}
		
		public UINavigationController (Type navigationBarType, Type toolbarType) : this (LookupClass (navigationBarType), LookupClass (toolbarType))
		{
		}
						      
	}
}

#endif // !WATCH
