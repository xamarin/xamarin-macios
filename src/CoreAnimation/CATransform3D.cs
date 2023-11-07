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
using System.Runtime.Versioning;

using ObjCRuntime;
using CoreGraphics;

#nullable enable

namespace CoreAnimation {

	// CATransform3D.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CATransform3D {
#if NET
		public nfloat M11, M12, M13, M14;
		public nfloat M21, M22, M23, M24;
		public nfloat M31, M32, M33, M34;
		public nfloat M41, M42, M43, M44;
#else
		[Obsolete ("Use 'M11' instead.")]
		public nfloat m11;
		[Obsolete ("Use 'M12' instead.")]
		public nfloat m12;
		[Obsolete ("Use 'M13' instead.")]
		public nfloat m13;
		[Obsolete ("Use 'M14' instead.")]
		public nfloat m14;
		[Obsolete ("Use 'M21' instead.")]
		public nfloat m21;
		[Obsolete ("Use 'M22' instead.")]
		public nfloat m22;
		[Obsolete ("Use 'M23' instead.")]
		public nfloat m23;
		[Obsolete ("Use 'M24' instead.")]
		public nfloat m24;
		[Obsolete ("Use 'M31' instead.")]
		public nfloat m31;
		[Obsolete ("Use 'M32' instead.")]
		public nfloat m32;
		[Obsolete ("Use 'M33' instead.")]
		public nfloat m33;
		[Obsolete ("Use 'M34' instead.")]
		public nfloat m34;
		[Obsolete ("Use 'M41' instead.")]
		public nfloat m41;
		[Obsolete ("Use 'M42' instead.")]
		public nfloat m42;
		[Obsolete ("Use 'M43' instead.")]
		public nfloat m43;
		[Obsolete ("Use 'M44' instead.")]
		public nfloat m44;

		public nfloat M11 { get => m11; set => m11 = value; }
		public nfloat M12 { get => m12; set => m12 = value; }
		public nfloat M13 { get => m13; set => m13 = value; }
		public nfloat M14 { get => m14; set => m14 = value; }
		public nfloat M21 { get => m21; set => m21 = value; }
		public nfloat M22 { get => m22; set => m22 = value; }
		public nfloat M23 { get => m23; set => m23 = value; }
		public nfloat M24 { get => m24; set => m24 = value; }
		public nfloat M31 { get => m31; set => m31 = value; }
		public nfloat M32 { get => m32; set => m32 = value; }
		public nfloat M33 { get => m33; set => m33 = value; }
		public nfloat M34 { get => m34; set => m34 = value; }
		public nfloat M41 { get => m41; set => m41 = value; }
		public nfloat M42 { get => m42; set => m42 = value; }
		public nfloat M43 { get => m43; set => m43 = value; }
		public nfloat M44 { get => m44; set => m44 = value; }
#endif

		static public readonly CATransform3D Identity;

		static CATransform3D ()
		{
			Identity = new CATransform3D ();
#if NET
			Identity.M11 = Identity.M22 = Identity.M33 = Identity.M44 = 1f;
#else
			Identity.m11 = Identity.m22 = Identity.m33 = Identity.m44 = 1f;
#endif
		}

