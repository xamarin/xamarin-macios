using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

#nullable enable

namespace SensorKit {

	public partial class SRSensorExtensions {

		[NoWatch, NoTV, NoMac]
		[iOS (14,0)]
		public static SRSensor GetSensorForDeletionRecords (this SRSensor self)
		{
			var constant = self.GetConstant ();
			if (constant == null)
				return SRSensor.Invalid;
			return GetValue (constant._GetSensorForDeletionRecordsFromSensor ());
		}
	}
}
