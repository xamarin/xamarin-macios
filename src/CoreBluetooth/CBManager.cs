#if IOS
using System;
using UIKit;

namespace CoreBluetooth {
	public partial class CBManager {

		[iOS (13,0)]
		public static CBManagerAuthorization Authorization {
			get {
				// in iOS 13.1 ithis is a static property, like other [tv|mac]OS
				if (UIDevice.CurrentDevice.CheckSystemVersion (13,1)) {
					return _SAuthorization;
				} else {
					// in iOS 13.0 this was, shortly (deprecated in 13.1), an instance property
					return new CBCentralManager ()._IAuthorization;
				}
			}
		}
	}
}
#endif
