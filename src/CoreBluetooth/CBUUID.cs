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

#nullable enable

namespace CoreBluetooth {

	public partial class CBUUID : IEquatable<CBUUID> {

		const string format16bits = "{0:x2}{1:x2}";
		const string format32bits = "{0:x2}{1:x2}{2:x2}{3:x2}";
		const string format128bits = "{0:x2}{1:x2}{2:x2}{3:x2}-0000-1000-8000-00805f9b34fb";

		const ulong highServiceBits = 0xfb349b5f80000080UL;
		const ulong lowServiceMask = 0x0010000000000000UL;

		public static CBUUID FromBytes (byte [] bytes)
		{
			if (bytes is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bytes));
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

		// to satisfy IEquatable<T>
		public unsafe bool Equals (CBUUID? obj)
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

		public unsafe string ToString (bool fullUuid)
		{
			NSData d = Data;
			if (d is null)
				return String.Empty;

			StringBuilder sb = new StringBuilder ();
			byte* p = (byte*) d.Bytes;

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

#if MONOMAC && !NET
		// workaround for 27160443 – Trello: https://trello.com/c/oqB27JA6
		// try new constant (10.13+) and fallback to the old/misnamed one
		public static NSString CharacteristicValidRangeString {
			get {
				return CBUUIDCharacteristicValidRangeString ?? CBUUIDValidRangeString;
			}
		}
#endif
	}
}
