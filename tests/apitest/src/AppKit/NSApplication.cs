using NUnit.Framework;
using System;

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
	public class NSApplicationTests
	{
		[Test]
		public void NSApplication_SendActionNullTest ()
		{
			NSApplication.SharedApplication.SendAction(new Selector("undo:"), null, new NSObject ());
		}
	}
}