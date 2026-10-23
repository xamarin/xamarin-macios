// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

#if !__TVOS__

using System;
using Foundation;
using NUnit.Framework;
using WidgetKit;

namespace MonoTouchFixtures.WidgetKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WidgetCenterTest {

		[Test]
		public void ReloadTimelines ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			Assert.DoesNotThrow (() => WidgetCenter.ReloadTimelines ("com.xamarin.monotouch-test.widget"));
		}

		[Test]
		public void ReloadAllTimelines ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			Assert.DoesNotThrow (() => WidgetCenter.ReloadAllTimelines ());
		}

		[Test]
		public void ReloadTimelines_Null ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			var exception = Assert.Throws<ArgumentNullException> (() => WidgetCenter.ReloadTimelines (null));
			Assert.That (exception.ParamName, Is.EqualTo ("kind"));
		}
	}
}

#endif // !__TVOS__
