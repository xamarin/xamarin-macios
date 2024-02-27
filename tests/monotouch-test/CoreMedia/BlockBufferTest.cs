//
// Unit tests for CMBlockBuffer
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using System.Reflection;

using Foundation;
using CoreMedia;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BlockBufferTest {

		[Test]
		public void CreateEmpty ()
		{
			CMBlockBufferError err;
			using (var bb = CMBlockBuffer.CreateEmpty (16, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), "error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 0), "DataLength");
			}
		}

		[Test]
		public void CMBlockBufferCustomBlockSource ()
		{
			var type = typeof (CMCustomBlockAllocator).GetNestedType ("CMBlockBufferCustomBlockSource", BindingFlags.NonPublic);
			Assert.NotNull (type, "CMBlockBufferCustomBlockSource");
			// it's 28 (not 32) bytes when executed on 64bits iOS, which implies it's packed to 4 bytes
#pragma warning disable IL3050 // Using member 'System.Runtime.InteropServices.Marshal.SizeOf(Type)' which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. Marshalling code for the object might not be available. Use the SizeOf<T> overload instead.
			Assert.That (Marshal.SizeOf (type), Is.EqualTo (4 + 3 * IntPtr.Size), "Size");
#pragma warning restore IL3050
		}

		[Test]
		public void AppendMemoryBlockTest ()
		{
			CMBlockBufferError err1;
			CMBlockBufferError err2;
			using (var bb = CMBlockBuffer.FromMemoryBlock (IntPtr.Zero, 16, null, 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err1)) {
				Assert.That (err1, Is.EqualTo (CMBlockBufferError.None), "FromMemoryBlock error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 5), "FromMemoryBlock DataLength");

				var ptr = Marshal.AllocHGlobal (16);
				err2 = bb.AppendMemoryBlock (ptr, 16, null, 0, 5, CMBlockBufferFlags.AssureMemoryNow);
				Assert.That (err2, Is.EqualTo (CMBlockBufferError.None), "AppendMemoryBlock error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 10), "AppendMemoryBlock DataLength");
			}

			using (var bb = CMBlockBuffer.FromMemoryBlock (IntPtr.Zero, 16, new CMCustomBlockAllocator (), 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err1)) {
				Assert.That (err1, Is.EqualTo (CMBlockBufferError.None), "FromMemoryBlock error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 5), "FromMemoryBlock DataLength");

				var ptr = Marshal.AllocHGlobal (16);
				err2 = bb.AppendMemoryBlock (ptr, 16, new CMCustomBlockAllocator (), 0, 5, CMBlockBufferFlags.AssureMemoryNow);
				Assert.That (err2, Is.EqualTo (CMBlockBufferError.None), "AppendMemoryBlock error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 10), "AppendMemoryBlock DataLength");
			}

			allocateCalled = false;
			freeCalled = false;
			using (var allocator = new CustomAllocator (this))
			using (var bb = CMBlockBuffer.FromMemoryBlock (IntPtr.Zero, 16, new CMCustomBlockAllocator (), 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err1)) {
				Assert.That (err1, Is.EqualTo (CMBlockBufferError.None), "FromMemoryBlock error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 5), "FromMemoryBlock DataLength");

				var ptr = Marshal.AllocHGlobal (16);
				err2 = bb.AppendMemoryBlock (ptr, 16, allocator, 0, 5, CMBlockBufferFlags.AssureMemoryNow);
				Assert.That (err2, Is.EqualTo (CMBlockBufferError.None), "AppendMemoryBlock error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 10), "FromMemoryBlock DataLength");
			}
			Assert.IsTrue (freeCalled, "FromMemoryBlock FreeCalled");
		}

		public bool allocateCalled;
		public bool freeCalled;

		[Test]
		public void FromMemoryBlockAndContiguousTest ()
		{
			CMBlockBufferError err1;
			CMBlockBufferError err2;
			using (var bb = CMBlockBuffer.FromMemoryBlock (IntPtr.Zero, 16, null, 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err1))
			using (var bc = CMBlockBuffer.CreateContiguous (bb, null, 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err2)) {
				Assert.That (err1, Is.EqualTo (CMBlockBufferError.None), "FromMemoryBlock error");
				Assert.That (err2, Is.EqualTo (CMBlockBufferError.None), "CreateContiguous error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 5), "FromMemoryBlock DataLength");
				Assert.That (bc.DataLength, Is.EqualTo ((nuint) 5), "CreateContiguous DataLength");
			}

			using (var allocator = new CMCustomBlockAllocator ())
			using (var bb = CMBlockBuffer.FromMemoryBlock (IntPtr.Zero, 16, allocator, 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err1))
			using (var bc = CMBlockBuffer.CreateContiguous (bb, allocator, 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err2)) {
				Assert.That (err1, Is.EqualTo (CMBlockBufferError.None), "FromMemoryBlock error");
				Assert.That (err2, Is.EqualTo (CMBlockBufferError.None), "CreateContiguous error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 5), "FromMemoryBlock DataLength");
				Assert.That (bc.DataLength, Is.EqualTo ((nuint) 5), "CreateContiguous DataLength");
			}

			using (var allocator = new CustomAllocator (this))
			using (var bb = CMBlockBuffer.FromMemoryBlock (IntPtr.Zero, 16, allocator, 0, 5, CMBlockBufferFlags.AssureMemoryNow, out err1)) {
				Assert.That (err1, Is.EqualTo (CMBlockBufferError.None), "FromMemoryBlock error");
				Assert.That (bb.DataLength, Is.EqualTo ((nuint) 5), "FromMemoryBlock DataLength");
				Assert.IsTrue (allocateCalled, "FromMemoryBlock AllocateCalled");
			}
			Assert.IsTrue (freeCalled, "FromMemoryBlock FreeCalled");
		}

		class CustomAllocator : CMCustomBlockAllocator {
			BlockBufferTest test;

			public CustomAllocator (BlockBufferTest bufferTest)
			{
				test = bufferTest;
			}

			public override IntPtr Allocate (nuint sizeInBytes)
			{
				test.allocateCalled = true;
				return base.Allocate (sizeInBytes);
			}

			public override void Free (IntPtr doomedMemoryBlock, nuint sizeInBytes)
			{
				test.freeCalled = true;
				base.Free (doomedMemoryBlock, sizeInBytes);
			}
		}

		[Test]
		public void FromMemoryBlockWithManagedMemory ()
		{
			byte [] data = new byte [32768];
			GCHandle pinned = GCHandle.Alloc (data, GCHandleType.Pinned);
			IntPtr pointer = pinned.AddrOfPinnedObject ();
			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (pointer, (uint) data.Length, null, 0, (uint) data.Length, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), "CMBlockBufferError");
				// dispose called before unpinning (ok)
			}
			pinned.Free ();
		}

		[Test]
		public void FromMemoryBlockWithByteArrayTest ()
		{
			byte [] data = new byte [32768];
			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (data, 0, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), "CMBlockBufferError");
				// dispose called before unpinning (ok)
			}
		}

		[Test]
		public void AppendMemoryBlockWithManagedMemory ()
		{
			byte [] data = new byte [32768];
			byte [] data2 = new byte [32768];
			GCHandle pinned = GCHandle.Alloc (data, GCHandleType.Pinned);
			GCHandle pinned2 = GCHandle.Alloc (data2, GCHandleType.Pinned);
			IntPtr pointer = pinned.AddrOfPinnedObject ();
			IntPtr pointer2 = pinned2.AddrOfPinnedObject ();
			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (pointer, (uint) data.Length, null, 0, (uint) data.Length, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");

				err = buf.AppendMemoryBlock (pointer2, (uint) data2.Length, null, 0, (uint) data2.Length, CMBlockBufferFlags.AssureMemoryNow);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
			}
			pinned2.Free ();
			pinned.Free ();
		}

		[Test]
		public void AppendMemoryBlockWithByteArrayTest ()
		{
			byte [] data = new byte [32768];
			byte [] data2 = new byte [32768];
			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (data, 0, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");

				err = buf.AppendMemoryBlock (data2, 0, CMBlockBufferFlags.AssureMemoryNow);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
			}
		}

		[Test]
		public void CopyDataBytesTest ()
		{
			byte [] data = new byte [] { 0x0, 0x1, 0x2, 0x3, 0x4 };
			byte [] destData = new byte [data.Length];
			GCHandle pinned = GCHandle.Alloc (data, GCHandleType.Pinned);
			GCHandle destPinned = GCHandle.Alloc (destData, GCHandleType.Pinned);
			IntPtr pointer = pinned.AddrOfPinnedObject ();
			IntPtr destPointer = destPinned.AddrOfPinnedObject ();

			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (pointer, (uint) data.Length, null, 0, (uint) data.Length, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");

				err = buf.CopyDataBytes (0, (uint) data.Length, destPointer);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
				for (int i = 0; i < data.Length; i++)
					Assert.AreEqual (data [0], destData [0], $"CMBlockBuffer CopyDataBytesTest iteration: {i}");
			}
			pinned.Free ();
			destPinned.Free ();
		}

		[Test]
		public void CopyDataBytesUsingManagedArrayTest ()
		{
			byte [] data = new byte [] { 0x0, 0x1, 0x2, 0x3, 0x4 };
			byte [] destData;
			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (data, 0, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");

				err = buf.CopyDataBytes (0, (uint) data.Length, out destData);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
				for (int i = 0; i < data.Length; i++)
					Assert.AreEqual (data [0], destData [0], $"CMBlockBuffer CopyDataBytesUsingManagedArrayTest iteration: {i}");
			}
		}

		[Test]
		public void ReplaceDataBytesTest ()
		{
			byte [] data = new byte [] { 0x0, 0x1, 0x2, 0x3, 0x4 };
			byte [] replaceData = new byte [] { 0x5, 0x5, 0x5, 0x5, 0x5 };
			GCHandle pinned = GCHandle.Alloc (data, GCHandleType.Pinned);
			GCHandle replacePinned = GCHandle.Alloc (replaceData, GCHandleType.Pinned);
			IntPtr pointer = pinned.AddrOfPinnedObject ();
			IntPtr replacePointer = replacePinned.AddrOfPinnedObject ();

			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (pointer, (uint) data.Length, null, 0, (uint) data.Length, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");

				err = buf.ReplaceDataBytes (replacePointer, 0, (uint) replaceData.Length);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
				for (int i = 0; i < data.Length; i++)
					Assert.AreEqual (0x5, data [0], $"CMBlockBuffer ReplaceDataBytesTest iteration: {i}");
			}
			pinned.Free ();
			replacePinned.Free ();
		}

		[Test]
		public void ReplaceDataBytesManagedTest ()
		{
			byte [] data = new byte [] { 0x0, 0x1, 0x2, 0x3, 0x4 };
			byte [] replaceData = new byte [] { 0x5, 0x5, 0x5, 0x5, 0x5 };

			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (data, 0, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");

				err = buf.ReplaceDataBytes (replaceData, 0);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
				for (int i = 0; i < data.Length; i++)
					Assert.AreEqual (0x5, data [0], $"CMBlockBuffer ReplaceDataBytesManagedTest iteration: {i}");
			}
		}

		[Test]
		public void AccessDataBytesTest ()
		{
			byte [] data = new byte [] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9 };
			byte [] tempBuffer = new byte [5];
			GCHandle pinned = GCHandle.Alloc (data, GCHandleType.Pinned);
			GCHandle tempBufferPinned = GCHandle.Alloc (tempBuffer, GCHandleType.Pinned);
			IntPtr pointer = pinned.AddrOfPinnedObject ();
			IntPtr tempBufferPtr = pinned.AddrOfPinnedObject ();
			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (pointer, (uint) data.Length, null, 0, (uint) data.Length, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");
				IntPtr outPtr = IntPtr.Zero;

				err = buf.AccessDataBytes (5, 5, tempBufferPtr, ref outPtr);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
				Marshal.Copy (outPtr, tempBuffer, 0, 5);

				for (int i = 0; i < tempBuffer.Length; i++)
					Assert.AreEqual ((byte) (i + 5), tempBuffer [i], $"CMBlockBuffer AccessDataBytesTest iteration: {i}");
			}
			pinned.Free ();
			tempBufferPinned.Free ();
		}

		[Test]
		public void GetDataPointerTest ()
		{
			byte [] data = new byte [] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9 };
			byte [] tempBuffer = new byte [5];
			GCHandle pinned = GCHandle.Alloc (data, GCHandleType.Pinned);
			IntPtr pointer = pinned.AddrOfPinnedObject ();
			CMBlockBufferError err;
			using (var buf = CMBlockBuffer.FromMemoryBlock (pointer, (uint) data.Length, null, 0, (uint) data.Length, CMBlockBufferFlags.AssureMemoryNow, out err)) {
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 1: {err}");
				IntPtr outPtr = IntPtr.Zero;
				nuint lengthAtOffset;
				nuint totalLength;

				err = buf.GetDataPointer (5, out lengthAtOffset, out totalLength, ref outPtr);
				Assert.That (err, Is.EqualTo (CMBlockBufferError.None), $"CMBlockBufferError 2: {err}");
				Marshal.Copy (outPtr, tempBuffer, 0, (int) lengthAtOffset);

				for (int i = 0; i < tempBuffer.Length; i++)
					Assert.AreEqual ((byte) (i + 5), tempBuffer [i], $"CMBlockBuffer GetDataPointerTest iteration: {i}");
			}
			pinned.Free ();
		}
	}
}
