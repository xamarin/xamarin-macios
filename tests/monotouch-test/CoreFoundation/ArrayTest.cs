#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ArrayTest {
		static string [] TestArray = new string [] { "a", "b", "??" };

		void VerifyArray (CFArray? a)
		{
			Assert.IsNotNull (a, "NotNull");
			Assert.AreEqual ((nint) 3, a.Count, "Count");
			for (var i = 0; i < a.Count; i++)
				Assert.AreEqual (TestArray [i], (string) CFString.FromHandle (a.GetValue (i), false), i.ToString ());
		}

		void VerifyArray (NSString []? a)
		{
			Assert.IsNotNull (a, "NotNull");
			Assert.AreEqual (3, a.Length, "Count");
			for (var i = 0; i < a.Length; i++)
				Assert.AreEqual (TestArray [i], (string) a [i], i.ToString ());
		}

		void VerifyArray (string []? a)
		{
			Assert.IsNotNull (a, "NotNull");
			Assert.AreEqual (3, a.Length, "Count");
			for (var i = 0; i < a.Length; i++)
				Assert.AreEqual (TestArray [i], (string) a [i], i.ToString ());
		}

		[Test]
		public void CreateTest ()
		{
			var handle = CFArray.Create (TestArray);
			using var a = Runtime.GetINativeObject<CFArray> (handle, true);
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (handle), "RC");
		}

		[Test]
		public void FromStringsTest ()
		{
			using var a = CFArray.FromStrings (TestArray);
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (a.Handle), "RC");
		}

		[Test]
		public void ArrayFromHandleTest ()
		{
			var handle = CFArray.Create (TestArray);
			var a = CFArray.ArrayFromHandle<NSString> (handle);
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (handle), "RC");
			CFRelease (handle);
		}

		[Test]
		public void ArrayFromHandleTest_bool_true ()
		{
			var handle = CFArray.Create (TestArray);
			CFRetain (handle);
			var a = CFArray.ArrayFromHandle<NSString> (handle, true);
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (handle), "RC");
			CFRelease (handle);
		}

		[Test]
		public void ArrayFromHandleTest_bool_false ()
		{
			var handle = CFArray.Create (TestArray);
			var a = CFArray.ArrayFromHandle<NSString> (handle, false);
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (handle), "RC");
			CFRelease (handle);
		}

		[Test]
		public void ArrayFromHandleFuncTest ()
		{
			var handle = CFArray.Create (TestArray);
			var a = CFArray.ArrayFromHandleFunc<string> (handle, (v) => CFString.FromHandle (v));
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (handle), "RC");
			CFRelease (handle);
		}

		[Test]
		public void ArrayFromHandleFuncTest_bool_true ()
		{
			var handle = CFArray.Create (TestArray);
			CFRetain (handle);
			var a = CFArray.ArrayFromHandleFunc<string> (handle, (v) => CFString.FromHandle (v), true);
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (handle), "RC");
			CFRelease (handle);
		}

		[Test]
		public void ArrayFromHandleFuncTest_bool_false ()
		{
			var handle = CFArray.Create (TestArray);
			var a = CFArray.ArrayFromHandleFunc<string> (handle, (v) => CFString.FromHandle (v), false);
			VerifyArray (a);
			Assert.AreEqual ((nint) 1, CFGetRetainCount (handle), "RC");
			CFRelease (handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static nint CFGetRetainCount (IntPtr handle);

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static void CFRetain (IntPtr obj);

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static void CFRelease (IntPtr obj);
	}
}