		[DllImport (Constants.QuartzLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CATransform3DIsIdentity (CATransform3D t);

		public bool IsIdentity {
			get {
				return CATransform3DIsIdentity (this);
			}
		}

		[DllImport (Constants.QuartzLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CATransform3DEqualToTransform (CATransform3D a, CATransform3D b);

		public bool Equals (CATransform3D other)
		{
			return CATransform3DEqualToTransform (this, other);
		}

		public override bool Equals (object? other)
		{
			if (!(other is CATransform3D))
				return false;
			return CATransform3DEqualToTransform (this, (CATransform3D) other);
		}

		public override int GetHashCode ()
		{
			var hash = new HashCode ();
			hash.Add (M11);
			hash.Add (M12);
			hash.Add (M13);
			hash.Add (M14);
			hash.Add (M21);
			hash.Add (M22);
			hash.Add (M23);
			hash.Add (M24);
			hash.Add (M31);
			hash.Add (M32);
			hash.Add (M33);
			hash.Add (M34);
			hash.Add (M41);
			hash.Add (M42);
			hash.Add (M43);
			hash.Add (M44);
			return hash.ToHashCode ();
		}

		// Transform matrix =  [1 0 0 0; 0 1 0 0; 0 0 1 0; tx ty tz 1]
		//[DllImport(Constants.QuartzLibrary)]
		//extern static CATransform3D CATransform3DMakeTranslation (float tx, float ty, float tz);
		public static CATransform3D MakeTranslation (nfloat tx, nfloat ty, nfloat tz)
		{
			//return CATransform3DMakeTranslation (tx, ty, tz);
			CATransform3D r = Identity;
#if NET
			r.M41 = tx;
			r.M42 = ty;
			r.M43 = tz;
#else
			r.m41 = tx;
			r.m42 = ty;
			r.m43 = tz;
#endif

			return r;
		}

		// Scales matrix = [sx 0 0 0; 0 sy 0 0; 0 0 sz 0; 0 0 0 1]
		//[DllImport(Constants.QuartzLibrary)]
		//extern static CATransform3D CATransform3DMakeScale (float sx, float sy, float sz);
		public static CATransform3D MakeScale (nfloat sx, nfloat sy, nfloat sz)
		{
			CATransform3D r = Identity;
#if NET
			r.M11 = sx;
			r.M22 = sy;
			r.M33 = sz;
#else
			r.m11 = sx;
			r.m22 = sy;
			r.m33 = sz;
#endif

			return r;
		}

		[DllImport (Constants.QuartzLibrary, EntryPoint = "CATransform3DMakeRotation")]
		public extern static CATransform3D MakeRotation (nfloat angle, nfloat x, nfloat y, nfloat z);

		[DllImport (Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DTranslate (CATransform3D t, nfloat tx, nfloat ty, nfloat tz);

		public CATransform3D Translate (nfloat tx, nfloat ty, nfloat tz)
		{
			return CATransform3DTranslate (this, tx, ty, tz);
		}

		[DllImport (Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DScale (CATransform3D t, nfloat sx, nfloat sy, nfloat sz);

		public CATransform3D Scale (nfloat sx, nfloat sy, nfloat sz)
		{
			return CATransform3DScale (this, sx, sy, sz);
		}
		public CATransform3D Scale (nfloat s)
		{
			return CATransform3DScale (this, s, s, s);
		}

		[DllImport (Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DRotate (CATransform3D t, nfloat angle, nfloat x, nfloat y, nfloat z);

		public CATransform3D Rotate (nfloat angle, nfloat x, nfloat y, nfloat z)
		{
			return CATransform3DRotate (this, angle, x, y, z);
		}

		[DllImport (Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DConcat (CATransform3D a, CATransform3D b);

		public CATransform3D Concat (CATransform3D b)
		{
			return CATransform3DConcat (this, b);
		}

		[DllImport (Constants.QuartzLibrary)]
		extern static CATransform3D CATransform3DInvert (CATransform3D t);

#if !NET
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

		[DllImport (Constants.QuartzLibrary, EntryPoint = "CATransform3DMakeAffineTransform")]
		public extern static CATransform3D MakeFromAffine (CGAffineTransform m);


		[DllImport (Constants.QuartzLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CATransform3DIsAffine (CATransform3D t);

		public bool IsAffine {
			get {
				return CATransform3DIsAffine (this);
			}
		}

		[DllImport (Constants.QuartzLibrary, EntryPoint = "CATransform3DGetAffineTransform")]
		public extern static CGAffineTransform GetAffine (CATransform3D t);

		public override string ToString ()
		{
			return String.Format ("[{0} {1} {2} {3}; {4} {5} {6} {7}; {8} {9} {10} {11}; {12} {13} {14} {15}]",
#if NET
					      M11, M12, M13, M14,
					      M21, M22, M23, M24,
					      M31, M32, M33, M34,
					      M41, M42, M43, M44);
#else
						  m11, m12, m13, m14,
						  m21, m22, m23, m24,
						  m31, m32, m33, m34,
						  m41, m42, m43, m44);
#endif
		}
	}
}
