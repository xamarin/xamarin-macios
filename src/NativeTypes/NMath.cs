//
// NMath.cs: if we could add these to System.Math we would
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET
namespace ObjCRuntime
#else
namespace System
#endif
{
	public static class NMath
	{
#if NO_NFLOAT_OPERATORS
		public static readonly nfloat E = new NFloat (Math.E);
		public static readonly nfloat PI = new NFloat (Math.PI);
#else
		public static readonly nfloat E = (nfloat)Math.E;
		public static readonly nfloat PI = (nfloat)Math.PI;
#endif

		public static nfloat Abs (nfloat value)
		{
#if NO_NFLOAT_OPERATORS
			return value.Value < 0 ? new NFloat (-value.Value) : value;
#else
			return value < 0 ? -value : value;
#endif
		}

		public static nint Abs (nint value)
		{
			if (value == nint.MinValue)
				throw new OverflowException ("Value is too small.");

			return value < 0 ? -value : value;
		}

		public static nfloat Ceiling (nfloat value)
		{
#if NO_NFLOAT_OPERATORS
			var result = Floor (value);
			if (result.Value != value.Value)
				result = new NFloat (result.Value + 1);

			return result;
#else
			var result = Floor (value);
			if (result != value)
				result++;

			return result;
#endif
		}

		public static long BigMul (nint a, nint b)
		{
			return (long)a * (long)b;
		}

		public static nint DivRem (nint a, nint b, out nint result)
		{
			result = a % b;
			return a / b;
		}

		public static nfloat Floor (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Floor (d.Value));
#else
			return (nfloat)Math.Floor ((double)d);
#endif
		}

		public static nfloat IEEERemainder (nfloat x, nfloat y)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.IEEERemainder (x.Value, y.Value));
#else
			return (nfloat)Math.IEEERemainder ((double)x, (double)y);
#endif
		}

		public static nfloat Log (nfloat a, nfloat newBase)
		{
#if NO_NFLOAT_OPERATORS
			if (newBase.Value == 1.0)
				return NFloatHelpers.NaN;

			var result = Log (a).Value / Log (newBase).Value;
			return new NFloat (result == -0f ? 0f : result);
#else
			if (newBase == 1.0)
				return nfloat.NaN;

			var result = Log (a) / Log (newBase);
			return result == -0f ? 0f : result;
#endif
		}

		public static nfloat Max (nfloat val1, nfloat val2)
		{
#if NO_NFLOAT_OPERATORS
			if (NFloatHelpers.IsNaN (val1) || NFloatHelpers.IsNaN (val2))
				return NFloatHelpers.NaN;

			return val1.Value > val2.Value ? val1 : val2;
#else
			if (nfloat.IsNaN (val1) || nfloat.IsNaN (val2))
				return nfloat.NaN;

			return val1 > val2 ? val1 : val2;
#endif
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
#if NO_NFLOAT_OPERATORS
			if (NFloatHelpers.IsNaN (val1) || NFloatHelpers.IsNaN (val2))
				return NFloatHelpers.NaN;

			return val1.Value < val2.Value ? val1 : val2;
#else
			if (nfloat.IsNaN (val1) || nfloat.IsNaN (val2))
				return nfloat.NaN;

			return val1 < val2 ? val1 : val2;
#endif
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
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Round (a.Value));
#else
			return (nfloat)Math.Round ((double)a);
#endif
		}

		public static nfloat Round (nfloat value, int digits)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Round (value.Value, digits));
#else
			return (nfloat)Math.Round ((double)value, digits);
#endif
		}

		public static nfloat Round (nfloat value, MidpointRounding mode)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Round (value.Value, mode));
#else
			return (nfloat)Math.Round ((double)value, mode);
#endif
		}

		public static nfloat Round (nfloat value, int digits, MidpointRounding mode)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Round (value.Value, digits, mode));
#else
			return (nfloat)Math.Round ((double)value, digits, mode);
#endif
		}

		public static nfloat Truncate (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			if (d.Value > 0)
				return Floor (d);
			else if (d.Value < 0)
				return Ceiling (d);
			else
				return d;
#else
			if (d > 0)
				return Floor (d);
			else if (d < 0)
				return Ceiling (d);
			else
				return d;
#endif
		}

		public static int Sign (nfloat value)
		{
#if NO_NFLOAT_OPERATORS
			if (NFloatHelpers.IsNaN (value))
				throw new ArithmeticException ("NAN");

			if (value.Value > 0)
				return 1;

			return value.Value == 0 ? 0 : -1;
#else
			if (nfloat.IsNaN (value))
				throw new ArithmeticException ("NAN");

			if (value > 0)
				return 1;

			return value == 0 ? 0 : -1;
#endif
		}

		public static int Sign (nint value)
		{
			if (value > 0)
				return 1;

			return value == 0 ? 0 : -1;
		}

		public static nfloat Sin (nfloat a)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Sin (a.Value));
#else
			return (nfloat)Math.Sin ((double)a);
#endif
		}

		public static nfloat Cos (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Cos (d.Value));
#else
			return (nfloat)Math.Cos ((double)d);
#endif
		}

		public static nfloat Tan (nfloat a)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Tan (a.Value));
#else
			return (nfloat)Math.Tan ((double)a);
#endif
		}

		public static nfloat Sinh (nfloat value)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Sinh (value.Value));
#else
			return (nfloat)Math.Sinh ((double)value);
#endif
		}

		public static nfloat Cosh (nfloat value)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Cosh (value.Value));
#else
			return (nfloat)Math.Cosh ((double)value);
#endif
		}

		public static nfloat Tanh (nfloat value)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Tanh (value.Value));
#else
			return (nfloat)Math.Tanh ((double)value);
#endif
		}

		public static nfloat Acos (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Acos (d.Value));
#else
			return (nfloat)Math.Acos ((double)d);
#endif
		}

		public static nfloat Asin (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Asin (d.Value));
#else
			return (nfloat)Math.Asin ((double)d);
#endif
		}

		public static nfloat Atan (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Atan (d.Value));
#else
			return (nfloat)Math.Atan ((double)d);
#endif
		}

		public static nfloat Atan2 (nfloat y, nfloat x)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Atan2 (y.Value, x.Value));
#else
			return (nfloat)Math.Atan2 ((double)y, (double)x);
#endif
		}

		public static nfloat Exp (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Exp (d.Value));
#else
			return (nfloat)Math.Exp ((double)d);
#endif
		}

		public static nfloat Log (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Log (d.Value));
#else
			return (nfloat)Math.Log ((double)d);
#endif
		}

		public static nfloat Log10 (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Log10 (d.Value));
#else
			return (nfloat)Math.Log10 ((double)d);
#endif
		}

		public static nfloat Pow (nfloat x, nfloat y)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Pow (x.Value, y.Value));
#else
			return (nfloat)Math.Pow ((double)x, (double)y);
#endif
		}

		public static nfloat Sqrt (nfloat d)
		{
#if NO_NFLOAT_OPERATORS
			return new NFloat (Math.Sqrt (d.Value));
#else
			return (nfloat)Math.Sqrt ((double)d);
#endif
		}
	}
}
