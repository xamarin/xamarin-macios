//
// Copyright 2019 Microsoft Corp.
//

#if !__MACCATALYST__

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace AppKit {

	public partial class NSView {

		delegate nint view_compare_func (IntPtr view1, IntPtr view2, IntPtr context);
		static view_compare_func view_comparer = view_compare;

		sealed class SortData
		{
			public Exception Exception;
			public Func<NSView, NSView, NSComparisonResult> Comparer;
		}

		[MonoPInvokeCallback (typeof (view_compare_func))]
		static nint view_compare (IntPtr view1, IntPtr view2, IntPtr context)
		{
			var data = (SortData) GCHandle.FromIntPtr (context).Target;
			try {
				var a = (NSView) Runtime.GetNSObject (view1);
				var b = (NSView) Runtime.GetNSObject (view2);
				return (nint) (long) data.Comparer (a, b);
			} catch (Exception e) {
				data.Exception = e;
				return (nint) (long) NSComparisonResult.Same;
			}
		}

		public void SortSubviews (Func<NSView, NSView, NSComparisonResult> comparer)
		{
			if (comparer == null)
				throw new ArgumentNullException (nameof (comparer));

			var func = Marshal.GetFunctionPointerForDelegate (view_comparer);
			var context = new SortData () { Comparer = comparer };
			var handle = GCHandle.Alloc (context);
			try {
				SortSubviews (func, GCHandle.ToIntPtr (handle));
				if (context.Exception != null)
					throw new Exception ($"An exception occurred during sorting.", context.Exception);
			} finally {
				handle.Free ();
			}
		}
	}
}
#endif // !__MACCATALYST__
