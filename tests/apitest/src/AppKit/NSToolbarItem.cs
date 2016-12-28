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
	public class NSToolbarItemTests
	{
		[Test]
		public void InitTests ()
		{
			NSSToolbarItem item = new NSToolbarItem ();
			Assert.IsNotNull (item);
			NSToolbarItemGroup group = new NSToolbarItemGroup ();
			Assert.IsNotNull (group);
		}
	}
}
