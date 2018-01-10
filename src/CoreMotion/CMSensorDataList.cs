//
// CMSensorDataList.cs:
//
// Copyright 2015, Xamarin Inc.
//
// Authors:
//   Rolf Bjarne Kvinge
//

using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;

namespace CoreMotion {
	public partial class CMSensorDataList : IEnumerable<CMAccelerometerData> {
#region IEnumerable implementation
		public IEnumerator<CMAccelerometerData> GetEnumerator ()
		{
			return new NSFastEnumerator<CMAccelerometerData> (this);
		}
#endregion

#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
#endregion
	}
}