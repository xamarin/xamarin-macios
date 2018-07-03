//
// CBUUID helpers
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//  Aaron Bockover <abock@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Text;
using Foundation;

namespace CoreBluetooth {

	public partial class CBUUID : IEquatable<CBUUID> {

		const string format16bits = "{0:x2}{1:x2}";
		const string format32bits = "{0:x2}{1:x2}{2:x2}{3:x2}";
		const string format128bits = "{0:x2}{1:x2}{2:x2}{3:x2}-0000-1000-8000-00805f9b34fb";

		const ulong highServiceBits = 0xfb349b5f80000080UL;
		const ulong lowServiceMask =  0x0010000000000000UL;

		public static CBUUID FromBytes (byte [] bytes)
		{
			if (bytes == null) {
				throw new ArgumentNullException ("bytes");
			} else if (bytes.Length != 2 && bytes.Length != 4 && bytes.Length != 16) {
				throw new ArgumentException ("must either be 2, 4, or 16 bytes long", "bytes");
			}

			using (var data = NSData.FromArray (bytes))
				return CBUUID.FromData (data);
		}

		public static CBUUID FromPartial (ushort servicePart)
		{
			return FromBytes (new [] {
				(byte)(servicePart >> 8),
				(byte)servicePart
			});
		}

		// allow roundtripping CBUUID.FromString (uuid.ToString ());
		// without string operations, ref: bug #7986
		public override string ToString ()
		{
			return ToString (false);
		}

		// note: having IEquatable<CBUUID> should be enough IMO
		public static bool operator == (CBUUID a, CBUUID b)
		{
			if (ReferenceEquals (a, null)) {
				return ReferenceEquals (b, null);
			}

			return a.Equals (b);
		}

		public static bool operator != (CBUUID a, CBUUID b)
		{
			return !(a == b);
		}

#if XAMCORE_2_0
		// to satisfy IEquatable<T>
		public unsafe bool Equals (CBUUID obj)
		{
			return base.Equals (obj);
		}

		// base class Equals is good enough
		// this fixes a compiler warning: CS0660: `CoreBluetooth.CBUUID' defines operator == or operator != but does not override Object.Equals(object o)
		public override bool Equals (object obj)
		{
			return base.Equals (obj);
		}

		// base class GetHashCode is good enough
		// this fixes a compiler warning: CS0661: `CoreBluetooth.CBUUID' defines operator == or operator != but does not override Object.GetHashCode()
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
#else
		public override bool Equals (object obj)
		{
			if (ReferenceEquals (this, obj)) {
				return true;
			}

			var other = obj as CBUUID;
			if (other != null) {
				return Equals (other);
			}

			return false;
		}

		public unsafe bool Equals (CBUUID obj)
		{
			if (obj == null) {
				return false;
			} else if (Data == null && obj.Data == null) {
				return true;
			} else if (Data == null || obj.Data == null) {
				return false;
			}

			var ad = Data;
			var bd = obj.Data;

			nuint a_len, b_len;
			IntPtr a, b;

			// Sort so that a is always smaller than b to reduce logic below
			if (ad.Length < bd.Length) {
				a_len = ad.Length;
				a = ad.Bytes;
				b_len = bd.Length;
				b = bd.Bytes;
			} else {
				a_len = bd.Length;
				a = bd.Bytes;
				b_len = ad.Length;
				b = ad.Bytes;
			}

			// If UUIDs are the same length, compare directly for each
			// supported size, 16, 32, and 128 bits. If the sizes differ,
			// one must be either 16 or 32 bits and the other 128 bits,
			// so mask the 16 and 32 bit ones with the Bluetooth Base UUID
			// (lowServiceMask . highServiceBits) and compare with the
			// other 128 bit UUID. Comparing 16-bit vs 32-bit UUIDs is
			// not supported.
			//
			// From what I gather from the googles and "Specification of
			// the Bluetooth System", Supplement to the Bluetooth Core
			// Specification, Version 1, 27 December 2011, 16 and 32 bit
			// service UUIDs cannot be equal, given that "The Service UUID
			// data types corresponding to 32-bit Service UUIDs shall not
			// be sent in advertising data packets." Therefore we just
			// return false, and we're really likely never to encounter a
			// 32-bit Service UUID in this API... --abock

			if (a_len == b_len) {
				switch (a_len) {
				case 2:
					return *((ushort *)a) == *((ushort *)b);
				case 4:
					return *((uint *)a) == *((uint *)b);
				case 16:
					return *((ulong *)a) == *((ulong *)b) &&
						*((ulong *)(a + 8)) == *((ulong *)(b + 8));
				}
			} else if (a_len == 2 && b_len == 16) {
				return *((ulong *)(b + 8)) == highServiceBits &&
					*((ulong *)b) == ((uint)(*((ushort *)a) << 16)
						| lowServiceMask);
			} else if (a_len == 4 && b_len == 16) {
				return *((ulong *)(b + 8)) == highServiceBits &&
					*((ulong *)b) == (*((uint *)a) | lowServiceMask);
			}

			return false;
		}

		public unsafe override int GetHashCode ()
		{
			// ensure we return something that satisfy:
			// "Two objects that are equal return hash codes that are equal."
			return (int) GetNativeHash ();
		}
#endif

		public unsafe string ToString (bool fullUuid)
		{
			NSData d = Data;
			if (d == null)
				return String.Empty;

			StringBuilder sb = new StringBuilder ();
			byte *p = (byte*) d.Bytes;

			switch (d.Length) {
			case 2:
				if (fullUuid) {
					sb.AppendFormat (format128bits, 0, 0, *p++, *p++);
				} else {
					sb.AppendFormat (format16bits, *p++, *p++);
				}
				break;
			case 4:
				sb.AppendFormat (fullUuid ? format128bits : format32bits, *p++, *p++, *p++, *p++);
				break;
			case 16:
				sb.AppendFormat ("{0:x2}{1:x2}{2:x2}{3:x2}-{4:x2}{5:x2}-{6:x2}{7:x2}-{8:x2}{9:x2}-{10:x2}{11:x2}{12:x2}{13:x2}{14:x2}{15:x2}",
					*p++, *p++, *p++, *p++, *p++, *p++, *p++, *p++,
					*p++, *p++, *p++, *p++, *p++, *p++, *p++, *p);
				break;
			}

			return sb.ToString ();
		}

#if MONOMAC
		// workaround for 27160443 – Trello: https://trello.com/c/oqB27JA6
		// try new constant (10.13+) and fallback to the old/misnamed one
		[Mac (10, 13)]
		public static NSString CharacteristicValidRangeString {
			get {
				return CBUUIDCharacteristicValidRangeString ?? CBUUIDValidRangeString;
			}
		}
#endif
	}
}
