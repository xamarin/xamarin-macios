//
// Unit tests for NSData
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2014 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSDataTest {

		[Test]
		public void ConstructorTest ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			var bytes = Marshal.AllocHGlobal (1);
			var deallocated = false;

			Marshal.WriteByte (bytes, 31);

			using (var data = new NSData (bytes, 1, (a, b) =>
				{
					deallocated = true;
					Marshal.FreeHGlobal (a);
					Assert.AreEqual (1, (int) b, "length in deallocator");
				}))
			{
				NSError error;
				var file = Path.GetTempFileName ();
				var url = NSUrl.FromFilename (file + ".url");

				Assert.IsTrue (data.Save (file, false, out error), "save 1");
				Assert.IsTrue (data.Save (file, true, out error), "save 2");
				Assert.IsTrue (data.Save (file, NSDataWritingOptions.Atomic, out error), "save 3");
				Assert.IsTrue (data.Save (url, false, out error), "save url 1");
				Assert.IsTrue (data.Save (url, true, out error), "save url 2");
				Assert.IsTrue (data.Save (url, NSDataWritingOptions.Atomic, out error), "save url 3");
			}

			Assert.IsTrue (deallocated, "deallocated");
		}

		[Test]
		public void FromEmptyArrayTest ()
		{
			Assert.That (NSData.FromArray (new byte [0] {}) != null, "#1");
		}
		
		[Test]
		public void FromFile ()
		{
			Assert.Null (NSData.FromFile ("does not exists"), "unexisting");
#if MONOMAC // Info.Plist isn't there to load from the same location on mac
			if (!TestRuntime.IsLinkAll)
				Assert.NotNull (NSData.FromFile (NSBundle.MainBundle.PathForResource ("runtime-options", "plist")), "runtime-options.plist");
#else
			Assert.NotNull (NSData.FromFile ("Info.plist"), "Info.plist");
#endif
		}

		[Test]
		public void FromFile_Options ()
		{
			NSError err;
			var n = NSData.FromFile ("does not exists", NSDataReadingOptions.Uncached, out err);
			Assert.Null (n, "unexisting");
			Assert.That (err.Code, Is.EqualTo ((nint) 260), "err");
		}

		[Test]
		public void AsStream ()
		{
			// suggested solution for http://stackoverflow.com/q/10623162/220643
			using (var data = NSData.FromArray (new byte [1] { 42 }))
			using (MemoryStream ms = new MemoryStream ()) {
				data.AsStream ().CopyTo (ms);
				byte[] result = ms.ToArray ();
				Assert.That (result.Length, Is.EqualTo (1), "Length");
				Assert.That (result [0], Is.EqualTo (42), "Content");
			}
		}

		[Test]
		public void ToArray ()
		{
			using (var data = NSData.FromArray (new byte[] { 1, 2, 3 })) {
				var arr = data.ToArray ();
				Assert.AreEqual (3, arr.Length, "Length");
				for (int i = 0; i < arr.Length; i++)
					Assert.AreEqual (i + 1, arr [i], "idx " + i.ToString ());
			}
		}

		[Test]
		public void ToEmptyArray ()
		{
			using (var data = NSData.FromArray (new byte[0])) {
				var arr = data.ToArray ();
				Assert.AreEqual (0, arr.Length, "Length");
			}
		}

		[Test]
		public void BytesLength ()
		{
			// suggested alternative for http://stackoverflow.com/q/10623162/220643
			using (var data = NSData.FromArray (new byte [1] { 42 })) {
				byte[] result = new byte[data.Length];
				Marshal.Copy (data.Bytes, result, 0, (int) data.Length);
				Assert.That (result.Length, Is.EqualTo (1), "Length");
				Assert.That (result [0], Is.EqualTo (42), "Content");
			}
		}

		[Test]
		public void Https ()
		{
#if __WATCHOS__
			if (global::ObjCRuntime.Runtime.Arch == global::ObjCRuntime.Arch.DEVICE) {
				// This error is returned: Error: The file “robots.txt” couldn’t be opened. The file “robots.txt” couldn’t be opened.
				Assert.Ignore ("NSData.FromUrl doesn't seem to work in watchOS");
			}
#endif
			using (var url = new NSUrl ("https://www.microsoft.com/robots.txt"))
			using (var x = NSData.FromUrl (url)) {
				Assert.That ((x != null) && (x.Length > 0));
			}
		}

		[Test]
		public void Base64_Short ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

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

			byte[] array = new byte [60];
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
				using (var base64a = new NSData ("~"+ s3, NSDataBase64DecodingOptions.IgnoreUnknownCharacters)) {
					Assert.That (base64a.GetBase64EncodedString (NSDataBase64EncodingOptions.None).Length, Is.EqualTo (80), "ctor(string)/None");
					Assert.Throws<Exception> (() => new NSData (data, NSDataBase64DecodingOptions.None), "invalid data");
				}
			}
		}

		[Test]
		public void FromStream ()
		{
			Assert.Throws<ArgumentNullException> (delegate {
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
				Assert.Null (NSData.FromStream (s), "!CanRead");
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
				Assert.Throws<ArgumentOutOfRangeException> (delegate {
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
			Assert.That (d.Length, Is.EqualTo (0), "1-empty");

			Assert.Throws<ArgumentNullException> (() => NSData.FromString (null, NSStringEncoding.Unicode), "2-null");
			d = NSData.FromString (String.Empty, NSStringEncoding.Unicode);
			Assert.That (d.Length, Is.EqualTo (2), "2-empty"); // 0xfffe unicode header

			// not sure it was a good choice to throw here (but breaking it would be worse)
			Assert.Throws<ArgumentNullException> (() => d = ((NSData) (string) null), "as-null");
			d = (NSData) String.Empty;
			Assert.That (d.Length, Is.EqualTo (0), "as-empty");
		}

		[Test]
		public void Base64String ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var d = new NSData ("WGFtYXJpbg==", NSDataBase64DecodingOptions.IgnoreUnknownCharacters)) {
				Assert.That (d.ToString (), Is.EqualTo ("Xamarin"));
			}
		}

		[Test]
		public void Base64Data ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var b = NSData.FromString ("WGFtYXJpbg=="))
			using (var d = new NSData (b, NSDataBase64DecodingOptions.IgnoreUnknownCharacters)) {
				Assert.That (d.ToString (), Is.EqualTo ("Xamarin"));
			}
		}

		[Test]
		public void ToString_17693 ()
		{
			byte[] data = { 0x10, 0x02, 0x0A, 0x42, 0xC0, 0xA8, 0x02, 0x1E };
			using (var ms = new MemoryStream (data))
			using (var d = NSData.FromStream (ms)) {
				// This cannot be converted to an UTF8 string and ToString should not throw
				// so we go back to showing the result of the `description` selector
				Assert.That (d.ToString (), Is.EqualTo (d.Description), "ToString");
			}
		}
	}
}
