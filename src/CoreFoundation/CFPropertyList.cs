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
	public class CFPropertyList : INativeObject, IDisposable
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

		public CFPropertyList (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public CFPropertyList (IntPtr handle) : this (handle, false)
		{
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFPropertyListCreateWithData (IntPtr allocator, IntPtr dataRef, CFPropertyListMutabilityOptions options, out CFPropertyListFormat format, /* CFError * */ out IntPtr error);


		public (CFPropertyList propertyList, CFPropertyListFormat format, NSError error)
			FromData (NSData data, CFPropertyListMutabilityOptions options = CFPropertyListMutabilityOptions.Immutable)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));
			if (data.Handle == IntPtr.Zero)
				throw new ObjectDisposedException (nameof (data));

			CFPropertyListFormat fmt;
			IntPtr error;
			var ret = CFPropertyListCreateWithData (IntPtr.Zero, data.Handle, options, out fmt, out error);
			if (ret != null)
				return (new CFPropertyList (ret, owns: true), fmt, null);
			return (null, CFPropertyListFormat.XmlFormat1, new NSError (error));
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFPropertyListCreateDeepCopy(IntPtr allocator, IntPtr propertyList, CFPropertyListMutabilityOptions mutabilityOption);

		public CFPropertyList DeepCopy (CFPropertyListMutabilityOptions options = CFPropertyListMutabilityOptions.MutableContainersAndLeaves)
		{
			return new CFPropertyList (CFPropertyListCreateDeepCopy (IntPtr.Zero, handle, options), owns: true);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /*CFDataRef*/IntPtr CFPropertyListCreateData(IntPtr allocator, IntPtr propertyList, CFPropertyListFormat format, CFPropertyListMutabilityOptions options, out IntPtr error);

		public (NSData data, NSError error) AsData (CFPropertyListFormat format = CFPropertyListFormat.BinaryFormat1)
		{
			IntPtr error;
			var x = CFPropertyListCreateData (IntPtr.Zero, handle, format, 0, out error);
			if (x == IntPtr.Zero)
				return (null, new NSError (error));
			return (new NSData (x), null);
		}		

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static bool CFPropertyListIsValid (IntPtr plist, CFPropertyListFormat format);

		public bool IsValid (CFPropertyListFormat format)
		{
			return CFPropertyListIsValid (handle, format);
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

	public enum CFPropertyListFormat {
		OpenStep = 1,
		XmlFormat1 = 100,
		BinaryFormat1 = 200
	}

	[Flags]
	public enum CFPropertyListMutabilityOptions {
		Immutable = 0,
		MutableContainers = 1 << 0,
		MutableContainersAndLeaves = 1 << 1
	}
}
