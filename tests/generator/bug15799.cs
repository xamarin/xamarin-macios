using System;
using System.Drawing;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreBluetooth;

namespace Bug15799 {

	// Device Manager
	[BaseType (typeof (NSObject))]
	public partial interface Foo : ICBCentralManagerDelegate, ICBPeripheralDelegate {
	}
}