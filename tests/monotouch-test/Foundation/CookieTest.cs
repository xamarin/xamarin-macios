//
// Unit tests for NSHttpCookie
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Net;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HttpCookieTest {
		
		// https://bugzilla.xamarin.com/show_bug.cgi?id=3603
		[Test]
		public void NSDictionaryCtor ()
		{
			using (var props = new NSMutableDictionary ()) {
#if XAMCORE_2_0
				props.Add (NSHttpCookie.KeyOriginUrl, new NSString ("http://yodawg.com"));
#else
				props.Add (NSHttpCookie.KeyOriginURL, new NSString ("http://yodawg.com"));
#endif
				props.Add (NSHttpCookie.KeyName, new NSString ("iherd"));
				props.Add (NSHttpCookie.KeyValue, new NSString ("ulikecookies"));
				
				// an invalid NSDictionary returns null from Objective-C but that
				// results in an 'empty' instance inside MonoTouch
				Assert.Throws<Exception> (() => new NSHttpCookie (props), "ctor");
	
				props.Add (NSHttpCookie.KeyPath, new NSString ("/"));
				using (var cookie = new NSHttpCookie (props)) {
					Assert.That (cookie.Handle, Is.Not.EqualTo (IntPtr.Zero), "ctor");
					
					Assert.That (cookie.Domain, Is.EqualTo ("yodawg.com"), "Domain");
					Assert.That (cookie.Name, Is.EqualTo ("iherd"), "Name");
					Assert.That (cookie.Value, Is.EqualTo ("ulikecookies"), "Value");
					Assert.That (cookie.Path, Is.EqualTo ("/"), "Path");
				}
			}
		}
		
		[Test]
		public void CookieFromProperties ()
		{
			using (var props = new NSMutableDictionary ()) {
#if XAMCORE_2_0
				props.Add (NSHttpCookie.KeyOriginUrl, new NSString ("http://yodawg.com"));
#else
				props.Add (NSHttpCookie.KeyOriginURL, new NSString ("http://yodawg.com"));
#endif
				props.Add (NSHttpCookie.KeyName, new NSString ("iherd"));
				props.Add (NSHttpCookie.KeyValue, new NSString ("ulikecookies"));
				
				var cookie = NSHttpCookie.CookieFromProperties (props);
				Assert.Null (cookie, "missing path");
	
				props.Add (NSHttpCookie.KeyPath, new NSString ("/"));
				using (cookie = NSHttpCookie.CookieFromProperties (props)) {
					Assert.NotNull (cookie, "w/path");
		
					Assert.That (cookie.Domain, Is.EqualTo ("yodawg.com"), "Domain");
					Assert.That (cookie.Name, Is.EqualTo ("iherd"), "Name");
					Assert.That (cookie.Value, Is.EqualTo ("ulikecookies"), "Value");
					Assert.That (cookie.Path, Is.EqualTo ("/"), "Path");
				}
			}
		}
		
		[Test]
		public void NiceTwoCtor ()
		{
			using (NSHttpCookie cookie = new NSHttpCookie ("iherd", "ulikecookies")) {
				Assert.That (cookie.Name, Is.EqualTo ("iherd"), "Name");
				Assert.That (cookie.Value, Is.EqualTo ("ulikecookies"), "Value");
				Assert.That (cookie.Path, Is.EqualTo ("/"), "Path");
				Assert.That (cookie.Domain, Is.EqualTo ("*"), "Domain");
			}
		}
		
		[Test]
		public void NiceThreeCtor ()
		{
			using (NSHttpCookie cookie = new NSHttpCookie ("iherd", "ulikecookies", "/", null)) {
				Assert.That (cookie.Name, Is.EqualTo ("iherd"), "Name");
				Assert.That (cookie.Value, Is.EqualTo ("ulikecookies"), "Value");
				Assert.That (cookie.Path, Is.EqualTo ("/"), "Path");
				Assert.That (cookie.Domain, Is.EqualTo ("*"), "Domain");
			}
		}

		[Test]
		public void NiceFourCtor ()
		{
			using (NSHttpCookie cookie = new NSHttpCookie ("iherd", "ulikecookies", "/", "yodawg.com")) {
				Assert.That (cookie.Name, Is.EqualTo ("iherd"), "Name");
				Assert.That (cookie.Value, Is.EqualTo ("ulikecookies"), "Value");
				Assert.That (cookie.Path, Is.EqualTo ("/"), "Path");
				Assert.That (cookie.Domain, Is.EqualTo ("yodawg.com"), "Domain");
			}
		}

		[Test]
		public void DotNetInteropMin ()
		{
			// an invalid NSDictionary returns null from Objective-C but that
			// results in an 'empty' instance inside MonoTouch
			using (var cookie = new NSHttpCookie (new Cookie ())) {
				Assert.That (cookie.Handle, Is.EqualTo (IntPtr.Zero), "ctor");
			}
		}

		[Test]
		public void DotNetInteropCommon ()
		{
			var c = new Cookie ("name", "key", "path", "domain");
			using (NSHttpCookie cookie = new NSHttpCookie (c)) {
				Assert.That (cookie.Name, Is.EqualTo ("name"), "Name");
				Assert.That (cookie.Value, Is.EqualTo ("key"), "Value");
				Assert.That (cookie.Path, Is.EqualTo ("path"), "Path");
				Assert.That (cookie.Domain, Is.EqualTo ("domain"), "Domain");
				// defaults
				Assert.Null (cookie.Comment, "Comment");
				Assert.Null (cookie.CommentUrl, "CommentUrl");
				Assert.Null (cookie.ExpiresDate, "ExpiresDate");
				Assert.False (cookie.IsHttpOnly, "IsHttpOnly");
				Assert.False (cookie.IsSecure, "IsSecure");
				Assert.True (cookie.IsSessionOnly, "IsSessionOnly"); // discard
				Assert.That (cookie.PortList.Length, Is.EqualTo (0), "PortList");
				Assert.NotNull (cookie.Properties, "Properties");
				Assert.That (cookie.Version, Is.EqualTo ((nuint) 0), "Version");
			}
		}
		
		[Test]
		public void DotNetInteropMax ()
		{
			var c = new Cookie ("name", "key", "path", "domain");
			c.Comment = "comment";
			c.CommentUri = new Uri ("http://comment.uri");
			c.Discard = true;
			c.Expires = DateTime.Now.AddDays (1);
			c.HttpOnly = false;
			c.Port = "\"80\"";
			c.Secure = true;
			c.Version = 1;
			using (NSHttpCookie cookie = new NSHttpCookie (c)) {
				Assert.That (cookie.Name, Is.EqualTo ("name"), "Name");
				Assert.That (cookie.Value, Is.EqualTo ("key"), "Value");
				Assert.That (cookie.Path, Is.EqualTo ("path"), "Path");
				Assert.That (cookie.Domain, Is.EqualTo ("domain"), "Domain");
				// custom values
				Assert.That (cookie.Comment, Is.EqualTo ("comment"), "Comment");
				Assert.That (cookie.CommentUrl.ToString (), Is.EqualTo ("http://comment.uri/"), "CommentUrl");
				Assert.Null (cookie.ExpiresDate, "ExpiresDate"); // session-only always returns null
				Assert.False (cookie.IsHttpOnly, "IsHttpOnly");
				Assert.True (cookie.IsSecure, "IsSecure");
				Assert.True (cookie.IsSessionOnly, "IsSessionOnly");
				Assert.That ((int) cookie.PortList [0], Is.EqualTo (80), "PortList");
				Assert.NotNull (cookie.Properties, "Properties");
				Assert.That (cookie.Version, Is.EqualTo ((nuint) 1), "Version");
			}
		}

		[Test]
		public void DotNetInterop_NonSession ()
		{
			var c = new Cookie ("name", "key", "path", "domain");
			c.Comment = "comment";
			c.CommentUri = new Uri ("http://comment.uri");
			c.Discard = false;
			c.Expires = DateTime.Now.AddDays (1);
			c.HttpOnly = false;
			c.Port = "\"80\"";
			c.Secure = true;
			c.Version = 1;
			using (NSHttpCookie cookie = new NSHttpCookie (c)) {
				Assert.That (cookie.Name, Is.EqualTo ("name"), "Name");
				Assert.That (cookie.Value, Is.EqualTo ("key"), "Value");
				Assert.That (cookie.Path, Is.EqualTo ("path"), "Path");
				Assert.That (cookie.Domain, Is.EqualTo ("domain"), "Domain");
				// custom values
				Assert.That (cookie.Comment, Is.EqualTo ("comment"), "Comment");
				Assert.That (cookie.CommentUrl.ToString (), Is.EqualTo ("http://comment.uri/"), "CommentUrl");
				DateTime dt1 = (DateTime) cookie.ExpiresDate; // does not include milliseconds (so we do a string match)
				DateTime dt2 = c.Expires.ToUniversalTime ();
				// There seems to be rounding somewhere, which means that some dates
				// can be rounded up. This means that these two dates might be a second off.
				// So assert that they're no more than a second apart.
				Assert.That (Math.Abs ((dt1 - dt2).TotalSeconds), Is.LessThan (1.0), $"ExpiresDate");
				Assert.False (cookie.IsHttpOnly, "IsHttpOnly");
				Assert.True (cookie.IsSecure, "IsSecure");
				Assert.IsFalse (cookie.IsSessionOnly, "IsSessionOnly");
				Assert.That ((int) cookie.PortList [0], Is.EqualTo (80), "PortList");
				Assert.NotNull (cookie.Properties, "Properties");
				Assert.That (cookie.Version, Is.EqualTo ((nuint) 1), "Version");
			}
		}

		[Test]
		public void PortList_4990 ()
		{
			Cookie c = new Cookie ();
			c.Comment = String.Empty;
			c.CommentUri = null;
			c.Discard = true;
			c.Domain = ".collectedit.com";
			c.Expired = false;
			c.Expires = DateTime.Now.AddMonths (1);
			c.HttpOnly = true;
			c.Name = ".ASPXAUTH";
			c.Path = "/";
			c.Port = String.Empty;
			c.Secure = false;
			c.Value = "abc";
			using (NSHttpCookie c1 = new NSHttpCookie (c))
			using (NSHttpCookie c2 = new NSHttpCookie (".ASPXAUTH", "abc", "/", ".collectedit.com")) {
				Assert.That (c1.Name, Is.EqualTo (c2.Name), "Name");
				Assert.That (c1.Value, Is.EqualTo (c2.Value), "Value");
				Assert.That (c1.Path, Is.EqualTo (c2.Path), "Path");
				Assert.That (c1.Domain, Is.EqualTo (c2.Domain), "Domain");
				Assert.That (c1.Comment, Is.EqualTo (c2.Comment), "Comment");
				Assert.That (c1.CommentUrl, Is.EqualTo (c2.CommentUrl), "CommentUrl");
				Assert.That (c1.ExpiresDate, Is.EqualTo (c2.ExpiresDate), "ExpiresDate");
				Assert.That (c1.IsHttpOnly, Is.EqualTo (c2.IsHttpOnly), "IsHttpOnly");
				Assert.That (c1.IsSecure, Is.EqualTo (c2.IsSecure), "IsSecure");
				Assert.That (c1.IsSessionOnly, Is.EqualTo (c2.IsSessionOnly), "IsSessionOnly");
				Assert.That (c1.PortList, Is.EqualTo (c2.PortList), "PortList");
				Assert.That (c1.Version, Is.EqualTo (c2.Version), "Version");
			}
		}
	}
}
