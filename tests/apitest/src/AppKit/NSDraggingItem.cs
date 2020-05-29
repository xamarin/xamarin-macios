using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSDraggingItemTests
	{
		[Test]
		public void NSDraggingItemConstructorTests ()
		{
#pragma warning disable 0219
			NSDraggingItem item = new NSDraggingItem ((NSString)"Testing");
			item = new NSDraggingItem (new MyPasteboard ());
#pragma warning restore 0219
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