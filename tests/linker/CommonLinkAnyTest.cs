using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NET
using System.Text.Json;
#endif

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace LinkAnyTest {
	// This test is included in both the LinkAll and LinkSdk projects for both iOS and macOS.
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CommonLinkAnyTest {
		[Test]
		public void Blocks ()
		{
			int i = 0;
			string b = null;
			NSSet s = new NSSet ("a", "b", "c");
			s.Enumerate (delegate (NSObject obj, ref bool stop)
			{
				stop = i++ == 1;
				b = obj.ToString ();
			});
			// test behavior (we did not break anything)
			Assert.AreEqual ("b", b, "Stop");
			// test that BlockLiteral is fully preserved
			int size = Marshal.SizeOf (typeof (BlockLiteral)); // e.g. unused 'reserved' must not be removed
			Assert.AreEqual (IntPtr.Size == 8 ? 48 : 28, size, "BlockLiteral size");

		}

		[Test]
		public void CallerFilePath ()
		{
			Bug7114 ();
		}

		// https://bugzilla.xamarin.com/show_bug.cgi?id=7114
		public static void Bug7114 ([CallerFilePath] string filePath = null)
		{
			Assert.IsNotNull (filePath, "CallerFilePath");
		}

#if NET
		[Test]
		public void AppContextGetData ()
		{
			// https://github.com/dotnet/runtime/issues/50290
			Assert.IsNotNull (AppContext.GetData ("APP_PATHS"), "APP_PATHS");
			Assert.IsNotNull (AppContext.GetData ("PINVOKE_OVERRIDE"), "PINVOKE_OVERRIDE");
		}
#endif

#if !__WATCHOS__
		[Test]
		public void BackingFieldInGenericType ()
		{
			// https://github.com/dotnet/linker/issues/3148
#if __MACOS__
			var view = new AppKit.NSView ();
#else
			var view = new UIKit.UIView ();
#endif
			GC.KeepAlive (view.HeightAnchor);
		}
#endif // !__WATCHOS__

#if NET
		[Test]
		public void JsonSerializer_Serialize ()
		{
			var a = JsonSerializer.Serialize (42);
			Assert.AreEqual ("42", a, "serialized 42");

			var b = JsonSerializer.Serialize (new int [] { 42, 3, 14, 15 });
			Assert.AreEqual ("[42,3,14,15]", b, "serialized array");
		}

		[Test]
		public void JsonSerializer_Deserialize ()
		{
			var a = JsonSerializer.Deserialize<int> ("42");
			Assert.AreEqual (42, a, "deserialized 42");

			var b = JsonSerializer.Deserialize<int []> ("[42,3,14,15]");
			CollectionAssert.AreEqual (new int [] { 42, 3, 14, 15 }, b, "deserialized array");
		}
#endif
	}
}
