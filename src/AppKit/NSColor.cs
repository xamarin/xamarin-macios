#if !__MACCATALYST__
using System;
using System.Text;
using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;

namespace AppKit {
	public partial class NSColor {

		public static NSColor FromRgb (nfloat red, nfloat green, nfloat blue)
		{
#if NO_NFLOAT_OPERATORS
			return FromRgba (red, green, blue, new NFloat (1.0f));
#else
			return FromRgba (red, green, blue, 1.0f);
#endif
		}

		public static NSColor FromRgb (byte red, byte green, byte blue)
		{
			return FromRgba (red/255.0f, green/255.0f, blue/255.0f, 1.0f);
		}

		public static NSColor FromRgb (int red, int green, int blue)
		{
			return FromRgb ((byte) red, (byte) green, (byte) blue);
		}

		public static NSColor FromRgba (byte red, byte green, byte blue, byte alpha)
		{
			return FromRgba (red/255.0f, green/255.0f, blue/255.0f, alpha/255.0f);
		}

		public static NSColor FromRgba (int red, int green, int blue, int alpha)
		{
			return FromRgba ((byte) red, (byte) green, (byte) blue, (byte) alpha);
		}

		public static NSColor FromRgba (double red, double green, double blue, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromRgba (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha));
#else
			return FromRgba ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha);
#endif
		}

		public static NSColor FromHsb (nfloat hue, nfloat saturation, nfloat brightness)
		{
#if NO_NFLOAT_OPERATORS
			return FromHsba (hue, saturation, brightness, new NFloat (1.0f));
#else
			return FromHsba (hue, saturation, brightness, 1.0f);
#endif
		}

		public static NSColor FromHsba (byte hue, byte saturation, byte brightness, byte alpha)
		{
			return FromHsba (hue / 255.0f, saturation / 255.0f, brightness / 255.0f, alpha / 255.0f);
		}

		public static NSColor FromHsba (double hue, double saturation, double brightness, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromHsba (new NFloat (hue), new NFloat (saturation), new NFloat (brightness), new NFloat (alpha));
#else
			return FromHsba ((nfloat) hue, (nfloat) saturation, (nfloat) brightness, (nfloat) alpha);
#endif
		}

		public static NSColor FromHsb (byte hue, byte saturation, byte brightness)
		{
			return FromHsba (hue / 255.0f, saturation / 255.0f, brightness / 255.0f, 1.0f);
		}

		public static NSColor FromHsba (int hue, int saturation, int brightness, int alpha)
		{
			return FromHsba ((byte) hue, (byte) saturation, (byte) brightness, (byte) alpha);
		}

		public static NSColor FromHsb (int hue, int saturation, int brightness)
		{
			return FromHsb ((byte) hue, (byte) saturation, (byte) brightness);
		}

		public static NSColor FromDeviceRgb (nfloat red, nfloat green, nfloat blue)
		{
#if NO_NFLOAT_OPERATORS
			return FromDeviceRgba (red, green, blue, new NFloat (1.0f));
#else
			return FromDeviceRgba (red, green, blue, 1.0f);
#endif
		}

		public static NSColor FromDeviceRgb (byte red, byte green, byte blue)
		{
			return FromDeviceRgba (red/255.0f, green/255.0f, blue/255.0f, 1.0f);
		}

		public static NSColor FromDeviceRgb (int red, int green, int blue)
		{
			return FromDeviceRgb ((byte) red, (byte) green, (byte) blue);
		}

		public static NSColor FromDeviceRgba (byte red, byte green, byte blue, byte alpha)
		{
			return FromDeviceRgba (red/255.0f, green/255.0f, blue/255.0f, alpha/255.0f);
		}

		public static NSColor FromDeviceRgba (int red, int green, int blue, int alpha)
		{
			return FromDeviceRgba ((byte) red, (byte) green, (byte) blue, (byte) alpha);
		}

		public static NSColor FromDeviceRgba (double red, double green, double blue, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromDeviceRgba (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha));
#else
			return FromDeviceRgba ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha);
