//
// CIColor.cs: Extensions
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2014 Xamarin Inc.
//
using System;
using System.Diagnostics;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
#if !MONOMAC
using UIKit;
#endif

namespace CoreImage {
	public partial class CIColor {

		public nfloat [] Components {
			get {
				var n = NumberOfComponents;
				var result = new nfloat [n];
				unsafe {
					nfloat *p = (nfloat *) GetComponents ();
					for (int i = 0; i < n; i++)
						result [i] = p [i];
				}
				return result;
			}
		}
	}
}
