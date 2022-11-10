// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__

using System;
using System.Drawing;
using System.Reflection;
using Foundation;
using MapKit;
using CoreGraphics;
using ObjCRuntime;
#if MONOMAC
using PlatformImage = AppKit.NSImage;
using PlatformView = AppKit.NSView;
#else
using UIKit;
using PlatformImage = UIKit.UIImage;
using PlatformView = UIKit.UIView;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MapKit {

#if !XAMCORE_3_0
	class AnnotationViewPoker : MKAnnotationView {

		static FieldInfo bkAnnotation;

		static AnnotationViewPoker ()
		{
			var t = typeof (MKAnnotationView);
			bkAnnotation = t.GetField ("__mt_Annotation_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}

		public AnnotationViewPoker (IMKAnnotation annotation) : base (annotation, "reuse")
		{
		}

		public NSObject AnnotationBackingField {
			get {
				return (NSObject) bkAnnotation.GetValue (this);
			}
		}
	}
#endif

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AnnotationViewTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);
		}

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (MKAnnotationView av = new MKAnnotationView (frame)) {
				Assert.That (av.Frame, Is.EqualTo (frame), "Frame");
				Assert.Null (av.Annotation, "Annotation");
			}
		}

		[Test]
		public void InitWithAnnotation ()
		{
			// using a null 'annotation' crash - but the property can be set to null later
			using (var a = new MKPolygon ())
			using (MKAnnotationView av = new MKAnnotationView (a, "reuse")) {
				Assert.AreSame (a, av.Annotation, "Annotation");
				av.Annotation = null;
			}
		}

#if !XAMCORE_3_0
		[Test]
		public void Annotation_BackingFields ()
		{
			if (AnnotationViewPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var a = new MKPolygon ())
			using (var av = new AnnotationViewPoker (a)) {
				Assert.AreSame (a, av.AnnotationBackingField, "1a");
				Assert.AreSame (a, av.Annotation, "2a");
			}
		}
#endif // !XAMCORE_3_0

		[Test]
		public void Default ()
		{
			using (var def = new MKAnnotationView ()) {
				Assert.IsNull (def.Annotation, "Annotation");
				Assert.AreEqual (def.CalloutOffset, CGPoint.Empty, "CalloutOffset");
				Assert.IsFalse (def.CanShowCallout, "CanShowCallout");
				Assert.AreEqual (def.CenterOffset, CGPoint.Empty, "CenterOffset");
				Assert.IsFalse (def.Draggable, "Draggable");
				Assert.That (def.DragState, Is.EqualTo (MKAnnotationViewDragState.None), "DragState");
				Assert.IsTrue (def.Enabled, "Enabled");
				Assert.IsFalse (def.Highlighted, "Highlighted");
				Assert.IsNull (def.Image, "Image");
				Assert.IsNull (def.LeftCalloutAccessoryView, "LeftCalloutAccessoryView");
				Assert.IsNull (def.ReuseIdentifier, "ReuseIdentifier");
				Assert.IsNull (def.RightCalloutAccessoryView, "RightCalloutAccessoryView");
				Assert.IsFalse (def.Selected, "Selected");
			}
		}

		[Test]
		public void Null ()
		{
			using (var def = new MKAnnotationView ()) {
				def.Annotation = null;
				def.Annotation = new MKPolygon ();
				Assert.IsNotNull (def.Annotation, "Annotation NN");
				def.Annotation = null;
				Assert.IsNull (def.Annotation, "Annotation N");

				def.Image = null;
				def.Image = new PlatformImage ();
				Assert.IsNotNull (def.Image, "Image NN");
				def.Image = null;
				Assert.IsNull (def.Image, "Image N");

				def.LeftCalloutAccessoryView = null;
				def.LeftCalloutAccessoryView = new PlatformView ();
				Assert.IsNotNull (def.LeftCalloutAccessoryView, "LeftCalloutAccessoryView NN");
				def.LeftCalloutAccessoryView = null;
				Assert.IsNull (def.LeftCalloutAccessoryView, "LeftCalloutAccessoryView N");

				def.RightCalloutAccessoryView = null;
				def.RightCalloutAccessoryView = new PlatformView ();
				Assert.IsNotNull (def.RightCalloutAccessoryView, "RightCalloutAccessoryView NN");
				def.RightCalloutAccessoryView = null;
				Assert.IsNull (def.RightCalloutAccessoryView, "RightCalloutAccessoryView N");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
