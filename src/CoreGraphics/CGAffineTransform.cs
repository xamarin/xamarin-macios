// 
// CGAffineTransform.cs: Implements the managed side
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2014 Xamarin Inc.
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

#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

	// CGAffineTransform.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CGAffineTransform {
#if NET
		public /* CGFloat */ nfloat A;
		public /* CGFloat */ nfloat B;
		public /* CGFloat */ nfloat C;
		public /* CGFloat */ nfloat D;
		public /* CGFloat */ nfloat Tx;
		public /* CGFloat */ nfloat Ty;

#if !XAMCORE_5_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'A' instead.")]
		public nfloat xx { get => A; set => A = value; }
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'B' instead.")]
		public nfloat yx { get => B; set => B = value; }
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'C' instead.")]
		public nfloat xy { get => C; set => C = value; }
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'D' instead.")]
		public nfloat yy { get => D; set => D = value; }
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'Tx' instead.")]
		public nfloat x0 { get => Tx; set => Tx = value; }
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'Ty' instead.")]
		public nfloat y0 { get => Ty; set => Ty = value; }
#endif // !XAMCORE_5_0
#else
		[Obsolete ("Use 'A' instead.")]
		public /* CGFloat */ nfloat xx;   // a
		[Obsolete ("Use 'B' instead.")]
		public /* CGFloat */ nfloat yx;   // b 
		[Obsolete ("Use 'C' instead.")]
		public /* CGFloat */ nfloat xy;   // c
		[Obsolete ("Use 'D' instead.")]
		public /* CGFloat */ nfloat yy;   // d
		[Obsolete ("Use 'Tx' instead.")]
		public /* CGFloat */ nfloat x0;   // tx
		[Obsolete ("Use 'Ty' instead.")]
		public /* CGFloat */ nfloat y0;   // ty

#pragma warning disable CS0618 // Type or member is obsolete
		public /* CGFloat */ nfloat A { get => xx; set => xx = value; }
		public /* CGFloat */ nfloat B { get => yx; set => yx = value; }
		public /* CGFloat */ nfloat C { get => xy; set => xy = value; }
		public /* CGFloat */ nfloat D { get => yy; set => yy = value; }
		public /* CGFloat */ nfloat Tx { get => x0; set => x0 = value; }
		public /* CGFloat */ nfloat Ty { get => y0; set => y0 = value; }
#pragma warning restore CS0618 // Type or member is obsolete

#endif // NET

#if !COREBUILD
		//
		// Constructors
		//
#if NET
		public CGAffineTransform (nfloat a, nfloat b, nfloat c, nfloat d, nfloat tx, nfloat ty)
		{
			this.A = a;
			this.B = b;
			this.C = c;
			this.D = d;
			this.Tx = tx;
			this.Ty = ty;
		}
#else
		public CGAffineTransform (nfloat xx, nfloat yx, nfloat xy, nfloat yy, nfloat x0, nfloat y0)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			this.xx = xx;
			this.yx = yx;
			this.xy = xy;
			this.yy = yy;
			this.x0 = x0;
			this.y0 = y0;
#pragma warning restore CS0618 // Type or member is obsolete
		}
