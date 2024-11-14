#nullable enable

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace CoreGraphics {

	[Serializable]
	public struct CGSize : IEquatable<CGSize> {
		nfloat width;
		nfloat height;

		public static readonly CGSize Empty;

#if !COREBUILD
		public static bool operator == (CGSize l, CGSize r)
		{
			// the following version of Equals cannot be removed by the linker, while == can be
			return l.Equals (r);
		}

		public static bool operator != (CGSize l, CGSize r)
		{
			return l.width != r.width || l.height != r.height;
		}

		public static CGSize operator + (CGSize l, CGSize r)
		{
			return new CGSize (l.width + r.Width, l.height + r.Height);
		}

		public static CGSize operator - (CGSize l, CGSize r)
		{
			return new CGSize (l.width - r.Width, l.height - r.Height);
		}

#if !NO_SYSTEM_DRAWING
		public static implicit operator CGSize (SizeF size)
		{
			return new CGSize (size.Width, size.Height);
		}

		public static implicit operator CGSize (Size size)
		{
			return new CGSize (size.Width, size.Height);
		}

		public static explicit operator SizeF (CGSize size)
		{
			return new SizeF ((float) size.Width, (float) size.Height);
		}

		public static explicit operator Size (CGSize size)
		{
			return new Size ((int) size.Width, (int) size.Height);
		}
#endif

		public static explicit operator CGPoint (CGSize size)
		{
			return new CGPoint (size.Width, size.Height);
		}

		public static CGSize Add (CGSize size1, CGSize size2)
		{
			return size1 + size2;
		}

		public static CGSize Subtract (CGSize size1, CGSize size2)
		{
			return size1 - size2;
		}

		public nfloat Width {
			get { return width; }
			set { width = value; }
		}

		public nfloat Height {
			get { return height; }
			set { height = value; }
		}

		public bool IsEmpty {
			get { return width == 0.0 && height == 0.0; }
		}
#endif // !COREBUILD

		public CGSize (nfloat width, nfloat height)
		{
			this.width = width;
			this.height = height;
		}

#if !COREBUILD
		public CGSize (double width, double height)
		{
			this.width = (nfloat) width;
			this.height = (nfloat) height;
		}

		public CGSize (float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		public CGSize (CGSize size)
		{
			this.width = size.width;
			this.height = size.height;
		}

		public static bool TryParse (NSDictionary? dictionaryRepresentation, out CGSize size)
		{
			if (dictionaryRepresentation is null) {
				size = Empty;
				return false;
			}
			unsafe {
				size = default;
				return NativeDrawingMethods.CGSizeMakeWithDictionaryRepresentation (dictionaryRepresentation.Handle, (CGSize*) Unsafe.AsPointer<CGSize> (ref size)) != 0;
			}
		}

		public NSDictionary ToDictionary ()
		{
			return new NSDictionary (NativeDrawingMethods.CGSizeCreateDictionaryRepresentation (this));
		}

		public CGSize (CGPoint point)
		{
			this.width = point.X;
			this.height = point.Y;
		}
#endif // !COREBUILD

		public override bool Equals (object? obj)
		{
			return (obj is CGSize t) && Equals (t);
		}

		public bool Equals (CGSize size)
		{
			return size.width == width && size.height == height;
		}

		public override int GetHashCode ()
		{
#if NET
			return HashCode.Combine (width, height);
#else
			var hash = 23;
			hash = hash * 31 + width.GetHashCode ();
			hash = hash * 31 + height.GetHashCode ();
			return hash;
#endif
		}

#if !COREBUILD
		public void Deconstruct (out nfloat width, out nfloat height)
		{
			width = Width;
			height = Height;
		}

		public CGSize ToRoundedCGSize ()
		{
			return new CGSize ((nfloat) Math.Round (width), (nfloat) Math.Round (height));
		}

		public CGPoint ToCGPoint ()
		{
			return (CGPoint) this;
		}

		public override string? ToString ()
		{
#if NET
			return CFString.FromHandle (NSStringFromCGSize (this));
#else
			return String.Format ("{{Width={0}, Height={1}}}",
				width.ToString (CultureInfo.CurrentCulture),
				height.ToString (CultureInfo.CurrentCulture)
			);
#endif
		}

#if NET
#if MONOMAC
		// <quote>When building for 64 bit systems, or building 32 bit like 64 bit, NSSize is typedef’d to CGSize.</quote>
		// https://developer.apple.com/documentation/foundation/nssize?language=objc
		[DllImport (Constants.FoundationLibrary, EntryPoint = "NSStringFromSize")]
		extern static /* NSString* */ IntPtr NSStringFromCGSize (/* NSRect */ CGSize size);
#else
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSString* */ IntPtr NSStringFromCGSize (CGSize size);
#endif // MONOMAC
#endif // !NET
#endif // !COREBUILD
	}
}
