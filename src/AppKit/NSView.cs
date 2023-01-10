//
// Copyright 2019 Microsoft Corp.
//

#if !__MACCATALYST__

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace AppKit {

	public partial class NSView {

#if !NET
		delegate nint view_compare_func (IntPtr view1, IntPtr view2, IntPtr context);
		static view_compare_func view_comparer = view_compare;
#endif

		sealed class SortData {
			public Exception? Exception;
			public Func<NSView, NSView, NSComparisonResult>? Comparer;
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (view_compare_func))]
#endif
		static nint view_compare (IntPtr view1, IntPtr view2, IntPtr context)
		{
			var data = GCHandle.FromIntPtr (context).Target as SortData;
			try {
				if (Runtime.GetNSObject (view1) is NSView a
						&& Runtime.GetNSObject (view2) is NSView b) {
					var result = data?.Comparer?.Invoke (a, b);
					return result is null ? (nint) (long) NSComparisonResult.Same : (nint) (long) result;
				} else {
					return (nint) (long) NSComparisonResult.Same;
				}
			} catch (Exception e) {
				if (data is not null)
					data.Exception = e;
				return (nint) (long) NSComparisonResult.Same;
			}
		}

		public unsafe void SortSubviews (Func<NSView, NSView, NSComparisonResult>? comparer)
		{
			if (comparer is null)
				throw new ArgumentNullException (nameof (comparer));

#if NET
			delegate* unmanaged<IntPtr, IntPtr, IntPtr, nint> fptr = &view_compare;
			var func = (IntPtr) fptr;
#else
			var func = Marshal.GetFunctionPointerForDelegate (view_comparer);
#endif
			var context = new SortData () { Comparer = comparer };
			var handle = GCHandle.Alloc (context);
			try {
				SortSubviews (func, GCHandle.ToIntPtr (handle));
				if (context.Exception is not null)
					throw new Exception ($"An exception occurred during sorting.", context.Exception);
			} finally {
				handle.Free ();
			}
		}
	}
}
#endif // !__MACCATALYST__
