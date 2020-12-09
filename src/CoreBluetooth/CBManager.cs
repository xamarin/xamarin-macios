#if IOS || WATCH
using System;
using ObjCRuntime;

#nullable enable

namespace CoreBluetooth {
	public partial class CBManager {

		[iOS (13,0), Watch (6,0)]
		public static CBManagerAuthorization Authorization {
			get {
				// in iOS 13.1 / Watch 6.1 this is a static property, like other [tv|mac]OS
#if IOS
				if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (13, 1)) {
#elif WATCH
				if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (6, 1)) {
#endif
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