#endif
		}

		public static NSColor FromDeviceHsb (nfloat hue, nfloat saturation, nfloat brightness)
		{
#if NO_NFLOAT_OPERATORS
			return FromDeviceHsba (hue, saturation, brightness, new NFloat (1.0f));
#else
			return FromDeviceHsba (hue, saturation, brightness, 1.0f);
#endif
		}

		public static NSColor FromDeviceHsba (byte hue, byte saturation, byte brightness, byte alpha)
		{
			return FromDeviceHsba (hue / 255.0f, saturation / 255.0f, brightness / 255.0f, alpha / 255.0f);
		}

		public static NSColor FromDeviceHsb (byte hue, byte saturation, byte brightness)
		{
			return FromDeviceHsba (hue / 255.0f, saturation / 255.0f, brightness / 255.0f, 1.0f);
		}

		public static NSColor FromDeviceHsba (int hue, int saturation, int brightness, int alpha)
		{
			return FromDeviceHsba ((byte) hue, (byte) saturation, (byte) brightness, (byte) alpha);
		}

		public static NSColor FromDeviceHsba (double hue, double saturation, double brightness, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromDeviceHsba (new NFloat (hue), new NFloat (saturation), new NFloat (brightness), new NFloat (alpha));
#else
			return FromDeviceHsba ((nfloat) hue, (nfloat) saturation, (nfloat) brightness, (nfloat) alpha);
#endif
		}

		public static NSColor FromDeviceHsb (int hue, int saturation, int brightness)
		{
			return FromDeviceHsb ((byte) hue, (byte) saturation, (byte) brightness);
		}

		public static NSColor FromDeviceHsb (double hue, double saturation, double brightness)
		{
#if NO_NFLOAT_OPERATORS
			return FromDeviceHsb (new NFloat (hue), new NFloat (saturation), new NFloat (brightness));
#else
			return FromDeviceHsb ((nfloat) hue, (nfloat) saturation, (nfloat) brightness);
#endif
		}

		public static NSColor FromDeviceCymk (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black)
		{
#if NO_NFLOAT_OPERATORS
			return FromDeviceCymka (cyan, magenta, yellow, black, new NFloat (1.0f));
#else
			return FromDeviceCymka (cyan, magenta, yellow, black, 1.0f);
#endif
		}

		public static NSColor FromDeviceCymka (double cyan, double magenta, double yellow, double black, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromDeviceCymka (new NFloat (cyan), new NFloat (magenta), new NFloat (yellow), new NFloat (black), new NFloat (alpha));
#else
			return FromDeviceCymka ((nfloat) cyan, (nfloat) magenta, (nfloat) yellow, (nfloat) black, (nfloat) alpha);
#endif
		}

		public static NSColor FromDeviceCymka (byte cyan, byte magenta, byte yellow, byte black, byte alpha)
		{
			return FromDeviceCymka (cyan / 255.0f, magenta / 255.0f, yellow / 255.0f, black / 255.0f, alpha / 255.0f);
		}

		public static NSColor FromDeviceCymk (byte cyan, byte magenta, byte yellow, byte black)
		{
			return FromDeviceCymka (cyan / 255.0f, magenta / 255.0f, yellow / 255.0f, black / 255.0f, 1.0f);
		}

		public static NSColor FromDeviceCymka (int cyan, int magenta, int yellow, int black, int alpha)
		{
			return FromDeviceCymka ((byte) cyan, (byte) magenta, (byte) yellow, (byte) black, (byte) alpha);
		}

		public static NSColor FromDeviceCymk (int cyan, int magenta, int yellow, int black)
		{
			return FromDeviceCymk ((byte) cyan, (byte) magenta, (byte) yellow, (byte) black);
		}

		public static NSColor FromCalibratedRgb (nfloat red, nfloat green, nfloat blue)
		{
#if NO_NFLOAT_OPERATORS
			return FromCalibratedRgba (red, green, blue, new NFloat (1.0f));
#else
			return FromCalibratedRgba (red, green, blue, 1.0f);
#endif
		}

		public static NSColor FromCalibratedRgb (byte red, byte green, byte blue)
		{
			return FromCalibratedRgba (red/255.0f, green/255.0f, blue/255.0f, 1.0f);
		}

		public static NSColor FromCalibratedRgb (int red, int green, int blue)
		{
			return FromCalibratedRgb ((byte) red, (byte) green, (byte) blue);
		}

		public static NSColor FromCalibratedRgba (byte red, byte green, byte blue, byte alpha)
		{
			return FromCalibratedRgba (red/255.0f, green/255.0f, blue/255.0f, alpha/255.0f);
		}

		public static NSColor FromCalibratedRgba (int red, int green, int blue, int alpha)
		{
			return FromCalibratedRgba ((byte) red, (byte) green, (byte) blue, (byte) alpha);
		}

		public static NSColor FromCalibratedRgba (double red, double green, double blue, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromCalibratedRgba (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha));
