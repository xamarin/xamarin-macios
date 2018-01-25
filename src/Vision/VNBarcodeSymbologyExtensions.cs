//
// VNBarcodeSymbologyExtensions.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using Foundation;

namespace Vision {
	public static partial class VNBarcodeSymbologyExtensions {
		public static NSString [] GetConstants (this VNBarcodeSymbology [] self)
		{
			if (self == null)
				throw new ArgumentNullException (nameof (self));

			var array = new NSString [self.Length];
			for (int n = 0; n < self.Length; n++)
				array [n] = self [n].GetConstant ();
			return array;
		}

		public static VNBarcodeSymbology [] GetValues (NSString [] constants)
		{
			if (constants == null)
				throw new ArgumentNullException (nameof (constants));

			var array = new VNBarcodeSymbology [constants.Length];
			for (int n = 0; n < constants.Length; n++)
				array [n] = GetValue (constants [n]);
			return array;
		}
	}
}
#endif
