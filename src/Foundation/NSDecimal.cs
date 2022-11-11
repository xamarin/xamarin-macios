//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
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
//
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace Foundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct NSDecimal
#if !COREBUILD
		: IEquatable<NSDecimal>
#endif
	{
		// unsigned int
		public int fields;
		// unsigned short [8]
		public short m1, m2, m3, m4, m5, m6, m7, m8;

#if !COREBUILD
		[DllImport (Constants.FoundationLibrary)]
		static extern nint NSDecimalCompare (ref NSDecimal left, ref NSDecimal right);
		public static NSComparisonResult Compare (ref NSDecimal left, ref NSDecimal right)
		{
			return (NSComparisonResult) (long) NSDecimalCompare (ref left, ref right);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern void NSDecimalRound (out NSDecimal result, ref NSDecimal number, nint scale, nuint mode);
		public static void Round (out NSDecimal result, ref NSDecimal number, nint scale, NSRoundingMode mode)
		{
			NSDecimalRound (out result, ref number, scale, (nuint) (ulong) mode);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern nuint NSDecimalNormalize (ref NSDecimal number1, ref NSDecimal number2);
		public static NSCalculationError Normalize (ref NSDecimal number1, ref NSDecimal number2)
		{
			return (NSCalculationError) (ulong) NSDecimalNormalize (ref number1, ref number2);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern nuint NSDecimalAdd (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, nuint mode);
		public static NSCalculationError Add (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, NSRoundingMode mode)
		{
			return (NSCalculationError) (ulong) NSDecimalAdd (out result, ref left, ref right, (nuint) (ulong) mode);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern nuint NSDecimalSubtract (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, nuint mode);
		public static NSCalculationError Subtract (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, NSRoundingMode mode)
		{
			return (NSCalculationError) (ulong) NSDecimalSubtract (out result, ref left, ref right, (nuint) (ulong) mode);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern nuint NSDecimalMultiply (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, nuint mode);
		public static NSCalculationError Multiply (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, NSRoundingMode mode)
		{
			return (NSCalculationError) (ulong) NSDecimalMultiply (out result, ref left, ref right, (nuint) (ulong) mode);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern nuint NSDecimalDivide (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, nuint mode);
		public static NSCalculationError Divide (out NSDecimal result, ref NSDecimal left, ref NSDecimal right, NSRoundingMode mode)
		{
			return (NSCalculationError) (ulong) NSDecimalDivide (out result, ref left, ref right, (nuint) (ulong) mode);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern nuint NSDecimalPower (out NSDecimal result, ref NSDecimal number, nint power, nuint mode);
		public static NSCalculationError Power (out NSDecimal result, ref NSDecimal number, nint power, NSRoundingMode mode)
		{
			return (NSCalculationError) (ulong) NSDecimalPower (out result, ref number, power, (nuint) (ulong) mode);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern nuint NSDecimalMultiplyByPowerOf10 (out NSDecimal result, ref NSDecimal number, short power10, nuint mode);
		public static NSCalculationError MultiplyByPowerOf10 (out NSDecimal result, ref NSDecimal number, short power10, NSRoundingMode mode)
		{
			return (NSCalculationError) (ulong) NSDecimalMultiplyByPowerOf10 (out result, ref number, power10, (nuint) (ulong) mode);
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr NSDecimalString (ref NSDecimal value, /* _Nullable */ IntPtr locale);

		public override string ToString ()
		{
			return new NSString (NSDecimalString (ref this, NSLocale.CurrentLocale.Handle));
		}

		public static NSDecimal operator + (NSDecimal left, NSDecimal right)
		{
			NSDecimal result;

			Add (out result, ref left, ref right, NSRoundingMode.Plain);
			return result;
		}

		public static NSDecimal operator - (NSDecimal left, NSDecimal right)
		{
			NSDecimal result;

			Subtract (out result, ref left, ref right, NSRoundingMode.Plain);
			return result;
		}

		public static NSDecimal operator * (NSDecimal left, NSDecimal right)
		{
			NSDecimal result;

			Multiply (out result, ref left, ref right, NSRoundingMode.Plain);
			return result;
		}

		public static NSDecimal operator / (NSDecimal left, NSDecimal right)
		{
			NSDecimal result;

			Divide (out result, ref left, ref right, NSRoundingMode.Plain);
			return result;
		}

		public static bool operator == (NSDecimal left, NSDecimal right)
		{
			return Compare (ref left, ref right) == NSComparisonResult.Same;
		}

		public static bool operator != (NSDecimal left, NSDecimal right)
		{
			return Compare (ref left, ref right) != NSComparisonResult.Same;
		}

		public static implicit operator NSDecimal (int value)
		{
			using var number = new NSNumber (value);
			return number.NSDecimalValue;
		}

		public static explicit operator int (NSDecimal value)
		{
			using var number = new NSDecimalNumber (value);
			return number.Int32Value;
		}

		public static implicit operator NSDecimal (float value)
		{
			using var number = new NSNumber (value);
			return number.NSDecimalValue;
		}

		public static explicit operator float (NSDecimal value)
		{
			using var number = new NSDecimalNumber (value);
			return number.FloatValue;
		}

		public static implicit operator NSDecimal (double value)
		{
			using var number = new NSNumber (value);
			return number.NSDecimalValue;
		}

		public static explicit operator double (NSDecimal value)
		{
			using var number = new NSDecimalNumber (value);
			return number.DoubleValue;
		}

		public static implicit operator NSDecimal (decimal value)
		{
			using var number = new NSDecimalNumber (value.ToString (CultureInfo.InvariantCulture));
			return number.NSDecimalValue;
		}

		public static explicit operator decimal (NSDecimal value)
		{
			using var number = new NSDecimalNumber (value);
			return Decimal.Parse (number.ToString (), CultureInfo.InvariantCulture);
		}

		public bool Equals (NSDecimal other)
		{
			return this == other;
		}

		public override bool Equals (object obj)
		{
			return obj is NSDecimal other && this == other;
		}

		public override int GetHashCode ()
		{
			// this is heavy weight :( but it's the only way to follow .NET rule where:
			// "If two objects compare as equal, the GetHashCode method for each object must return the same value."
			// otherwise the same value (e.g. 100) can be represented with different values (e.g.
			// by varying mantissa an exponent)
			return ToString ().GetHashCode ();
		}
#endif // !COREBUILD
	}
}
