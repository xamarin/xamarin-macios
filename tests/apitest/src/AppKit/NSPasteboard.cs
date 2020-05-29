using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSPasteboardTests
	{
		[Test]
		public void NSPasteboardTests_WriteObjectTests ()
		{
			NSPasteboard b = NSPasteboard.CreateWithUniqueName();
			b.WriteObjects (new INSPasteboardWriting [] { (NSString)"asfd" });
			b.WriteObjects (new NSPasteboardWriting [] { new MyPasteboard () });
		}
		
		class MyPasteboard2 : NSPasteboardReading
		{
			public override NSObject InitWithPasteboardPropertyList (NSObject propertyList, string type)
			{
				return new NSObject ();
			}
		}
		
		class MyPasteboard : NSPasteboardWriting
		{
			public override NSObject GetPasteboardPropertyListForType (string type)
			{
				return new NSObject ();
			}

			public override string[] GetWritableTypesForPasteboard (NSPasteboard pasteboard)
			{
				return new string [] {};
			}

			public override NSPasteboardWritingOptions GetWritingOptionsForType (string type, NSPasteboard pasteboard)
			{
				return NSPasteboardWritingOptions.WritingPromised;
			}
		}
	}
}
