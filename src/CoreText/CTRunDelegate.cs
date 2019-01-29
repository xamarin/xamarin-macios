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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

namespace CoreText {

#region Run Delegate Callbacks
	delegate void CTRunDelegateDeallocateCallback (IntPtr refCon);
	delegate nfloat CTRunDelegateGetCallback (IntPtr refCon);

	[StructLayout (LayoutKind.Sequential)]
	class CTRunDelegateCallbacks {
		public /* CFIndex */ nint version;
		public CTRunDelegateDeallocateCallback dealloc;
		public CTRunDelegateGetCallback getAscent;
		public CTRunDelegateGetCallback getDescent;
		public CTRunDelegateGetCallback getWidth;
	}
#endregion

	public class CTRunDelegateOperations : IDisposable {

		internal GCHandle handle;

		protected CTRunDelegateOperations ()
		{
			handle = GCHandle.Alloc (this);
		}

#if XAMCORE_2_0
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
#else
		public virtual void Dispose ()
		{
		}
#endif

#if XAMCORE_4_0
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

		internal CTRunDelegateCallbacks GetCallbacks ()
		{
			var callbacks = new CTRunDelegateCallbacks () {
				version = 1, // kCTRunDelegateVersion1
				dealloc = Deallocate,
				getAscent = GetAscent,
				getDescent = GetDescent,
				getWidth = GetWidth,
			};
			return callbacks;
		}

		[MonoPInvokeCallback (typeof (CTRunDelegateDeallocateCallback))]
		static void Deallocate (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self == null)
				return;

			self.Dispose ();

			if (self.handle.IsAllocated)
				self.handle.Free ();
			self.handle = new GCHandle ();
		}

		internal static CTRunDelegateOperations GetOperations (IntPtr refCon)
		{
			GCHandle c = GCHandle.FromIntPtr (refCon);

			return c.Target as CTRunDelegateOperations;
		}

		[MonoPInvokeCallback (typeof (CTRunDelegateGetCallback))]
		static nfloat GetAscent (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self == null)
				return 0;
			return (nfloat) self.GetAscent ();
		}

		[MonoPInvokeCallback (typeof (CTRunDelegateGetCallback))]
		static nfloat GetDescent (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self == null)
				return 0;
			return (nfloat) self.GetDescent ();
		}

		[MonoPInvokeCallback (typeof (CTRunDelegateGetCallback))]
		static nfloat GetWidth (IntPtr refCon)
		{
			var self = GetOperations (refCon);
			if (self == null)
				return 0;
			return (nfloat) self.GetWidth ();
		}
	}

	public class CTRunDelegate : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTRunDelegate (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException ("handle");

			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}
		
		public IntPtr Handle {
			get {return handle;}
		}

		~CTRunDelegate ()
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

#region RunDelegate Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTRunDelegateCreate (CTRunDelegateCallbacks callbacks, IntPtr refCon);

		CTRunDelegateCallbacks callbacks; // prevent GC since they are called from native code

		public CTRunDelegate (CTRunDelegateOperations operations)
		{
			if (operations == null)
				throw ConstructorError.ArgumentNull (this, "operations");

			callbacks = operations.GetCallbacks ();
			handle = CTRunDelegateCreate (callbacks, GCHandle.ToIntPtr (operations.handle));
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}
#endregion

#region Run Delegate Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTRunDelegateGetRefCon (IntPtr runDelegate);

		public CTRunDelegateOperations Operations {
			get {
				return CTRunDelegateOperations.GetOperations (CTRunDelegateGetRefCon (handle));
			}
		}
#endregion
	}
}

