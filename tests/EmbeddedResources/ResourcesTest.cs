//
// Resource Bundling Tests
//
// Authors:
//	Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Resources;
using System.Globalization;
using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

namespace EmbeddedResources {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ResourcesTest {

		[Test]
		public void Embedded ()
		{
			var manager = new ResourceManager ("EmbeddedResources.Welcome", typeof(ResourcesTest).Assembly);
			
			Assert.AreEqual ("Welcome", manager.GetString ("String1", new CultureInfo ("en")), "en");
			Assert.AreEqual ("Willkommen", manager.GetString ("String1", new CultureInfo ("de")), "de");
			Assert.AreEqual ("Bienvenido", manager.GetString ("String1", new CultureInfo ("es")), "es");
		}
	}
}