#else
			return FromCalibratedRgba ((nfloat) red, (nfloat) green, (nfloat) blue, (nfloat) alpha);
#endif
		}

		public static NSColor FromCalibratedHsb (nfloat hue, nfloat saturation, nfloat brightness)
		{
#if NO_NFLOAT_OPERATORS
			return FromCalibratedHsba (hue, saturation, brightness, new NFloat (1.0f));
#else
			return FromCalibratedHsba (hue, saturation, brightness, 1.0f);
#endif
		}

		public static NSColor FromCalibratedHsb (double hue, double saturation, double brightness)
		{
			return FromCalibratedHsba (hue, saturation, brightness, 1.0f);
		}

		public static NSColor FromCalibratedHsba (double hue, double saturation, double brightness, double alpha)
		{
#if NO_NFLOAT_OPERATORS
			return FromCalibratedHsba (new NFloat (hue), new NFloat (saturation), new NFloat (brightness), new NFloat (alpha));
#else
			return FromCalibratedHsba ((nfloat) hue, (nfloat) saturation, (nfloat) brightness, (nfloat) alpha);
#endif
		}

		public static NSColor FromCalibratedHsba (byte hue, byte saturation, byte brightness, byte alpha)
		{
			return FromCalibratedHsba (hue / 255.0f, saturation / 255.0f, brightness / 255.0f, alpha / 255.0f);
		}

		public static NSColor FromCalibratedHsb (byte hue, byte saturation, byte brightness)
		{
			return FromCalibratedHsba (hue / 255.0f, saturation / 255.0f, brightness / 255.0f, 1.0f);
		}

		public static NSColor FromCalibratedHsba (int hue, int saturation, int brightness, int alpha)
		{
			return FromCalibratedHsba ((byte) hue, (byte) saturation, (byte) brightness, (byte) alpha);
		}

		public static NSColor FromCalibratedHsb (int hue, int saturation, int brightness)
		{
			return FromCalibratedHsb ((byte) hue, (byte) saturation, (byte) brightness);
		}

		public static NSColor FromColorSpace (NSColorSpace space, nfloat [] components)
		{
			if (components == null)
				throw new ArgumentNullException ("components");

			var pNativeFloatArray = IntPtr.Zero;
			
			try {
				pNativeFloatArray = Marshal.AllocHGlobal (components.Length * IntPtr.Size);
#if NO_NFLOAT_OPERATORS
				NFloatHelpers.CopyArray (components, 0, pNativeFloatArray, components.Length);
#else
				nfloat.CopyArray (components, 0, pNativeFloatArray, components.Length);
#endif
				return _FromColorSpace (space, pNativeFloatArray, components.Length);
			} finally {
				Marshal.FreeHGlobal (pNativeFloatArray);
			}
		}

		public void GetComponents (out nfloat [] components)
		{
			var pNativeFloatArray = IntPtr.Zero;

			try {
				var count = (int)ComponentCount;
			 	components = new nfloat [count];
				pNativeFloatArray = Marshal.AllocHGlobal (count * IntPtr.Size);
				_GetComponents (pNativeFloatArray);

#if NO_NFLOAT_OPERATORS
				NFloatHelpers.CopyArray (pNativeFloatArray, components, 0, count);
#else
				nfloat.CopyArray (pNativeFloatArray, components, 0, count);
#endif
			} finally {
				Marshal.FreeHGlobal (pNativeFloatArray);
			}
		}

		public override string ToString ()
		{
			try {
				string name = this.ColorSpaceName;
				if (name == "NSNamedColorSpace")
					return this.LocalizedCatalogNameComponent +"/" + this.LocalizedColorNameComponent;
				if (name == "NSPatternColorSpace")
					return "Pattern Color: " + this.PatternImage.Name;
				
				StringBuilder sb = new StringBuilder (this.ColorSpace.LocalizedName);
				nfloat[] components;
				this.GetComponents (out components);
				if (components.Length > 0)
					sb.Append ("(" + components [0]);
				for (int i = 1; i < components.Length; i++)
					sb.Append ("," + components [i]);
				sb.Append (")");
				
				return sb.ToString ();
			} catch {
				//fallback to base method if we have an unexpected condition.
				return base.ToString ();
			}
		}

		[Obsolete ("Use 'UnderPageBackgroundColor' instead.")]
		public static NSColor UnderPageBackground {
			get {
				return UnderPageBackgroundColor;
			}
		}

	}
}
#endif // !__MACCATALYST__
