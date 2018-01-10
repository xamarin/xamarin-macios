// 
// CMMemoryPool.cs: Implements the managed CMMemoryPool
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2014 Xamarin Inc
//

using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace CoreMedia {

	[iOS (6,0)][Mac (10,8)]
	public partial class CMMemoryPool : IDisposable, INativeObject
	{
		IntPtr handle;

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMMemoryPoolRef */ IntPtr CMMemoryPoolCreate (/* CFDictionaryRef */ IntPtr options);

		public CMMemoryPool ()
		{
			handle = CMMemoryPoolCreate (IntPtr.Zero);
		}

#if !COREBUILD
		public CMMemoryPool (TimeSpan ageOutPeriod)
		{
			using (var n = new NSNumber (ageOutPeriod.TotalSeconds))
			using (var dict = new NSDictionary (AgeOutPeriodSelector, n)) {
				handle = CMMemoryPoolCreate (dict.Handle);
			}
		}
#endif

		~CMMemoryPool ()
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
			if (Handle != IntPtr.Zero){
				CFObject.CFRelease (Handle);
				handle = IntPtr.Zero;
			}
		}

		public IntPtr Handle { 
			get {
				return handle;
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CFAllocatorRef */ IntPtr CMMemoryPoolGetAllocator (/* CMMemoryPoolRef */ IntPtr pool);

		public CFAllocator GetAllocator ()
		{
			return new CFAllocator (CMMemoryPoolGetAllocator (Handle), false);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMMemoryPoolFlush (/* CMMemoryPoolRef */ IntPtr pool);

		public void Flush ()
		{
			CMMemoryPoolFlush (Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMMemoryPoolInvalidate (/* CMMemoryPoolRef */ IntPtr pool);

		public void Invalidate ()
		{
			CMMemoryPoolInvalidate (Handle);
		}
	}
}
