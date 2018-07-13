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
using ObjCRuntime;
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

		[Test]
		public void GetMatchingFontDescriptorsCollectionOptionsTest ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			using (var collection = new CTFontCollection (null)) {
				var fd1 = collection.GetMatchingFontDescriptors ();
				var fd2 = collection.GetMatchingFontDescriptors (options: null); // documented to return the same thing as the parameterless if null
				Assert.NotNull (fd1, "fd1");
				Assert.NotNull (fd2, "fd2");
				Assert.AreEqual (fd1.Length, fd2.Length, "equal collections");

				var fd3 = collection.GetMatchingFontDescriptors (new CTFontCollectionOptions { RemoveDuplicates = true });
				Assert.NotNull (fd3, "fd3");
			}
		}
	}
}
