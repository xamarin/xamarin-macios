//
// Unit tests for NSCoder
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CoderTest {
		[Test]
		public void EncodeDecodeTest ()
		{
			var buffer = new byte [] { 3, 14, 15 };
			var obj = new NSString ();
			var ptr = Marshal.AllocHGlobal (buffer.Length);

			for (int i = 0; i < buffer.Length; i++)
				Marshal.WriteByte (ptr, i, buffer [i]);

			using (var mutableData = new NSMutableData (1024)) {
				using (var coder = new NSKeyedArchiver (mutableData)) {
					coder.Encode (obj, "obj");
					coder.Encode (buffer, "buffer");
					coder.Encode (Int32.MaxValue, "int32");
					coder.Encode (float.MaxValue, "float");
					coder.Encode (double.MaxValue, "double");
					coder.Encode (true, "bool");
					coder.Encode (long.MaxValue, "long");
					coder.Encode (buffer, 2, 1, "buffer2");
					coder.Encode (nint.MaxValue, "nint");
					coder.EncodeBlock (ptr, buffer.Length, "block");
					coder.FinishEncoding ();
				}

				using (var decoder = new NSKeyedUnarchiver (mutableData)) {
					Assert.IsNotNull (decoder.DecodeObject ("obj"));
					var buf = decoder.DecodeBytes ("buffer");
					Assert.AreEqual (buf.Length, buffer.Length, "buffer.length");
					for (int i = 0; i < buf.Length; i++)
						Assert.AreEqual (buf [i], buffer [i], "buffer [" + i.ToString () + "]");
					Assert.AreEqual (Int32.MaxValue, decoder.DecodeInt ("int32"));
					Assert.AreEqual (float.MaxValue, decoder.DecodeFloat ("float"));
					Assert.AreEqual (true, decoder.DecodeBool ("bool"));
					Assert.AreEqual (long.MaxValue, decoder.DecodeLong ("long"));
					buf = decoder.DecodeBytes ("buffer2");
					Assert.AreEqual (buf.Length, buffer.Length, "buffer2.length");
					for (int i = 0; i < buf.Length; i++)
						Assert.AreEqual (buf [i], buffer [i], "buffer2 [" + i.ToString () + "]");
					Assert.AreEqual (nint.MaxValue, decoder.DecodeNInt ("nint"));

					buf = decoder.DecodeBytes ("block");
					Assert.AreEqual (buf.Length, buffer.Length, "block.length");
					for (int i = 0; i < buf.Length; i++)
						Assert.AreEqual (buf [i], buffer [i], "block [" + i.ToString () + "]");
				}
			}

			Marshal.FreeHGlobal (ptr);

		}
	}
}
