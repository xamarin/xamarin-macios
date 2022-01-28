
#if NET

#if !(MTOUCH || MMP || BUNDLER) && NET
global using nfloat = System.Runtime.InteropServices.NFloat;
#endif

using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
	static class NFloatHelpers {
#if ARCH_32
		public static readonly int Size = 4;

		public static readonly nfloat MaxValue = new NFloat (Single.MaxValue);
		public static readonly nfloat MinValue = new NFloat (Single.MinValue);
		public static readonly nfloat Epsilon = new NFloat (Single.Epsilon);
		public static readonly nfloat NaN = new NFloat (Single.NaN);
		public static readonly nfloat NegativeInfinity = new NFloat (Single.NegativeInfinity);
		public static readonly nfloat PositiveInfinity = new NFloat (Single.PositiveInfinity);
#else
		public static readonly int Size = 8;

		public static readonly nfloat MaxValue =  new NFloat (Double.MaxValue);
		public static readonly nfloat MinValue =  new NFloat (Double.MinValue);
		public static readonly nfloat Epsilon = new NFloat (Double.Epsilon);
		public static readonly nfloat NaN = new NFloat (Double.NaN);
		public static readonly nfloat NegativeInfinity = new NFloat (Double.NegativeInfinity);
		public static readonly nfloat PositiveInfinity = new NFloat (Double.PositiveInfinity);
#endif

#if ARCH_32
		public static bool IsNaN              (nfloat f) { return Single.IsNaN ((Single)f.Value); }
		public static bool IsInfinity         (nfloat f) { return Single.IsInfinity ((Single)f.Value); }
		public static bool IsPositiveInfinity (nfloat f) { return Single.IsPositiveInfinity ((Single)f.Value); }
		public static bool IsNegativeInfinity (nfloat f) { return Single.IsNegativeInfinity ((Single)f.Value); }
#else
		public static bool IsNaN              (nfloat f) { return Double.IsNaN (f.Value); }
		public static bool IsInfinity         (nfloat f) { return Double.IsInfinity (f.Value); }
		public static bool IsPositiveInfinity (nfloat f) { return Double.IsPositiveInfinity (f.Value); }
		public static bool IsNegativeInfinity (nfloat f) { return Double.IsNegativeInfinity (f.Value); }
#endif

		public static void CopyArray (IntPtr source, nfloat [] destination, int startIndex, int length)
		{
			if (source == IntPtr.Zero)
				throw new ArgumentNullException ("source");
			if (destination is null)
				throw new ArgumentNullException ("destination");
			if (destination.Rank != 1)
				throw new ArgumentException ("destination", "array is multi-dimensional");
			if (startIndex < 0)
				throw new ArgumentException ("startIndex", "must be >= 0");
			if (length < 0)
				throw new ArgumentException ("length", "must be >= 0");
			if (startIndex + length > destination.Length)
				throw new ArgumentException ("length", "startIndex + length > destination.Length");

			unsafe {
				NFloat *src = (NFloat *) source;
				for (int i = 0; i < length; i++) {
					destination [i + startIndex] = src [i];
				}
			}
		}

		public static void CopyArray (nfloat [] source, int startIndex, IntPtr destination, int length)
		{
			if (source is null)
				throw new ArgumentNullException ("source");
			if (destination == IntPtr.Zero)
				throw new ArgumentNullException ("destination");
			if (source.Rank != 1)
				throw new ArgumentException ("source", "array is multi-dimensional");
			if (startIndex < 0)
				throw new ArgumentException ("startIndex", "must be >= 0");
			if (length < 0)
				throw new ArgumentException ("length", "must be >= 0");
			if (startIndex + length > source.Length)
				throw new ArgumentException ("length", "startIndex + length > source.Length");

			unsafe {
				NFloat *dst = (NFloat *) destination;
				for (int i = 0; i < length; i++) {
					dst [i] = source [i + startIndex];
				}
			}
		}
	}

	static class NFloatOperators {

		public static bool Equals (NFloat a, NFloat b)
		{
			return a.Value == b.Value;
		}

		public static bool NotEquals (NFloat a, NFloat b)
		{
			return a.Value != b.Value;
		}

		public static NFloat Add (NFloat a, NFloat b)
		{
			return new NFloat (a.Value + b.Value);
		}

		public static NFloat Subtract (NFloat a, NFloat b)
		{
			return new NFloat (a.Value - b.Value);
		}

		public static float ToFloat (NFloat value)
		{
			return (float) value.Value;
		}

		public static decimal ToDecimal (NFloat value)
		{
			return (decimal) value.Value;
		}

		public static int ToInt32 (NFloat value)
		{
			return (int) value.Value;
		}

		public static byte ToByte (NFloat value)
		{
			return (byte) value.Value;
		}

		public static NFloat ToNFloat (float value)
		{
			return new NFloat (value);
		}

		public static NFloat ToNFloat (double value)
		{
			return new NFloat (value);
		}

		public static NFloat ToNFloat (decimal value)
		{
			return new NFloat ((double) value);
		}
	}
}

#endif // NET
