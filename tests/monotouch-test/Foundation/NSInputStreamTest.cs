
namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSInputStreamTest {
		[Test]
		public void SubclassedCtor ()
		{
			using (var s = new SubStream ()) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		class SubStream : NSInputStream {
		}

		[Test]
		public void Path ()
		{
			using (var s = new NSInputStream ("Info.plist")) {
				// initWithFileAtPath: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void Data ()
		{
#if MONOMAC || __MACCATALYST__
			// Info.Plist isn't there to load from the same location on mac
			var plistPath = global::System.IO.Path.Combine (NSBundle.MainBundle.BundlePath, "Contents", "Info.plist");
#else
			var plistPath = global::System.IO.Path.Combine (NSBundle.MainBundle.BundlePath, "Info.plist");
#endif
			using (var d = NSData.FromFile (plistPath))
			using (var s = new NSInputStream (d)) {
				// initWithData: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void Url ()
		{
			using (var u = NSUrl.FromFilename ("Info.plist"))
			using (var s = new NSInputStream (u)) {
				// initWithURL: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public unsafe void Read ()
		{
			using (var data = NSData.FromArray (new byte [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })) {
				using (var s = NSInputStream.FromData (data)) {
					byte [] arr = new byte [10];
					s.Open ();
					Assert.That (s.HasBytesAvailable (), Is.True);
					Assert.That (s.Read (arr, 2), Is.EqualTo ((nint) 2), "#a 1");
					Assert.That (arr [0], Is.EqualTo (0), "#a[0]");
					Assert.That (arr [1], Is.EqualTo (1), "#a[1]");
				}

				using (var s = new NSInputStream (data)) {
					byte [] arr = new byte [10];
					s.Open ();
					Assert.That (s.HasBytesAvailable (), Is.True);
					Assert.That (s.Read (arr, 1, 2), Is.EqualTo ((nint) 2), "#b 1");
					Assert.That (arr [0], Is.EqualTo (0), "#b[0]");
					Assert.That (arr [1], Is.EqualTo (0), "#b[1]");
					Assert.That (arr [2], Is.EqualTo (1), "#b[2]");
				}

				using (var s = new NSInputStream (data)) {
					byte [] arr = new byte [10];
					s.Open ();
					Assert.That (s.HasBytesAvailable (), Is.True);
					fixed (byte* ptr = &arr [2])
						Assert.That (s.Read ((IntPtr) ptr, 2), Is.EqualTo ((nint) 2), "#c 1");
					Assert.That (arr [0], Is.EqualTo (0), "#c[0]");
					Assert.That (arr [1], Is.EqualTo (0), "#c[1]");
					Assert.That (arr [2], Is.EqualTo (0), "#c[2]");
					Assert.That (arr [3], Is.EqualTo (1), "#c[3]");
				}
			}
		}

		[Test]
		public void ReadSpan ()
		{
			using var data = NSData.FromArray (new byte [] { 1, 2, 3 });
			using var stream = new NSInputStream (data);
			stream.Open ();
			Span<byte> buffer = stackalloc byte [] { 99, 99, 99, 99, 99, 99 };
			Assert.That (stream.Read (Span<byte>.Empty), Is.EqualTo ((nint) 0), "Empty");
			Assert.That (stream.Read (buffer.Slice (1, 4)), Is.EqualTo ((nint) 3), "Partial read");
			Assert.That (buffer.ToArray (), Is.EqualTo (new byte [] { 99, 1, 2, 3, 99, 99 }), "Destination bounds");
			Assert.That (stream.Read (buffer), Is.EqualTo ((nint) 0), "EOF");
			Assert.That (buffer.ToArray (), Is.EqualTo (new byte [] { 99, 1, 2, 3, 99, 99 }), "EOF destination");
		}

		class ReadOverrideStream : NSInputStream {
			public nuint LastLength { get; private set; }

			public override nint Read (IntPtr buffer, nuint len)
			{
				Assert.That (buffer, Is.Not.EqualTo (IntPtr.Zero), "Nonzero buffer");
				LastLength = len;
				if (len == 0)
					return 0;
				Marshal.WriteByte (buffer, 42);
				return 1;
			}
		}

		[Test]
		public void ReadSpanSubclass ()
		{
			using var stream = new ReadOverrideStream ();
			Span<byte> buffer = stackalloc byte [2];
			Assert.That (stream.Read (buffer), Is.EqualTo ((nint) 1), "Override result");
			Assert.That (stream.LastLength, Is.EqualTo ((nuint) 2), "Override length");
			Assert.That (buffer [0], Is.EqualTo (42), "Override data");
			Assert.That (stream.Read (Span<byte>.Empty), Is.EqualTo ((nint) 0), "Empty override");
			Assert.That (stream.LastLength, Is.EqualTo ((nuint) 0), "Empty override length");
		}

		[Test]
		public void ReadValidation ()
		{
			using var data = NSData.FromArray (new byte [] { 1, 2, 3 });
			using var stream = new NSInputStream (data);
			var buffer = new byte [3];
			Assert.Throws<ArgumentNullException> (() => stream.Read (null, 1), "Null");
			Assert.Throws<ArgumentNullException> (() => stream.Read (null, 0, 1), "Null range");
			Assert.Throws<ArgumentNullException> (() => stream.Read (IntPtr.Zero, 0), "Null pointer");
			Assert.Throws<ArgumentException> (() => stream.Read (buffer, 4), "Length");
			Assert.Throws<ArgumentException> (() => stream.Read (buffer, nuint.MaxValue), "Length overflow");
			Assert.Throws<ArgumentException> (() => stream.Read (buffer, 1, 3), "Range");
			Assert.Throws<ArgumentException> (() => stream.Read (buffer, 1, nuint.MaxValue), "Range overflow");
			Assert.Throws<ArgumentException> (() => stream.Read (buffer, buffer.Length + 1, 0), "Offset past end");
			Assert.Throws<IndexOutOfRangeException> (() => stream.Read (buffer, -1, 1), "Negative offset");
			Assert.Throws<IndexOutOfRangeException> (() => stream.Read (buffer, buffer.Length, 0), "Legacy end offset");
		}

		[Test]
		public void ReadSpanError ()
		{
			using var data = NSData.FromArray (new byte [] { 1 });
			using var stream = new NSInputStream (data);
			stream.Open ();
			stream.Close ();
			Span<byte> buffer = stackalloc byte [1];
			Assert.That (stream.Read (buffer), Is.EqualTo ((nint) (-1)), "Closed stream");
		}
	}
}
