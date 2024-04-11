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


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[Serializable]
	public struct CGPoint : IEquatable<CGPoint> {
		nfloat x;
		nfloat y;

		public static readonly CGPoint Empty;

#if !COREBUILD
		public static bool operator == (CGPoint l, CGPoint r)
		{
			// the following version of Equals cannot be removed by the linker, while == can be
			return l.Equals (r);
		}

		public static bool operator != (CGPoint l, CGPoint r)
		{
			return l.x != r.x || l.y != r.y;
		}

		public static CGPoint operator + (CGPoint l, CGSize r)
		{
			return new CGPoint (l.x + r.Width, l.y + r.Height);
		}

		public static CGPoint operator - (CGPoint l, CGSize r)
		{
			return new CGPoint (l.x - r.Width, l.y - r.Height);
		}

#if !NO_SYSTEM_DRAWING
		public static implicit operator CGPoint (PointF point)
		{
			return new CGPoint (point.X, point.Y);
		}

		public static implicit operator CGPoint (Point point)
		{
			return new CGPoint (point.X, point.Y);
		}

		public static explicit operator PointF (CGPoint point)
		{
			return new PointF ((float) point.X, (float) point.Y);
		}

		public static explicit operator Point (CGPoint point)
		{
			return new Point ((int) point.X, (int) point.Y);
		}
#endif

		public static CGPoint Add (CGPoint point, CGSize size)
		{
			return point + size;
		}

		public static CGPoint Subtract (CGPoint point, CGSize size)
		{
			return point - size;
		}

		public nfloat X {
			get { return x; }
			set { x = value; }
		}

		public nfloat Y {
			get { return y; }
			set { y = value; }
		}

		public bool IsEmpty {
			get { return x == 0.0 && y == 0.0; }
		}
#endif // !COREBUILD

		public CGPoint (nfloat x, nfloat y)
		{
			this.x = x;
			this.y = y;
		}

#if !COREBUILD
		public CGPoint (double x, double y)
		{
			this.x = (nfloat) x;
			this.y = (nfloat) y;
		}

		public CGPoint (float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public CGPoint (CGPoint point)
		{
			this.x = point.x;
			this.y = point.y;
		}

		public static bool TryParse (NSDictionary? dictionaryRepresentation, out CGPoint point)
		{
			if (dictionaryRepresentation is null) {
				point = Empty;
				return false;
			}
			unsafe {
				point = default;
				return NativeDrawingMethods.CGPointMakeWithDictionaryRepresentation (dictionaryRepresentation.Handle, (CGPoint*) Unsafe.AsPointer<CGPoint> (ref point)) != 0;
			}
		}

		public NSDictionary ToDictionary ()
		{
			return new NSDictionary (NativeDrawingMethods.CGPointCreateDictionaryRepresentation (this));
		}
#endif // !COREBUILD

		public override bool Equals (object? obj)
		{
			return (obj is CGPoint t) && Equals (t);
		}

		public bool Equals (CGPoint point)
		{
			return point.x == x && point.y == y;
		}

		public override int GetHashCode ()
		{
#if NET
			return HashCode.Combine (x, y);
#else
			var hash = 23;
			hash = hash * 31 + x.GetHashCode ();
			hash = hash * 31 + y.GetHashCode ();
			return hash;
#endif
		}

#if !COREBUILD
		public void Deconstruct (out nfloat x, out nfloat y)
		{
			x = X;
			y = Y;
		}

		public override string? ToString ()
		{
#if NET
			return CFString.FromHandle (NSStringFromCGPoint (this));
#else
			return String.Format ("{{X={0}, Y={1}}}",
				x.ToString (CultureInfo.CurrentCulture),
				y.ToString (CultureInfo.CurrentCulture)
			);
#endif
		}

#if NET
#if MONOMAC
		// <quote>When building for 64 bit systems, or building 32 bit like 64 bit, NSPoint is typedef’d to CGPoint.</quote>
		// https://developer.apple.com/documentation/foundation/nspoint?language=objc
		[DllImport (Constants.FoundationLibrary, EntryPoint = "NSStringFromPoint")]
		extern static /* NSString* */ IntPtr NSStringFromCGPoint (/* NSPoint */ CGPoint point);
#else
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSString* */ IntPtr NSStringFromCGPoint (CGPoint point);
#endif // MONOMAC
#endif // !NET
#endif // !COREBUILD
	}
}
