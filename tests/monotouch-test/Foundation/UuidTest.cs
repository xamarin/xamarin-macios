//
// Unit tests for NSUuid
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System.IO;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UuidTest {

		[Test]
		public void SpanBytes ()
		{
			byte [] source = { 99, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 88 };
			byte [] expected = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			using var uuid = new NSUuid (source.AsSpan (1));
			source [1] = 99;
			Assert.That (uuid.GetBytes (), Is.EqualTo (expected), "Slice, excess input and independent copy");
			Span<byte> destination = stackalloc byte [18];
			destination.Fill (77);
			uuid.GetBytes (destination.Slice (1));
			Assert.That (destination.Slice (1, 16).ToArray (), Is.EqualTo (expected), "Span output");
			Assert.That (destination [0], Is.EqualTo (77), "Prefix");
			Assert.That (destination [17], Is.EqualTo (77), "Suffix");
			using var stackUuid = new NSUuid (destination.Slice (1, 16));
			destination [1] = 99;
			Assert.That (stackUuid.GetBytes (), Is.EqualTo (expected), "Exact stack input");
			uuid.GetBytes (destination.Slice (0, 16));
			Assert.That (destination.Slice (0, 16).ToArray (), Is.EqualTo (expected), "Exact output");
			using var arrayUuid = new NSUuid (new byte [17]);
			Assert.That (arrayUuid.GetBytes (), Is.EqualTo (new byte [16]), "Excess array input");
			using var stringUuid = new NSUuid ("00010203-0405-0607-0809-0A0B0C0D0E0F");
			Assert.That (stringUuid.GetBytes (), Is.EqualTo (expected), "String constructor");
			byte [] shortBuffer = new byte [15];
			Assert.Throws<ArgumentException> (() => new NSUuid (shortBuffer.AsSpan ()), "Short input");
			Assert.Throws<ArgumentException> (() => new NSUuid (ReadOnlySpan<byte>.Empty), "Empty input");
			Assert.Throws<ArgumentException> (() => uuid.GetBytes (shortBuffer.AsSpan ()), "Short output");
			Assert.That (shortBuffer, Is.EqualTo (new byte [15]), "Unchanged on failure");
			Assert.Throws<ArgumentException> (() => uuid.GetBytes (Span<byte>.Empty), "Empty output");
		}

		[Test]
		public void Constructors ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			var uuid = new NSUuid (new byte [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
			Assert.That (uuid, Is.Not.EqualTo (null), "constructed");

			var bytes = uuid.GetBytes ();
			Assert.That (bytes.Length, Is.EqualTo (16), "lenght");

			for (int i = 0; i < 16; i++)
				Assert.That (bytes [i], Is.EqualTo (i), "value " + i);
		}

		[Test]
		public void ConstructorFailures ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			try {
				var uuid = new NSUuid ((byte []) null);
				Assert.Fail ("Should have t;hrown an exception");
			} catch (ArgumentNullException) {
				// good
			} catch (Exception e) {
				Assert.Fail ($"Unexpected exception {e}");
			}

			try {
				var uuid = new NSUuid (new byte [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
				Assert.Fail ("Should have thrown an ArgumentException");
			} catch (ArgumentException) {
				// ok
			} catch (Exception e) {
				Assert.Fail ($"Expected an ArgumentException {e}");
			}
		}
	}
}
