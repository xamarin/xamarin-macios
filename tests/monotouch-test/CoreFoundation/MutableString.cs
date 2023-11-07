using System;
using Foundation;
using CoreFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MutableString {

		[Test]
		public void CreateString0 ()
		{
			using (var s = new CFMutableString ()) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (s.ToString (), Is.Empty, "ToString");
			}
		}

		[Test]
		public void CreateString1 ()
		{
			Assert.Throws<ArgumentNullException> (() => new CFMutableString ((string) null), "null");
			using (var s = new CFMutableString ("bonjour!")) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-1");
			}
		}

		[Test]
		public void CreateString2 ()
		{
			Assert.Throws<ArgumentException> (() => new CFMutableString ("", -1), "negative");
			using (var s = new CFMutableString ("bonjour!", 20)) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (s.ToString (), Is.EqualTo ("bonjour!"), "ToString");
			}
		}

		[Test]
		public void CreateCFString1 ()
		{
			Assert.Throws<ArgumentNullException> (() => new CFMutableString ((CFString) null), "null");
			using (var c = new CFString ("bonjour"))
			using (var s = new CFMutableString (c)) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (s.ToString (), Is.EqualTo ("bonjour"), "ToString");
			}
		}

		[Test]
		public void CreateCFString2 ()
		{
			using (var c = new CFString ("bonjour")) {
				Assert.Throws<ArgumentException> (() => new CFMutableString (c, -1), "negative");
				using (var s = new CFMutableString (c, 4)) {
					Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
					Assert.That (s.ToString (), Is.EqualTo ("bonjour"), "ToString");
				}
			}
		}

		[Test]
		public void AppendString ()
		{
			using (var s = new CFMutableString ()) {
				Assert.Throws<ArgumentNullException> (() => s.Append ((string) null), "null");
				// from NSHipster
				s.Append ("Énġlišh långuãge lẳcks iñterêßţing diaçrïtičş!");
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (s.ToString (), Is.EqualTo ("Énġlišh långuãge lẳcks iñterêßţing diaçrïtičş!"), "ToString");
			}
		}

		[Test]
		public void AppendString_Unicode ()
		{
			using (var s = new CFMutableString ("Bonjour")) {
				s.Append (" à tous les \ud83d\udc11!");
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				// make it fail and you see the sheep printed in the touchunit runner UI :)
				Assert.That (s.ToString (), Is.EqualTo ("Bonjour à tous les \ud83d\udc11!"), "ToString");
			}
		}

		[Test]
		public void AppendString_RtL ()
		{
			using (var s = new CFMutableString ()) {
				s.Append ("שלום");
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (s.ToString (), Is.EqualTo ("שלום"), "ToString");
			}
		}

		[Test]
		public void TransformNoRangeEnum ()
		{
			using (var s = new CFMutableString ("Bonjour à tous!")) {
				Assert.True (s.Transform (CFStringTransform.ToXmlHex, false), "Transform-1");
				Assert.That (s.ToString (), Is.EqualTo ("Bonjour &#xE0; tous!"), "ToString-1");

				Assert.True (s.Transform (CFStringTransform.ToXmlHex, true), "Transform-2");
				Assert.That (s.ToString (), Is.EqualTo ("Bonjour à tous!"), "ToString-2");
			}
		}

		[Test]
		public void TransformNull ()
		{
			using (var s = new CFMutableString ("hello")) {
				Assert.Throws<ArgumentNullException> (() => s.Transform ((string) null, false), "null-1");
				Assert.Throws<ArgumentNullException> (() => s.Transform ((CFString) null, true), "null-2");
				Assert.Throws<ArgumentNullException> (() => s.Transform ((NSString) null, false), "null-3");
				var r = new CFRange (2, 2);
				Assert.Throws<ArgumentNullException> (() => s.Transform ((string) null, true), "null-4");
				Assert.Throws<ArgumentNullException> (() => s.Transform ((CFString) null, false), "null-5");
				Assert.Throws<ArgumentNullException> (() => s.Transform ((NSString) null, true), "null-6");
			}
		}

		[Test]
		public void TransformRangeEnum ()
		{
			var r = new CFRange (0, 15);
			using (var s = new CFMutableString ("Bonjour à tous!")) {
				Assert.True (s.Transform (ref r, CFStringTransform.ToXmlHex, false), "Transform-1");
				Assert.That (s.ToString (), Is.EqualTo ("Bonjour &#xE0; tous!"), "ToString-1");
				Assert.That (r.Length, Is.EqualTo (20), "Length-1");

				Assert.True (s.Transform (ref r, CFStringTransform.ToXmlHex, true), "Transform-2");
				Assert.That (s.ToString (), Is.EqualTo ("Bonjour à tous!"), "ToString-2");
				Assert.That (r.Length, Is.EqualTo (15), "Length-2");
			}
		}

		[Test]
		public void TransformICU ()
		{
			using (var s = new CFMutableString ("hello world")) {
				Assert.True (s.Transform ("Title", false), "Transform-1");
				Assert.That (s.ToString (), Is.EqualTo ("Hello World"), "ToString-1");

				Assert.True (s.Transform ((NSString) "Title", true), "Transform-2");
				Assert.That (s.ToString (), Is.EqualTo ("hello world"), "ToString-2");
			}
		}
	}
}
