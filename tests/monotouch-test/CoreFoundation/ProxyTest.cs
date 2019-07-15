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
		public void TestPACParsingScript ()
		{
			// get the path for the pac file, try to parse it and ensure that 
			// our cb was called
#if MONOMAC
 			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/example.pac");
#else
			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");
#endif
			var script = File.ReadAllText (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");
			var proxies = CFNetwork.ExecuteProxyAutoConfigurationScript (script, targetUri);
			Assert.AreEqual (1, proxies.Length, "Length");
			// assert the data of the proxy, although we are really testing the js used
			Assert.AreEqual (8080, proxies [0].Port, "Port");
		}

		[Test]
		public void TestPACParsingUrl ()
		{
#if MONOMAC
 			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/example.pac");
#else
			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");
#endif
			var pacUri = new Uri (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");
			var proxies = CFNetwork.ExecuteProxyAutoConfigurationUrl (pacUri, targetUri);
			Assert.AreEqual (1, proxies.Length, "Length");
			// assert the data of the proxy, although we are really testing the js used
			Assert.AreEqual (8080, proxies [0].Port, "Port");
		}
	}
}
