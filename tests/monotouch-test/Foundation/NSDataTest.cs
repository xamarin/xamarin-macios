//
// Unit tests for NSData
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2014 Xamarin Inc. All rights reserved.
//

using System.IO;
using System.Numerics;

using CoreGraphics;

#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using Xamarin.Utils;

using MonoTests.System.Net.Http;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSDataTest {

		[Test]
		public void ConstructorTest ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			var bytes = Marshal.AllocHGlobal (1);
			var deallocated = false;

			Marshal.WriteByte (bytes, 31);

			using (var data = new NSData (bytes, 1, (a, b) => {
				deallocated = true;
				Marshal.FreeHGlobal (a);
				Assert.That ((int) b, Is.EqualTo (1), "length in deallocator");
			})) {
				NSError error;
				var file = Path.GetTempFileName ();
				var url = NSUrl.FromFilename (file + ".url");

				Assert.That (data.Save (file, false, out error), Is.True, "save 1");
				Assert.That (data.Save (file, true, out error), Is.True, "save 2");
				Assert.That (data.Save (file, NSDataWritingOptions.Atomic, out error), Is.True, "save 3");
				Assert.That (data.Save (url, false, out error), Is.True, "save url 1");
				Assert.That (data.Save (url, true, out error), Is.True, "save url 2");
				Assert.That (data.Save (url, NSDataWritingOptions.Atomic, out error), Is.True, "save url 3");
			}

			Assert.That (deallocated, Is.True, "deallocated");
		}

		[Test]
		public void FromEmptyArrayTest ()
		{
			Assert.That (NSData.FromArray (new byte [0] { }) is not null, "#1");
		}

		[Test]
		public void FromArraySpan ()
		{
			byte [] source = { 0, 1, 2, 3, 4 };
			using var data = NSData.FromArray (source.AsSpan (1, 3));
			source [1] = 99;
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 3 }), "Independent copy");
			var copy = data.ToArray ();
			copy [0] = 88;
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 3 }), "Independent array");

			Span<byte> stackBytes = stackalloc byte [] { 5, 6, 7 };
			using var stackData = NSData.FromArray (stackBytes);
			stackBytes [0] = 99;
			Assert.That (stackData.ToArray (), Is.EqualTo (new byte [] { 5, 6, 7 }), "Stack copy");

			using var empty = NSData.FromArray (ReadOnlySpan<byte>.Empty);
			Assert.That (empty.Length, Is.EqualTo ((nuint) 0), "Empty");
			empty.GetBytes (Span<byte>.Empty);
			Assert.Throws<ArgumentNullException> (() => NSData.FromArray ((byte []) null), "Null array");
		}

		[Test]
		public void GetBytesSpan ()
		{
			using var data = NSData.FromArray (new byte [] { 1, 2, 3 });
			Span<byte> destination = stackalloc byte [] { 99, 99, 99, 99, 99 };
			data.GetBytes (destination.Slice (1));
			Assert.That (destination.ToArray (), Is.EqualTo (new byte [] { 99, 1, 2, 3, 99 }), "Slice and suffix");
			data.GetBytes (destination.Slice (0, 3));
			Assert.That (destination.Slice (0, 3).ToArray (), Is.EqualTo (new byte [] { 1, 2, 3 }), "Exact");
			destination [0] = 88;
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 3 }), "Independent destination");
			byte [] shortBuffer = { 99, 99 };
			Assert.Throws<ArgumentException> (() => data.GetBytes (shortBuffer.AsSpan ()), "Too small");
			Assert.That (shortBuffer, Is.EqualTo (new byte [] { 99, 99 }), "Unchanged on failure");
			Assert.Throws<ArgumentException> (() => data.GetBytes (Span<byte>.Empty), "Empty destination");
		}

		[Test]
		public void GetBytesSpanRange ()
		{
			using var data = NSData.FromArray (new byte [] { 1, 2, 3, 4 });
			Span<byte> destination = stackalloc byte [] { 99, 99, 99, 99 };
			data.GetBytes (destination.Slice (1), new NSRange (1, 2));
			Assert.That (destination.ToArray (), Is.EqualTo (new byte [] { 99, 2, 3, 99 }), "Range and suffix");
			data.GetBytes (destination.Slice (0, 2), new NSRange (2, 2));
			Assert.That (destination.Slice (0, 2).ToArray (), Is.EqualTo (new byte [] { 3, 4 }), "Exact");
			data.GetBytes (Span<byte>.Empty, new NSRange (4, 0));
			Assert.Throws<ArgumentException> (() => data.GetBytes (new byte [1].AsSpan (), new NSRange (0, 2)), "Too small");
			foreach (var range in new [] { new NSRange (-1, 1), new NSRange (0, -1), new NSRange (5, 0), new NSRange (3, 2), new NSRange (1, nint.MaxValue) })
				Assert.Throws<ArgumentOutOfRangeException> (() => data.GetBytes (new byte [4].AsSpan (), range), range.ToString ());
			using var empty = NSData.FromArray (ReadOnlySpan<byte>.Empty);
			empty.GetBytes (Span<byte>.Empty, new NSRange (0, 0));
		}

		[Test]
		public unsafe void UnmanagedBytes ()
		{
			byte* source = stackalloc byte [] { 1, 2, 3, 4 };
			using var data = NSData.FromBytes ((IntPtr) source, 4);
			source [0] = 99;
			byte* destination = stackalloc byte [4];
			data.GetBytes ((IntPtr) destination, (nuint) 4);
			Assert.That (new ReadOnlySpan<byte> (destination, 4).ToArray (), Is.EqualTo (new byte [] { 1, 2, 3, 4 }), "Pointer length");
			data.GetBytes ((IntPtr) destination, new NSRange (1, 2));
			Assert.That (new ReadOnlySpan<byte> (destination, 4).ToArray (), Is.EqualTo (new byte [] { 2, 3, 3, 4 }), "Pointer range");
		}

		[Test]
		public void FromFile ()
		{
			Assert.That (NSData.FromFile ("does not exists"), Is.Null, "unexisting");
#if MONOMAC || __MACCATALYST__
			// Info.Plist isn't there to load from the same location on mac
			var plistPath = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents", "Info.plist");
#else
			var plistPath = Path.Combine (NSBundle.MainBundle.BundlePath, "Info.plist");
#endif
			Assert.That (NSData.FromFile (plistPath), Is.Not.Null, "Info.plist");
		}

		[Test]
		public void FromFile_Options ()
		{
			NSError err;
			var n = NSData.FromFile ("does not exists", NSDataReadingOptions.Uncached, out err);
			Assert.That (n, Is.Null, "unexisting");
			Assert.That (err.Code, Is.EqualTo ((nint) 260), "err");
		}

		[Test]
		public void AsStream ()
		{
			// suggested solution for http://stackoverflow.com/q/10623162/220643
			using (var data = NSData.FromArray (new byte [1] { 42 }))
			using (MemoryStream ms = new MemoryStream ()) {
				data.AsStream ().CopyTo (ms);
				byte [] result = ms.ToArray ();
				Assert.That (result.Length, Is.EqualTo (1), "Length");
				Assert.That (result [0], Is.EqualTo (42), "Content");
			}
		}

		[Test]
		public void ToArray ()
		{
			using (var data = NSData.FromArray (new byte [] { 1, 2, 3 })) {
				var arr = data.ToArray ();
				Assert.That (arr.Length, Is.EqualTo (3), "Length");
				for (int i = 0; i < arr.Length; i++)
					Assert.That (arr [i], Is.EqualTo (i + 1), "idx " + i.ToString ());
			}
		}

		[Test]
		public void ToEmptyArray ()
		{
			using (var data = NSData.FromArray (new byte [0])) {
				var arr = data.ToArray ();
				Assert.That (arr.Length, Is.EqualTo (0), "Length");
			}
		}

		[Test]
		public void BytesLength ()
		{
			// suggested alternative for http://stackoverflow.com/q/10623162/220643
			using (var data = NSData.FromArray (new byte [1] { 42 })) {
				byte [] result = new byte [data.Length];
				Marshal.Copy (data.Bytes, result, 0, (int) data.Length);
				Assert.That (result.Length, Is.EqualTo (1), "Length");
				Assert.That (result [0], Is.EqualTo (42), "Content");
			}
		}

		[Test]
		public void Https ()
		{
			// Https seems broken on our macOS 10.9 bot, so skip this test.
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			// we have network issues, try several urls, if one works, be happy, else fail
			for (var i = 0; i < NetworkResources.RobotsUrls.Length; i++) {
				NSError error;
				using (var nsUrl = new NSUrl (NetworkResources.RobotsUrls [i]))
				using (var x = NSData.FromUrl (nsUrl, NSDataReadingOptions.Uncached, out error)) {
					if (error is not null)
						continue;
					Assert.That (x is not null);
					Assert.That (x.Length > 0);
					return;
				}
			}
			Assert.Fail ("None of the urls could be fetch.");
		}

		[Test]
		public void Base64_Short ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			using (var data = NSData.FromArray (new byte [1] { 42 })) {
				string s1 = data.GetBase64EncodedString (NSDataBase64EncodingOptions.EndLineWithCarriageReturn);
				Assert.That (s1, Is.EqualTo ("Kg=="), "EndLineWithCarriageReturn");
				string s2 = data.GetBase64EncodedString (NSDataBase64EncodingOptions.EndLineWithLineFeed);
				Assert.That (s2, Is.EqualTo ("Kg=="), "EndLineWithLineFeed");
				string s3 = data.GetBase64EncodedString (NSDataBase64EncodingOptions.EndLineWithCarriageReturn | NSDataBase64EncodingOptions.EndLineWithCarriageReturn);
				Assert.That (s3, Is.EqualTo ("Kg=="), "EndLineWithCarriageReturn/EndLineWithLineFeed");
				// '~' will be ignored
				using (var base64a = new NSData ("~" + s3, NSDataBase64DecodingOptions.IgnoreUnknownCharacters)) {
					Assert.That (base64a.GetBase64EncodedString (NSDataBase64EncodingOptions.None), Is.EqualTo ("Kg=="), "ctor(string)/None");
					Assert.Throws<Exception> (() => new NSData (data, NSDataBase64DecodingOptions.None), "invalid data");
				}
			}
		}

		[Test]
		public void Base64_Long ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			byte [] array = new byte [60];
			using (var data = NSData.FromArray (array)) {
				string s0 = data.GetBase64EncodedString (NSDataBase64EncodingOptions.EndLineWithCarriageReturn);
				Assert.That (s0.Length, Is.EqualTo (80), "no line limit/break");
				string s1 = data.GetBase64EncodedString (NSDataBase64EncodingOptions.EndLineWithCarriageReturn | NSDataBase64EncodingOptions.SixtyFourCharacterLineLength);
				Assert.That (s1.Length, Is.EqualTo (81), "break 64 + CR");
				string s2 = data.GetBase64EncodedString (NSDataBase64EncodingOptions.EndLineWithLineFeed | NSDataBase64EncodingOptions.SeventySixCharacterLineLength);
				Assert.That (s2.Length, Is.EqualTo (81), "break 76 + LF");
				string s3 = data.GetBase64EncodedString (NSDataBase64EncodingOptions.EndLineWithLineFeed | NSDataBase64EncodingOptions.EndLineWithCarriageReturn | NSDataBase64EncodingOptions.SixtyFourCharacterLineLength);
				Assert.That (s3.Length, Is.EqualTo (82), "break 76 + CR/LF");
				// '~' will be ignored
				using (var base64a = new NSData ("~" + s3, NSDataBase64DecodingOptions.IgnoreUnknownCharacters)) {
					Assert.That (base64a.GetBase64EncodedString (NSDataBase64EncodingOptions.None).Length, Is.EqualTo (80), "ctor(string)/None");
					Assert.Throws<Exception> (() => new NSData (data, NSDataBase64DecodingOptions.None), "invalid data");
				}
			}
		}

		[Test]
		public void FromStream ()
		{
			Assert.Throws<ArgumentNullException> (delegate
			{
				NSData.FromStream (null);
			}, "null");
			using (var d = NSData.FromStream (Stream.Null)) {
				Assert.That (d.Length, Is.EqualTo ((nuint) 0), "Length");
			}
		}

		class CanNotReadStream : MemoryStream {
			public override bool CanRead {
				get { return false; }
			}
		}

		[Test]
		public void FromStream_CanNotRead ()
		{
			using (var s = new CanNotReadStream ()) {
				Assert.That (NSData.FromStream (s), Is.Null, "!CanRead");
			}
		}

		class CanNotSeekStream : MemoryStream {

			public CanNotSeekStream () : base (new byte [8])
			{
			}

			public override bool CanSeek {
				get { return false; }
			}
		}

		[Test]
		public void FromStream_CanNotSeek ()
		{
			using (var s = new CanNotSeekStream ())
			using (var d = NSData.FromStream (s)) {
				Assert.That (d.Length, Is.EqualTo ((nuint) 8), "Length");
			}
		}

		class NegativeLengthStream : MemoryStream {

			bool can_seek;

			public NegativeLengthStream (bool canSeek) : base (new byte [12])
			{
				can_seek = canSeek;
			}

			public override bool CanSeek {
				get { return can_seek; }
			}

			public override long Length {
				get { return -1; }
			}
		}

		[Test]
		public void FromStream_Negative ()
		{
			using (var s = new NegativeLengthStream (false))
			using (var d = NSData.FromStream (s)) {
				Assert.That (d.Length, Is.EqualTo ((nuint) 12), "Length");
			}

			// that would be a very buggy stream implementation
			using (var s = new NegativeLengthStream (true)) {
				Assert.Throws<ArgumentOutOfRangeException> (delegate
				{
					NSData.FromStream (s);
				}, "negative");
			}
		}

		class NoLengthStream : MemoryStream {

			public NoLengthStream () : base (new byte [16])
			{
			}

			public override long Length {
				get { throw new NotSupportedException (); }
			}
		}

		[Test]
		public void FromStream_NoLength ()
		{
			using (var s = new NoLengthStream ())
			using (var d = NSData.FromStream (s)) {
				Assert.That (d.Length, Is.EqualTo ((nuint) 16), "Length");
			}
		}

		[Test]
		public void FromStream_Position ()
		{
			using (var s = new MemoryStream (new byte [20])) {
				s.Position = 10;
				using (var d = NSData.FromStream (s)) {
					Assert.That (d.Length, Is.EqualTo ((nuint) 10), "Length");
				}
			}
		}

		[Test]
		public void FromString ()
		{
			Assert.Throws<ArgumentNullException> (() => NSData.FromString (null), "1-null");
			var d = NSData.FromString (String.Empty);
			Assert.That (d.Length, Is.EqualTo ((nuint) 0), "1-empty");

			Assert.Throws<ArgumentNullException> (() => NSData.FromString (null, NSStringEncoding.Unicode), "2-null");
			d = NSData.FromString (String.Empty, NSStringEncoding.Unicode);
			Assert.That (d.Length, Is.EqualTo ((nuint) 2), "2-empty"); // 0xfffe unicode header

			// not sure it was a good choice to throw here (but breaking it would be worse)
			Assert.Throws<ArgumentNullException> (() => d = ((NSData) (string) null), "as-null");
			d = (NSData) String.Empty;
			Assert.That (d.Length, Is.EqualTo ((nuint) 0), "as-empty");
		}

		[Test]
		public void Base64String ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			using (var d = new NSData ("WGFtYXJpbg==", NSDataBase64DecodingOptions.IgnoreUnknownCharacters)) {
				Assert.That (d.ToString (), Is.EqualTo ("Xamarin"));
			}
		}

		[Test]
		public void Base64Data ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			using (var b = NSData.FromString ("WGFtYXJpbg=="))
			using (var d = new NSData (b, NSDataBase64DecodingOptions.IgnoreUnknownCharacters)) {
				Assert.That (d.ToString (), Is.EqualTo ("Xamarin"));
			}
		}

		[Test]
		public void ToString_17693 ()
		{
			byte [] data = { 0x10, 0x02, 0x0A, 0x42, 0xC0, 0xA8, 0x02, 0x1E };
			using (var ms = new MemoryStream (data))
			using (var d = NSData.FromStream (ms)) {
				// This cannot be converted to an UTF8 string and ToString should not throw
				// so we go back to showing the result of the `description` selector
				Assert.That (d.ToString (), Is.EqualTo (d.Description), "ToString");
			}
		}

		[Test]
		public void ToFromValueType ()
		{
			Assert.Multiple (() => {
				var matrix = new NMatrix3 (1, 2, 3, 4, 5, 6, 7, 8, 9);
				using var data = NSData.CreateFromValueType<NMatrix3> (matrix);
				nint structSize;
				unsafe {
					structSize = sizeof (NMatrix3);
				}
				Assert.That ((nint) data.Length, Is.EqualTo ((nint) structSize), "Length");

				var matrix2 = data.ToValueType<NMatrix3> ();
				Assert.That (matrix2, Is.EqualTo (matrix), "RoundTrip");

				// not enough data in the NSData
				Assert.Throws<ArgumentOutOfRangeException> (() => data.ToValueType<Matrix4x4> (), "Matrix4x4");

				// reading less than the full NSData contents is OK
				Assert.That (data.ToValueType<float> (), Is.EqualTo ((float) 1), "float");

				using var emptyData = NSData.FromArray (new byte [0]);
				var emptyVT = data.ToValueType<EmptyValueType> ();
				Assert.That (emptyVT, Is.EqualTo (default (EmptyValueType)), "Empty Value Type");

				emptyVT = new EmptyValueType ();
				using var emptyData2 = NSData.CreateFromValueType<EmptyValueType> (emptyVT);
				int emptyValueTypeSize = 0;
				unsafe {
					emptyValueTypeSize = sizeof (EmptyValueType);
				}
				Assert.That ((int) emptyData2.Length, Is.EqualTo (emptyValueTypeSize), "Empty Value Type 2");
			});
		}

		struct EmptyValueType { }
	}
}
