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
#if !__WATCHOS__ && !MONOMAC
		[Test]
		public void TestPACParsingScript ()
		{
			// get the path for the pac file, try to parse it and ensure that 
			// our cb was called
			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");
			NSError error = null;
			var script = File.ReadAllText (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");
			var proxies = CFNetwork.ExecuteProxyAutoConfigurationScript (script, targetUri, out error);
			Assert.IsNull (error, "Null error");
			Assert.AreEqual (1, proxies.Length, "Length");
			// assert the data of the proxy, although we are really testing the js used
			Assert.AreEqual (8080, proxies [0].Port, "Port");
		}

		[Test]
		public void TestPACParsingScriptRunLoop ()
		{
			// get the path for the pac file, try to parse it and ensure that 
			// our cb was called
			CFProxy [] proxies = null;
			NSError error = null;
			NSObject cbClient = null;

			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");

			var script = File.ReadAllText (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");
			var runLoop = CFRunLoop.Current;

			// get source, execute run loop in a diff thread

			CFNetwork.CFProxyAutoConfigurationResultCallback cb = delegate (NSObject c, CFProxy [] l, NSError e) {
				proxies = l;
				error = e;
				cbClient = c;
				runLoop.Stop ();
			};
			var source = CFNetwork.ExecuteProxyAutoConfigurationScript (script, targetUri, cb, new CFStreamClientContext ());
			using (var mode = new NSString ("Xamarin.iOS.Proxy")) {
				runLoop.AddSource (source, mode);
				// block until cb is called
				runLoop.RunInMode (mode, double.MaxValue, false);
				runLoop.RemoveSource (source, mode);
				Assert.IsNotNull (cbClient, "Null client");
				Assert.IsNull (error, "Null error");
				Assert.IsNotNull (proxies, "Not null proxies");
				Assert.AreEqual (1, proxies.Length, "Length");
				// assert the data of the proxy, although we are really testing the js used
				Assert.AreEqual (8080, proxies [0].Port, "Port");
			}
		}

		[Test]
		public void TestPACParsingAsync ()
		{
			CFProxy [] proxies = null;
			NSError error = null;
			NSObject cbClient = null;
			bool done = false;
			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");

			var script = File.ReadAllText (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");
			
			Exception ex;
			bool foundProxies;
			// similar to the other tests, but we want to ensure that the async/await API works
			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () => {
				try {
					CancellationTokenSource cancelSource = new CancellationTokenSource ();
					CancellationToken cancelToken = cancelSource.Token;
					var result = await CFNetwork.ExecuteProxyAutoConfigurationScriptAsync (script, targetUri, cancelToken);
					proxies = result.proxies;
					error = result.error;
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);
			Assert.IsNull (cbClient, "Null client");
			Assert.IsNull (error, "Null error");
			Assert.IsNotNull (proxies, "Not null proxies");
			Assert.AreEqual (1, proxies.Length, "Length");
			// assert the data of the proxy, although we are really testing the js used
			Assert.AreEqual (8080, proxies [0].Port, "Port");
		}

		[Test]
		public void TestPACParsingUrl ()
		{
			NSError error;
			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");
			var pacUri = new Uri (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");
			var proxies = CFNetwork.ExecuteProxyAutoConfigurationUrl (pacUri, targetUri, out error);
			Assert.IsNull (error, "Null error");
			Assert.AreEqual (1, proxies.Length, "Length");
			// assert the data of the proxy, although we are really testing the js used
			Assert.AreEqual (8080, proxies [0].Port, "Port");
		}

		[Test]
		public void TestPACParsingUrlRunLoop ()
		{
			// get the path for the pac file, try to parse it and ensure that 
			// our cb was called
			CFProxy [] proxies = null;
			NSError error = null;
			NSObject cbClient = null;

			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");
			var pacUri = new Uri (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");
			var runLoop = CFRunLoop.Current;

			// get source, execute run loop in a diff thread

			CFNetwork.CFProxyAutoConfigurationResultCallback cb = delegate (NSObject c, CFProxy [] l, NSError e) {
				proxies = l;
				error = e;
				cbClient = c;
				runLoop.Stop ();
			};
			var source = CFNetwork.ExecuteProxyAutoConfigurationUrl (pacUri, targetUri, cb, new CFStreamClientContext ());
			using (var mode = new NSString ("Xamarin.iOS.Proxy")) {
				runLoop.AddSource (source, mode);
				// block until cb is called
				runLoop.RunInMode (mode, double.MaxValue, false);
				runLoop.RemoveSource (source, mode);
				Assert.IsNotNull (cbClient, "Null client");
				Assert.IsNull (error, "Null error");
				Assert.IsNotNull (proxies, "Not null proxies");
				Assert.AreEqual (1, proxies.Length, "Length");
				// assert the data of the proxy, although we are really testing the js used
				Assert.AreEqual (8080, proxies [0].Port, "Port");
			}
		}

		[Test]
		public void TestPACParsingUrlAsync ()
		{
			CFProxy [] proxies = null;
			NSError error = null;
			NSObject cbClient = null;
			bool done = false;
			string pacPath = Path.Combine (NSBundle.MainBundle.BundlePath, "example.pac");
			var pacUri = new Uri (pacPath);
			var targetUri = new Uri ("http://docs.xamarin.com");

			Exception ex;
			bool foundProxies;
			// similar to the other tests, but we want to ensure that the async/await API works
			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () => {
				try {
					CancellationTokenSource cancelSource = new CancellationTokenSource ();
					CancellationToken cancelToken = cancelSource.Token;
					var result = await CFNetwork.ExecuteProxyAutoConfigurationUrlAsync (pacUri, targetUri, cancelToken);
					proxies = result.proxies;
					error = result.error;
				} catch (Exception e) {
					ex = e;
				} finally {
					done = true;
				}
			}, () => done);
			Assert.IsNull (cbClient, "Null client");
			Assert.IsNull (error, "Null error");
			Assert.IsNotNull (proxies, "Not null proxies");
			Assert.AreEqual (1, proxies.Length, "Length");
			// assert the data of the proxy, although we are really testing the js used
			Assert.AreEqual (8080, proxies [0].Port, "Port");
		}
#endif
	}
}
