//
// MDLVertexAttribute Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLVertexAttributeTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0+ or macOS 10.11+");

			if (
#if !MONOMAC
				Runtime.Arch == Arch.SIMULATOR && 
#endif
				IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				object dummy;
				using (var obj = new MDLVertexAttribute ()) {
					dummy = obj.BufferIndex;
					dummy = obj.Format;
					dummy = obj.InitializationValue;
					dummy = obj.Name;
					dummy = obj.Offset;
				}
			}
		}

		[Test]
		public void Ctors ()
		{
			using (var obj = new MDLVertexAttribute ("name", MDLVertexFormat.Float3, 1, 2)) {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual (MDLVertexFormat.Float3, obj.Format, "Format");
				Assert.AreEqual ((nuint) 1, obj.Offset, "Offset");
				Assert.AreEqual ((nuint) 2, obj.BufferIndex, "BufferIndex");
				Asserts.AreEqual (new Vector4 (0, 0, 0, 1), obj.InitializationValue, "InitializationValue");
			}
		}

		[Test]
		public void Properties ()
		{
			var V4 = new Vector4 (1, 2, 3, 4);

			using (var obj = new MDLVertexAttribute ("name", MDLVertexFormat.Float3, 1, 2)) {
				obj.Name = "new name";
				Assert.AreEqual ("new name", obj.Name, "Name");

				obj.Format = MDLVertexFormat.Float2;
				Assert.AreEqual (MDLVertexFormat.Float2, obj.Format, "Format");

				obj.Offset = 4;
				Assert.AreEqual ((nuint) 4, obj.Offset, "Offset");

				obj.BufferIndex = 9;
				Assert.AreEqual ((nuint) 9, obj.BufferIndex, "BufferIndex");
			}

			using (var obj = new MDLVertexAttribute ("name", MDLVertexFormat.Float3, 1, 2)) {
				obj.InitializationValue = V4;
				Asserts.AreEqual (V4, obj.InitializationValue, "InitializationValue");
			}
		}
	}
}

#endif // !__WATCHOS__
