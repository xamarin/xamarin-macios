using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using nint = System.Int32;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSObjectTests
	{
		[Test]
		async public Task NSObjectTests_InvokeTest ()
		{
			bool hit = false;
			NSApplication.SharedApplication.Invoke (() => hit = true, 1);
			// Wait for 10 second, then give up
			for (int i = 0; i < 1000; ++i) {
				if (hit)
					return;
				await Task.Delay (10);
			}
			Assert.Fail ("Did not see events after 10 second");
		}
	}
}