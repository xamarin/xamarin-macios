#if __MACOS__
using NUnit.Framework;
using System;

using AppKit;
using ObjCRuntime;
using Foundation;
using ScriptingBridge;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Xamarin.Mac.Tests {

	public class MySBApp : SBApplication {
		public MySBApp () : base (NSObjectFlag.Empty) { }

		public MySBApp (NSCoder coder) : base (coder) { }

		public MySBApp (NSUrl url) : base (url) { }

		public MySBApp (int pid) : base (pid) { }

		public MySBApp (string ident) : base (ident) { }

		protected MySBApp (NSObjectFlag t) : base (t) { }

		protected internal MySBApp (NativeHandle handle) : base (handle) { }
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SBApplicationTest {

		[Test]
		public void TestGetApplicationFromBundleIdentifier ()
		{
			const string knownBundle = "com.apple.finder";
			const string unknownBundle = "com.unknown.bundle";
#if !NET
			using (var app1 = SBApplication.FromBundleIdentifier (knownBundle))
			using (var app2 = SBApplication.FromBundleIdentifier<MySBApp> (knownBundle))
			using (var app3 = SBApplication.FromBundleIdentifier (unknownBundle))
			using (var app4 = SBApplication.FromBundleIdentifier<MySBApp> (unknownBundle))
#else
			using (var app1 = SBApplication.GetApplication (knownBundle))
			using (var app2 = SBApplication.GetApplication<MySBApp> (knownBundle))
			using (var app3 = SBApplication.GetApplication (unknownBundle))
			using (var app4 = SBApplication.GetApplication<MySBApp> (unknownBundle))
#endif
			{
				Assert.IsNotNull (app1, "SBApplication from known bundle is null");
				Assert.IsNotNull (app2, "MySBApp from known bundle is null");
				Assert.IsNull (app3, "SBApplication from unknown bundle is non-null");
				Assert.IsNull (app4, "MySBApp from unknown bundle is non-null");
			}
		}

		[Test]
		public void TestGetApplicationFromUrl ()
		{
			using (NSUrl knownUrl = new NSUrl ("http://www.xamarin.com"))
#if !NET
			using (var app1 = SBApplication.FromURL (knownUrl))
			using (var app2 = SBApplication.FromURL<MySBApp> (knownUrl))
#else
			using (var app1 = SBApplication.GetApplication (knownUrl))
			using (var app2 = SBApplication.GetApplication<MySBApp> (knownUrl))
#endif
			{
				Assert.IsNotNull (app1, "SBApplication from known URL is null");
				Assert.IsNotNull (app2, "MySBApp from known URL is null");
			}
		}

		[Test]
		public void TestGetApplicationFromPid ()
		{
			int knownPid = System.Diagnostics.Process.GetCurrentProcess ().Id;
			int unknownPid = -1; // valid pid is > 0
#if !NET
			using (var app1 = SBApplication.FromProcessIdentifier (knownPid))
			using (var app2 = SBApplication.FromProcessIdentifier<MySBApp> (knownPid))
			using (var app3 = SBApplication.FromProcessIdentifier (unknownPid))
			using (var app4 = SBApplication.FromProcessIdentifier<MySBApp> (unknownPid))
#else
			using (var app1 = SBApplication.GetApplication (knownPid))
			using (var app2 = SBApplication.GetApplication<MySBApp> (knownPid))
			using (var app3 = SBApplication.GetApplication (unknownPid))
			using (var app4 = SBApplication.GetApplication<MySBApp> (unknownPid))
#endif
			{
				Assert.IsNotNull (app1, "SBApplication from known pid is null");
				Assert.IsNotNull (app2, "MySBApp from known pid is null");
				Assert.IsNotNull (app3, "SBApplication from unknown pid is null");
				Assert.IsNotNull (app4, "MySBApp from unknown pid is null");
			}
		}
	}
}


#endif // __MACOS__
