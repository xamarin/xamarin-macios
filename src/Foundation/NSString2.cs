//
// NSString2.cs: Support code added after the generator has run for NSString
// 
// Copyright 2009-2010, Novell, Inc.
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
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace Foundation {

	public partial class NSString : IComparable<NSString> {
		const string selDataUsingEncodingAllow = "dataUsingEncoding:allowLossyConversion:";

#if MONOMAC
		static IntPtr selDataUsingEncodingAllowHandle = Selector.GetHandle (selDataUsingEncodingAllow);
#endif

		public NSData Encode (NSStringEncoding enc, bool allowLossyConversion = false)
		{
#if MONOMAC
			return new NSData (Messaging.IntPtr_objc_msgSend_IntPtr_bool (Handle, selDataUsingEncodingAllowHandle, (IntPtr) (int) enc, allowLossyConversion));
#else
			return new NSData (Messaging.IntPtr_objc_msgSend_IntPtr_bool (Handle, Selector.GetHandle (selDataUsingEncodingAllow), (IntPtr) (int) enc, allowLossyConversion));
#endif
		}

		public static NSString FromData (NSData data, NSStringEncoding encoding)
		{
			if (data == null) 
				throw new ArgumentNullException ("data");
			NSString ret = null;
			try { 
				ret = new NSString (data, encoding);
			} catch (Exception) {
			
			}
			return ret;
		}

		public int CompareTo (NSString other)
		{
			return (int)Compare (other);
		}

		// [Export ("initWithContentsOfURL:encoding:error")]
		// IntPtr Constructor (NSUrl url, NSStringEncoding encoding, out NSError error);
		// 
		// [Export ("initWithContentsOfURL:usedEncoding:error:")]
		// IntPtr Constructor (NSUrl url, out NSStringEncoding encoding, out NSError error);
		// 
		// [Export ("initWithBytes:length:encoding")]
		// IntPtr Constructor (IntPtr bytes, int length, NSStringEncoding encoding);
		
		public char this [nint idx] {
			get {
				return _characterAtIndex (idx);
			}
		}

#if !XAMCORE_4_0 && !MONOMAC
		[Obsolete ("Use 'GetLocalizedUserNotificationString' that takes 'NSString' to preserve localization.")]
		public static string GetLocalizedUserNotificationString (string key, params NSObject[] arguments) {
			return GetLocalizedUserNotificationString ((NSString) key, arguments);
		}
#endif
	}
}
