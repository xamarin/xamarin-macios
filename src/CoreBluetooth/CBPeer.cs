#if IOS
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;
using XamCore.CoreFoundation;

namespace XamCore.CoreBluetooth {
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
