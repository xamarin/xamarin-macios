using System;
using System.Linq;
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
	public class NSIndexSetTests
	{
		[Test]
		public void NSIndexSet_ConstructorTest ()
		{
#pragma warning disable 0219
			NSIndexSet a = new NSIndexSet ((int)5);
#if XAMCORE_2_0
			NSIndexSet b = new NSIndexSet ((uint)5);
			NSIndexSet c = new NSIndexSet ((nint)5);
#endif
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
