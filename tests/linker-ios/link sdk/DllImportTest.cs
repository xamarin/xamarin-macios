//
// Unit tests for [DllImport]
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace LinkSdk {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DllImportTest {

		class NestedFirstLevel {

			public class NestedSecondLevel {

				[DllImport ("__Internal")]
				public extern static string xamarin_get_locale_country_code ();
			}
		}

		[Test]
		public void ScanForStrip_17327 ()
		{
			// note: must be tested on a release (strip'ed) build
			Assert.NotNull (NestedFirstLevel.NestedSecondLevel.xamarin_get_locale_country_code ());
		}

		[Test]
		public void Sqlite3 ()
		{
			var lib = Dlfcn.dlopen ("/usr/lib/libsqlite3.dylib", 0);
			Assert.That (lib, Is.Not.EqualTo (IntPtr.Zero), "/usr/lib/libsqlite3.dylib");
			try {
				Assert.That (Dlfcn.dlsym (lib, "sqlite3_bind_int"), Is.Not.EqualTo (IntPtr.Zero), "sqlite3_bind_int");
				// iOS does not have some symbols defined - if that change/fail in the future we'll need to update Mono.Data.Sqlite
				// note: Apple devices (at least iOS and AppleTV) running 10.x have a more recent version of libsqlite which includes _key and _rekey
				var version = TestRuntime.CheckXcodeVersion (8, 0);
				if (version && (Runtime.Arch == Arch.DEVICE)) {
					Assert.That (Dlfcn.dlsym (lib, "sqlite3_key"), Is.Not.EqualTo (IntPtr.Zero), "sqlite3_key");
					Assert.That (Dlfcn.dlsym (lib, "sqlite3_rekey"), Is.Not.EqualTo (IntPtr.Zero), "sqlite3_rekey");
				} else {
					Assert.That (Dlfcn.dlsym (lib, "sqlite3_key"), Is.EqualTo (IntPtr.Zero), "sqlite3_key");
					Assert.That (Dlfcn.dlsym (lib, "sqlite3_rekey"), Is.EqualTo (IntPtr.Zero), "sqlite3_rekey");
				}
				Assert.That (Dlfcn.dlsym (lib, "sqlite3_column_database_name"), Is.EqualTo (IntPtr.Zero), "sqlite3_column_database_name");
				Assert.That (Dlfcn.dlsym (lib, "sqlite3_column_database_name16"), Is.EqualTo (IntPtr.Zero), "sqlite3_column_database_name16");
				Assert.That (Dlfcn.dlsym (lib, "sqlite3_column_origin_name"), Is.EqualTo (IntPtr.Zero), "sqlite3_column_origin_name");
				Assert.That (Dlfcn.dlsym (lib, "sqlite3_column_origin_name16"), Is.EqualTo (IntPtr.Zero), "sqlite3_column_origin_name16");
				Assert.That (Dlfcn.dlsym (lib, "sqlite3_column_table_name"), Is.EqualTo (IntPtr.Zero), "sqlite3_column_table_name");
				Assert.That (Dlfcn.dlsym (lib, "sqlite3_column_table_name16"), Is.EqualTo (IntPtr.Zero), "sqlite3_column_table_name16");
			}
			finally {
				Dlfcn.dlclose (lib);
			}
		}

		[Test]
		public void LackOfCapget ()
		{
			// OSX/iOS libc (libSystem.dylib) does not have a capget - which breaks dlsym=false (required for tvOS)
			// iOS (tvOS/watchOS) does not support Process to run ping either so it ends up with a InvalidOperationException
			// which is now "optimized" to reduce code size (and remove DllImport) until we implement ping (see: #964)
			var p = new Ping ();
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (delegate { p.Send ("localhost"); });
#else
			Assert.Throws<InvalidOperationException> (delegate { p.Send ("localhost"); });
#endif
		}
	}
}