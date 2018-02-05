using System;

using Foundation;
using CoreFoundation;

namespace CoreBluetooth {

	// The init ctor is invalid, but to present a nicer API (the delegate is optional/
	// hidden if events are desired) we fake it and provide a null delegate. This
	// is intentional and should not be obsoleted like the others below.
	public partial class CBCentralManager {

		public CBCentralManager () : this (new _CBCentralManagerDelegate (), null)
		{
		}

		public CBCentralManager (DispatchQueue dispatchQueue) : this (new _CBCentralManagerDelegate (), dispatchQueue)
		{
		}
	}

#if !MONOMAC && !XAMCORE_2_0
	// most default .ctor MonoTouch provided turns out to be invalid
	// they crash when used on OSX -> Xamarin.Mac
	// they don't really work on iOS either @ http://stackoverflow.com/a/13660275/220643

	public partial class CBATTRequest {
		
		[Obsolete ("This type is not meant to be created by user code.", true)]
		public CBATTRequest ()
		{
		}
	}

	public partial class CBCentral {
		
		[Obsolete ("This type is not meant to be created by user code.", true)]
		public CBCentral ()
		{
		}
	}

	public partial class CBCharacteristic {
		
		[Obsolete ("This type is not meant to be created by user code. Use CBMutableCharacteristic", true)]
		public CBCharacteristic ()
		{
		}
	}

	public partial class CBMutableCharacteristic {
		
		[Obsolete ("Use .ctor(CBUUID,CBCharacteristicProperties,NSData,CBAttributePermissions)", true)]
		public CBMutableCharacteristic ()
		{
		}
	}

	public partial class CBDescriptor {
		
		[Obsolete ("This type is not meant to be created by user code. Use CBMutableDescriptor", true)]
		public CBDescriptor ()
		{
		}
	}

	public partial class CBMutableDescriptor {
		
		[Obsolete ("Use .ctor(CBUUID,NSObject)", true)]
		public CBMutableDescriptor ()
		{
		}
	}

	public partial class CBPeripheral {
		
		[Obsolete ("This type is not meant to be created by user code.", true)]
		public CBPeripheral ()
		{
		}
	}

	public partial class CBService {
		
		[Obsolete ("This type is not meant to be created by user code. Use CBMutableService", true)]
		public CBService ()
		{
		}
	}

	public partial class CBMutableService {
		
		[Obsolete ("Use .ctor (CBUUID,bool)", true)]
		public CBMutableService ()
		{
		}
	}

	public partial class CBUUID {

		[Obsolete ("Use FromString or FromData to create a valid CBUUID instance", true)]
		public CBUUID ()
		{
		}
	}
#endif
#if !MONOMAC && !XAMCORE_3_0
	public partial class CBPeer {

		[Obsolete ("This type is not meant to be created by user code.", true)]
		public CBPeer ()
		{
		}
	}
#endif
#if !WATCH && !XAMCORE_4_0
	public partial class CBCentralManager {

		public new virtual CBCentralManagerState State {
			get {
				return (CBCentralManagerState)base.State;
			}
		}
	}

	public partial class CBPeripheralManager {

		public virtual CBPeripheralManagerState State {
			get {
				return (CBPeripheralManagerState)base.State;
			}
		}
	}
#endif
}
