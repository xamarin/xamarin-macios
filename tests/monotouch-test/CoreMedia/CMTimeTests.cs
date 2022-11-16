//
// Unit tests for CMTime
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//
using System;
using Foundation;
using CoreMedia;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMTimeTests {

		[Test]
		public void PropertiesTest ()
		{
			CMTime v;

			v = new CMTime (1, 2);
			Assert.That (v.Value, Is.EqualTo (1), "Value");
			Assert.That (v.TimeScale, Is.EqualTo (2), "TimeScale");
			Assert.That (!v.IsInvalid, "IsInvalid");
			Assert.That (v.AbsoluteValue.Description, Is.EqualTo ("{1/2 = 0.500}"), "AbsoluteValue");
			Assert.That (v.Description, Is.EqualTo ("{1/2 = 0.500}"), "Description");
			Assert.That (!v.IsIndefinite, "IsIndefinite");
			Assert.That (!v.IsNegativeInfinity, "IsNegativeInfinity");
			Assert.That (!v.IsPositiveInfinity, "IsPositiveInfinity");
			Assert.That (v.Seconds, Is.EqualTo (0.5), "Seconds");
			Assert.That (v.TimeEpoch, Is.EqualTo (0), "TimeEpoch");
			Assert.That (v.TimeFlags == CMTime.Flags.Valid, "TimeFlag");
		}

		[Test]
		public void MethodsTest ()
		{
			CMTime v, w, x, y;
			v = new CMTime (1, 2);
			w = new CMTime (1, 2);
			x = new CMTime (2, 1);
			y = new CMTime (2, 2);

			// equality operators
			Assert.That (v == w, "Equality #1");
			Assert.That (!(v == x), "Equality #2");
			Assert.That (v != x, "Inequality #1");
			Assert.That (!(v != w), "Inequality #2");
			Assert.That (CMTime.Compare (v, w), Is.EqualTo (0), "Compare #1");
			Assert.That (CMTime.Compare (v, x) != 0, "Compare #2");
			Assert.That (v.Equals (w), "Equals #1");
			Assert.That (!x.Equals (v), "Equals #2");

			// addition operator
			Assert.That (v + w == new CMTime (2, 2), "Addition #1");
			Assert.That (CMTime.Add (v, w) == new CMTime (2, 2), "Addition #2");

			// subtraction operator
			Assert.That (v - w == new CMTime (0, 2), "Subtraction #1");
			Assert.That (CMTime.Subtract (v, w) == new CMTime (0, 2), "Subtraction #2");

			// multiplication operators
			Assert.That (v * 2 == new CMTime (2, 2), "Multiplication * int, #1");
			Assert.That (CMTime.Multiply (v, 3) == new CMTime (3, 2), "Multiplication * int, #2");
			Assert.That (v * 4.0 == new CMTime (4, 2), "Multiplication * double, #1");
			Assert.That (CMTime.Multiply (v, 5.0) == new CMTime (5, 2), "Multiplication * double, #2");

			// ConvertScale
			Assert.That (new CMTime (10, 2).ConvertScale (1, CMTimeRoundingMethod.Default) == new CMTime (5, 1), "ConvertScale #1");

			// FromSeconds
			Assert.That (CMTime.FromSeconds (20, 1) == new CMTime (20, 1), "FromSeconds #1");

			// GetMaximum
			Assert.That (CMTime.GetMaximum (v, y) == y, "GetMaximum #1");

			// GetMinimum
			Assert.That (CMTime.GetMinimum (v, y) == v, "GetMinimum #1");

			using (var d = x.ToDictionary ()) {
				Assert.That (d.RetainCount, Is.EqualTo ((nuint) 1), "RetainCount");
				Assert.That (d.Count, Is.EqualTo ((nuint) 4), "Count");

				var time = CMTime.FromDictionary (d);
				Assert.That (time, Is.EqualTo (x), "FromDictionary");
			}
		}

		[Test]
		public void MultiplyByRatio ()
		{
			TestRuntime.AssertXcodeVersion (5, 1);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			var t = new CMTime (1000, 1);
			t = CMTime.Multiply (t, 20, 10);
			Assert.That (t.Value, Is.EqualTo (2000), "Value");
		}

		[Test]
		public void CMTimeRangeConstants ()
		{
			// Just make sure accessing the constants doesn't hard crash us
			Assert.DoesNotThrow (() => { var x = CMTimeRange.Zero; }, "CMTimeRangeConstants - Zero");
			Assert.DoesNotThrow (() => { var x = CMTimeRange.InvalidRange; }, "CMTimeRangeConstants - InvalidRange");
			Assert.DoesNotThrow (() => { var x = CMTimeRange.InvalidMapping; }, "CMTimeRangeConstants - InvalidMapping");
#if !XAMCORE_3_0
			Assert.DoesNotThrow (() => { var x = CMTimeRange.Invalid; }, "CMTimeRangeConstants - Invalid");
#endif
			if (TestRuntime.CheckXcodeVersion (7, 0)) {
				Assert.DoesNotThrow (() => { var x = CMTimeRange.InvalidMapping; }, "CMTimeRangeConstants - InvalidMapping");
				Assert.DoesNotThrow (() => { var x = CMTimeRange.TimeMappingSourceKey; }, "CMTimeRangeConstants - TimeMappingSourceKey");
				Assert.DoesNotThrow (() => { var x = CMTimeRange.TimeMappingTargetKey; }, "CMTimeRangeConstants - TimeMappingTargetKey");
			}
		}

		[Test]
		public void CMTimeMappingFactoryMethods ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Inconclusive ("Requires iOS 9.0+ or macOS 10.11+");

			var first = new CMTimeRange () { Duration = new CMTime (12, 1), Start = new CMTime (1, 1) };
			var second = new CMTimeRange () { Duration = new CMTime (4, 1), Start = new CMTime (1, 1) };

			CMTimeMapping map = CMTimeMapping.Create (first, second);
			CompareCMTimeRange (map.Source, first, "CMTimeMapping.Create");
			CompareCMTimeRange (map.Target, second, "CMTimeMapping.Create");

			map = CMTimeMapping.CreateEmpty (first);
			CompareCMTimeRange (map.Source, CMTimeRange.InvalidRange, "CMTimeMapping.CreateEmpty");
			CompareCMTimeRange (map.Target, first, "CMTimeMapping.CreateEmpty");

			map = CMTimeMapping.CreateFromDictionary (new NSDictionary ());
			CompareCMTimeRange (map.Source, CMTimeRange.InvalidRange, "CMTimeMapping.CreateFromDictionary");
			CompareCMTimeRange (map.Target, CMTimeRange.InvalidRange, "CMTimeMapping.CreateFromDictionary");

			Assert.IsNotNull (map.AsDictionary (), "CMTimeMapping AsDictionary");

			Assert.IsNotNull (map.Description, "CMTimeMapping Description");
		}

		void CompareCMTimeRange (CMTimeRange first, CMTimeRange second, string description)
		{
			Assert.AreEqual (first.Duration, second.Duration, "CompareCMTimeRange - duration - " + description);
			Assert.AreEqual (first.Start, second.Start, "CompareCMTimeRange - start - " + description);
		}

#if !__WATCHOS__
		[Test]
		public void CMTimeStrongDictionary ()
		{
			var time = new CMTime (1000, 1);
			var timeDict = new CMTimeDict {
				Time = time
			};
			var retrievedTime = timeDict.Time;
			Assert.IsTrue (time == retrievedTime, "CMTimeStrongDictionary");
		}

		class CMTimeDict : DictionaryContainer {
			static NSString TimeKey = new NSString ("TimeKey");
			public CMTime? Time {
				get { return GetCMTimeValue (TimeKey); }
				set { SetCMTimeValue (TimeKey, value); }
			}
		}
#endif // !__WATCHOS__
	}
}
