#if __MACOS__
using System;
using System.Linq;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSIndexSetTests {
		[Test]
		public void NSIndexSet_ConstructorTest ()
		{
#pragma warning disable 0219
			NSIndexSet a = new NSIndexSet ((int) 5);
			NSIndexSet b = new NSIndexSet ((uint) 5);
			NSIndexSet c = new NSIndexSet ((nint) 5);
#pragma warning restore 0219
		}

		[Test]
		public void NSIndexSet_EmptyToList ()
		{
			NSIndexSet a = new NSIndexSet ();
#pragma warning disable 0219
			var b = a.ToList ();
#pragma warning restore 0219
		}

		[Test]
		public void NSIndexSet_EmptyToArray ()
		{
			NSIndexSet a = new NSIndexSet ();
#pragma warning disable 0219
			var b = a.ToArray ();
#pragma warning restore 0219
		}
	}
}
#endif // __MACOS__
