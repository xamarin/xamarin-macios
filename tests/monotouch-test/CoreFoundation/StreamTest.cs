// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CoreFoundation;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StreamTest {
		[Test]
		public void ReadWriteSpan ()
		{
			CFStream.CreateBoundPair (out var read, out var write, 16);
			using (read)
			using (write) {
				read.Open ();
				write.Open ();
				ReadOnlySpan<byte> source = stackalloc byte [] { 99, 1, 2, 3, 99 };
				Span<byte> destination = stackalloc byte [] { 99, 99, 99, 99, 99, 99 };
				Assert.That (write.Write (ReadOnlySpan<byte>.Empty), Is.EqualTo (0), "Empty write");
				Assert.That (read.Read (Span<byte>.Empty), Is.EqualTo ((nint) 0), "Empty read");
				Assert.That (write.Write (source.Slice (1, 3)), Is.EqualTo (3), "Write");
				Assert.That (read.Read (destination.Slice (1, 4)), Is.EqualTo ((nint) 3), "Partial read");
				Assert.That (destination.ToArray (), Is.EqualTo (new byte [] { 99, 1, 2, 3, 99, 99 }), "Destination bounds");
				write.Close ();
				Assert.That (read.Read (destination), Is.EqualTo ((nint) 0), "EOF");
				Assert.That (destination.ToArray (), Is.EqualTo (new byte [] { 99, 1, 2, 3, 99, 99 }), "EOF destination");
			}
		}

		[Test]
		public void ReadWriteArray ()
		{
			CFStream.CreateBoundPair (out var read, out var write, 16);
			using (read)
			using (write) {
				read.Open ();
				write.Open ();
				var source = new byte [] { 1, 2, 3 };
				var destination = new byte [3];
				Assert.That (write.Write (source), Is.EqualTo (3), "Array write");
				Assert.That (read.Read (destination), Is.EqualTo ((nint) 3), "Array read");
				Assert.That (destination, Is.EqualTo (source), "Array data");
				Array.Fill (destination, (byte) 99);
				Assert.That (write.Write (source, 1, 2), Is.EqualTo (2), "Range write");
				Assert.That (read.Read (destination, 1, 2), Is.EqualTo ((nint) 2), "Range read");
				Assert.That (destination, Is.EqualTo (new byte [] { 99, 2, 3 }), "Range data");
			}
		}

		[Test]
		public void WriteSpanPartial ()
		{
			CFStream.CreateBoundPair (out var read, out var write, 2);
			using (read)
			using (write) {
				read.Open ();
				write.Open ();
				ReadOnlySpan<byte> source = stackalloc byte [] { 1, 2, 3 };
				Span<byte> destination = stackalloc byte [3];
				Assert.That (write.Write (source), Is.EqualTo (2), "Partial write");
				Assert.That (read.Read (destination), Is.EqualTo ((nint) 2), "Read");
				Assert.That (destination.Slice (0, 2).ToArray (), Is.EqualTo (new byte [] { 1, 2 }), "Destination");
			}
		}

		[Test]
		public void Validation ()
		{
			CFStream.CreateBoundPair (out var read, out var write, 16);
			using (read)
			using (write) {
				var buffer = new byte [3];
				Assert.Throws<ArgumentNullException> (() => read.Read ((byte []) null), "Read null");
				Assert.Throws<ArgumentNullException> (() => write.Write ((byte []) null), "Write null");
				Assert.Throws<ArgumentNullException> (() => read.Read (null, 0, 1), "Read null range");
				Assert.Throws<ArgumentNullException> (() => write.Write (null, 0, 1), "Write null range");
				Assert.Throws<ArgumentException> (() => read.Read (new byte [0]), "Read empty array");
				Assert.Throws<ArgumentException> (() => write.Write (new byte [0]), "Write empty array");
				Assert.Throws<ArgumentException> (() => read.Read (buffer, 0, 0), "Read zero count");
				Assert.Throws<ArgumentException> (() => write.Write (buffer, 0, 0), "Write zero count");
				Assert.Throws<ArgumentException> (() => read.Read (buffer, -1, 1), "Read negative offset");
				Assert.Throws<ArgumentException> (() => write.Write (buffer, -1, 1), "Write negative offset");
				Assert.Throws<ArgumentException> (() => read.Read (buffer, 0, -1), "Read negative count");
				Assert.Throws<ArgumentException> (() => write.Write (buffer, 0, -1), "Write negative count");
				Assert.Throws<ArgumentException> (() => read.Read (buffer, 1, 3), "Read range");
				Assert.Throws<ArgumentException> (() => write.Write (buffer, 1, 3), "Write range");
				Assert.Throws<ArgumentException> (() => read.Read (buffer, 1, int.MaxValue), "Read count overflow");
				Assert.Throws<ArgumentException> (() => read.Read (buffer, int.MaxValue, 1), "Read offset overflow");
				Assert.Throws<ArgumentException> (() => write.Write (buffer, 1, nint.MaxValue), "Write count overflow");
				Assert.Throws<ArgumentException> (() => write.Write (buffer, nint.MaxValue, 1), "Write offset overflow");
				Assert.That (read.Read (Span<byte>.Empty), Is.EqualTo ((nint) 0), "Unopened empty read");
				Assert.That (write.Write (ReadOnlySpan<byte>.Empty), Is.EqualTo (0), "Unopened empty write");
				Assert.That (read.Read (buffer.AsSpan ()), Is.EqualTo ((nint) (-1)), "Unopened read error");
				Assert.That (write.Write (buffer.AsSpan ()), Is.EqualTo (-1), "Unopened write error");
				read.Dispose ();
				write.Dispose ();
				Assert.Throws<ObjectDisposedException> (() => read.Read (buffer.AsSpan ()), "Disposed read");
				Assert.Throws<ObjectDisposedException> (() => write.Write (buffer.AsSpan ()), "Disposed write");
				Assert.Throws<ObjectDisposedException> (() => read.Read (Span<byte>.Empty), "Disposed empty read");
				Assert.Throws<ObjectDisposedException> (() => write.Write (ReadOnlySpan<byte>.Empty), "Disposed empty write");
				Assert.Throws<ObjectDisposedException> (() => read.Read (buffer), "Disposed array read");
				Assert.Throws<ObjectDisposedException> (() => write.Write (buffer), "Disposed array write");
			}
		}
	}
}
