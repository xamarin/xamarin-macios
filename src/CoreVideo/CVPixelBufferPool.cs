// 
// CVPixelBufferPool.cs: Implements the managed CVPixelBufferPool
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2010 Novell, Inc
// Copyright 2012-2014 Xamarin Inc.
//
using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreVideo {

	// CVPixelBufferPool.h
	[Watch (4,0)]
	public partial class CVPixelBufferPool : INativeObject
#if !COREBUILD
		, IDisposable
#endif
		{
#if !COREBUILD
#if !XAMCORE_2_0
		public static readonly NSString MinimumBufferCountKey;
		public static readonly NSString MaximumBufferAgeKey;

		static CVPixelBufferPool ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreVideoLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				MinimumBufferCountKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferPoolMinimumBufferCountKey");
				MaximumBufferAgeKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferPoolMaximumBufferAgeKey");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
#endif

		IntPtr handle;

		internal CVPixelBufferPool (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw new ArgumentException ("Invalid parameters to context creation");

			this.handle = CVPixelBufferPoolRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CVPixelBufferPool (IntPtr handle, bool owns)
		{
			if (!owns)
				CVPixelBufferPoolRetain (handle);

			this.handle = handle;
		}

		~CVPixelBufferPool ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVPixelBufferPoolRelease (/* CVPixelBufferPoolRef __nullable */ IntPtr handle);
		
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CVPixelBufferPoolRef __nullable */ IntPtr CVPixelBufferPoolRetain (
			/* CVPixelBufferPoolRef __nullable */ IntPtr handle);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CVPixelBufferPoolRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static nint CVPixelBufferPoolGetTypeID ();
		public nint TypeID {
			get {
				return CVPixelBufferPoolGetTypeID ();
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CVPixelBufferPoolGetPixelBufferAttributes (
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pool);

		// TODO: Return type is CVPixelBufferAttributes but need different name when this one is not WeakXXXX
		public NSDictionary PixelBufferAttributes {
			get {
				return Runtime.GetNSObject<NSDictionary> (CVPixelBufferPoolGetPixelBufferAttributes (handle));
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CVPixelBufferPoolGetAttributes (
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pool);

		public NSDictionary Attributes {
			get {
				return Runtime.GetNSObject<NSDictionary> (CVPixelBufferPoolGetAttributes (handle));
			}
		}

		public CVPixelBufferPoolSettings Settings {
			get {
				var attr = Attributes;
				return attr == null ? null : new CVPixelBufferPoolSettings (attr);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferPoolCreatePixelBuffer (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pixelBufferPool,
			/* CVPixelBufferRef  __nullable * __nonnull */ out IntPtr pixelBufferOut);

		public CVPixelBuffer CreatePixelBuffer ()
		{
			IntPtr pixelBufferOut;
			CVReturn ret = CVPixelBufferPoolCreatePixelBuffer (IntPtr.Zero, handle, out pixelBufferOut);

			if (ret != CVReturn.Success)
				throw new Exception ("CVPixelBufferPoolCreatePixelBuffer returned " + ret.ToString ());

			return new CVPixelBuffer (pixelBufferOut, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferPoolCreatePixelBufferWithAuxAttributes (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CVPixelBufferPoolRef __nonnull */ IntPtr pixelBufferPool,
			/* CFDictionaryRef __nullable */ IntPtr auxAttributes,
			/* CVPixelBufferRef  __nullable * __nonnull */ out IntPtr pixelBufferOut);

		public CVPixelBuffer CreatePixelBuffer (CVPixelBufferPoolAllocationSettings allocationSettings, out CVReturn error)
		{
			IntPtr pb;
			error = CVPixelBufferPoolCreatePixelBufferWithAuxAttributes (IntPtr.Zero, handle, allocationSettings.GetHandle (), out pb);
			if (error != CVReturn.Success)
				return null;

			return new CVPixelBuffer (pb, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferPoolCreate (/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CFDictionaryRef __nullable */ IntPtr poolAttributes,
			/* CFDictionaryRef __nullable */ IntPtr pixelBufferAttributes,
			/* CVPixelBufferPoolRef  __nullable * __nonnull */ out IntPtr poolOut);

		[Advice ("Use overload with CVPixelBufferPoolSettings")]
		public CVPixelBufferPool (NSDictionary poolAttributes, NSDictionary pixelBufferAttributes)
		{
			CVReturn ret = CVPixelBufferPoolCreate (IntPtr.Zero, poolAttributes.GetHandle (), 
				pixelBufferAttributes.GetHandle (), out handle);

			if (ret != CVReturn.Success)
				throw new Exception ("CVPixelBufferPoolCreate returned " + ret.ToString ());
		}

		public CVPixelBufferPool (CVPixelBufferPoolSettings settings, CVPixelBufferAttributes pixelBufferAttributes)
			: this (settings.GetDictionary (), pixelBufferAttributes.GetDictionary ())
		{
		}


		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreVideoLibrary)]
		static extern void CVPixelBufferPoolFlush (/* CVPixelBufferPoolRef __nonnull */ IntPtr pool,
			CVPixelBufferPoolFlushFlags options);

		[iOS (9,0)][Mac (10,11)]
		public void Flush (CVPixelBufferPoolFlushFlags options)
		{
			CVPixelBufferPoolFlush (handle, options);
		}

#endif // !COREBUILD
	}
}
