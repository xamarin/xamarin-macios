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
using System.Reflection;
using System.Reflection.Emit;
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
#if __TVOS__
			bool in_interpreter = false;
			try {
				AssemblyName aName = new AssemblyName ("DynamicAssemblyExample");
				AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly (aName, AssemblyBuilderAccess.RunAndSave);
				in_interpreter = true;
			} catch (PlatformNotSupportedException) {
				// we do not have the interpreter, lets continue
			}
			if (in_interpreter)
				Assert.Ignore ("This test is disabled on TVOS."); // Randomly crashed on tvOS -> https://github.com/xamarin/maccore/issues/1909
#endif

#if MONOMAC
			var manager = new ResourceManager ("xammac_tests.EmbeddedResources.Welcome", typeof (ResourcesTest).Assembly);
#else
			var manager = new ResourceManager ("EmbeddedResources.Welcome", typeof(ResourcesTest).Assembly);
#endif

			Assert.AreEqual ("Welcome", manager.GetString ("String1", new CultureInfo ("en")), "en");
			Assert.AreEqual ("G'day", manager.GetString ("String1", new CultureInfo ("en-AU")), "en-AU");
			Assert.AreEqual ("Willkommen", manager.GetString ("String1", new CultureInfo ("de")), "de");
			Assert.AreEqual ("Willkommen", manager.GetString ("String1", new CultureInfo ("de-DE")), "de-DE");
			Assert.AreEqual ("Bienvenido", manager.GetString ("String1", new CultureInfo ("es")), "es");
			Assert.AreEqual ("Bienvenido", manager.GetString ("String1", new CultureInfo ("es-AR")), "es-AR");
			Assert.AreEqual ("Bienvenido", manager.GetString ("String1", new CultureInfo ("es-ES")), "es-ES");
		}
	}
}