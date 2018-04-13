//
// Unit tests for CTFontCollection
//
// Author:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2018 Microsoft. All rights reserved.
//

using System;
using System.Linq;
#if XAMCORE_2_0
using Foundation;
using CoreText;
#else
using MonoTouch.CoreText;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CTFontCollectionTest {

		[SetUp]
		public void Setup ()
		{
			// CoreText was introduced in watchOS 2.2
			TestRuntime.AssertXcodeVersion (7, 3);
		}

		[Test]
		public void GetMatchingFontDescriptorsTest ()
		{
			var collection = new CTFontCollection (null);
			var sortIsCalled = false;
			var descList = collection.GetMatchingFontDescriptors ((CTFontDescriptor x, CTFontDescriptor y) => {
				sortIsCalled = true;
				return 0;
			});

			Assert.IsTrue (sortIsCalled, "GetMatchingFontDescriptors delegate is called");

			// Native crash (can't assert on it) if https://github.com/xamarin/xamarin-macios/pull/3871 fix not present.
			descList.First ().GetAttributes ();
		}
	}
}
