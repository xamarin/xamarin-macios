// 
// CTRunDelegate.cs: Implements the managed CTRunDelegate
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
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

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

	#region Run Delegate Callbacks
	delegate void CTRunDelegateDeallocateCallback (IntPtr refCon);
	delegate nfloat CTRunDelegateGetCallback (IntPtr refCon);

#if NET
	[StructLayout (LayoutKind.Sequential)]
	struct CTRunDelegateCallbacks {
		public /* CFIndex */ nint version;
		public unsafe delegate* unmanaged<IntPtr, void> dealloc;
		public unsafe delegate* unmanaged<IntPtr, nfloat> getAscent;
		public unsafe delegate* unmanaged<IntPtr, nfloat> getDescent;
		public unsafe delegate* unmanaged<IntPtr, nfloat> getWidth;
	}
#else
	[StructLayout (LayoutKind.Sequential)]
	struct CTRunDelegateCallbacks {
		public /* CFIndex */ nint version;
		public CTRunDelegateDeallocateCallback dealloc;
		public CTRunDelegateGetCallback getAscent;
		public CTRunDelegateGetCallback getDescent;
		public CTRunDelegateGetCallback getWidth;
	}
#endif
	#endregion

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTRunDelegateOperations : IDisposable {
		// This instance is kept alive using a GCHandle until the Deallocate callback has been called,
		// which is called when the corresponding CTRunDelegate is freed (retainCount reaches 0).
		// This even means that the GCHandle is not freed if Dispose is called manually.
		GCHandle handle;

		public IntPtr Handle {
			get { return GCHandle.ToIntPtr (handle); }
		}

		protected CTRunDelegateOperations ()
		{
			handle = GCHandle.Alloc (this);
		}

		~CTRunDelegateOperations ()
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
		}

#if NET
		public virtual nfloat GetAscent ()
		{
			return 0;
		}

		public virtual nfloat GetDescent ()
		{
			return 0;
		}

		public virtual nfloat GetWidth ()
		{
			return 0;
		}
#else
		public virtual float GetAscent ()
		{
			return 0.0f;
		}

		public virtual float GetDescent ()
		{
			return 0.0f;
		}

		public virtual float GetWidth ()
		{
			return 0.0f;
		}
#endif

		CTRunDelegateCallbacks? callbacks; // prevent GC since they are called from native code
		internal CTRunDelegateCallbacks GetCallbacks ()
		{
			if (!callbacks.HasValue) {
#if NET
				unsafe {
					callbacks = new CTRunDelegateCallbacks () {
						version = 1,
						dealloc = &Deallocate,
						getAscent = &GetAscent,
						getDescent = &GetDescent,
						getWidth = &GetWidth,
					};
				}
#else
				callbacks = new CTRunDelegateCallbacks () {
					version = 1, // kCTRunDelegateVersion1
					dealloc = Deallocate,
					getAscent = GetAscent,
					getDescent = GetDescent,
					getWidth = GetWidth,
				};
#endif
			}
			return callbacks.Value;
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CTRunDelegateDeallocateCallback))]
#endif
		static void Deallocate (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self is null)
				return;

			self.Dispose ();

			if (self.handle.IsAllocated)
				self.handle.Free ();
			self.handle = new GCHandle ();
		}

		internal static CTRunDelegateOperations? GetOperations (IntPtr refCon)
		{
			GCHandle c = GCHandle.FromIntPtr (refCon);

			return c.Target as CTRunDelegateOperations;
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CTRunDelegateGetCallback))]
#endif
		static nfloat GetAscent (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self is null)
				return 0;
			return (nfloat) self.GetAscent ();
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CTRunDelegateGetCallback))]
#endif
		static nfloat GetDescent (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self is null)
				return 0;
			return (nfloat) self.GetDescent ();
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CTRunDelegateGetCallback))]
#endif
		static nfloat GetWidth (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self is null)
				return 0;
			return (nfloat) self.GetWidth ();
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTRunDelegate : NativeObject, IDisposable {
		[Preserve (Conditional = true)]
		internal CTRunDelegate (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		#region RunDelegate Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTRunDelegateCreate (ref CTRunDelegateCallbacks callbacks, IntPtr refCon);

		static IntPtr Create (CTRunDelegateOperations operations)
		{
			if (operations is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (operations));

			CTRunDelegateCallbacks callbacks = operations.GetCallbacks ();
			return CTRunDelegateCreate (ref callbacks, operations.Handle);
		}

		public CTRunDelegate (CTRunDelegateOperations operations)
			: base (Create (operations), true)
		{
		}
		#endregion

		#region Run Delegate Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTRunDelegateGetRefCon (IntPtr runDelegate);

		public CTRunDelegateOperations? Operations {
			get {
				return CTRunDelegateOperations.GetOperations (CTRunDelegateGetRefCon (Handle));
			}
		}
		#endregion
	}
}
