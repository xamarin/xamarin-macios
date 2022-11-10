#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSTableColumnTests {
		NSTableColumn column;

		[SetUp]
		public void SetUp ()
		{
			column = new NSTableColumn ();
		}

		[Test]
		public void NSTableColumnShouldChangeTitle ()
		{
			Asserts.EnsureYosemite ();

			var title = column.Title;
			column.Title = "Test";

			Assert.IsFalse (column.Title == title, "NSTableColumnShouldChangeTitle - Failed to set the Title property");
		}
	}
}

#endif // __MACOS__
