#if __MACOS__
//
// Unit tests for CBUUID
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.
//

using System;

using Foundation;
using CoreBluetooth;
using ObjCRuntime;

using NUnit.Framework;
using Xamarin.Mac.Tests;

namespace MonoMacFixtures.CoreBluetooth {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CBUUIDTest {
		[Test]
		public void Roundtrip_16bits ()
		{
			using (CBUUID uuid = CBUUID.FromString ("1234")) {
				Assert.That (uuid.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.IsNotNull (uuid.Data, "Data");
				Assert.That (uuid.ToString (false), Is.EqualTo ("1234"), "ToString(false)");
				Assert.That (uuid.ToString (true), Is.EqualTo ("00001234-0000-1000-8000-00805f9b34fb"), "ToString(true)");
				using (CBUUID u2 = CBUUID.FromString (uuid.ToString ())) {
					Assert.That (u2.ToString (), Is.EqualTo (uuid.ToString ()), "Roundtrip");
				}
			}
		}

		[Test]
		public void Roundtrip_128bits ()
		{
			using (CBUUID uuid = CBUUID.FromString ("12345678-90AB-CDEF-cafe-c80c20443d0b")) {
				Assert.That (uuid.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.IsNotNull (uuid.Data, "Data");
				Assert.That (uuid.ToString (false), Is.EqualTo (uuid.ToString (true)), "ToString");
				using (CBUUID u2 = CBUUID.FromString (uuid.ToString ())) {
					Assert.That (u2.ToString (), Is.EqualTo (uuid.ToString ()), "Roundtrip");
				}
			}
		}

		static CBUUID MakeFull (byte a, byte b)
		{
			return MakeFull (0, 0, a, b);
		}

		static CBUUID MakeFull (byte a, byte b, byte c, byte d)
		{
			return CBUUID.FromBytes (new byte [] {
				a,    b,    c,    d,    0x00, 0x00, 0x10, 0x00,
				0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb
			});
		}

		[Test]
		public void Null ()
		{
			Assert.That (null as CBUUID, Is.Null);
			Assert.That (null as CBUUID, Is.EqualTo (null as CBUUID));
		}

		[Test]
		public void FullRandomEquals ()
		{
			var guid = Guid.NewGuid ();
			Assert.That (CBUUID.FromBytes (guid.ToByteArray ()),
				Is.EqualTo (CBUUID.FromBytes (guid.ToByteArray ())));
		}

		[Test]
		public void FullRandomNotEqual ()
		{
			Assert.That (CBUUID.FromBytes (Guid.NewGuid ().ToByteArray ()),
				Is.Not.EqualTo (CBUUID.FromBytes (Guid.NewGuid ().ToByteArray ())));
		}

		[Test]
		public void PartialEquals ()
		{
			var guid = new byte [] { 0xaa, 0xbb };
			Assert.That (CBUUID.FromBytes (guid),
				Is.EqualTo (CBUUID.FromBytes (guid)));

			// ObjC exception: Data <aabbccdd> does not represent a valid UUID
			//			guid = new byte [] { 0xaa, 0xbb, 0xcc, 0xdd };
			//			Assert.That (CBUUID.FromBytes (guid),
			//				Is.EqualTo (CBUUID.FromBytes (guid)));

			Assert.That (CBUUID.FromPartial (0x1234),
				Is.EqualTo (CBUUID.FromPartial (0x1234)));

			Assert.That (CBUUID.FromString ("1234"),
				Is.EqualTo (CBUUID.FromBytes (new byte [] { 0x12, 0x34 })));

			// ObjC exception: Data <12345678> does not represent a valid UUID
			//			Assert.That (CBUUID.FromString ("12345678"),
			//				Is.EqualTo (CBUUID.FromBytes (new byte [] { 0x12, 0x34, 0x56, 0x78 })));
		}

		[Test]
		public void PartialEqualsFull ()
		{
			Assert.That (CBUUID.FromPartial (0x0127), Is.EqualTo (MakeFull (0x01, 0x27)));

			// ObjC exception: Data <abcdef12> does not represent a valid UUID
			//			Assert.That (CBUUID.FromBytes (new byte [] { 0xab, 0xcd, 0xef, 0x12 }),
			//				Is.EqualTo (MakeFull (0xab, 0xcd, 0xef, 0x12)));

			Assert.That (CBUUID.FromString ("1234"),
				Is.EqualTo (CBUUID.FromString ("00001234-0000-1000-8000-00805f9b34fb")));

			// ObjC exception: Data <12345678> does not represent a valid UUID
			//			Assert.That (CBUUID.FromString ("12345678"),
			//				Is.EqualTo (CBUUID.FromString ("12345678-0000-1000-8000-00805f9b34fb")));
		}

		[Test]
		public void PartialsOfDifferentSizeNotEqual ()
		{
			// ObjC exception: Data <12345678> does not represent a valid UUID
			Assert.That (CBUUID.FromPartial (0x1234), Is.Not.EqualTo (
				CBUUID.FromString ("12345678-0000-1000-8000-00805f9b34fb")));
		}
	}
}
#endif // __MACOS__
