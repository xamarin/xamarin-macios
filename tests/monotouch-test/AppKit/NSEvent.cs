#if __MACOS__
using NUnit.Framework;
using System;

using AppKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSEventTests {
		[Test]
		public void Create ()
		{
			using var cgevent = new CGEvent (null, (ushort) 1, true);
			using var nsevent = NSEvent.Create (cgevent);
			Assert.AreEqual ((int) cgevent.EventType, (int) nsevent.Type, "[Event]Type");
		}
	}
}
#endif // __MACOS__
