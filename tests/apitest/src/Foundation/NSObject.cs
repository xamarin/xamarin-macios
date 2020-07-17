using System;
using System.Threading;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSObjectTests
	{
		[Test]
		public void NSObjectTests_InvokeTest ()
		{
			bool hit = false;
			NSApplication.SharedApplication.Invoke (() => hit = true, 1);
			TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => { }, () => hit);
			Assert.IsTrue (hit, "Did not see events after 10 seconds");
		}
	}
}
