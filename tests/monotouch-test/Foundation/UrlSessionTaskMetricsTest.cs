//
// Unit tests for NSUrlSessionTaskMetrics
//
// Authors:
//	Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlSessionTaskMetricsTest {

		[Test]
		public void Properties ()
		{
			TestRuntime.AssertXcodeVersion (8,0);

			using (var stm = new NSUrlSessionTaskMetrics ()) {
				// in iOS10 those selectors do not respond - but they do work (forwarded to __NSCFURLSessionTaskMetrics type ?)
				Assert.That (stm.RedirectCount, Is.EqualTo (0), "RedirectCount");
				Assert.That (stm.TaskInterval.Duration, Is.EqualTo (0), "TaskInterval");
				Assert.Null (stm.TransactionMetrics, "TransactionMetrics");
			}
		}
	}
}
