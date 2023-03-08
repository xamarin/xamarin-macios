//
// Resource Bundling Tests
//
// Authors:
//	Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022 Microsoft Corp. All rights reserved.
//

using System;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

using Foundation;
using ObjCRuntime;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ResourcesTest {

		[Test]
		public void Embedded ()
		{
			var manager = new ResourceManager ("monotouchtest.Welcome", typeof (ResourcesTest).Assembly);

			Assert.AreEqual ("Welcome!", manager.GetString ("String1", new CultureInfo ("en")), "en");
			Assert.AreEqual ("G'day!", manager.GetString ("String1", new CultureInfo ("en-AU")), "en-AU");
			Assert.AreEqual ("Willkommen!", manager.GetString ("String1", new CultureInfo ("de")), "de");
			Assert.AreEqual ("Willkommen!", manager.GetString ("String1", new CultureInfo ("de-DE")), "de-DE");
			Assert.AreEqual ("Bienvenido!", manager.GetString ("String1", new CultureInfo ("es")), "es");
			Assert.AreEqual ("Bienvenido!", manager.GetString ("String1", new CultureInfo ("es-AR")), "es-AR");
			Assert.AreEqual ("Bienvenido!", manager.GetString ("String1", new CultureInfo ("es-ES")), "es-ES");
		}
	}
}
