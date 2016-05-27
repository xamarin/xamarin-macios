using System;
using System.Collections.Generic;
using System.Reflection;
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
	public class NSWindowControllerTests
	{
		[Test]
		public void NSWindowController_ShowWindowTest ()
		{
			NSWindowController c = new NSWindowController ();
			c.ShowWindow (null);
		}
	}
}
