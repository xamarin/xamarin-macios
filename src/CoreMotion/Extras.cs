//
// CoreMotion's extra methods
//
using Foundation;
using System;

namespace CoreMotion {

#if !XAMCORE_2_0
	public partial class CMAttitude {

		[Obsolete ("This type is not meant to be created by application code")]
		public CMAttitude () : base (IntPtr.Zero)
		{
			// calling ToString, 'description' selector, would crash the application
		}
	}

	public partial class CMDeviceMotion {

		[Obsolete ("This type is not meant to be created by application code")]
		public CMDeviceMotion () : base (IntPtr.Zero)
		{
			// calling ToString, 'description' selector, would crash the application
		}
	}

	public partial class CMGyroData {

		[Obsolete ("This type is not meant to be created by application code")]
		public CMGyroData () : base (IntPtr.Zero)
		{
			// calling ToString, 'description' selector, would crash the application
		}
	}

	public partial class CMMagnetometerData {

		[Obsolete ("This type is not meant to be created by application code")]
		public CMMagnetometerData () : base (IntPtr.Zero)
		{
			// calling ToString, 'description' selector, would crash the application
		}
	}

	public partial class CMLogItem {

		[Obsolete ("This type is not meant to be created by application code")]
		public CMLogItem () : base (IntPtr.Zero)
		{
			// calling ObjC init would crash when calling it's Timestamp property
		}
	}
#endif

	public partial class CMAccelerometerData {

#if !XAMCORE_2_0
		[Obsolete ("This type is not meant to be created by application code")]
		public CMAccelerometerData ()
		{
			// calling ObjC init would crash when calling it's (base) Timestamp property
			// or Acceleration property - which is exactly what ToString does (and what
			// the debugger does since it calls ToString).
		}
#endif

		public override string ToString ()
		{
			return String.Format ("t={0} {1}", Acceleration.ToString (), Timestamp);
		}
	}
}