//
// Unit tests for NSMutableData
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MutableDataTest {

		[Test]
		public void AppendBytes ()
		{
			using var data = new NSMutableData ();
			byte [] source = { 1, 2, 3, 4 };
			data.AppendBytes (source);
			data.AppendBytes (source, 1, 2);
			data.AppendBytes (source.AsSpan (2, 2));
			Span<byte> stackBytes = stackalloc byte [] { 5, 6 };
			data.AppendBytes (stackBytes);
			source [0] = 99;
			stackBytes [0] = 99;
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 3, 4, 2, 3, 3, 4, 5, 6 }), "Copied appends");

			data.AppendBytes (ReadOnlySpan<byte>.Empty);
			data.AppendBytes (new byte [0]);
			data.AppendBytes (source, source.Length, 0);
			data.AppendBytes (new byte [0], 0, 0);
			Assert.That (data.Length, Is.EqualTo ((nuint) 10), "Empty appends");
			Assert.Throws<ArgumentNullException> (() => data.AppendBytes ((byte []) null), "Null array");
			Assert.Throws<ArgumentNullException> (() => data.AppendBytes (null, 0, 0), "Null range array");
			Assert.Throws<ArgumentException> (() => data.AppendBytes (source, -1, 1), "Negative start");
			Assert.Throws<ArgumentException> (() => data.AppendBytes (source, 5, 0), "Start beyond end");
			Assert.Throws<ArgumentException> (() => data.AppendBytes (source, 0, -1), "Negative length");
			Assert.Throws<ArgumentException> (() => data.AppendBytes (source, 3, 2), "Range beyond end");
			Assert.Throws<ArgumentException> (() => data.AppendBytes (source, 1, nint.MaxValue), "Overflow");
			Assert.That (data.Length, Is.EqualTo ((nuint) 10), "Invalid appends");
		}

		[Test]
		public void ReplaceBytesSpan ()
		{
			using var data = new NSMutableData ();
			data.AppendBytes (new byte [] { 1, 2, 3, 4 });
			Span<byte> replacement = stackalloc byte [] { 5, 6, 7 };
			data.ReplaceBytes (new NSRange (1, 2), replacement.Slice (1));
			replacement [1] = 99;
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 6, 7, 4 }), "Equal size and copy independence");
			data.ReplaceBytes (new NSRange (1, 1), new byte [] { 8, 9 }.AsSpan ());
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 8, 9, 7, 4 }), "Grow");
			data.ReplaceBytes (new NSRange (1, 3), new byte [] { 2 }.AsSpan ());
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 4 }), "Shrink");
			data.ReplaceBytes (new NSRange (3, 0), new byte [] { 5 }.AsSpan ());
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 4, 5 }), "Insert at end");
			data.ReplaceBytes (new NSRange (1, 2), ReadOnlySpan<byte>.Empty);
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 5 }), "Delete");
			data.ReplaceBytes (new NSRange (2, 0), ReadOnlySpan<byte>.Empty);
			foreach (var range in new [] { new NSRange (-1, 1), new NSRange (0, -1), new NSRange (3, 0), new NSRange (1, 2), new NSRange (1, nint.MaxValue) })
				Assert.Throws<ArgumentOutOfRangeException> (() => data.ReplaceBytes (range, ReadOnlySpan<byte>.Empty), range.ToString ());
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 5 }), "Invalid replacements");
			data.ReplaceBytes (new NSRange (0, 2), ReadOnlySpan<byte>.Empty);
			data.ReplaceBytes (new NSRange (0, 0), ReadOnlySpan<byte>.Empty);
			Assert.That (data.Length, Is.EqualTo ((nuint) 0), "Empty");
		}

		[Test]
		public unsafe void UnmanagedBytes ()
		{
			using var data = new NSMutableData ();
			byte* source = stackalloc byte [] { 1, 2, 3 };
			data.AppendBytes ((IntPtr) source, 3);
			source [0] = 99;
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 2, 3 }), "Pointer append copies");
			data.ReplaceBytes (new NSRange (1, 1), (IntPtr) source);
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 99, 3 }), "Pointer replacement");
			data.ReplaceBytes (new NSRange (1, 1), (IntPtr) source, 3);
			Assert.That (data.ToArray (), Is.EqualTo (new byte [] { 1, 99, 2, 3, 3 }), "Pointer replacement length");
			Span<byte> destination = stackalloc byte [5];
			data.GetBytes (destination);
			Assert.That (destination.ToArray (), Is.EqualTo (data.ToArray ()), "Inherited span read");
		}

		[Test]
		public void FromCapacity ()
		{
			Assert.Throws<ArgumentOutOfRangeException> (delegate
			{
				NSMutableData.FromCapacity (-1);
			}, "negative");
			using (var empty = NSMutableData.FromCapacity (0)) {
				Assert.That (empty.Length, Is.EqualTo ((nuint) 0), "Length");
			}
		}

		[Test]
		public void FromLength ()
		{
			Assert.Throws<ArgumentOutOfRangeException> (delegate
			{
				NSMutableData.FromLength (-1);
			}, "negative");
			using (var empty = NSMutableData.FromLength (0)) {
				Assert.That (empty.Length, Is.EqualTo ((nuint) 0), "Length");
			}
		}

		[Test]
		public void Constructor ()
		{
			// the bound constructor is for capacity (not length)
			TestDelegate check_capacity = delegate
			{
				uint capacity = (uint) Int32.MaxValue + 2;
				Console.WriteLine ("Trying to allocate {0} bytes, this may cause malloc errors", capacity);
				new NSMutableData (capacity).Dispose ();
			};

			if (IntPtr.Size == 4) {
				Assert.Throws<ArgumentOutOfRangeException> (check_capacity, "negative");
			} else {
				// this can either fail with an Exception due to not enough memory to allocate 2GB (typical on device), or it can succeed (usually in the sim).
				try {
					check_capacity ();
					// It worked, that's fine.
				} catch (Exception ex) {
					// Verify that the exception is an OOM (i.e. native code failed to init the object).
					Assert.That (ex.GetType (), Is.SameAs (typeof (Exception)), "exception type");
					Assert.That (ex.Message, Does.StartWith ("Could not initialize an instance of the type 'Foundation.NSMutableData': the native 'initWithCapacity:' method returned nil."), "OOM");
				}
			}

			using (var empty = new NSMutableData (0)) {
				Assert.That (empty.Length, Is.EqualTo ((nuint) 0), "Length");
			}
		}
	}
}
