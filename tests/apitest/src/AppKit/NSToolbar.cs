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
	public class NSToolbarTests
	{
		NSToolbar toolbar;

		[SetUp]
		public void SetUp ()
		{
			toolbar = new NSToolbar (NSToolbar.NSToolbarSeparatorItemIdentifier);
		}

		[Test]
		public void NSToolbarShouldChangeAllowsExtensionItems ()
		{
			Asserts.EnsureYosemite ();

			var allows = toolbar.AllowsExtensionItems;
			toolbar.AllowsExtensionItems = !allows;

			Assert.IsFalse (toolbar.AllowsExtensionItems == allows, "NSToolbarShouldChangeAllowsExtensionItems - Failed to set the AllowsExtensionItems property");
		}
	}
}