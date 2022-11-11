//
// Unit tests for CBUUID
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//  Aaron Bockover <abock@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
using CoreBluetooth;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreBluetooth {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UuidTest {

		[Test]
		public void Roundtrip_16bits ()
		{
			using (CBUUID uuid = CBUUID.FromString ("1234")) {
				Assert.That (uuid.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.IsNotNull (uuid.Data, "Data");
				var firstExpected = "Unknown (<1234>)";
				var secondExpected = "1234";
				Assert.That (uuid.Description, Is.EqualTo (firstExpected).Or.EqualTo (secondExpected), "Description");
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
				var firstExpected = "Unknown (<12345678 90abcdef cafec80c 20443d0b>)";
				var secondExpected = "12345678-90AB-CDEF-CAFE-C80C20443D0B";
				Assert.That (uuid.Description, Is.EqualTo (firstExpected).Or.EqualTo (secondExpected), "Description");
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
		public void Equality_Null ()
		{
			Assert.That (null as CBUUID, Is.Null);
			Assert.That (null as CBUUID, Is.EqualTo (null as CBUUID));
		}

		[Test]
		public void Equality_FullRandomEquals ()
		{
			var guid = Guid.NewGuid ();
			Assert.That (CBUUID.FromBytes (guid.ToByteArray ()),
				Is.EqualTo (CBUUID.FromBytes (guid.ToByteArray ())));
		}

		[Test]
		public void Equality_FullRandomNotEqual ()
		{
			Assert.That (CBUUID.FromBytes (Guid.NewGuid ().ToByteArray ()),
				Is.Not.EqualTo (CBUUID.FromBytes (Guid.NewGuid ().ToByteArray ())));
		}

		[Test]
		public void Equality_PartialEquals ()
		{
			var guid = new byte [] { 0xaa, 0xbb };
			using (var u1 = CBUUID.FromBytes (guid))
			using (var u2 = CBUUID.FromBytes (guid)) {
				Assert.True (u1.Equals ((object) u2), "Equals-1a");
				Assert.True (u1.Equals ((NSObject) u2), "Equals-1b");
				Assert.True (u1.Equals ((CBUUID) u2), "Equals-1b");
				Assert.That (u1.GetHashCode (), Is.EqualTo (u2.GetHashCode ()), "GetHashCode-1");
			}

			using (var u1 = CBUUID.FromPartial (0x1234))
			using (var u2 = CBUUID.FromPartial (0x1234)) {
				Assert.True (u1.Equals ((object) u2), "Equals-2a");
				Assert.True (u1.Equals ((NSObject) u2), "Equals-2b");
				Assert.True (u1.Equals ((CBUUID) u2), "Equals-2b");
				Assert.That (u1.GetHashCode (), Is.EqualTo (u2.GetHashCode ()), "GetHashCode-2");
			}

			using (var u1 = CBUUID.FromString ("1234"))
			using (var u2 = CBUUID.FromBytes (new byte [] { 0x12, 0x34 })) {
				Assert.True (u1.Equals ((object) u2), "Equals-3a");
				Assert.True (u1.Equals ((NSObject) u2), "Equals-3b");
				Assert.True (u1.Equals ((CBUUID) u2), "Equals-3b");
				Assert.That (u1.GetHashCode (), Is.EqualTo (u2.GetHashCode ()), "GetHashCode-3");
			}
#if MONOMAC
			if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 10)) {
				guid = new byte [] { 0xaa, 0xbb, 0xcc, 0xdd };
				Assert.That (CBUUID.FromBytes (guid),
					Is.EqualTo (CBUUID.FromBytes (guid)));

				Assert.That (CBUUID.FromString ("12345678"),
					Is.EqualTo (CBUUID.FromBytes (new byte [] { 0x12, 0x34, 0x56, 0x78 })));
			}
#endif
		}

		[Test]
		public void Equality_PartialEqualsFull ()
		{
			using (var u1 = CBUUID.FromPartial (0x0127))
			using (var u2 = MakeFull (0x01, 0x27)) {
				Assert.True (u1.Equals ((object) u2), "Equals-1a");
				Assert.True (u1.Equals ((NSObject) u2), "Equals-1b");
				Assert.True (u1.Equals ((CBUUID) u2), "Equals-1b");
				Assert.That (u1.GetHashCode (), Is.EqualTo (u2.GetHashCode ()), "GetHashCode-1");
			}

			using (var u1 = CBUUID.FromBytes (new byte [] { 0xab, 0xcd }))
			using (var u2 = MakeFull (0xab, 0xcd)) {
				Assert.True (u1.Equals ((object) u2), "Equals-2a");
				Assert.True (u1.Equals ((NSObject) u2), "Equals-2b");
				Assert.True (u1.Equals ((CBUUID) u2), "Equals-2b");
				Assert.That (u1.GetHashCode (), Is.EqualTo (u2.GetHashCode ()), "GetHashCode-2");
			}

			using (var u1 = CBUUID.FromString ("1234"))
			using (var u2 = CBUUID.FromString ("00001234-0000-1000-8000-00805f9b34fb")) {
				Assert.True (u1.Equals ((object) u2), "Equals-3a");
				Assert.True (u1.Equals ((NSObject) u2), "Equals-3b");
				Assert.True (u1.Equals ((CBUUID) u2), "Equals-3b");
				Assert.That (u1.GetHashCode (), Is.EqualTo (u2.GetHashCode ()), "GetHashCode-3");
			}
#if MONOMAC
			if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 10)) {
				Assert.That (CBUUID.FromBytes (new byte [] { 0xab, 0xcd, 0xef, 0x12 }),
					Is.EqualTo (MakeFull (0xab, 0xcd, 0xef, 0x12)));

				Assert.That (CBUUID.FromString ("12345678"),
					Is.EqualTo (CBUUID.FromString ("12345678-0000-1000-8000-00805f9b34fb")));
			}
#endif
		}

		[Test]
		public void Equality_PartialsOfDifferentSizeNotEqual ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
#if MONOMAC
			Assert.That (CBUUID.FromPartial (0x1234), Is.Not.EqualTo (
				CBUUID.FromBytes (new byte [] { 0x12, 0x34, 0x56, 0x78 })));
#endif
		}
	}
}

#endif // !__WATCHOS__
