//
// Unit tests for UITraitOverrides
//
// Authors:
//	Rolf Bjarne Kvinge  <rolf@xamarin.com>
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

#if !__WATCHOS__ && !__MACOS__ && NET

using System;
using System.IO;
using System.Reflection;
using System.Threading;

using Foundation;
using UIKit;
using ObjCRuntime;

using NUnit.Framework;

using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UITraitOverridesTest {

		[Test]
		public void RegisterForTraitChanges_ClassArray ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				return vc.RegisterForTraitChanges (new [] {
						new Class (typeof (UITraitVerticalSizeClass)),
						new Class (typeof (UITraitHorizontalSizeClass)),
						}, callback);
			}, "ClassArray ");
		}

		[Test]
		public void RegisterForTraitChanges_TypeArray ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				return vc.RegisterForTraitChanges (new [] {
						typeof (UITraitVerticalSizeClass),
						typeof (UITraitHorizontalSizeClass),
						}, callback);
			}, "TypeArray ");
		}

		[Test]
		public void RegisterForTraitChanges_ParamsTypeArray ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				return vc.RegisterForTraitChanges (callback,
						typeof (UITraitVerticalSizeClass),
						typeof (UITraitHorizontalSizeClass));
			}, "ParamsTypeArray ");
		}

		[Test]
		public void RegisterForTraitChanges_Generic ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				// return vc.RegisterForTraitChanges<NSObject> (callback);
				return vc.RegisterForTraitChanges<UITraitHorizontalSizeClass, UITraitVerticalSizeClass> (callback);
			}, "Generic ");
		}

		void CallbackTest (Func<IUITraitChangeObservable, Action<IUITraitEnvironment, UITraitCollection>, IUITraitChangeRegistration> registerFunc, string prefix)
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var vc = new UIViewController ();
			using var label = new UILabel () {
				TextAlignment = UITextAlignment.Center,
			};
			vc.View!.AddSubview (label);

			UIUserInterfaceSizeClass? horizontal = null;
			UIUserInterfaceSizeClass? vertical = null;
			var msgPrefix = "?";
			var callbackCounter = 0;
			Exception? ex = null;

			Action<IUITraitEnvironment, UITraitCollection> callback = (env, coll) => {
				try {
					Assert.AreEqual (horizontal is not null, vc.TraitOverrides.ContainsTrait<UITraitHorizontalSizeClass> (), $"{prefix}{msgPrefix} Horizontal A");
					Assert.AreEqual (horizontal is not null, vc.TraitOverrides.ContainsTrait (typeof (UITraitHorizontalSizeClass)), $"{prefix}{msgPrefix} Horizontal B");
					Assert.AreEqual (horizontal is not null, vc.TraitOverrides.ContainsTrait (new Class (typeof (UITraitHorizontalSizeClass))), $"{prefix}{msgPrefix} Horizontal C");
					Assert.AreEqual (vertical is not null, vc.TraitOverrides.ContainsTrait<UITraitVerticalSizeClass> (), $"{prefix}{msgPrefix} Vertical A");
					Assert.AreEqual (vertical is not null, vc.TraitOverrides.ContainsTrait (typeof (UITraitVerticalSizeClass)), $"{prefix}{msgPrefix} Vertical B");
					Assert.AreEqual (vertical is not null, vc.TraitOverrides.ContainsTrait (new Class (typeof (UITraitVerticalSizeClass))), $"{prefix}{msgPrefix} Vertical C");
					if (horizontal is not null) {
						Assert.AreEqual (horizontal.Value, vc.TraitOverrides.HorizontalSizeClass, $"{prefix}{msgPrefix} Horizontal Value");
					}
					if (vertical is not null) {
						Assert.AreEqual (vertical.Value, vc.TraitOverrides.VerticalSizeClass, $"{prefix}{msgPrefix} Vertical Value");
					}
					callbackCounter++;
				} catch (Exception e) {
					ex = e;
				}
			};

			// Figure out the initial values so that we can choose a different value when setting them.
			var tc = UITraitCollection.CurrentTraitCollection;
			UIUserInterfaceSizeClass? initialHorizonal = null;
			UIUserInterfaceSizeClass? initialVertical = null;
			tc.PerformAsCurrentTraitCollection (() => {
				initialHorizonal = tc.HorizontalSizeClass;
				initialVertical = tc.VerticalSizeClass;
			});

			var firstHorizontal = (initialHorizonal == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Compact : UIUserInterfaceSizeClass.Regular;
			var firstVertical = (initialVertical == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Compact : UIUserInterfaceSizeClass.Regular;
			var secondHorizontal = (initialHorizonal == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Regular : UIUserInterfaceSizeClass.Compact;
			var secondVertical = (initialVertical == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Regular : UIUserInterfaceSizeClass.Compact;

			var token = registerFunc (vc, callback);

			horizontal = firstHorizontal;
			vc.TraitOverrides.HorizontalSizeClass = horizontal.Value;
			Assert.AreEqual (1, callbackCounter, $"{prefix}CallbackCounter 1");
			Assert.IsNull (ex, $"{prefix}Exception 1");

			horizontal = secondHorizontal;
			vc.TraitOverrides.HorizontalSizeClass = horizontal.Value;
			Assert.AreEqual (2, callbackCounter, $"{prefix}CallbackCounter 2");
			Assert.IsNull (ex, $"{prefix}Exception 2");

			vertical = firstVertical;
			vc.TraitOverrides.VerticalSizeClass = vertical.Value;
			Assert.AreEqual (3, callbackCounter, $"{prefix}CallbackCounter 3");
			Assert.IsNull (ex, $"{prefix}Exception 3");

			vertical = secondVertical;
			vc.TraitOverrides.VerticalSizeClass = vertical.Value;
			Assert.AreEqual (4, callbackCounter, $"{prefix}CallbackCounter 4");
			Assert.IsNull (ex, $"{prefix}Exception 4");

			vertical = null;
			vc.TraitOverrides.RemoveTrait (typeof (UITraitVerticalSizeClass));
			Assert.AreEqual (4, callbackCounter, $"{prefix}CallbackCounter 5");
			Assert.IsNull (ex, $"{prefix}Exception 5");

			horizontal = null;
			vc.TraitOverrides.RemoveTrait<UITraitHorizontalSizeClass> ();
			Assert.AreEqual (4, callbackCounter, $"{prefix}CallbackCounter 6");
			Assert.IsNull (ex, $"{prefix}Exception 6");

			horizontal = firstHorizontal;
			vc.TraitOverrides.HorizontalSizeClass = horizontal.Value;
			Assert.AreEqual (5, callbackCounter, $"{prefix}CallbackCounter 7");
			Assert.IsNull (ex, $"{prefix}Exception 7");

			horizontal = null;
			vc.TraitOverrides.RemoveTrait (new Class (typeof (UITraitHorizontalSizeClass)));
			Assert.AreEqual (5, callbackCounter, $"{prefix}CallbackCounter 8");
			Assert.IsNull (ex, $"{prefix}Exception 8");

			vc.UnregisterForTraitChanges (token);
		}
	}
}

#endif // !__WATCHOS__ && !__MACOS__
