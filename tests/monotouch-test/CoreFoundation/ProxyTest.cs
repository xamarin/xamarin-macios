//
// Unit tests for CFProxy[Settings]
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Threading;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ProxyTest {
		
		[Test]
		public void Fields ()
		{
			// documented but symbols are missing
			// this test will fail if Apple decide to include them in the future
			IntPtr lib = Dlfcn.dlopen (Constants.CoreFoundationLibrary, 0);
			try {
				// http://developer.apple.com/library/ios/documentation/CoreFoundation/Reference/CFProxySupport/Reference/reference.html#//apple_ref/doc/c_ref/kCFProxyAutoConfigurationHTTPResponseKey
				Assert.That (Dlfcn.dlsym (lib, "kCFProxyAutoConfigurationHTTPResponseKey"), Is.EqualTo (IntPtr.Zero), "kCFProxyAutoConfigurationHTTPResponseKey");
				// http://developer.apple.com/library/ios/documentation/CoreFoundation/Reference/CFProxySupport/Reference/reference.html#//apple_ref/doc/c_ref/kCFNetworkProxiesProxyAutoConfigJavaScript
				Assert.That (Dlfcn.dlsym (lib, "kCFNetworkProxiesProxyAutoConfigJavaScript"), Is.EqualTo (IntPtr.Zero), "kCFNetworkProxiesProxyAutoConfigJavaScript");
			}
			finally {
				Dlfcn.dlclose (lib);
			}
		}
		
		[Test]
		public void TestPACParsing ()
		{
			var cbCalled = new AutoResetEvent (false);
			CFProxy proxy;
			// get the path for the pac file, try to parse it and ensure that 
			// our cb was called
#if MONOMAC
 			string uncompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/example.pac");
#else
 			string uncompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");
#endif
			NSObject.InvokeInBackground (() => {
				var runLoop = NSRunLoop.Current;
				var cfRunLoop = NSRunLoop.Main.GetCFRunLoop ();

				using (var path = new NSString (uncompressedFilePath))
				using (var url = new NSUrl ("http://docs.xamarin.com")) {
					var runLoopSource = CFNetwork.ExecuteProxyAutoConfigurationScript (path, url, (client, proxyList, error) => {
						Assert.AreEqual (1, proxyList.Length);
						Console.WriteLine ("GO IT!!");
					}, new CFStreamClientContext ());
					Assert.NotNull (runLoopSource, "runLoopSource");
					cfRunLoop.AddSource (runLoopSource, CFRunLoop.ModeCommon);
				}
				// This is our "killswitch" to fail the test if we take too long
				NSTimer timer = NSTimer.CreateTimer (30, t => {
					Console.WriteLine ("Test Failed");
					//throw new InvalidOperationException ();
				});

				try {
					runLoop.AddTimer (timer, NSRunLoopMode.Common);
					runLoop.Run ();
					Console.WriteLine ("DONE");
				} catch (Exception e) {
					Console.WriteLine (e);
				}
			});
		}
	}
}
