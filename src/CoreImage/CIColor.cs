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
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using CoreGraphics;
#if !MONOMAC
using UIKit;
#endif

#nullable enable

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

		public static CIColor FromRgba (float red, float green, float blue, float alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromRgba (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha));
#else
			return FromRgba ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha);
#endif
		}

		// FIXME: availability attributes
		public static CIColor? FromRgba (float red, float green, float blue, float alpha, CGColorSpace colorSpace)
		{
#if NO_NFLOAT_OPERATORS
			return FromRgba (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha), colorSpace);
#else
			return FromRgba ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha, colorSpace);
#endif
		}

		public static CIColor? FromRgb (float red, float green, float blue)
		{
#if NO_NFLOAT_OPERATORS
			return FromRgb (new NFloat (red), new NFloat (green), new NFloat (blue));
#else
			return FromRgb ((nfloat) red, (nfloat) green, (nfloat) blue);
#endif
		}

		// FIXME: availability attributes
		public static CIColor? FromRgb (float red, float green, float blue, CGColorSpace colorSpace)
		{
#if NO_NFLOAT_OPERATORS
			return FromRgb (new NFloat (red), new NFloat (green), new NFloat (blue), colorSpace);
#else
			return FromRgb ((nfloat) red, (nfloat) green, (nfloat) blue, colorSpace);
#endif
		}

		public CIColor (float red, float green, float blue)
#if NO_NFLOAT_OPERATORS
			: this (new NFloat (red), new NFloat (green), new NFloat (blue))
#else
			: this ((nfloat) red, (nfloat) green, (nfloat) blue)
#endif
		{
		}

		// FIXME: availability attributes
		public CIColor (float red, float green, float blue, CGColorSpace colorSpace)
#if NO_NFLOAT_OPERATORS
			: this (new NFloat (red), new NFloat (green), new NFloat (blue), colorSpace)
#else
			: this ((nfloat) red, (nfloat) green, (nfloat) blue, colorSpace)
#endif
		{
		}

		public CIColor (float red, float green, float blue, float alpha)
#if NO_NFLOAT_OPERATORS
			: this (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha))
#else
			: this ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha)
#endif
		{

		}

		// FIXME: availability attributes
		public CIColor (float red, float green, float blue, float alpha, CGColorSpace colorSpace)
#if NO_NFLOAT_OPERATORS
			: this (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha), colorSpace)
#else
			: this ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha, colorSpace)
#endif
		{
		}

	}
}