#endif // NET

		// Identity
		public static CGAffineTransform MakeIdentity ()
		{
			return new CGAffineTransform (1, 0, 0, 1, 0, 0);
		}

		public static CGAffineTransform MakeRotation (nfloat angle)
		{
			var cos = (nfloat) Math.Cos (angle);
			var sin = (nfloat) Math.Sin (angle);
			return new CGAffineTransform (
				cos, sin,
				-sin, cos,
				0, 0);
		}

		public static CGAffineTransform MakeScale (nfloat sx, nfloat sy)
		{
			return new CGAffineTransform (sx, 0, 0, sy, 0, 0);
		}

		public static CGAffineTransform MakeTranslation (nfloat tx, nfloat ty)
		{
			return new CGAffineTransform (1, 0, 0, 1, tx, ty);
		}

		//
		// Operations
		//
		public static CGAffineTransform Multiply (CGAffineTransform a, CGAffineTransform b)
		{
#if NET
			return new CGAffineTransform (a.A * b.A + a.B * b.C,
						      a.A * b.B + a.B * b.D,
						      a.C * b.A + a.D * b.C,
						      a.C * b.B + a.D * b.D,
						      a.Tx * b.A + a.Ty * b.C + b.Tx,
						      a.Tx * b.B + a.Ty * b.D + b.Ty);
#else
#pragma warning disable CS0618 // Type or member is obsolete
			return new CGAffineTransform (a.xx * b.xx + a.yx * b.xy,
							  a.xx * b.yx + a.yx * b.yy,
							  a.xy * b.xx + a.yy * b.xy,
							  a.xy * b.yx + a.yy * b.yy,
							  a.x0 * b.xx + a.y0 * b.xy + b.x0,
							  a.x0 * b.yx + a.y0 * b.yy + b.y0);
#pragma warning restore CS0618 // Type or member is obsolete
#endif // NET
		}

		public void Multiply (CGAffineTransform b)
		{
			var a = this;
#if NET
			A = a.A * b.A + a.B * b.C;
			B = a.A * b.B + a.B * b.D;
			C = a.C * b.A + a.D * b.C;
			D = a.C * b.B + a.D * b.D;
			Tx = a.Tx * b.A + a.Ty * b.C + b.Tx;
			Ty = a.Tx * b.B + a.Ty * b.D + b.Ty;
#else
#pragma warning disable CS0618 // Type or member is obsolete
			xx = a.xx * b.xx + a.yx * b.xy;
			yx = a.xx * b.yx + a.yx * b.yy;
			xy = a.xy * b.xx + a.yy * b.xy;
			yy = a.xy * b.yx + a.yy * b.yy;
			x0 = a.x0 * b.xx + a.y0 * b.xy + b.x0;
			y0 = a.x0 * b.yx + a.y0 * b.yy + b.y0;
#pragma warning restore CS0618 // Type or member is obsolete

#endif // NET
		}

		public void Scale (nfloat sx, nfloat sy, MatrixOrder order)
		{
			switch (order) {
			case MatrixOrder.Prepend: // The new operation is applied before the old operation.
				this = Multiply (MakeScale (sx, sy), this); // t' = [ sx 0 0 sy 0 0 ] * t – Swift equivalent
				break;
			case MatrixOrder.Append: // The new operation is applied after the old operation.
				this = Multiply (this, MakeScale (sx, sy)); // t' = t * [ sx 0 0 sy 0 0 ]
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (order));
			}
		}

		[Advice ("By default, the new operation is applied after the old operation: t' = t * [ sx 0 0 sy 0 0 ].\nTo have the same behavior as the native Swift API, pass 'MatrixOrder.Prepend' to 'Scale (nfloat, nfloat, MatrixOrder)'.")]
		public void Scale (nfloat sx, nfloat sy)
		{
			Scale (sx, sy, MatrixOrder.Append);
		}

		public static CGAffineTransform Scale (CGAffineTransform transform, nfloat sx, nfloat sy)
		{
#if NET
			return new CGAffineTransform (
				sx * transform.A,
				sx * transform.B,
				sy * transform.C,
				sy * transform.D,
				transform.Tx,
				transform.Ty);
#else
#pragma warning disable CS0618 // Type or member is obsolete
			return new CGAffineTransform (
				sx * transform.xx,
				sx * transform.yx,
				sy * transform.xy,
				sy * transform.yy,
				transform.x0,
				transform.y0);
#pragma warning restore CS0618 // Type or member is obsolete
#endif // NET
		}

		public void Translate (nfloat tx, nfloat ty, MatrixOrder order)
		{
			switch (order) {
			case MatrixOrder.Prepend: // The new operation is applied before the old operation.
				this = Multiply (MakeTranslation (tx, ty), this); // t' = [ 1 0 0 1 tx ty ] * t – Swift equivalent
				break;
			case MatrixOrder.Append: // The new operation is applied after the old operation.
				this = Multiply (this, MakeTranslation (tx, ty)); // t' = t * [ 1 0 0 1 tx ty ]
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (order));
			}
		}

		[Advice ("By default, the new operation is applied after the old operation: t' = t * [ 1 0 0 1 tx ty ].\nTo have the same behavior as the native Swift API, pass 'MatrixOrder.Prepend' to 'Translate (nfloat, nfloat, MatrixOrder)'.")]
		public void Translate (nfloat tx, nfloat ty)
		{
			Translate (tx, ty, MatrixOrder.Append);
		}

		public static CGAffineTransform Translate (CGAffineTransform transform, nfloat tx, nfloat ty)
		{
#if NET
			return new CGAffineTransform (
				transform.A,
				transform.B,
				transform.C,
				transform.D,
				tx * transform.A + ty * transform.C + transform.Tx,
				tx * transform.B + ty * transform.D + transform.Ty);
#else
#pragma warning disable CS0618 // Type or member is obsolete
			return new CGAffineTransform (
				transform.xx,
				transform.yx,
				transform.xy,
				transform.yy,
				tx * transform.xx + ty * transform.xy + transform.x0,
				tx * transform.yx + ty * transform.yy + transform.y0);
#pragma warning disable CS0618 // Type or member is obsolete

#endif // NET
		}

		public void Rotate (nfloat angle, MatrixOrder order)
		{
			switch (order) {
			case MatrixOrder.Prepend: // The new operation is applied before the old operation.
				this = Multiply (MakeRotation (angle), this); // t' = [ cos(angle) sin(angle) -sin(angle) cos(angle) 0 0 ] * t – Swift equivalent
				break;
			case MatrixOrder.Append: // The new operation is applied after the old operation.
				this = Multiply (this, MakeRotation (angle)); // t' = t * [ cos(angle) sin(angle) -sin(angle) cos(angle) 0 0 ]
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (order));
			}
		}

		[Advice ("By default, the new operation is applied after the old operation: t' = t * [ cos(angle) sin(angle) -sin(angle) cos(angle) 0 0 ].\nTo have the same behavior as the native Swift API, pass 'MatrixOrder.Prepend' to 'Rotate (nfloat, MatrixOrder)'.")]
		public void Rotate (nfloat angle)
		{
			Rotate (angle, MatrixOrder.Append);
		}

		public static CGAffineTransform Rotate (CGAffineTransform transform, nfloat angle)
		{
#if NET
			var cos = (nfloat) Math.Cos (angle);
			var sin = (nfloat) Math.Sin (angle);

			return new CGAffineTransform (
				cos * transform.A + sin * transform.C,
				cos * transform.B + sin * transform.D,
				cos * transform.C - sin * transform.A,
				cos * transform.D - sin * transform.B,
				transform.Tx,
				transform.Ty);
#else
			var cos = (nfloat) Math.Cos (angle);
			var sin = (nfloat) Math.Sin (angle);

			return new CGAffineTransform (
				cos * transform.xx + sin * transform.xy,
				cos * transform.yx + sin * transform.yy,
				cos * transform.xy - sin * transform.xx,
				cos * transform.yy - sin * transform.yx,
				transform.x0,
				transform.y0);
#endif
		}

		public bool IsIdentity {
			get {
#if NET
				return A == 1 && B == 0 && C == 0 && D == 1 && Tx == 0 && Ty == 0;
#else
				return xx == 1 && yx == 0 && xy == 0 && yy == 1 && x0 == 0 && y0 == 0;
#endif
			}
		}

