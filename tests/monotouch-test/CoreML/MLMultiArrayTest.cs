//
// Unit tests for MLMultiArrayConstraint
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreML;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreML {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MLMultiArrayTest {
		[Test]
		public void Ctors ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			NSError err;
			var shape = new nint [] { 1 };
			var strides = new nint [] { 0 };
			var nsshape = new NSNumber [] { NSNumber.FromNInt (1) };
			var nsstrides = new NSNumber [] { NSNumber.FromNInt (0) };

			using (var arr = new MLMultiArray (shape, MLMultiArrayDataType.Int32, out err)) {
				Assert.AreEqual (shape, arr.Shape, "1 Shape");
				Assert.AreEqual (MLMultiArrayDataType.Int32, arr.DataType, "1 DataType");
				Assert.IsNull (err, "1 err");
			}

			using (var arr = new MLMultiArray (IntPtr.Zero, shape, MLMultiArrayDataType.Float32, strides, (v) => Marshal.FreeHGlobal (v), out err)) {
				Assert.AreEqual (shape, arr.Shape, "2 Shape");
				Assert.AreEqual (MLMultiArrayDataType.Float32, arr.DataType, "2 DataType");
				Assert.AreEqual (strides, arr.Strides, "2 Strides");
				Assert.IsNull (err, "2 err");
			}

			using (var arr = new MLMultiArray (IntPtr.Zero, nsshape, MLMultiArrayDataType.Double, nsstrides, (v) => Marshal.FreeHGlobal (v), out err)) {
				Assert.AreEqual (shape, arr.Shape, "3 Shape");
				Assert.AreEqual (MLMultiArrayDataType.Double, arr.DataType, "3 DataType");
				Assert.AreEqual (strides, arr.Strides, "3 Strides");
				Assert.AreEqual (IntPtr.Zero, arr.DataPointer, "3 DataPointer");
				Assert.IsNull (err, "3 err");
			}

			using (var arr = new MLMultiArray (nsshape, MLMultiArrayDataType.Int32, out err)) {
				Assert.AreEqual (shape, arr.Shape, "4 Shape");
				Assert.AreEqual (MLMultiArrayDataType.Int32, arr.DataType, "4 DataType");
				Assert.IsNull (err, "4 err");
			}
		}

		[Test]
		public void Indexers ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			NSError err;
			var shape = new nint [] { 10 };
			using (var arr = new MLMultiArray (shape, MLMultiArrayDataType.Int32, out err)) {
				Assert.IsNull (err, "err");
				Assert.AreEqual ((nint) 10, arr.Count, "Count");
				Assert.AreEqual (new nint [] { 10 }, arr.Shape, "Shape");
				Assert.AreEqual (new nint [] { 1 }, arr.Strides, "Strides");

				arr [0] = 0; // MLMultiArray's elements aren't zero-initialized
				Assert.AreEqual (0, arr [0].Int32Value, "a");
				Assert.AreEqual (0, arr [new nint [] { 0 }].Int32Value, "b");
				Assert.AreEqual (0, arr [new NSNumber [] { NSNumber.FromNInt (0) }].Int32Value, "c nint");
				Assert.AreEqual (0, arr [new NSNumber [] { NSNumber.FromInt32 (0) }].Int32Value, "c int32");
				Assert.AreEqual (0, arr [new NSNumber [] { NSNumber.FromByte (0) }].Int32Value, "c byte");
				Assert.AreEqual (0, arr [new NSNumber [] { NSNumber.FromFloat (0) }].Int32Value, "c float");

				Assert.AreEqual (0, arr.GetObject (0).Int32Value, "GetObject a");
				Assert.AreEqual (0, arr.GetObject (new nint [] { 0 }).Int32Value, "GetObject b");
				Assert.AreEqual (0, arr.GetObject (new NSNumber [] { NSNumber.FromNInt (0) }).Int32Value, "GetObject c nint");
				Assert.AreEqual (0, arr.GetObject (new NSNumber [] { NSNumber.FromInt32 (0) }).Int32Value, "GetObject c int32");
				Assert.AreEqual (0, arr.GetObject (new NSNumber [] { NSNumber.FromByte (0) }).Int32Value, "GetObject c byte");
				Assert.AreEqual (0, arr.GetObject (new NSNumber [] { NSNumber.FromFloat (0) }).Int32Value, "GetObject c float");

				arr [1] = NSNumber.FromInt32 (1);
				arr [new nint [] { 2 }] = NSNumber.FromInt32 (2);
				arr [new NSNumber [] { NSNumber.FromUInt16 (3) }] = NSNumber.FromInt32 (3);
				arr.SetObject (NSNumber.FromInt32 (4), 4);
				arr.SetObject (NSNumber.FromInt32 (5), new nint [] { 5 });
				arr.SetObject (NSNumber.FromInt32 (6), new NSNumber [] { NSNumber.FromSByte (6) });

				Assert.AreEqual (1, arr [1].Int32Value, "1");
				Assert.AreEqual (2, arr [2].Int32Value, "2");
				Assert.AreEqual (3, arr [3].Int32Value, "3");
				Assert.AreEqual (4, arr [4].Int32Value, "4");
				Assert.AreEqual (5, arr [5].Int32Value, "5");
				Assert.AreEqual (6, arr [6].Int32Value, "6");
			}

			// multi-dimensional
			shape = new nint [] { 7, 7, 7 };
			using (var arr = new MLMultiArray (shape, MLMultiArrayDataType.Int32, out err)) {
				Assert.IsNull (err, "err");
				Assert.AreEqual (shape [0] * shape [1] * shape [2], arr.Count, "Count");

				arr [0, 0, 0] = 0; // MLMultiArray's elements aren't zero-initialized
				Assert.AreEqual (0, arr [0, 0, 0].Int32Value, "a");
				Assert.AreEqual (0, arr [new nint [] { 0, 0, 0 }].Int32Value, "b");
				Assert.AreEqual (0, arr [new NSNumber [] { NSNumber.FromNInt (0), NSNumber.FromNInt (0), NSNumber.FromNInt (0) }].Int32Value, "c nint");

				Assert.AreEqual (0, arr.GetObject (0, 0, 0).Int32Value, "GetObject a");
				Assert.AreEqual (0, arr.GetObject (new nint [] { 0, 0, 0 }).Int32Value, "GetObject b");
				Assert.AreEqual (0, arr.GetObject (new NSNumber [] { NSNumber.FromNInt (0), NSNumber.FromNInt (0), NSNumber.FromNInt (0) }).Int32Value, "GetObject c nint");

				arr [1, 1, 1] = NSNumber.FromInt32 (1);
				arr [new nint [] { 2, 2, 2 }] = NSNumber.FromInt32 (2);
				arr [new NSNumber [] { NSNumber.FromUInt16 (3), NSNumber.FromUInt16 (3), NSNumber.FromUInt16 (3) }] = NSNumber.FromInt32 (3);
				arr.SetObject (NSNumber.FromInt32 (4), 4, 4, 4);
				arr.SetObject (NSNumber.FromInt32 (5), new nint [] { 5, 5, 5 });
				arr.SetObject (NSNumber.FromInt32 (6), new NSNumber [] { NSNumber.FromSByte (6), NSNumber.FromSByte (6), NSNumber.FromSByte (6) });

				Assert.AreEqual (1, arr [1, 1, 1].Int32Value, "1");
				Assert.AreEqual (2, arr [2, 2, 2].Int32Value, "2");
				Assert.AreEqual (3, arr [3, 3, 3].Int32Value, "3");
				Assert.AreEqual (4, arr [4, 4, 4].Int32Value, "4");
				Assert.AreEqual (5, arr [5, 5, 5].Int32Value, "5");
				Assert.AreEqual (6, arr [6, 6, 6].Int32Value, "6");
			}
		}
	}
}
