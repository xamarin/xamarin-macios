//
// UIColor.cs: Extensions to UIColor
//
// Authors:
//   Geoff Norton
//
// Copyright 2009, Novell, Inc.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
#if !COREBUILD
using CoreGraphics;
#endif

namespace UIKit {

	public partial class UIColor {

		public static UIColor FromRGB (double red, double green, double blue)
		{
			return FromRGBA (red, green, blue, 1.0);
		}

		public static UIColor FromRGB (nfloat red, nfloat green, nfloat blue)
		{
#if NO_NFLOAT_OPERATORS
			return FromRGBA (red, green, blue, new NFloat (1.0f));
#else
			return FromRGBA (red, green, blue, 1.0f);
#endif
		}

		public static UIColor FromRGB (byte red, byte green, byte blue)
		{
			return FromRGBA (red/255.0f, green/255.0f, blue/255.0f, 1.0f);
		}

		public static UIColor FromRGB (int red, int green, int blue)
		{
			return FromRGB ((byte) red, (byte) green, (byte) blue);
		}

		public static UIColor FromRGBA (byte red, byte green, byte blue, byte alpha)
		{
			return FromRGBA (red/255.0f, green/255.0f, blue/255.0f, alpha/255.0f);
		}

		public static UIColor FromRGBA (double red, double green, double blue, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromRGBA (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha));
#else
			return FromRGBA ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha);
#endif
		}

		public static UIColor FromRGBA (int red, int green, int blue, int alpha)
		{
			return FromRGBA ((byte) red, (byte) green, (byte) blue, (byte) alpha);
		}

		public static UIColor FromHSB (nfloat hue, nfloat saturation, nfloat brightness)
		{
#if NO_NFLOAT_OPERATORS
			return FromHSBA (hue, saturation, brightness, new NFloat (1.0f));
#else
			return FromHSBA (hue, saturation, brightness, 1.0f);
#endif
		}
		
		// note: replacing this managed code with "getRed:green:blue:alpha:" would break RGB methods
		public void GetRGBA (out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha)
		{
			using (var cv = CGColor){
				nfloat [] result = cv.Components;

				switch (result.Length) {
					case 2:
						red = result[0];
						green = result[0];
						blue = result[0];
						alpha = result[1];
						break;
					case 3:
						red = result[0];
						green = result[1];
						blue = result[2];
#if NO_NFLOAT_OPERATORS
						alpha = new NFloat (1.0f);
#else
						alpha = 1.0f;
#endif
						break;
					case 4:
						red = result[0];
						green = result[1];
						blue = result[2];
						alpha = result[3];
						break;
					default:
						throw new Exception ("Unsupported colorspace component length: " + result.Length);
				}
			}
		}
		
		static nfloat Max (nfloat a, nfloat b)
		{
#if NO_NFLOAT_OPERATORS
			return a.Value > b.Value ? a : b;
#else
			return a > b ? a : b;
#endif
		}

		static nfloat Min (nfloat a, nfloat b)
		{
#if NO_NFLOAT_OPERATORS
			return a.Value < b.Value ? a : b;
#else
			return a < b ? a : b;
#endif
		}

		// note: replacing this managed code with "getHue:saturation:brightness:alpha:" would break HSB methods
		public void GetHSBA (out nfloat hue, out nfloat saturation, out nfloat brightness, out nfloat alpha)
		{
			using (var cv = CGColor){
				nfloat [] result = cv.Components;
				nfloat red, green, blue;

				switch (result.Length) {
					case 2:
						red = result[0];
						green = result[0];
						blue = result[0];
						alpha = result[1];
						break;
					case 3:
						red = result[0];
						green = result[1];
						blue = result[2];
#if NO_NFLOAT_OPERATORS
						alpha = new NFloat (1.0f);
#else
						alpha = 1.0f;
#endif
						break;
					case 4:
						red = result[0];
						green = result[1];
						blue = result[2];
						alpha = result[3];
						break;
					default:
						throw new Exception ("Unsupported colorspace component length: " + result.Length);
				}
				

				var maxv = Max (red, Max (green, blue));
				var minv = Min (red, Min (green, blue));
				  
				brightness = maxv;

#if NO_NFLOAT_OPERATORS
				var delta = maxv.Value-minv.Value;
				if (maxv.Value != 0.0)
					saturation = new NFloat (delta/maxv.Value);
				else
					saturation = new NFloat (0);

				if (saturation.Value == 0)
					hue = new NFloat (0);
				else {
					if (red.Value == brightness.Value)
						hue = new NFloat ((green.Value - blue.Value) / delta);
					else if (green.Value == maxv.Value)
						hue = new NFloat (2.0f + (blue.Value-red.Value)/delta);
					else
						hue = new NFloat (4.0f + (red.Value-green.Value)/delta);

					hue = new NFloat (hue.Value / 6.0f);
					if (hue.Value <= 0.0f)
						hue = new NFloat (hue.Value + 1.0f);
				}
#else
				var delta = maxv-minv;
				if (maxv != 0.0)
					saturation = delta/maxv;
				else
					saturation = 0;

				if (saturation == 0)
					hue = 0;
				else {
					if (red == brightness)
						hue = (green - blue) / delta;
					else if (green == maxv)
						hue = 2.0f + (blue-red)/delta;
					else
						hue = 4.0f + (red-green)/delta;

					hue = hue / 6.0f;
					if (hue <= 0.0f)
						hue += 1.0f;
				}
#endif
			}
		}
		
		public override string ToString ()
		{
			nfloat r, g, b, a;
			try {
				GetRGBA (out r, out g, out b, out a);
				return String.Format ("UIColor [A={0}, R={1}, G={2}, B={3}]",
#if NO_NFLOAT_OPERATORS
					(byte) (a.Value * 255), (byte) (r.Value * 255), (byte) (g.Value * 255), (byte) (b.Value * 255));
#else
					(byte) (a * 255), (byte) (r * 255), (byte) (g * 255), (byte) (b * 255));
#endif
			}
			catch (Exception) {
				// e.g. patterns will return "kCGColorSpaceModelPattern 1", see bug #7362
				return Description;
			}
		}
		
#if false
		public static UIColor FromColor (System.Drawing.Color color)
		{
			return new UIColor (color.R/255.0f, color.G/255.0f, color.B/255.0f, color.A/255.0f);
		}
#endif
	}
}
