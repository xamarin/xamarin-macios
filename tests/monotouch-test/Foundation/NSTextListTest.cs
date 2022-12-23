using System;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Foundation;
using ObjCRuntime;
using Xamarin.Utils;

#if !__WATCHOS__

#if HAS_UIKIT
using UIKit;
#else
using AppKit;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSTextListTest {

		[TestCase ("{decimal}.")]
		[TestCase ("•")]
		public void Constructor_CustomFormat (string format)
		{
			bool makeUnorderedList = false;
			var textList = new NSTextList (format);
			Assert.AreEqual (format, textList.CustomMarkerFormat, "CustomMarkerFormat");
#if NET
			Assert.AreEqual (NSTextListMarkerFormats.CustomString, textList.MarkerFormat, "MarkerFormat");
#else
			Assert.AreEqual (format, textList.MarkerFormat, "MarkerFormat");
#endif // NET
			Assert.AreEqual (NSTextListOptions.None, textList.ListOptions, "ListOptions");
		}

		[TestCase ("{decimal}.", NSTextListOptions.None)]
		[TestCase ("•", NSTextListOptions.PrependEnclosingMarker)]
		public void Constructor_CustomFormat_2 (string format, NSTextListOptions options)
		{
			bool makeUnorderedList = false;
			var textList = new NSTextList (format, options);
			Assert.AreEqual (format, textList.CustomMarkerFormat, "CustomMarkerFormat");
#if NET
			Assert.AreEqual (NSTextListMarkerFormats.CustomString, textList.MarkerFormat, "MarkerFormat");
#else
			Assert.AreEqual (format, textList.MarkerFormat, "MarkerFormat");
#endif // NET
			Assert.AreEqual (options, textList.ListOptions, "ListOptions");
		}


		[TestCase (NSTextListMarkerFormats.Check, NSTextListOptions.None)]
		[TestCase (NSTextListMarkerFormats.Box, NSTextListOptions.PrependEnclosingMarker)]
		public void Constructor_TypedFormat_2 (NSTextListMarkerFormats format, NSTextListOptions options)
		{
			bool makeUnorderedList = false;
			var textList = new NSTextList (format, options);
			Assert.AreEqual ((string) format.GetConstant ()!, textList.CustomMarkerFormat, "CustomMarkerFormat");
#if NET
			Assert.AreEqual (format, textList.MarkerFormat, "MarkerFormat");
#else
			Assert.AreEqual ((string) format.GetConstant ()!, textList.MarkerFormat, "MarkerFormat");
#endif // NET
			Assert.AreEqual (options, textList.ListOptions, "ListOptions");
		}

		[TestCase (NSTextListMarkerFormats.Circle)]
		[TestCase (NSTextListMarkerFormats.Diamond)]
		public void Constructor_TypedFormat (NSTextListMarkerFormats format)
		{
			bool makeUnorderedList = false;
			var textList = new NSTextList (format);
			Assert.AreEqual ((string) format.GetConstant ()!, textList.CustomMarkerFormat, "CustomMarkerFormat");
#if NET
			Assert.AreEqual (format, textList.MarkerFormat, "MarkerFormat");
#else
			Assert.AreEqual ((string) format.GetConstant ()!, textList.MarkerFormat, "MarkerFormat");
#endif // NET
			Assert.AreEqual (NSTextListOptions.None, textList.ListOptions, "ListOptions");
		}
	}
}

#endif // !__WATCHOS__
