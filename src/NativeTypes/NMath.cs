//
// NMath.cs: if we could add these to System.Math we would
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

#if NET
namespace ObjCRuntime
#else
namespace System
#endif
{
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class NMath {
		public static readonly nfloat E = (nfloat) Math.E;
		public static readonly nfloat PI = (nfloat) Math.PI;

		public static nfloat Abs (nfloat value)
		{
			return value < 0 ? -value : value;
		}

		public static nint Abs (nint value)
		{
			if (value == nint.MinValue)
				throw new OverflowException ("Value is too small.");

			return value < 0 ? -value : value;
		}

		public static nfloat Ceiling (nfloat value)
		{
			var result = Floor (value);
			if (result != value)
				result++;

			return result;
		}

		public static long BigMul (nint a, nint b)
		{
			return (long) a * (long) b;
		}

		public static nint DivRem (nint a, nint b, out nint result)
		{
			result = a % b;
			return a / b;
		}

		public static nfloat Floor (nfloat d)
		{
			return (nfloat) Math.Floor ((double) d);
		}

		public static nfloat IEEERemainder (nfloat x, nfloat y)
		{
			return (nfloat) Math.IEEERemainder ((double) x, (double) y);
		}

		public static nfloat Log (nfloat a, nfloat newBase)
		{
			if (newBase == 1.0)
				return nfloat.NaN;

			var result = Log (a) / Log (newBase);
			return result == -0f ? 0f : result;
		}

		public static nfloat Max (nfloat val1, nfloat val2)
		{
			if (nfloat.IsNaN (val1) || nfloat.IsNaN (val2))
				return nfloat.NaN;

			return val1 > val2 ? val1 : val2;
		}

		public static nint Max (nint val1, nint val2)
		{
			return val1 > val2 ? val1 : val2;
		}

		public static nuint Max (nuint val1, nuint val2)
		{
			return val1 > val2 ? val1 : val2;
		}

		public static nfloat Min (nfloat val1, nfloat val2)
		{
			if (nfloat.IsNaN (val1) || nfloat.IsNaN (val2))
				return nfloat.NaN;

			return val1 < val2 ? val1 : val2;
		}

		public static nint Min (nint val1, nint val2)
		{
			return val1 < val2 ? val1 : val2;
		}

		public static nuint Min (nuint val1, nuint val2)
		{
			return val1 < val2 ? val1 : val2;
		}

		public static nfloat Round (nfloat a)
		{
			return (nfloat) Math.Round ((double) a);
		}

		public static nfloat Round (nfloat value, int digits)
		{
			return (nfloat) Math.Round ((double) value, digits);
		}

		public static nfloat Round (nfloat value, MidpointRounding mode)
		{
			return (nfloat) Math.Round ((double) value, mode);
		}

		public static nfloat Round (nfloat value, int digits, MidpointRounding mode)
		{
			return (nfloat) Math.Round ((double) value, digits, mode);
		}

		public static nfloat Truncate (nfloat d)
		{
			if (d > 0)
				return Floor (d);
			else if (d < 0)
				return Ceiling (d);
			else
				return d;
		}

		public static int Sign (nfloat value)
		{
			if (nfloat.IsNaN (value))
				throw new ArithmeticException ("NAN");

			if (value > 0)
				return 1;

			return value == 0 ? 0 : -1;
		}

		public static int Sign (nint value)
		{
			if (value > 0)
				return 1;

			return value == 0 ? 0 : -1;
		}

		public static nfloat Sin (nfloat a)
		{
			return (nfloat) Math.Sin ((double) a);
		}

		public static nfloat Cos (nfloat d)
		{
			return (nfloat) Math.Cos ((double) d);
		}

		public static nfloat Tan (nfloat a)
		{
			return (nfloat) Math.Tan ((double) a);
		}

		public static nfloat Sinh (nfloat value)
		{
			return (nfloat) Math.Sinh ((double) value);
		}

		public static nfloat Cosh (nfloat value)
		{
			return (nfloat) Math.Cosh ((double) value);
		}

		public static nfloat Tanh (nfloat value)
		{
			return (nfloat) Math.Tanh ((double) value);
		}

		public static nfloat Acos (nfloat d)
		{
			return (nfloat) Math.Acos ((double) d);
		}

		public static nfloat Asin (nfloat d)
		{
			return (nfloat) Math.Asin ((double) d);
		}

		public static nfloat Atan (nfloat d)
		{
			return (nfloat) Math.Atan ((double) d);
		}

		public static nfloat Atan2 (nfloat y, nfloat x)
		{
			return (nfloat) Math.Atan2 ((double) y, (double) x);
		}

		public static nfloat Exp (nfloat d)
		{
			return (nfloat) Math.Exp ((double) d);
		}

		public static nfloat Log (nfloat d)
		{
			return (nfloat) Math.Log ((double) d);
		}

		public static nfloat Log10 (nfloat d)
		{
			return (nfloat) Math.Log10 ((double) d);
		}

		public static nfloat Pow (nfloat x, nfloat y)
		{
			return (nfloat) Math.Pow ((double) x, (double) y);
		}

		public static nfloat Sqrt (nfloat d)
		{
			return (nfloat) Math.Sqrt ((double) d);
		}
	}
}
