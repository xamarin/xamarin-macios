#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSDraggingItemTests {
		[Test]
		public void NSDraggingItemConstructorTests ()
		{
#pragma warning disable 0219
			NSDraggingItem item = new NSDraggingItem ((NSString) "Testing");
			item = new NSDraggingItem (new MyPasteboard ());
#pragma warning restore 0219
		}

#if NET
		class MyPasteboard : NSObject, INSPasteboardWriting
#else
		class MyPasteboard : NSPasteboardWriting
#endif
		{
#if NET
			NSObject INSPasteboardWriting.GetPasteboardPropertyListForType (string type)
#else
			public override NSObject GetPasteboardPropertyListForType (string type)
#endif
			{
				return new NSObject ();
			}

#if NET
			string [] INSPasteboardWriting.GetWritableTypesForPasteboard (NSPasteboard pasteboard)
#else
			public override string[] GetWritableTypesForPasteboard (NSPasteboard pasteboard)
#endif
			{
				return new string [] { };
			}

#if NET
			[Export ("writingOptionsForType:pasteboard:")]
			public NSPasteboardWritingOptions GetWritingOptionsForType (string type, NSPasteboard pasteboard)
#else
		
			public override NSPasteboardWritingOptions GetWritingOptionsForType (string type, NSPasteboard pasteboard)
#endif
			{
				return NSPasteboardWritingOptions.WritingPromised;
			}
		}
	}
}
#endif // __MACOS__
