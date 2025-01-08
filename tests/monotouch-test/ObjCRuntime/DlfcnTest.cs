//
// Unit tests for Dlfcn
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DlfcnTest {

		[Test]
		public void OpenClose_libSystem ()
		{
			IntPtr handle = Dlfcn.dlopen ("/usr/lib/libSystem.dylib", 0);
			Assert.That (handle, Is.Not.EqualTo (IntPtr.Zero), "dlopen");
			var err = Dlfcn.dlclose (handle);
			var expected = 0;
#if !MONOMAC && !__MACCATALYST__
			if (Runtime.Arch == Arch.DEVICE && TestRuntime.CheckXcodeVersion (7, 0) && !TestRuntime.CheckXcodeVersion (10, 0)) {
				// Apple is doing some funky stuff with dlopen... this condition is to track if this change during betas
				expected = -1;
			}
#endif
			Assert.That (err, Is.EqualTo (expected), "dlclose");
		}

#if NET
		[Test]
		public void GetVariables ()
		{
			var symbol = "x_native_field";
			var handle = (IntPtr) Dlfcn.RTLD.Default;

			Assert.AreNotEqual (IntPtr.Zero, Dlfcn.dlsym (handle, symbol), "Symbol");

			var originalValue = Dlfcn.GetUInt64 (handle, symbol);
			Assert.Multiple (() => {
				unchecked {
					// the n(uint) and (U)IntPtr asserts only work in 64-bit, which is fine because we only care about 64-bit right now.
					Assert.AreEqual ((ushort) 0x8899, (ushort) Dlfcn.GetInt16 (handle, symbol), "GetInt16");
					Assert.AreEqual ((uint) 0xeeff8899, (uint) Dlfcn.GetInt32 (handle, symbol), "GetInt32");
					Assert.AreEqual ((ulong) 0xaabbccddeeff8899, (ulong) Dlfcn.GetInt64 (handle, symbol), "GetInt64");
					Assert.AreEqual ((nuint) 0xaabbccddeeff8899, (nuint) Dlfcn.GetNInt (handle, symbol), "GetNInt");
					Assert.AreEqual ((ushort) 0x8899, Dlfcn.GetUInt16 (handle, symbol), "GetUInt16");
					Assert.AreEqual ((uint) 0xeeff8899, Dlfcn.GetUInt32 (handle, symbol), "GetUInt32");
					Assert.AreEqual ((ulong) 0xaabbccddeeff8899, Dlfcn.GetUInt64 (handle, symbol), "GetUInt64");
					Assert.AreEqual ((nuint) 0xaabbccddeeff8899, Dlfcn.GetNUInt (handle, symbol), "GetNUInt");
					Assert.AreEqual ((nfloat) (-7.757653393002521E-103), Dlfcn.GetNFloat (handle, symbol), "GetNFloat");
					Assert.AreEqual (-7.7576533930025207E-103d, Dlfcn.GetDouble (handle, symbol), "GetDouble");
					Assert.AreEqual ((nuint) 0xaabbccddeeff8899, (nuint) Dlfcn.GetIntPtr (handle, symbol), "GetIntPtr"); // won't work in 32-bit, but we don't care about that anymore
					Assert.AreEqual ((nuint) 0xaabbccddeeff8899, Dlfcn.GetUIntPtr (handle, symbol), "GetUIntPtr");
					Assert.AreEqual ((nint) 0xaabbccddeeff8899, Dlfcn.GetStruct<nint> (handle, symbol), "GetStruct<nint>"); // won't work in 32-bit, but we don't care about that anymore
					Assert.AreEqual ((nuint) 0xaabbccddeeff8899, Dlfcn.GetStruct<nuint> (handle, symbol), "GetStruct<nuint>"); // won't work in 32-bit, but we don't care about that anymore
					Assert.AreEqual ((long) 0xaabbccddeeff8899, Dlfcn.GetStruct<long> (handle, symbol), "GetStruct<long>");
					Assert.AreEqual ((ulong) 0xaabbccddeeff8899, Dlfcn.GetStruct<ulong> (handle, symbol), "GetStruct<ulong>");
					Assert.AreEqual ((int) 0xeeff8899, Dlfcn.GetStruct<int> (handle, symbol), "GetStruct<int>");
					Assert.AreEqual ((uint) 0xeeff8899, Dlfcn.GetStruct<uint> (handle, symbol), "GetStruct<uint>");
					Assert.AreEqual ((ulong) 0xaabbccddeeff8899, Dlfcn.GetStruct<SomeValue> (handle, symbol).Value, "GetStruct<SomeValue>");
					Assert.AreEqual (-3.9541907E+28f, Dlfcn.GetStruct<float> (handle, symbol), "GetStruct<float>");
					Assert.AreEqual (-7.7576533930025207E-103d, Dlfcn.GetStruct<double> (handle, symbol), "GetStruct<double>");

					Assert.AreEqual ((ulong) 0, Dlfcn.GetStruct<ulong> (handle, "inexistent_symbol"), "GetStruct<ulong> inexistent");
					Assert.AreEqual ((ulong) 0, Dlfcn.GetStruct<SomeValue> (handle, "inexistent_symbol").Value, "GetStruct<SomeValue> inexistent");

					Dlfcn.SetInt16 (handle, symbol, 0x77);
					Assert.AreEqual ((short) 0x77, Dlfcn.GetInt16 (handle, symbol), "SetInt16");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetInt32 (handle, symbol, 0x77);
					Assert.AreEqual ((int) 0x77, Dlfcn.GetInt32 (handle, symbol), "SetInt32");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetInt64 (handle, symbol, 0x77);
					Assert.AreEqual ((long) 0x77, Dlfcn.GetInt64 (handle, symbol), "SetInt64");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetNInt (handle, symbol, 0x77);
					Assert.AreEqual ((nint) 0x77, Dlfcn.GetNInt (handle, symbol), "SetNInt");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetUInt16 (handle, symbol, 0x77);
					Assert.AreEqual ((ushort) 0x77, Dlfcn.GetUInt16 (handle, symbol), "SetUInt16");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetUInt32 (handle, symbol, 0x77);
					Assert.AreEqual ((uint) 0x77, Dlfcn.GetUInt32 (handle, symbol), "SetUInt32");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetUInt64 (handle, symbol, 0x77);
					Assert.AreEqual ((ulong) 0x77, Dlfcn.GetUInt64 (handle, symbol), "SetUInt64");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetNUInt (handle, symbol, 0x77);
					Assert.AreEqual ((nuint) 0x77, Dlfcn.GetNUInt (handle, symbol), "SetNUInt");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetNFloat (handle, symbol, 0x77);
					Assert.AreEqual ((nfloat) 0x77, Dlfcn.GetNFloat (handle, symbol), "SetNFloat");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetDouble (handle, symbol, 0x77);
					Assert.AreEqual (0x77, Dlfcn.GetDouble (handle, symbol), "SetDouble");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetIntPtr (handle, symbol, 0x77);
					Assert.AreEqual ((nint) 0x77, Dlfcn.GetIntPtr (handle, symbol), "SetIntPtr");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);

					Dlfcn.SetUIntPtr (handle, symbol, 0x77);
					Assert.AreEqual ((nuint) 0x77, Dlfcn.GetUIntPtr (handle, symbol), "SetUIntPtr");
					Dlfcn.SetUInt64 (handle, symbol, originalValue);
				}
			});
		}

#pragma warning disable CS0649 // Field 'DlfcnTest.SomeValue.Value' is never assigned to, and will always have its default value 0
		struct SomeValue {
			public ulong Value;
		}
#pragma warning restore CS0649
#endif
	}
}
