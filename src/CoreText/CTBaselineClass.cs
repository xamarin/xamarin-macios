// 
// CTBaselineClass.cs: CTFont name specifier constants
//
// Authors: Marek Safar (marek.safar@gmail.com)
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2012, 2014 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;

using ObjCRuntime;
using Foundation;

namespace CoreText {

	// Convenience enum for string values in ObjC.
	public enum CTBaselineClass {
		Roman,
		IdeographicCentered,
		IdeographicLow,
		IdeographicHigh,
		Hanging,
		Math,
	}

	static class CTBaselineClassID {
		public static readonly NSString Roman;
		public static readonly NSString IdeographicCentered;
		public static readonly NSString IdeographicLow;
		public static readonly NSString IdeographicHigh;
		public static readonly NSString Hanging;
		public static readonly NSString Math;

		static CTBaselineClassID ()
		{
			var handle = Libraries.CoreText.Handle;
			Roman = Dlfcn.GetStringConstant (handle, "kCTBaselineClassRoman");
			IdeographicCentered = Dlfcn.GetStringConstant (handle, "kCTBaselineClassIdeographicCentered");
			IdeographicLow = Dlfcn.GetStringConstant (handle, "kCTBaselineClassIdeographicLow");
			IdeographicHigh = Dlfcn.GetStringConstant (handle, "kCTBaselineClassIdeographicHigh");
			Hanging = Dlfcn.GetStringConstant (handle, "kCTBaselineClassHanging");
			Math = Dlfcn.GetStringConstant (handle, "kCTBaselineClassMath");
		}

		public static NSString ToNSString (CTBaselineClass key)
		{
			switch (key) {
				case CTBaselineClass.Roman:                return Roman;
				case CTBaselineClass.IdeographicCentered:  return IdeographicCentered;
				case CTBaselineClass.IdeographicLow:       return IdeographicLow;
				case CTBaselineClass.IdeographicHigh:      return IdeographicHigh;
				case CTBaselineClass.Hanging:              return Hanging;
				case CTBaselineClass.Math:                 return Math;
			}

			throw new ArgumentOutOfRangeException ("key");
		}

		public static CTBaselineClass FromHandle (IntPtr handle)
		{
			if (handle == Roman.Handle)                return CTBaselineClass.Roman;
			if (handle == IdeographicCentered.Handle)  return CTBaselineClass.IdeographicCentered;
			if (handle == IdeographicLow.Handle)       return CTBaselineClass.IdeographicLow;
			if (handle == IdeographicHigh.Handle)      return CTBaselineClass.IdeographicHigh;
			if (handle == Hanging.Handle)              return CTBaselineClass.Hanging;
			if (handle == Math.Handle)                 return CTBaselineClass.Math;

			throw new ArgumentOutOfRangeException ("handle");
		}
	}

	// Convenience enum for string values in ObjC.
	public enum CTBaselineFont {
		Reference,
		Original
	}

	static class CTBaselineFondID {
		public static readonly NSString Reference;
		public static readonly NSString Original;

		static CTBaselineFondID ()
		{
			var handle = Libraries.CoreText.Handle;
			Reference = Dlfcn.GetStringConstant (handle, "kCTBaselineReferenceFont");
			Original = Dlfcn.GetStringConstant (handle, "kCTBaselineOriginalFont");
		}

		public static NSString ToNSString (CTBaselineFont key)
		{
			switch (key) {
				case CTBaselineFont.Reference: return Reference;
				case CTBaselineFont.Original:  return Original;
			}

			throw new ArgumentOutOfRangeException ("key");
		}
	}
}

