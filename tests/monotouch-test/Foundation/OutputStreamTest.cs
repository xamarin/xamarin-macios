//
// Unit tests for NSOutputStream
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

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
					Assert.That (obj, Is.Not.Null, "a");
					Assert.That (Marshal.ReadByte (obj.Bytes, 0), Is.EqualTo (1), "a[0]");
					Assert.That (Marshal.ReadByte (obj.Bytes, 1), Is.EqualTo (2), "a[1]");
					Assert.That (Marshal.ReadByte (obj.Bytes, 2), Is.EqualTo (3), "a[2]");
				}
			}

			using (var s = new NSOutputStream ()) {
				s.Open ();
				s.Write (new byte [] { 1, 2, 3 });
				using (var obj = s [NSStream.DataWrittenToMemoryStreamKey] as NSData) {
					Assert.That (obj, Is.Not.Null, "a");
					Assert.That (Marshal.ReadByte (obj.Bytes, 0), Is.EqualTo (1), "b[0]");
					Assert.That (Marshal.ReadByte (obj.Bytes, 1), Is.EqualTo (2), "b[1]");
					Assert.That (Marshal.ReadByte (obj.Bytes, 2), Is.EqualTo (3), "b[2]");
				}
			}

			using (var s = (NSOutputStream) NSOutputStream.OutputStreamToMemory ()) {
				s.Open ();
				s.Write (new byte [] { 1, 2, 3 }, 2, 1);
				using (var obj = s [NSStream.DataWrittenToMemoryStreamKey] as NSData) {
					Assert.That (obj, Is.Not.Null, "a");
					Assert.That (Marshal.ReadByte (obj.Bytes, 0), Is.EqualTo (3), "c[0]");
				}
			}
		}

		[Test]
		public void WriteSpan ()
		{
			using var stream = new NSOutputStream ();
			stream.Open ();
			ReadOnlySpan<byte> buffer = stackalloc byte [] { 99, 1, 2, 3, 99 };
			Assert.That (stream.Write (buffer.Slice (1, 3)), Is.EqualTo ((nint) 3), "Write");
			Assert.That (stream.Write (ReadOnlySpan<byte>.Empty), Is.EqualTo ((nint) 0), "Empty");
			using var data = stream [NSStream.DataWrittenToMemoryStreamKey] as NSData;
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 3 }), "Source bounds");
		}

		[Test]
		public void WriteSpanPartial ()
		{
			NSStream.CreateBoundPair (out var read, out var write, 2);
			using (read)
			using (write) {
				read.Open ();
				write.Open ();
				ReadOnlySpan<byte> buffer = stackalloc byte [] { 1, 2, 3 };
				Assert.That (write.Write (buffer), Is.EqualTo ((nint) 2), "Partial write");
				Span<byte> destination = stackalloc byte [3];
				Assert.That (read.Read (destination), Is.EqualTo ((nint) 2), "Read");
				Assert.That (destination.Slice (0, 2).ToArray (), Is.EqualTo (new byte [] { 1, 2 }), "Destination");
			}
		}

		class WriteOverrideStream : NSOutputStream {
			public nuint LastLength { get; private set; }

			[Export ("write:maxLength:")]
			public nint Write (IntPtr buffer, nuint len)
			{
				Assert.That (buffer, Is.Not.EqualTo (IntPtr.Zero), "Nonzero buffer");
				LastLength = len;
				return len == 0 ? 0 : -1;
			}
		}

		[Test]
		public void WriteSpanSubclass ()
		{
			using var stream = new WriteOverrideStream ();
			ReadOnlySpan<byte> buffer = stackalloc byte [2];
			Assert.That (stream.Write (buffer), Is.EqualTo ((nint) (-1)), "Native override error");
			Assert.That (stream.LastLength, Is.EqualTo ((nuint) 2), "Native override length");
			Assert.That (stream.Write (ReadOnlySpan<byte>.Empty), Is.EqualTo ((nint) 0), "Empty native override");
			Assert.That (stream.LastLength, Is.EqualTo ((nuint) 0), "Empty native override length");
		}

		[Test]
		public void WriteSpanError ()
		{
			using var stream = new NSOutputStream ();
			stream.Open ();
			stream.Close ();
			ReadOnlySpan<byte> buffer = stackalloc byte [1];
			Assert.That (stream.Write (buffer), Is.EqualTo ((nint) (-1)), "Closed stream");
		}

		[Test]
		public void WriteValidation ()
		{
			using var stream = new NSOutputStream ();
			var buffer = new byte [3];
			Assert.Throws<ArgumentNullException> (() => stream.Write ((byte []) null), "Null");
			Assert.Throws<ArgumentNullException> (() => stream.Write (null, 1), "Null length");
			Assert.Throws<ArgumentNullException> (() => stream.Write (null, 0, 1), "Null range");
			Assert.Throws<ArgumentException> (() => stream.Write (buffer, 4), "Length");
			Assert.Throws<ArgumentException> (() => stream.Write (buffer, nuint.MaxValue), "Length overflow");
			Assert.Throws<ArgumentException> (() => stream.Write (buffer, 1, 3), "Range");
			Assert.Throws<ArgumentException> (() => stream.Write (buffer, 1, nuint.MaxValue), "Range overflow");
			Assert.Throws<ArgumentException> (() => stream.Write (buffer, buffer.Length + 1, 0), "Offset past end");
			Assert.Throws<IndexOutOfRangeException> (() => stream.Write (buffer, -1, 1), "Negative offset");
			Assert.Throws<IndexOutOfRangeException> (() => stream.Write (buffer, buffer.Length, 0), "Legacy end offset");
		}
	}
}
