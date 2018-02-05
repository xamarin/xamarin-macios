// 
// VTSession.cs: Property setting/reading
//
// Authors: Miguel de Icaza (miguel@xamarin.com)
//     
// Copyright 2014-2015 Xamarin Inc.
//
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;
using CoreVideo;

namespace VideoToolbox {		
	[Mac (10,8), iOS (8,0), TV (10,2)]
	public class VTSession : INativeObject, IDisposable {
		IntPtr handle;

		/* invoked by marshallers */
		protected internal VTSession (IntPtr handle)
		{
			this.handle = handle;
			CFObject.CFRetain (this.handle);
		}

		public IntPtr Handle {
			get {return handle; }
		}

		[Preserve (Conditional=true)]
		internal VTSession (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (this.handle);
		}
		
		~VTSession ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		// All of them returns OSStatus mapped to VTStatus enum

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTSessionSetProperty (IntPtr handle, IntPtr propertyKey, IntPtr value);

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTSessionCopyProperty (IntPtr handle, IntPtr propertyKey, /* CFAllocator */ IntPtr allocator, out IntPtr propertyValueOut);

		[DllImport (Constants.VideoToolboxLibrary)]
		internal extern static VTStatus VTSessionSetProperties (IntPtr handle, IntPtr propertyDictionary);

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTSessionCopySerializableProperties (IntPtr handle, /* CFAllocator */ IntPtr allocator, out IntPtr dictionaryOut);

		[DllImport (Constants.VideoToolboxLibrary)]
		extern static VTStatus VTSessionCopySupportedPropertyDictionary (/* VTSessionRef */ IntPtr session, /* CFDictionaryRef* */ out IntPtr supportedPropertyDictionaryOut);

		public VTStatus SetProperties (VTPropertyOptions options)
		{
			if (options == null)
				throw new ArgumentNullException ("options");

			return VTSessionSetProperties (handle, options.Dictionary.Handle);
		}

		public VTStatus SetProperty (NSString propertyKey, NSObject value)
		{
			if (propertyKey == null)
				throw new ArgumentNullException ("propertyKey");

			return VTSessionSetProperty (handle, propertyKey.Handle, value != null ? value.Handle : IntPtr.Zero);
		}

		public VTPropertyOptions GetProperties ()
		{
			IntPtr ret;
			var result = VTSessionCopySerializableProperties (handle, IntPtr.Zero, out ret);
			if (result != VTStatus.Ok || ret == IntPtr.Zero)
				return null;

			var dict = Runtime.GetNSObject<NSDictionary> (ret);
			return new VTPropertyOptions (dict);
		}

		public NSObject GetProperty (NSString propertyKey)
		{
			if (propertyKey == null)
				throw new ArgumentNullException ("propertyKey");

			IntPtr ret;
			if (VTSessionCopyProperty (handle, propertyKey.Handle, IntPtr.Zero, out ret) != VTStatus.Ok || ret == IntPtr.Zero)
				return null;
			var obj = Runtime.GetNSObject (ret);
			obj.DangerousRelease ();
			return obj;
		}

		public NSDictionary GetSerializableProperties ()
		{
			IntPtr ret;
			var result = VTSessionCopySerializableProperties (Handle, IntPtr.Zero, out ret);
			if (result != VTStatus.Ok || ret == IntPtr.Zero)
				return null;

			var dict = Runtime.GetNSObject<NSDictionary> (ret);
			dict.DangerousRelease ();
			return dict;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public NSDictionary GetSupportedProperties ()
		{
			IntPtr ret;
			var result = VTSessionCopySupportedPropertyDictionary (Handle, out ret);
			if (result != VTStatus.Ok || ret == IntPtr.Zero)
				return null;
			
			var dict = Runtime.GetNSObject<NSDictionary> (ret);
			dict.DangerousRelease ();
			return dict;
		}
	}
}
