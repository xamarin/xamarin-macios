// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using CoreLocation;
using MapKit;
#else
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MapKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ShapeTest {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertMacSystemVersion (10, 9, throwIfOtherPlatform: false);
		}
		
		// used for types that inherits from MKShape
		static public void CheckShape (MKShape shape)
		{
			// MKShape provides read/write properties for Title and Subtitle
			// even if they are read-only according to MKAnnotation 
			// http://xamarin.assistly.com/agent/case/5441
			shape.Title = "Title";
			Assert.That (shape.Title, Is.EqualTo ("Title"), "Title/set/get");
			shape.Subtitle = "Subtitle";
			Assert.That (shape.Subtitle, Is.EqualTo ("Subtitle"), "Subtitle/set/get");
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
