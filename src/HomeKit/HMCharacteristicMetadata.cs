using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	[iOS (8,0)]
	public partial class HMCharacteristicMetadata
	{
		public HMCharacteristicMetadataUnits Units {
			get {
				var u = _Units;

				if (u == HMCharacteristicMetadataUnitsInternal.Celsius)
					return HMCharacteristicMetadataUnits.Celsius;
				if (u == HMCharacteristicMetadataUnitsInternal.Fahrenheit)
					return HMCharacteristicMetadataUnits.Fahrenheit;
				if (u == HMCharacteristicMetadataUnitsInternal.Percentage)
					return HMCharacteristicMetadataUnits.Percentage;
				if (u == HMCharacteristicMetadataUnitsInternal.ArcDegree)
					return HMCharacteristicMetadataUnits.ArcDegree;
				if (u == HMCharacteristicMetadataUnitsInternal.Seconds)
					return HMCharacteristicMetadataUnits.Seconds;
				if (u == HMCharacteristicMetadataUnitsInternal.Lux)
					return HMCharacteristicMetadataUnits.Lux;

				return HMCharacteristicMetadataUnits.None;
			}
		}

		public HMCharacteristicMetadataFormat Format {
			get {
				var f = _Format;
				if (f == HMCharacteristicMetadataFormatKeys._Bool)
					return HMCharacteristicMetadataFormat.Bool;
				if (f == HMCharacteristicMetadataFormatKeys._Int)
					return HMCharacteristicMetadataFormat.Int;
				if (f == HMCharacteristicMetadataFormatKeys._Float)
					return HMCharacteristicMetadataFormat.Float;
				if (f == HMCharacteristicMetadataFormatKeys._String)
					return HMCharacteristicMetadataFormat.String;
				if (f == HMCharacteristicMetadataFormatKeys._Array)
					return HMCharacteristicMetadataFormat.Array;
				if (f == HMCharacteristicMetadataFormatKeys._Dictionary)
					return HMCharacteristicMetadataFormat.Dictionary;
				if (f == HMCharacteristicMetadataFormatKeys._UInt8)
					return HMCharacteristicMetadataFormat.UInt8;
				if (f == HMCharacteristicMetadataFormatKeys._UInt16)
					return HMCharacteristicMetadataFormat.UInt16;
				if (f == HMCharacteristicMetadataFormatKeys._UInt32)
					return HMCharacteristicMetadataFormat.UInt32;
				if (f == HMCharacteristicMetadataFormatKeys._UInt64)
					return HMCharacteristicMetadataFormat.UInt64;
				if (f == HMCharacteristicMetadataFormatKeys._Data)
					return HMCharacteristicMetadataFormat.Data;
				if (f == HMCharacteristicMetadataFormatKeys._Tlv8)
					return HMCharacteristicMetadataFormat.Tlv8;
				return HMCharacteristicMetadataFormat.None;
			}
		}
	}
}
