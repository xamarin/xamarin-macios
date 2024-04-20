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
	public struct CGRect : IEquatable<CGRect> {
		nfloat x;
		nfloat y;
		nfloat width;
		nfloat height;

#if !COREBUILD
		[Field ("CGRectZero", "CoreGraphics")] // unused but helps xtro
#endif
		public static readonly CGRect Empty;

#if !COREBUILD
		[Field ("CGRectNull", "CoreGraphics")] // unused but helps xtro
		public static CGRect Null {
			get { return Dlfcn.GetCGRect (Libraries.CoreGraphics.Handle, "CGRectNull"); }
		}

		[Field ("CGRectInfinite", "CoreGraphics")] // unused but helps xtro
		public static CGRect Infinite {
			get { return Dlfcn.GetCGRect (Libraries.CoreGraphics.Handle, "CGRectInfinite"); }
		}

		public static bool operator == (CGRect left, CGRect right)
		{
			// the following version of Equals cannot be removed by the linker, while == can be
			return left.Equals (right);
		}

		public static bool operator != (CGRect left, CGRect right)
		{
			return
				left.X != right.X ||
				left.Y != right.Y ||
				left.Width != right.Width ||
				left.Height != right.Height;
		}

#if !NO_SYSTEM_DRAWING
		public static implicit operator CGRect (RectangleF rect)
		{
			return new CGRect (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static implicit operator CGRect (Rectangle rect)
		{
			return new CGRect (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static explicit operator RectangleF (CGRect rect)
		{
			return new RectangleF ((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height);
		}

		public static explicit operator Rectangle (CGRect rect)
		{
			return new Rectangle ((int) rect.X, (int) rect.Y, (int) rect.Width, (int) rect.Height);
		}
#endif

		public static CGRect Intersect (CGRect a, CGRect b)
		{
			// MS.NET returns a non-empty rectangle if the two rectangles
			// touch each other
			if (!a.IntersectsWithInclusive (b)) {
				return Empty;
			}

			return FromLTRB (
				(nfloat) Math.Max (a.Left, b.Left),
				(nfloat) Math.Max (a.Top, b.Top),
				(nfloat) Math.Min (a.Right, b.Right),
				(nfloat) Math.Min (a.Bottom, b.Bottom)
			);
		}

		public void Intersect (CGRect rect)
		{
			this = CGRect.Intersect (this, rect);
		}

		public static CGRect Union (CGRect a, CGRect b)
		{
			return FromLTRB (
				(nfloat) Math.Min (a.Left, b.Left),
				(nfloat) Math.Min (a.Top, b.Top),
				(nfloat) Math.Max (a.Right, b.Right),
				(nfloat) Math.Max (a.Bottom, b.Bottom)
			);
		}

		public static CGRect FromLTRB (nfloat left, nfloat top, nfloat right, nfloat bottom)
		{
			return new CGRect (left, top, right - left, bottom - top);
		}

		public static CGRect Inflate (CGRect rect, nfloat x, nfloat y)
		{
			var inflated = new CGRect (rect.X, rect.Y, rect.Width, rect.Height);
			inflated.Inflate (x, y);
			return inflated;
		}

		public bool IsEmpty {
			get { return width == 0.0 || height == 0.0; }
		}

		public nfloat X {
			get { return x; }
			set { x = value; }
		}

		public nfloat Y {
			get { return y; }
			set { y = value; }
		}

		public nfloat Width {
			get { return width; }
			set { width = value; }
		}

		public nfloat Height {
			get { return height; }
			set { height = value; }
		}

		public nfloat Top {
			get { return Y; }
		}

		public nfloat Bottom {
			get { return Y + Height; }
		}

		public nfloat Left {
			get { return X; }
		}

		public nfloat Right {
			get { return X + Width; }
		}

		public CGPoint Location {
			get { return new CGPoint (x, y); }
			set {
				x = value.X;
				y = value.Y;
			}
		}

		public CGSize Size {
			get { return new CGSize (width, height); }
			set {
				width = value.Width;
				height = value.Height;
			}
		}

		public CGRect (CGPoint location, CGSize size)
		{
			x = location.X;
			y = location.Y;
			width = size.Width;
			height = size.Height;
		}
#endif // !COREBUILD

		public CGRect (nfloat x, nfloat y, nfloat width, nfloat height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

#if !COREBUILD
		public CGRect (double x, double y, double width, double height)
		{
			this.x = (nfloat) x;
			this.y = (nfloat) y;
			this.width = (nfloat) width;
			this.height = (nfloat) height;
		}


		public CGRect (float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public bool Contains (nfloat x, nfloat y)
		{
			return
				x >= Left &&
				x < Right &&
				y >= Top &&
				y < Bottom;
		}

		public bool Contains (float x, float y)
		{
			return Contains ((nfloat) x, (nfloat) y);
		}

		public bool Contains (double x, double y)
		{
			return Contains ((nfloat) x, (nfloat) y);
		}

		public bool Contains (CGPoint point)
		{
			return Contains (point.X, point.Y);
		}

		public bool Contains (CGRect rect)
		{
			return
				X <= rect.X &&
				Right >= rect.Right &&
				Y <= rect.Y &&
				Bottom >= rect.Bottom;
		}

		public void Inflate (nfloat x, nfloat y)
		{
			this.x -= x;
			this.y -= y;
			width += x * 2;
			height += y * 2;
		}

		public void Inflate (float x, float y)
		{
			Inflate ((nfloat) x, (nfloat) y);
		}

		public void Inflate (double x, double y)
		{
			Inflate ((nfloat) x, (nfloat) y);
		}

		public void Inflate (CGSize size)
		{
			Inflate (size.Width, size.Height);
		}

		public void Offset (nfloat x, nfloat y)
		{
			X += x;
			Y += y;
		}

		public void Offset (float x, float y)
		{
			Offset ((nfloat) x, (nfloat) y);
		}

		public void Offset (double x, double y)
		{
			Offset ((nfloat) x, (nfloat) y);
		}

		public void Offset (CGPoint pos)
		{
			Offset (pos.X, pos.Y);
		}

		public bool IntersectsWith (CGRect rect)
		{
			return !(
				Left >= rect.Right ||
				Right <= rect.Left ||
				Top >= rect.Bottom ||
				Bottom <= rect.Top
			);
		}

		private bool IntersectsWithInclusive (CGRect r)
		{
			return !(
				Left > r.Right ||
				Right < r.Left ||
				Top > r.Bottom ||
				Bottom < r.Top
			);
		}
#endif // !COREBUILD

		public override bool Equals (object? obj)
		{
			return (obj is CGRect rect) && Equals (rect);
		}

		public bool Equals (CGRect rect)
		{
			return
				x == rect.x &&
				y == rect.y &&
				width == rect.width &&
				height == rect.height;
		}

		public override int GetHashCode ()
		{
#if NET
			return HashCode.Combine (x, y, width, height);
#else
			var hash = 23;
			hash = hash * 31 + x.GetHashCode ();
			hash = hash * 31 + y.GetHashCode ();
			hash = hash * 31 + width.GetHashCode ();
			hash = hash * 31 + height.GetHashCode ();
			return hash;
#endif
		}

#if !COREBUILD
		public override string? ToString ()
		{
#if NET
			return CFString.FromHandle (NSStringFromCGRect (this));
#else
			return String.Format ("{{X={0},Y={1},Width={2},Height={3}}}",
				x, y, width, height);
#endif
		}

		public void Deconstruct (out nfloat x, out nfloat y, out nfloat width, out nfloat height)
		{
			x = X;
			y = Y;
			width = Width;
			height = Height;
		}

		public void Deconstruct (out CGPoint location, out CGSize size)
		{
			location = Location;
			size = Size;
		}

		public static bool TryParse (NSDictionary? dictionaryRepresentation, out CGRect rect)
		{
			if (dictionaryRepresentation is null) {
				rect = Empty;
				return false;
			}
			rect = default;
			unsafe {
				return NativeDrawingMethods.CGRectMakeWithDictionaryRepresentation (dictionaryRepresentation.Handle, (CGRect*) Unsafe.AsPointer<CGRect> (ref rect)) != 0;
			}
		}

		public NSDictionary ToDictionary ()
		{
			return new NSDictionary (NativeDrawingMethods.CGRectCreateDictionaryRepresentation (this));
		}

#if NET
#if MONOMAC
		// <quote>When building for 64 bit systems, or building 32 bit like 64 bit, NSRect is typedef’d to CGRect.</quote>
		// https://developer.apple.com/documentation/foundation/nsrect?language=objc
		[DllImport (Constants.FoundationLibrary, EntryPoint = "NSStringFromRect")]
		extern static /* NSString* */ IntPtr NSStringFromCGRect (/* NSRect */ CGRect rect);
#else
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSString* */ IntPtr NSStringFromCGRect (CGRect rect);
#endif // MONOMAC
#endif // !NET
#endif // !COREBUILD
	}
}
