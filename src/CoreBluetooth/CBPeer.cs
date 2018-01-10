#if IOS
using ObjCRuntime;
using Foundation;
using System;
using CoreFoundation;

namespace CoreBluetooth {
	public partial class CBPeer  {
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
		public virtual CBUUID UUID { 
			get {
				return CBUUID.FromCFUUID (_UUID);	
			}
		}
	}
}
#endif
