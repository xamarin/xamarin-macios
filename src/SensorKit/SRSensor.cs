using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace SensorKit {

	public partial class SRSensorExtensions {

		public static SRSensor GetSensorForDeletionRecords (this SRSensor self)
		{
			var constant = self.GetConstant ();
			if (constant is null)
				return SRSensor.Invalid;
			return GetValue (constant._GetSensorForDeletionRecordsFromSensor ());
		}
	}
}
