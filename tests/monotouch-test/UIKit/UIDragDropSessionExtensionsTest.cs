//
// Unit tests for UIDragDropSessionExtensionsTest
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//	
//
// Copyright 2017 Microsoft.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UIDragDropSessionExtensionsTest {

		[Test]
		public void LoadObjectsTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (9,0))
				Assert.Ignore ("Ignoring tests: Requires iOS11+");

			var test = new DropSession ();
			test.CanLoadObjects (typeof (UIImage));
			test.LoadObjects<UIImage> (null);
		}
	}

	class DropSession : NSObject, IUIDropSession {
		public IUIDragSession LocalDragSession => throw new NotImplementedException ();

		public UIDropSessionProgressIndicatorStyle ProgressIndicatorStyle { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }

		public UIDragItem [] Items => throw new NotImplementedException ();

		public bool AllowsMoveOperation => throw new NotImplementedException ();

		public bool RestrictedToDraggingApplication => throw new NotImplementedException ();

		public bool CanLoadObjects (Class itemProviderReadingClass)
		{
			Assert.That (itemProviderReadingClass.Handle, Is.EqualTo (new Class (typeof (UIImage)).Handle), "UIDragDropSessionExtensionsTest did not convert the type properly for 'CanLoadObjects'.");
			return true;
		}

		public bool HasConformingItems (string [] typeIdentifiers)
		{
			throw new NotImplementedException ();
		}

		public NSProgress LoadObjects (Class itemProviderReadingClass, Action<INSItemProviderReading []> completion)
		{
			Assert.That (itemProviderReadingClass.Handle, Is.EqualTo (new Class (typeof (UIImage)).Handle), "UIDragDropSessionExtensionsTest did not convert the type properly for 'LoadObjects'.");
			return new NSProgress ();
		}

		public CGPoint LocationInView (UIView view)
		{
			throw new NotImplementedException ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__