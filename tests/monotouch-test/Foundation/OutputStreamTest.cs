//
// Unit tests for NSOutputStream
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OutputStreamTest {

		[Test]
		public void Path ()
		{
			using (var s = new NSOutputStream ("Info.plist", false)) {
				// initToFileAtPath:append: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void Memory ()
		{
			using (var s = new NSOutputStream ()) {
				// initToMemory does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public unsafe void Write ()
		{
			using (var s = (NSOutputStream) NSOutputStream.OutputStreamToMemory ()) {
				s.Open ();
				s.Write (new byte [] { 1, 2, 3 }, 3);
				using (var obj = s [NSStream.DataWrittenToMemoryStreamKey] as NSData) {
					Assert.IsNotNull (obj, "a");
					Assert.AreEqual (1, Marshal.ReadByte (obj.Bytes, 0), "a[0]");
					Assert.AreEqual (2, Marshal.ReadByte (obj.Bytes, 1), "a[1]");
					Assert.AreEqual (3, Marshal.ReadByte (obj.Bytes, 2), "a[2]");
				}
			}

			using (var s = new NSOutputStream ()) {
				s.Open ();
				s.Write (new byte [] { 1, 2, 3 });
				using (var obj = s [NSStream.DataWrittenToMemoryStreamKey] as NSData) {
					Assert.IsNotNull (obj, "a");
					Assert.AreEqual (1, Marshal.ReadByte (obj.Bytes, 0), "b[0]");
					Assert.AreEqual (2, Marshal.ReadByte (obj.Bytes, 1), "b[1]");
					Assert.AreEqual (3, Marshal.ReadByte (obj.Bytes, 2), "b[2]");
				}
			}

			using (var s = (NSOutputStream) NSOutputStream.OutputStreamToMemory ()) {
				s.Open ();
				s.Write (new byte [] { 1, 2, 3 }, 2, 1);
				using (var obj = s [NSStream.DataWrittenToMemoryStreamKey] as NSData) {
					Assert.IsNotNull (obj, "a");
					Assert.AreEqual (3, Marshal.ReadByte (obj.Bytes, 0), "c[0]");
				}
			}
		}
	}
}
