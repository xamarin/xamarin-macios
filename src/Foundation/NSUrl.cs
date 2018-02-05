// Copyright 2011-2014 Xamarin Inc
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
using System;

namespace Foundation {

#if XAMCORE_2_0
		// Equals(Object) and GetHashCode are provided by NSObject
		// NSObject.GetHashCode calls GetNativeHash, which means it matches Equals (NSUrl)' behavior (which also calls the native implementation), so there's no need to override it.
		// NSObject.Equals calls the native isEqual: implementation, so that's fine as well, and no need to override.
#pragma warning disable 660 // `Foundation.NSUrl' defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable 661 // `Foundation.NSUrl' defines operator == or operator != but does not override Object.GetHashCode()
#endif
	public partial class NSUrl : IEquatable<NSUrl> {

		public NSUrl (string path, string relativeToUrl)
			: this (path, new NSUrl (relativeToUrl))
		{
		}

#if !XAMCORE_2_0
		public override bool Equals (object t)
		{
			if (t == null)
				return false;
			
			if (t is NSUrl){
				return IsEqual ((NSUrl) t);
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return (int) GetNativeHash ();
		}
#endif
		// but NSUrl has it's own isEqual: selector, which we re-expose in a more .NET-ish way
		public bool Equals (NSUrl url)
		{
			if (url == null)
				return false;
			// we can only ask `isEqual:` to test equality if both objects are direct bindings
			return IsDirectBinding && url.IsDirectBinding ? IsEqual (url) : Equals ((object) url);
		}

		// Converts from an NSURL to a System.Uri
		public static implicit operator Uri (NSUrl url)
		{
			if (url.RelativePath == url.Path)
				return new Uri (url.AbsoluteString, UriKind.Absolute);
			else
				return new Uri (url.RelativePath, UriKind.Relative);
		}

		public static implicit operator NSUrl (Uri uri)
		{
			if (uri.IsAbsoluteUri)
				return new NSUrl (uri.AbsoluteUri);
			else
				return new NSUrl (uri.PathAndQuery);
		}

		public static NSUrl FromFilename (string url)
		{
			return new NSUrl (url, false);
		}
		
		public NSUrl MakeRelative (string url)
		{
			return _FromStringRelative (url, this);
		}

		public override string ToString ()
		{
			return AbsoluteString ?? base.ToString ();
		}

#if !XAMCORE_2_0
		[Obsolete ("Use the overload with an 'NSString' constant.")]
		public bool TryGetResource (string key, out NSObject value, out NSError error)
		{
			using (var nsKey = new NSString (key))
				return GetResourceValue (out value, nsKey, out error);
		}

		[Obsolete ("Use the overload with an 'NSString' constant.")]
		public bool TryGetResource (string key, out NSObject value)
		{
			NSError error;
			using (var nsKey = new NSString (key))
				return GetResourceValue (out value, nsKey, out error);
		}

		[Obsolete ("Use the overload with an 'NSString' constant.")]
		public bool SetResource (string key, NSObject value, out NSError error)
		{
			using (var nsKey = new NSString (key))
				return SetResourceValue (value, nsKey, out error);
		}

		[Obsolete ("Use the overload with an 'NSString' constant.")]
		public bool SetResource (string key, NSObject value)
		{
			NSError error;
			using (var nsKey = new NSString (key))
				return SetResourceValue (value, nsKey, out error);
		}
#endif
		public bool TryGetResource (NSString nsUrlResourceKey, out NSObject value, out NSError error)
		{
			return GetResourceValue (out value, nsUrlResourceKey, out error);
		}

		public bool TryGetResource (NSString nsUrlResourceKey, out NSObject value)
		{
			NSError error;
			return GetResourceValue (out value, nsUrlResourceKey, out error);
		}

		public bool SetResource (NSString nsUrlResourceKey, NSObject value, out NSError error)
		{
			return SetResourceValue (value, nsUrlResourceKey, out error);
		}

		public bool SetResource (NSString nsUrlResourceKey, NSObject value)
		{
			NSError error;
			return SetResourceValue (value, nsUrlResourceKey, out error);
		}

		public int Port {
			get {
				return (int) (this.PortNumber ?? -1);
			}
		}

		public static bool operator == (NSUrl x, NSUrl y)
		{
			if ((object) x == (object) y) // If both are null, or both are same instance, return true.
				return true;

			if (((object) x == null) || ((object) y == null)) // If one is null, but not both, return false.
				return false;

			return x.Equals (y);
		}

		public static bool operator != (NSUrl x, NSUrl y)
		{
			return !(x == y);
		}
	}

#if !XAMCORE_3_0
	public static partial class NSUrl_PromisedItems {

		[Obsolete ("Use overload with an 'out NSObject value' parameter.")]
		public static bool GetPromisedItemResourceValue (NSUrl This, NSObject value, NSString key, out NSError error)
		{
			return This.GetPromisedItemResourceValue (out value, key, out error);
		}
	}
#endif
}
