//
// VNDetectBarcodesRequest.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;

namespace Vision {
	public partial class VNDetectBarcodesRequest {

		public VNBarcodeSymbology [] Symbologies {
			get { return VNBarcodeSymbologyExtensions.GetValues (WeakSymbologies); }
			set { WeakSymbologies = value.GetConstants (); }
		}
	}
}
#endif
