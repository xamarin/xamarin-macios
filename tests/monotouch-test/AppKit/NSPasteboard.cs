#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSPasteboardTests {
		[Test]
		public void NSPasteboardTests_WriteObjectTests ()
		{
			NSPasteboard b = NSPasteboard.CreateWithUniqueName ();
			if (b is null)
				Assert.Inconclusive ("NSPasteboard could not be provided by the OS.");
			b.WriteObjects (new INSPasteboardWriting [] { (NSString) "asfd" });
#if NET
			b.WriteObjects (new INSPasteboardWriting [] { new MyPasteboard () });
#else
			b.WriteObjects (new NSPasteboardWriting [] { new MyPasteboard () });
#endif
			// from the docs: the lifetime of a unique pasteboard is not related to the lifetime of the creating app,
			// you must release a unique pasteboard by calling releaseGlobally to avoid possible leaks. 
			b.ReleaseGlobally ();
		}

#if NET
		class MyPasteboard2 : NSObject, INSPasteboardReading
#else
		class MyPasteboard2 : NSPasteboardReading
#endif
		{
#if !NET
			public override NSObject InitWithPasteboardPropertyList (NSObject propertyList, string type)
			{
				return new NSObject ();
			}
#endif
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
