using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSPasteboardTests
	{
		[Test]
		public void NSPasteboardTests_WriteObjectTests ()
		{
			NSPasteboard b = NSPasteboard.CreateWithUniqueName();
			if (b == null)
				Assert.Inconclusive ("NSPasteboard could not be provided by the OS.");
			b.WriteObjects (new INSPasteboardWriting [] { (NSString)"asfd" });
			b.WriteObjects (new NSPasteboardWriting [] { new MyPasteboard () });
#if !XAMCORE_2_0
			// Awesome backwards compat API
			b.WriteObjects (new NSPasteboardReading [] { new MyPasteboard2 () });
#endif
			// from the docs: the lifetime of a unique pasteboard is not related to the lifetime of the creating app,
			// you must release a unique pasteboard by calling releaseGlobally to avoid possible leaks. 
			b.ReleaseGlobally (); 
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
