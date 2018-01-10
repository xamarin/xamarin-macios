//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright 2011, 2012 Xamarin Inc
using ObjCRuntime;
using System;
using System.Net;

namespace Foundation {
	public partial class NSHttpCookie {

#if !XAMCORE_2_0
		// sadly they were not readonly
		public static NSString KeyName;
		public static NSString KeyValue;
		public static NSString KeyOriginURL;
		public static NSString KeyVersion;
		public static NSString KeyDomain;
		public static NSString KeyPath;
		public static NSString KeySecure;
		public static NSString KeyExpires;
		public static NSString KeyComment;
		public static NSString KeyCommentURL;
		public static NSString KeyDiscard;
		public static NSString KeyMaximumAge;
		public static NSString KeyPort;

		static NSHttpCookie ()
		{
			var handle = Libraries.Foundation.Handle;
			if (handle == IntPtr.Zero)
				return;

			KeyName = Dlfcn.GetStringConstant (handle, "NSHTTPCookieName");
			KeyValue = Dlfcn.GetStringConstant (handle, "NSHTTPCookieValue");
			KeyOriginURL = Dlfcn.GetStringConstant (handle, "NSHTTPCookieOriginURL");
			KeyVersion = Dlfcn.GetStringConstant (handle, "NSHTTPCookieVersion");
			KeyDomain = Dlfcn.GetStringConstant (handle, "NSHTTPCookieDomain");
			KeyPath = Dlfcn.GetStringConstant (handle, "NSHTTPCookiePath");
			KeySecure = Dlfcn.GetStringConstant (handle, "NSHTTPCookieSecure");
			KeyExpires = Dlfcn.GetStringConstant (handle, "NSHTTPCookieExpires");
			KeyComment = Dlfcn.GetStringConstant (handle, "NSHTTPCookieComment");
			KeyCommentURL = Dlfcn.GetStringConstant (handle, "NSHTTPCookieCommentURL");
			KeyDiscard = Dlfcn.GetStringConstant (handle, "NSHTTPCookieDiscard");
			KeyMaximumAge = Dlfcn.GetStringConstant (handle, "NSHTTPCookieMaximumAge");
			KeyPort = Dlfcn.GetStringConstant (handle, "NSHTTPCookiePort");
		}
#endif
		
		// same order as System.Net.Cookie
		// http://msdn.microsoft.com/en-us/library/a18ka3h2.aspx
		public NSHttpCookie (string name, string value) : this (name, value, null, null)
		{
			CreateCookie (name, value, null, null, null, null, null, null, null, null, null, null);
		}
		
		public NSHttpCookie (string name, string value, string path) : this (name, value, path, null)
		{
			CreateCookie (name, value, path, null, null, null, null, null, null, null, null, null);
		}

		public NSHttpCookie (string name, string value, string path, string domain)
		{
			CreateCookie (name, value, path, domain, null, null, null, null, null, null, null, null);
		}
		
		// FIXME: should we expose more complex/long ctor or point people to use a Cookie ?

		public NSHttpCookie (Cookie cookie)
		{
			if (cookie == null)
				throw new ArgumentNullException ("cookie");
			
			string commentUrl = cookie.CommentUri != null ? cookie.CommentUri.ToString () : null;
			bool? discard = null;
			if (cookie.Discard)
				discard = true;
			CreateCookie (cookie.Name, cookie.Value, cookie.Path, cookie.Domain, cookie.Comment, commentUrl, discard, cookie.Expires, null, cookie.Port, cookie.Secure, cookie.Version);
		}
		
		void CreateCookie (string name, string value, string path, string domain, string comment, string commentUrl, bool? discard, DateTime? expires, int? maximumAge, string ports, bool? secure, int? version)
		{
			// mandatory checks or defaults
			if (name == null)
				throw new ArgumentNullException ("name");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (String.IsNullOrEmpty (path))
				path = "/"; // default in .net
			if (String.IsNullOrEmpty (domain))
				domain = "*";

			using (var properties = new NSMutableDictionary ()) {
				// mandatory to create the cookie
				properties.Add (NSHttpCookie.KeyName, new NSString (name));
				properties.Add (NSHttpCookie.KeyValue, new NSString (value));
				properties.Add (NSHttpCookie.KeyPath, new NSString (path));
				properties.Add (NSHttpCookie.KeyDomain, new NSString (domain));
				
				// optional to create the cookie
				if (!String.IsNullOrEmpty (comment))
					properties.Add (NSHttpCookie.KeyComment, new NSString (comment));
				if (!String.IsNullOrEmpty (commentUrl))
#if XAMCORE_2_0
					properties.Add (NSHttpCookie.KeyCommentUrl, new NSString (commentUrl));
#else
					properties.Add (NSHttpCookie.KeyCommentURL, new NSString (commentUrl));
#endif
				if (discard.HasValue)
					properties.Add (NSHttpCookie.KeyDiscard, new NSString (discard.Value ? "TRUE" : "FALSE"));
				if (expires.HasValue && expires.Value != DateTime.MinValue)
					properties.Add (NSHttpCookie.KeyExpires, (NSDate) expires.Value);
				if (maximumAge.HasValue)
					properties.Add (NSHttpCookie.KeyMaximumAge, new NSString (maximumAge.Value.ToString ()));
				if (!String.IsNullOrEmpty (ports))
					properties.Add (NSHttpCookie.KeyPort, new NSString (ports.Replace ("\"", String.Empty)));
				// any value means secure is true
				if (secure.HasValue && secure.Value)
					properties.Add (NSHttpCookie.KeySecure, new NSString ("1"));
				if (version.HasValue)
					properties.Add (NSHttpCookie.KeyVersion, new NSString (version.Value.ToString ()));
				
				if (IsDirectBinding) {
					Handle = Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("initWithProperties:"), properties.Handle);
				} else {
					Handle = Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.GetHandle ("initWithProperties:"), properties.Handle);
				}
			}
		}
	}
}