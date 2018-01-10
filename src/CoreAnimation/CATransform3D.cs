// 
// CATransform3D.cs: Implements the managed CATransform3D
//
// Authors:
//   Miguel de Icaza
//     
// Copyright 2009 Novell, Inc
// Copyright 2014 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreGraphics;

namespace CoreAnimation {

	// CATransform3D.h
	[StructLayout(LayoutKind.Sequential)]
	public struct CATransform3D {
		public nfloat m11, m12, m13, m14;
		public nfloat m21, m22, m23, m24;
		public nfloat m31, m32, m33, m34;
		public nfloat m41, m42, m43, m44;

		static public readonly CATransform3D Identity;

		static CATransform3D ()
		{
			Identity = new CATransform3D ();
			Identity.m11 = 1f;
			Identity.m22 = 1f;
			Identity.m33 = 1f;
			Identity.m44 = 1f;
		}
		
		[DllImport(Constants.QuartzLibrary)]
		extern static bool CATransform3DIsIdentity (CATransform3D t);

		public bool IsIdentity {
			get {
				return CATransform3DIsIdentity (this);
			}
		}

		[DllImport(Constants.QuartzLibrary)]
		extern static bool CATransform3DEqualToTransform (CATransform3D a, CATransform3D b);

		public bool Equals (CATransform3D other)
		{
			return CATransform3DEqualToTransform (this, other);
		}

		public override bool Equals (object other)
		{
			if (!(other is CATransform3D))
				return false;
			return CATransform3DEqualToTransform (this, (CATransform3D) other);
		}

		public override int GetHashCode ()
		{
			unsafe {
				int code = (int) m11;
				fixed (nfloat *fp = &m11){
					int *ip = (int *) fp;
					for (int i = 1; i < 4 * IntPtr.Size; i++){
						code = code ^ ip [i];
					}
				}
				return code;
			}
		}

		// Transform matrix =  [1 0 0 0; 0 1 0 0; 0 0 1 0; tx ty tz 1]
		//[DllImport(Constants.QuartzLibrary)]
		//extern static CATransform3D CATransform3DMakeTranslation (float tx, float ty, float tz);
		public static CATransform3D MakeTranslation (nfloat tx, nfloat ty, nfloat tz)
		{
			//return CATransform3DMakeTranslation (tx, ty, tz);
			CATransform3D r = Identity;
			r.m41 = tx;
			r.m42 = ty;
			r.m43 = tz;

			return r;
		}

		// Scales matrix = [sx 0 0 0; 0 sy 0 0; 0 0 sz 0; 0 0 0 1]
		//[DllImport(Constants.QuartzLibrary)]
		//extern static CATransform3D CATransform3DMakeScale (float sx, float sy, float sz);
		public static CATransform3D MakeScale (nfloat sx, nfloat sy, nfloat sz)
		{
			CATransform3D r = Identity;
			r.m11 = sx;
			r.m22 = sy;
			r.m33 = sz;

			return r;
		}
		
		[DllImport(Constants.QuartzLibrary, EntryPoint="CATransform3DMakeRotation")]
		public extern static CATransform3D MakeRotation (nfloat angle, nfloat x, nfloat y, nfloat z);

		[DllImport(Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DTranslate (CATransform3D t, nfloat tx, nfloat ty, nfloat tz);

		public CATransform3D Translate (nfloat tx, nfloat ty, nfloat tz)
		{
			return CATransform3DTranslate (this, tx, ty, tz);
		}
		
		[DllImport(Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DScale (CATransform3D t, nfloat sx, nfloat sy, nfloat sz);

		public CATransform3D Scale (nfloat sx, nfloat sy, nfloat sz)
		{
			return CATransform3DScale (this, sx, sy, sz);
		}
		public CATransform3D Scale (nfloat s)
		{
			return CATransform3DScale (this, s, s, s);
		}
		
		[DllImport(Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DRotate (CATransform3D t, nfloat angle, nfloat x, nfloat y, nfloat z);

		public CATransform3D Rotate (nfloat angle, nfloat x, nfloat y, nfloat z)
		{
			return CATransform3DRotate (this, angle, x, y, z);
		}
		
		[DllImport(Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DConcat (CATransform3D a, CATransform3D b);

		public CATransform3D Concat (CATransform3D b)
		{
			return CATransform3DConcat (this, b);
		}
		
		[DllImport(Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DInvert (CATransform3D t);

#if !XAMCORE_4_0
		[Obsolete ("Use Invert() as the argument to this method is unused.")]
		public CATransform3D Invert (CATransform3D t)
		{
			return CATransform3DInvert (this);
		}
#endif
		public CATransform3D Invert ()
		{
			return CATransform3DInvert (this);
		}

		[DllImport(Constants.QuartzLibrary, EntryPoint="CATransform3DMakeAffineTransform")]
		public extern static CATransform3D MakeFromAffine (CGAffineTransform m);
		

		[DllImport(Constants.QuartzLibrary)]
		extern static bool CATransform3DIsAffine (CATransform3D t);

		public bool IsAffine {
			get {
				return CATransform3DIsAffine (this);
			}
		}

		[DllImport(Constants.QuartzLibrary, EntryPoint="CATransform3DGetAffineTransform")]
		public extern static CGAffineTransform GetAffine (CATransform3D t);

		public override string ToString ()
		{
			return String.Format ("[{0} {1} {2} {3}; {4} {5} {6} {7}; {8} {9} {10} {11}; {12} {13} {14} {15}]",
					      m11, m12, m13, m14,
					      m21, m22, m23, m24,
					      m31, m32, m33, m34,
					      m41, m42, m43, m44);
		}
	}
}
