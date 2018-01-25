// 
// CGGeometry.cs: CGGeometry.h helpers
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2014 Xamarin Inc
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
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

	// untyped enum -> CGGeometry.h
	public enum CGRectEdge : uint {
		MinXEdge,
		MinYEdge,
		MaxXEdge,
		MaxYEdge,
	}

#if !COREBUILD && !XAMCORE_2_0
	public static class PointFExtensions {

		// CGGeometry.h
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CGPointCreateDictionaryRepresentation (CGPoint point);

		// This exact method is defined on CGPoint in the Unified API, so there's no need for the extension method.
		public static NSDictionary ToDictionary (this CGPoint self)
		{
			return new NSDictionary (CGPointCreateDictionaryRepresentation (self));
		}
	}

	public static class NSDictionaryExtensions {

		// CGGeometry.h
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPointMakeWithDictionaryRepresentation (/* CFDictionaryRef */ IntPtr dict, out CGPoint ret);

		// Use CGPoint.TryParse (NSDictionary, out CGPoint) instead in the Unified API.
		// Not sure how to best advice Classic API users here, since the method does 
		// not exist in the Classic API (so an Obsolete method can't point anywhere).
		public static bool ToPoint (this NSDictionary dictionary, out CGPoint point)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			
			return CGPointMakeWithDictionaryRepresentation (dictionary.Handle, out point);
		}
	}
#endif
	
	public static class RectangleFExtensions {

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGFloat */ nfloat CGRectGetMinX (CGRect rect);

		public static nfloat GetMinX (this CGRect self)
		{
			return CGRectGetMinX (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGFloat */ nfloat CGRectGetMidX (CGRect rect);

		public static nfloat GetMidX (this CGRect self)
		{
			return CGRectGetMidX (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGFloat */ nfloat CGRectGetMaxX (CGRect rect);

		public static nfloat GetMaxX (this CGRect self)
		{
			return CGRectGetMaxX (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGFloat */ nfloat CGRectGetMinY (CGRect rect);

		public static nfloat GetMinY (this CGRect self)
		{
			return CGRectGetMinY (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGFloat */ nfloat CGRectGetMidY (CGRect rect);

		public static nfloat GetMidY (this CGRect self)
		{
			return CGRectGetMidY (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGFloat */ nfloat CGRectGetMaxY (CGRect rect);

		public static nfloat GetMaxY (this CGRect self)
		{
			return CGRectGetMaxY (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGRect CGRectStandardize (CGRect rect);

		public static CGRect Standardize (this CGRect self)
		{
			return CGRectStandardize (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern bool CGRectIsNull (CGRect rect);

		public static bool IsNull (this CGRect self)
		{
			return CGRectIsNull (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern bool CGRectIsInfinite (CGRect rect);

		public static bool IsInfinite (this CGRect self)
		{
			return CGRectIsInfinite (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGRect CGRectInset (CGRect rect, /* GCFloat */ nfloat dx, /* GCFloat */ nfloat dy);

		public static CGRect Inset (this CGRect self, nfloat dx, nfloat dy)
		{
			return CGRectInset (self, dx, dy);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGRect CGRectIntegral (CGRect rect);

		public static CGRect Integral (this CGRect self)
		{
			return CGRectIntegral (self);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGRect CGRectUnion (CGRect r1, CGRect r2);

		public static CGRect UnionWith (this CGRect self, CGRect other)
		{
			return CGRectUnion (self, other);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern void CGRectDivide (CGRect rect, out CGRect slice, out CGRect remainder, /* GCFloat */ nfloat amount, CGRectEdge edge);

		public static void Divide (this CGRect self, nfloat amount, CGRectEdge edge, out CGRect slice, out CGRect remainder)
		{
			CGRectDivide (self, out slice, out remainder, amount, edge);
		}
	}
}

