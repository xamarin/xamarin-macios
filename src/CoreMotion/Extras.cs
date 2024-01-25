//
// CoreMotion's extra methods
//

#nullable enable

using Foundation;

using System;

namespace CoreMotion {

	public partial class CMAccelerometerData {
		public override string ToString ()
		{
			return String.Format ("t={0} {1}", Acceleration.ToString (), Timestamp);
		}
	}
}
