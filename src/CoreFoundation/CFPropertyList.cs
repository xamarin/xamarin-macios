//
// CFPropertyList.cs: partial internal binding for CFPropertyList
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin, Inc.

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreFoundation
{
	// This is currently incomplete and thus marked internal;
	// it is only used by CFPreferences, and I'm not necessarily
	// sure we'd want to expose it? -abock
	internal class CFPropertyList : INativeObject, IDisposable
	{
		static nint CFDataTypeID = CFData.GetTypeID ();
		static nint CFStringTypeID = CFString.GetTypeID ();
		static nint CFArrayTypeID = CFArray.GetTypeID ();
		static nint CFDictionaryTypeID = CFDictionary.GetTypeID ();
		static nint CFBooleanTypeID = CFBoolean.GetTypeID ();

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern nint CFDateGetTypeID ();

		static nint CFDateTypeID = CFDateGetTypeID ();

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern nint CFNumberGetTypeID ();

		static nint CFNumberTypeID = CFNumberGetTypeID ();

		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public CFPropertyList (IntPtr handle)
		{
			this.handle = handle;
		}

		~CFPropertyList ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		public object Value {
			get {
				if (handle == IntPtr.Zero) {
					return null;
				}

				var typeid = CFType.GetTypeID (handle);

				if (typeid == CFDataTypeID) {
					return new NSData (handle);
				} else if (typeid == CFStringTypeID) {
					return new NSString (handle);
				} else if (typeid == CFArrayTypeID) {
					return new NSArray (handle);
				} else if (typeid == CFDictionaryTypeID) {
					return new NSDictionary (handle);
				} else if (typeid == CFDateTypeID) {
					return new NSDate (handle);
				} else if (typeid == CFBooleanTypeID) {
					return (bool)new NSNumber (handle);
				} else if (typeid == CFNumberTypeID) {
					return new NSNumber (handle);
				}

				return null;
			}
		}
	}
}
