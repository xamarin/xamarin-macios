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
using NUnit.Framework;

#if NET
using System.Numerics;
#else
using CoreGraphics;
using OpenTK;
#endif

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
