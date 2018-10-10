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

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

	// CGAffineTransform.h
	[StructLayout(LayoutKind.Sequential)]
	public struct CGAffineTransform {
		public /* CGFloat */ nfloat xx;   // a
		public /* CGFloat */ nfloat yx;   // b 
		public /* CGFloat */ nfloat xy;   // c
		public /* CGFloat */ nfloat yy;   // d
		public /* CGFloat */ nfloat x0;   // tx
		public /* CGFloat */ nfloat y0;   // ty

		//
		// Constructors
		//
		public CGAffineTransform (nfloat xx, nfloat yx, nfloat xy, nfloat yy, nfloat x0, nfloat y0)
		{
			this.xx = xx;
			this.yx = yx;
			this.xy = xy;
			this.yy = yy;
			this.x0 = x0;
			this.y0 = y0;
		}
		
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
			return new CGAffineTransform (a.xx * b.xx + a.yx * b.xy,
						      a.xx * b.yx + a.yx * b.yy,
						      a.xy * b.xx + a.yy * b.xy,
						      a.xy * b.yx + a.yy * b.yy,
						      a.x0 * b.xx + a.y0 * b.xy + b.x0,
						      a.x0 * b.yx + a.y0 * b.yy + b.y0);
		}

		public void Multiply (CGAffineTransform b)
		{
			var a = this;
			xx = a.xx * b.xx + a.yx * b.xy;
			yx = a.xx * b.yx + a.yx * b.yy;
			xy = a.xy * b.xx + a.yy * b.xy;
			yy = a.xy * b.yx + a.yy * b.yy;
			x0 = a.x0 * b.xx + a.y0 * b.xy + b.x0;
			y0 = a.x0 * b.yx + a.y0 * b.yy + b.y0;
		}

		public enum MatrixOrder {
			Prepend = 0,
			Append = 1,
		}

		public void Scale (nfloat sx, nfloat sy, MatrixOrder order)
		{
			switch (order) {
			case MatrixOrder.Append: // The new operation is applied after the old operation.
				this = Multiply (this, MakeScale (sx, sy)); // t' = t * [ sx 0 0 sy 0 0 ]
				break;
			case MatrixOrder.Prepend: // The new operation is applied before the old operation.
				this = Multiply (MakeScale (sx, sy), this); // t' = [ sx 0 0 sy 0 0 ] * t – Swift equivalent
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (order));
			}
		}
		
		[Advice ("The new operation is applied after the old operation: t' = t * [ sx 0 0 sy 0 0 ].\nTo have the same behavior as the native Swift API, pass 'MatrixOrder.Prepend' to 'Scale (nfloat, nfloat, MatrixOrder)'.")]
		public void Scale (nfloat sx, nfloat sy)
		{
			Scale (sx, sy, MatrixOrder.Append);
		}

		public static CGAffineTransform Scale (CGAffineTransform transform, nfloat sx, nfloat sy)
		{
			return new CGAffineTransform (
				sx * transform.xx,
				sx * transform.yx,
				sy * transform.xy,
				sy * transform.yy,
				transform.x0,
				transform.y0);
		}

		public void Translate (nfloat tx, nfloat ty)
		{
			Multiply (MakeTranslation (tx, ty));
		}

		public static CGAffineTransform Translate (CGAffineTransform transform, nfloat tx, nfloat ty)
		{
			return new CGAffineTransform (
				transform.xx,
				transform.yx,
				transform.xy,
				transform.yy,
				tx * transform.xx + ty * transform.xy + transform.x0,
				tx * transform.yx + ty * transform.yy + transform.y0);
		}

		public void Rotate (nfloat angle)
		{
			Multiply (MakeRotation (angle));
		}
			
		public static CGAffineTransform Rotate (CGAffineTransform transform, nfloat angle)
		{
			var cos = (nfloat) Math.Cos (angle);
			var sin = (nfloat) Math.Sin (angle);

			return new CGAffineTransform (
				cos * transform.xx + sin * transform.xy,
				cos * transform.yx + sin * transform.yy,
				cos * transform.xy - sin * transform.xx,
				cos * transform.yy - sin * transform.yx,
				transform.x0,
				transform.y0);
		}

		public bool IsIdentity {
			get {
				return xx == 1 && yx == 0 && xy == 0 && yy == 1 && x0 == 0 && y0 == 0;
			}
		}
		
		public override String ToString ()
		{
			String s = String.Format ("xx:{0:##0.0#} yx:{1:##0.0#} xy:{2:##0.0#} yy:{3:##0.0#} x0:{4:##0.0#} y0:{5:##0.0#}", xx, yx, xy, yy, x0, y0);
			return s;
		}

		public static bool operator == (CGAffineTransform lhs, CGAffineTransform rhs)
		{
			return (lhs.xx == rhs.xx && lhs.xy == rhs.xy &&
				lhs.yx == rhs.yx && lhs.yy == rhs.yy &&
				lhs.x0 == rhs.x0 && lhs.y0 == rhs.y0 );
		}

		public static bool operator != (CGAffineTransform lhs, CGAffineTransform rhs)
		{
			return !(lhs==rhs);
		}

		public static CGAffineTransform operator * (CGAffineTransform a, CGAffineTransform b)
		{
			return new CGAffineTransform (a.xx * b.xx + a.yx * b.xy,
						      a.xx * b.yx + a.yx * b.yy,
						      a.xy * b.xx + a.yy * b.xy,
						      a.xy * b.yx + a.yy * b.yy,
						      a.x0 * b.xx + a.y0 * b.xy + b.x0,
						      a.x0 * b.yx + a.y0 * b.yy + b.y0);
		}

		public override bool Equals(object o)
		{
			if (! (o is CGAffineTransform))
				return false;
			else
				return (this == (CGAffineTransform) o);
		}

		public override int GetHashCode()
		{
			return  (int)this.xx ^ (int)this.xy ^
					(int)this.yx ^ (int)this.yy ^
					(int)this.x0 ^ (int)this.y0;
		}

		public CGPoint TransformPoint (CGPoint point)
		{
			return new CGPoint (xx * point.X + xy * point.Y + x0,
					    yx * point.X + yy * point.Y + y0);
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
	}
}
