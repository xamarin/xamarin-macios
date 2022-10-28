//
// Unit tests for UIPasteConfigurationSupportingTest
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
//
// Copyright 2017 Microsoft.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using ObjCRuntime;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UIPasteConfigurationSupportingTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void UIViewControllerPasteTest ()
		{
			var viewController = new ViewControllerPoker ();
			viewController.PasteConfiguration = new UIPasteConfiguration (typeof (UIImage));
			viewController.Paste (new NSItemProvider [] { new NSItemProvider (new UIImage ()) });
		}

		[Test]
		public void UIViewPasteTest ()
		{
			var view = new ViewPoker ();
			view.PasteConfiguration = new UIPasteConfiguration (typeof (UIImage));
			view.Paste (new NSItemProvider [] { new NSItemProvider (new UIImage ()) });
		}

		[Test]
		public void SKNodeTest ()
		{
			var node = new NodePoker ();
			node.PasteConfiguration = new UIPasteConfiguration (typeof (UIImage));
			node.Paste (new NSItemProvider [] { new NSItemProvider (new UIImage ()) });
		}

		class ViewControllerPoker : UIViewController {

			public override void Paste (NSItemProvider [] itemProviders)
			{
				Assert.IsTrue (itemProviders [0].CanLoadObject (typeof (UIImage)));
			}
		}

		class ViewPoker : UIView {

			public override void Paste (NSItemProvider [] itemProviders)
			{
				Assert.IsTrue (itemProviders [0].CanLoadObject (typeof (UIImage)));
			}
		}

		class NodePoker : SKNode {

			public override void Paste (NSItemProvider [] itemProviders)
			{
				Assert.IsTrue (itemProviders [0].CanLoadObject (typeof (UIImage)));
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
