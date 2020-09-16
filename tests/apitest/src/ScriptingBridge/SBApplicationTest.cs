using NUnit.Framework;
using System;

using AppKit;
using ObjCRuntime;
using Foundation;
using ScriptingBridge;

namespace Xamarin.Mac.Tests {

	public class MySbApp : SBApplication {
		public MySbApp () : base (NSObjectFlag.Empty) { }

		public MySbApp (NSCoder coder) : base (coder) { }

		public MySbApp (NSUrl url) : base (url) { }

		public MySbApp (int pid) : base (pid) { }

		public MySbApp (string ident) : base (ident) { }

		protected MySbApp (NSObjectFlag t) : base (t) { }

		protected internal MySbApp (IntPtr handle) : base (handle) { }
	}

	[TestFixture]
	public class SBApplicationTest {

		[TestFixture]
		public class GetSBApplicationTest {

			[Test]
			public void TestGetApplicationFromBundleIdentifier ()
			{
				const string knownBundle = "com.apple.finder";
				const string unknownBundle = "com.unknown.bundle";
#if !XAMCORE_4_0
				var app2 = SBApplication.FromBundleIdentifier<MySbApp> (knownBundle);
				Assert.IsNotNull (app2);
				var app1 = SBApplication.FromBundleIdentifier (unknownBundle);
				Assert.IsNotNull (app1);
				var app4 = SBApplication.FromBundleIdentifier<MySbApp> (unknownBundle);

				var app3 = SBApplication.FromBundleIdentifier (unknownBundle);
				Assert.IsNull (app3);
				Assert.IsNull (app4);
#else
				var app1 = SBApplication.GetApplicationFromBundleIdentifier (knownBundle);
				Assert.IsNotNull (app1);
				var app2 = SBApplication.GetApplicationFromBundleIdentifier<MySbApp> (knownBundle);
				Assert.IsNotNull (app2);
				var app3 = SBApplication.GetApplicationFromBundleIdentifier (unknownBundle);
				var app4 = SBApplication.GetApplicationFromBundleIdentifier<MySbApp> (unknownBundle);
				Assert.IsNull (app3);
				Assert.IsNull (app4);
#endif
			}

			[Test]
			public void TestGetApplicationFromUrl ()
			{
				NSUrl knownUrl = new NSUrl("http://www.xamarin.com");
				NSUrl unknownUrl = new NSUrl ("thisisnotaurl");
#if !XAMCORE_4_0
				var app1 = SBApplication.FromURL (knownUrl);
				Assert.IsNotNull (app1);
				var app2 = SBApplication.FromURL<MySbApp> (knownUrl);
				Assert.IsNotNull (app2);
				var app3 = SBApplication.FromURL (unknownUrl);
				var app4 = SBApplication.FromURL<MySbApp> (unknownUrl);
				Assert.IsNull (app3);
				Assert.IsNull (app4);
#else
				var app1 = SBApplication.GetApplicationFromUrl (knownUrl);
				Assert.IsNotNull (app1);
				var app2 = SBApplication.GetApplicationFromUrl<MySbApp> (knownUrl);
				Assert.IsNotNull (app2);
				var app3 = SBApplication.GetApplicationFromUrl (unknownUrl);
				var app4 = SBApplication.GetApplicationFromUrl<MySbApp> (unknownUrl);
				Assert.IsNull (app3);
				Assert.IsNull (app4);
#endif
			}

			[Test]
			public void TestGetApplicationFromPid ()
			{
				int knownPid = 1;
				int unknownPid = -1; // valid pid is > 0?
#if !XAMCORE_4_0
				var app1 = SBApplication.FromProcessIdentifier (knownPid);
				Assert.IsNotNull (app1);
				var app2 = SBApplication.FromProcessIdentifier<MySbApp> (knownPid);
				Assert.IsNotNull (app2);
				var app3 = SBApplication.FromProcessIdentifier (unknownPid);
				var app4 = SBApplication.FromProcessIdentifier<MySbApp> (unknownPid);
				Assert.IsNull (app3);
				Assert.IsNull (app4);
#else
				var app1 = SBApplication.GetApplicationFromProcessIdentifier (knownPid);
				Assert.IsNotNull (app1);
				var app2 = SBApplication.GetApplicationFromProcessIdentifier<MySbApp> (knownPid);
				Assert.IsNotNull (app2);
				var app3 = SBApplication.GetApplicationFromProcessIdentifier (unknownPid);
				var app4 = SBApplication.GetApplicationFromProcessIdentifier<MySbApp> (unknownPid);
				Assert.IsNull (app3);
				Assert.IsNull (app4);
#endif
			}
		}
	}
}


