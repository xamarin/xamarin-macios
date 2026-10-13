//
// Unit tests for CALayer
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved.
//

using System.Diagnostics;
using System.Linq;
using System.Threading;
using CoreGraphics;
using CoreAnimation;

namespace MonoTouchFixtures.CoreAnimation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LayerTest {

		[Test]
		public void Mask ()
		{
			using (CALayer layer = new CALayer ()) {
				Assert.That (layer.Mask, Is.Null, "Mask/default");
				layer.Mask = new CALayer ();
				Assert.That (layer.Mask, Is.Not.Null, "Mask/assigned");
				layer.Mask = null;
				Assert.That (layer.Mask, Is.Null, "Mask/nullable");
			}
		}

		[Test]
		public void CAActionTest ()
		{
			// bug 2441
			CAActionTestClass obj = new CAActionTestClass ();
			Assert.That (obj.ActionForKey ("animation"), Is.Null, "a");
			Assert.That (obj.Actions, Is.Null, "b");
			Assert.That (CAActionTestClass.DefaultActionForKey ("animation"), Is.Null, "c");

			var animationKey = new NSString ("animation");
			var basicAnimationKey = new NSString ("basicAnimation");
			var dict = NSDictionary.FromObjectsAndKeys (
				new NSObject [] { new CABasicAnimation (), new CAAnimation () },
				new NSObject [] { basicAnimationKey, animationKey }
			);
			obj.Actions = dict;
			Assert.That (obj.Actions == dict, "d");

			Assert.That (obj.ActionForKey ("animation") == dict [animationKey], "e");
			Assert.That (obj.ActionForKey ("basicAnimation") == dict [basicAnimationKey], "f");
			Assert.That (CAActionTestClass.DefaultActionForKey ("animation"), Is.Null, "g");
			Assert.That (CALayer.DefaultActionForKey ("animation"), Is.Null, "h");
		}

		class CAActionTestClass : CALayer {

		}

		[Test]
		public void ConvertPoint ()
		{
			using (CALayer layer = new CALayer ()) {
				Assert.That (layer.ConvertPointFromLayer (CGPoint.Empty, null).IsEmpty, Is.True, "From/Empty/null");
				Assert.That (layer.ConvertPointToLayer (CGPoint.Empty, null).IsEmpty, Is.True, "To/Empty/null");
			}
		}

		[Test]
		public void ConvertRect ()
		{
			using (CALayer layer = new CALayer ()) {
				Assert.That (layer.ConvertRectFromLayer (CGRect.Empty, null).IsEmpty, Is.True, "From/Empty/null");
				Assert.That (layer.ConvertRectToLayer (CGRect.Empty, null).IsEmpty, Is.True, "To/Empty/null");
			}
		}

		[Test]
		public void ConvertTime ()
		{
			using (CALayer layer = new CALayer ()) {
				Assert.That (layer.ConvertTimeFromLayer (0.0d, null), Is.EqualTo (0.0d), "From/0.0d/null");
				Assert.That (layer.ConvertTimeToLayer (0.0d, null), Is.EqualTo (0.0d), "To/0.0d/null");
			}
		}

		[Test]
		public void AddAnimation ()
		{
			using (var layer = new CALayer ()) {
				var animation = new CABasicAnimation ();
				Assert.That (layer.AnimationForKey ("key"), Is.Null, "#key A");
				layer.AddAnimation (animation, "key");
				Assert.That (layer.AnimationForKey ("key"), Is.Not.Null, "#key B");
			}
		}


		static int TextLayersDisposed;
		static int Generation;
		[Test]
		public void TestBug26532 ()
		{
			TextLayersDisposed = 0;
			Generation++;

			const int layerCount = 50;
			Exception ex = null;
			var thread = new Thread (() => {
				try {
					var frame = new CGRect (0, 0, 200, 200);
					using (var layer = new CALayer ()) {
						for (int i = 0; i < layerCount; i++) {
							TextCALayer textLayer = new TextCALayer () {
								Secret = "42",
							};
							layer.AddSublayer (textLayer);
						}

						GC.Collect ();

						foreach (var slayer in layer.Sublayers.OfType<TextCALayer> ()) {
							Assert.That (slayer.Secret, Is.EqualTo ("42"));
						}

						foreach (var slayer in layer.Sublayers.OfType<TextCALayer> ())
							slayer.RemoveFromSuperLayer ();
					}
				} catch (Exception e) {
					ex = e;
				}
			}) {
				IsBackground = true,
			};
			thread.Start ();
			Assert.That (thread.Join (TimeSpan.FromSeconds (10)), Is.True, "Thread.Join timed out");

			var watch = new Stopwatch ();
			watch.Start ();
			while (watch.ElapsedMilliseconds < 2000 && TextLayersDisposed < layerCount / 2) {
				GC.Collect ();
				NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.05));
			}

			Assert.That (ex, Is.Null, "Exceptions");
			Assert.That (TextLayersDisposed, Is.AtLeast (layerCount / 2), "disposed text layers");
		}

		public class TextCALayer : CALayer {
			public string Secret;
			public int generation;

			public TextCALayer ()
			{
				generation = Generation;
			}

			protected override void Dispose (bool disposing)
			{
				if (generation == Generation) {
					TextLayersDisposed++;
				} else {
					Console.WriteLine ("TextCALayer.Dispose called for an object from a previous test run.");
				}
				base.Dispose (disposing);
			}
		}

		class Layer : CALayer { }
		class LayerDelegate : CALayerDelegate { }

		[Test]
		public void TestCALayerDelegateDispose ()
		{
			var del = new LayerDelegate ();
			var t = new Thread (() => {
				var l = new Layer ();
				l.Delegate = del;
				l.Dispose ();
				CATransaction.Flush ();
			}) {
				IsBackground = true,
			};
			t.Start ();
			Assert.That (t.Join (TimeSpan.FromSeconds (5)), Is.True, "Thread.Join timed out");
			GC.Collect ();

			NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.1));

			GC.Collect ();
			del.Dispose ();
		}
	}
}
