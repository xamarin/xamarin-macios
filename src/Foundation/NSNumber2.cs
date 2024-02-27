//
// Copyright 2010, Novell, Inc.
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {
	public partial class NSNumber : NSValue
#if COREBUILD
	{
#else
	, IComparable, IComparable<NSNumber>, IEquatable<NSNumber> {

		public static implicit operator NSNumber (float value)
		{
			return FromFloat (value);
		}

		public static implicit operator NSNumber (double value)
		{
			return FromDouble (value);
		}

		public static implicit operator NSNumber (bool value)
		{
			return FromBoolean (value);
		}

		public static implicit operator NSNumber (sbyte value)
		{
			return FromSByte (value);
		}

		public static implicit operator NSNumber (byte value)
		{
			return FromByte (value);
		}

		public static implicit operator NSNumber (short value)
		{
			return FromInt16 (value);
		}

		public static implicit operator NSNumber (ushort value)
		{
			return FromUInt16 (value);
		}

		public static implicit operator NSNumber (int value)
		{
			return FromInt32 (value);
		}

		public static implicit operator NSNumber (uint value)
		{
			return FromUInt32 (value);
		}

		public static implicit operator NSNumber (long value)
		{
			return FromInt64 (value);
		}

		public static implicit operator NSNumber (ulong value)
		{
			return FromUInt64 (value);
		}

		public static explicit operator byte (NSNumber source)
		{
			return source.ByteValue;
		}

		public static explicit operator sbyte (NSNumber source)
		{
			return source.SByteValue;
		}

		public static explicit operator short (NSNumber source)
		{
			return source.Int16Value;
		}

		public static explicit operator ushort (NSNumber source)
		{
			return source.UInt16Value;
		}

		public static explicit operator int (NSNumber source)
		{
			return source.Int32Value;
		}

		public static explicit operator uint (NSNumber source)
		{
			return source.UInt32Value;
		}

		public static explicit operator long (NSNumber source)
		{
			return source.Int64Value;
		}

		public static explicit operator ulong (NSNumber source)
		{
			return source.UInt64Value;
		}

		public static explicit operator float (NSNumber source)
		{
			return source.FloatValue;
		}

		public static explicit operator double (NSNumber source)
		{
			return source.DoubleValue;
		}

		public static explicit operator bool (NSNumber source)
		{
			return source.BoolValue;
		}

		public NSNumber (nfloat value) :
#if ARCH_64
			this ((double)value)
#else
			this ((float) value)
#endif
		{
		}

		public nfloat NFloatValue {
			get {
#if ARCH_64
				return (nfloat)DoubleValue;
#else
				return (nfloat) FloatValue;
#endif
			}
		}

		public static NSNumber FromNFloat (nfloat value)
		{
#if ARCH_64
			return (FromDouble ((double)value));
#else
			return (FromFloat ((float) value));
#endif
		}

		public override string ToString ()
		{
			return StringValue;
		}

		public int CompareTo (object obj)
		{
			return CompareTo (obj as NSNumber);
		}

		public int CompareTo (NSNumber other)
		{
			// value must not be `nil` to call the `compare:` selector
			// that match well with the not same type of .NET check
			if (other is null)
				throw new ArgumentException ("other");
			return (int) Compare (other);
		}

		// should be present when implementing IComparable
		public override bool Equals (object other)
		{
			return Equals (other as NSNumber);
		}

		// IEquatable<NSNumber>
		public bool Equals (NSNumber other)
		{
			if (other is null)
				return false;
			return IsEqualToNumber (other);
		}

		public override int GetHashCode ()
		{
			// this is heavy weight :( but it's the only way to follow .NET rule where:
			// "If two objects compare as equal, the GetHashCode method for each object must return the same value."
			// otherwise NSNumber (1) needs to be != from NSNumber (1d), a breaking change from classic and 
			// something that's really not obvious
			return StringValue.GetHashCode ();
		}
#endif
	}
}
