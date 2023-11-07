using System;
using System.Drawing;
using ObjCRuntime;
using Foundation;
using UIKit;
using CoreBluetooth;

namespace Bug15799 {

	// Device Manager
	[BaseType (typeof (NSObject))]
	public partial interface Foo : ICBCentralManagerDelegate, ICBPeripheralDelegate {
	}
}
