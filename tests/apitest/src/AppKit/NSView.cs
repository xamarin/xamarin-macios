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
	public class NSViewTests
	{
		NSView view;

		[SetUp]
		public void SetUp ()
		{
			view = new NSView ();
		}

		[Test]
		public void NSViewShouldAddGestureRecognizer ()
		{
			Asserts.EnsureYosemite ();

			var length = 0;
			if (view.GestureRecognizers != null)
				length = view.GestureRecognizers.Length;
			view.AddGestureRecognizer (new NSGestureRecognizer ());

			Assert.IsTrue (view.GestureRecognizers.Length == length + 1, "NSViewShouldAddGestureRecognizer - Failed to add recognizer, count didn't change.");
		}

		[Test]
		public void NSViewShouldRemoveGestureRecognizer ()
		{
			Asserts.EnsureYosemite ();

			var recognizer = new NSClickGestureRecognizer ();
			view.AddGestureRecognizer (recognizer);

			Assert.IsTrue (view.GestureRecognizers.Length != 0, "NSViewShouldRemoveGestureRecognizer - Failed to add gesture recognizer");

			view.RemoveGestureRecognizer (recognizer);

			Assert.IsTrue (view.GestureRecognizers.Length == 0, "NSViewShouldRemoveGestureRecognizer - Failed to remove gesture recognizer");
		}

		[Test]
		public void NSViewShouldChangeGestureRecognizers ()
		{
			Asserts.EnsureYosemite ();

			var recognizers = view.GestureRecognizers;
			view.GestureRecognizers = new NSGestureRecognizer [] { new NSClickGestureRecognizer (), new NSPanGestureRecognizer () };

			Assert.IsFalse (view.GestureRecognizers == recognizers);
		}

		[Test]
		public void AllItemsWithNSMenuShouldAllowNull ()
		{
			// Can't test typeof (NSResponder) since it is abstract
			List <Type> types = new List<Type> { typeof (NSCell), typeof (NSMenuItem), typeof (NSPathControl),
				typeof (NSPopUpButton), typeof (NSPopUpButtonCell) };
			if (IntPtr.Size == 4)
				types.Add (typeof (NSMenuView)); // NSMenuView is 32-bit only

			foreach (Type t in types) {
				object o = Activator.CreateInstance (t);
				PropertyInfo prop = t.GetProperty("Menu", BindingFlags.Public | BindingFlags.Instance);
				prop.SetValue (o, null, null);
			}

			// NSStateBarItem can't be created via default constructor
			NSStatusBar.SystemStatusBar.CreateStatusItem (10).Menu = null;
		}
	}
}