#if NET && !MONOMAC
		// on macOS NSAffineTransform is an ObjC type
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSString */ IntPtr NSStringFromCGAffineTransform (CGAffineTransform transform);
#endif

		public override String? ToString ()
		{
#if NET
#if MONOMAC
			var s = $"[{A}, {B}, {C}, {D}, {Tx}, {Ty}]";
#else
			var s = CFString.FromHandle (NSStringFromCGAffineTransform (this));
#endif
#else
			var s = String.Format ("xx:{0:##0.0#} yx:{1:##0.0#} xy:{2:##0.0#} yy:{3:##0.0#} x0:{4:##0.0#} y0:{5:##0.0#}", xx, yx, xy, yy, x0, y0);
#endif
			return s;
		}

		public static bool operator == (CGAffineTransform lhs, CGAffineTransform rhs)
		{
#if NET
			return (lhs.A == rhs.A && lhs.C == rhs.C &&
				lhs.B == rhs.B && lhs.D == rhs.D &&
				lhs.Tx == rhs.Tx && lhs.Ty == rhs.Ty);
#else
			return (lhs.xx == rhs.xx && lhs.xy == rhs.xy &&
				lhs.yx == rhs.yx && lhs.yy == rhs.yy &&
				lhs.x0 == rhs.x0 && lhs.y0 == rhs.y0);
#endif
		}

		public static bool operator != (CGAffineTransform lhs, CGAffineTransform rhs)
		{
			return !(lhs == rhs);
		}

		public static CGAffineTransform operator * (CGAffineTransform a, CGAffineTransform b)
		{
#if NET
			return new CGAffineTransform (a.A * b.A + a.B * b.C,
						      a.A * b.B + a.B * b.D,
						      a.C * b.A + a.D * b.C,
						      a.C * b.B + a.D * b.D,
						      a.Tx * b.A + a.Ty * b.C + b.Tx,
						      a.Tx * b.B + a.Ty * b.D + b.Ty);
#else
			return new CGAffineTransform (a.xx * b.xx + a.yx * b.xy,
							  a.xx * b.yx + a.yx * b.yy,
							  a.xy * b.xx + a.yy * b.xy,
							  a.xy * b.yx + a.yy * b.yy,
							  a.x0 * b.xx + a.y0 * b.xy + b.x0,
							  a.x0 * b.yx + a.y0 * b.yy + b.y0);
#endif
		}

		public override bool Equals (object? o)
		{
			if (o is CGAffineTransform transform) {
				return this == transform;
			} else
				return false;
		}

		public override int GetHashCode ()
		{
#if NET
			return HashCode.Combine (A, C, B, D, Tx, Ty);
#else
			return (int) this.xx ^ (int) this.xy ^
					(int) this.yx ^ (int) this.yy ^
					(int) this.x0 ^ (int) this.y0;
#endif
		}

		public CGPoint TransformPoint (CGPoint point)
		{
#if NET
			return new CGPoint (A * point.X + C * point.Y + Tx,
					    B * point.X + D * point.Y + Ty);
#else
			return new CGPoint (xx * point.X + xy * point.Y + x0,
						yx * point.X + yy * point.Y + y0);
#endif
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		public extern static CGRect CGRectApplyAffineTransform (CGRect rect, CGAffineTransform t);

		public CGRect TransformRect (CGRect rect)
		{
			return CGRectApplyAffineTransform (rect, this);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGSize CGSizeApplyAffineTransform (CGSize rect, CGAffineTransform t);

		public CGSize TransformSize (CGSize size)
		{
			return CGSizeApplyAffineTransform (size, this);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		public extern static CGAffineTransform CGAffineTransformInvert (CGAffineTransform t);

		public CGAffineTransform Invert ()
		{
			return CGAffineTransformInvert (this);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGAffineTransformComponents CGAffineTransformDecompose (CGAffineTransform transform);

		public CGAffineTransformComponents Decompose ()
		{
			return CGAffineTransformDecompose (this);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGAffineTransform CGAffineTransformMakeWithComponents (CGAffineTransformComponents components);

		public static CGAffineTransform MakeWithComponents (CGAffineTransformComponents components)
		{
			return CGAffineTransformMakeWithComponents (components);
		}
#endif // !COREBUILD
	}
}
