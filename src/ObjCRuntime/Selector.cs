//
// Copyright 2010, Novell, Inc.
// Copyright 2013 - 2014 Xamarin Inc.
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
using Foundation;
using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ObjCRuntime {
	public partial class Selector : IEquatable<Selector>, INativeObject {
		internal const string Alloc = "alloc";
		internal const string Class = "class";
		internal const string Release = "release";
		internal const string Retain = "retain";
		internal const string Autorelease = "autorelease";
		internal const string PerformSelectorOnMainThreadWithObjectWaitUntilDone = "performSelectorOnMainThread:withObject:waitUntilDone:";

		NativeHandle handle;
		string? name;

		public Selector (NativeHandle sel)
		{
			if (sel_isMapped (sel) == 0)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (nameof (sel), "Not a selector handle.");

			this.handle = sel;
		}

		// this .ctor is required, like for any INativeObject implementation
		// even if selectors are not disposable
		[Preserve (Conditional = true)]
		internal Selector (NativeHandle handle, bool /* unused */ owns)
		{
			this.handle = handle;
		}

		public Selector (string name)
		{
			this.name = name;
			handle = GetHandle (name);
		}

		/// <summary>Handle (pointer) to the unmanaged selector representation.</summary>
		///         <value>A pointer to the unmanaged selector representation.</value>
		///         <remarks>
		///           <para>This IntPtr is the handle to the underlying unmanaged representation for this selector.</para>
		///         </remarks>
		public NativeHandle Handle {
			get { return handle; }
		}

		/// <summary>Name of this selector.</summary>
		///         <value />
		///         <remarks>
		///         </remarks>
		public string Name {
			get {
				if (name is null)
					name = GetName (handle);
				return name;
			}
		}

		public static bool operator != (Selector left, Selector right)
		{
			return !(left == right);
		}

		public static bool operator == (Selector left, Selector right)
		{
			if (left is null)
				return (right is null);
			if (right is null)
				return false;

			// note: there's a sel_isEqual function but it's safe to compare pointers
			// ref: https://opensource.apple.com/source/objc4/objc4-551.1/runtime/objc-sel.mm.auto.html
			return left.handle == right.handle;
		}

		public override bool Equals (object? right)
		{
			return Equals (right as Selector);
		}

		public bool Equals (Selector? right)
		{
			if (right is null)
				return false;

			return handle == right.handle;
		}

		public override int GetHashCode ()
		{
			return handle.GetHashCode ();
		}

		internal static string GetName (IntPtr handle)
		{
			return Marshal.PtrToStringAuto (sel_getName (handle))!;
		}

		// return null, instead of throwing, if an invalid pointer is used (e.g. IntPtr.Zero)
		// so this looks better in the debugger watch when no selector is assigned (ref: #10876)
		public static Selector? FromHandle (NativeHandle sel)
		{
			if (sel_isMapped (sel) == 0)
				return null;
			// create the selector without duplicating the sel_isMapped check
			return new Selector (sel, false);
		}

		/// <summary>Creates a managed Selector instance from a native selector.</summary>
		/// <param name="selector">The native selector handle.</param>
		/// <param name="owns">Whether the caller owns the native selector handle or not.</param>
		/// <remarks>It's not possible to free a selector, so the <paramref name="owns" /> parameter is ignored.</remarks>
		public static Selector? FromHandle (NativeHandle selector, bool owns)
		{
			return FromHandle (selector);
		}

		public static Selector Register (NativeHandle handle)
		{
			return new Selector (handle);
		}

		// objc/runtime.h
		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static /* const char* */ IntPtr sel_getName (/* SEL */ IntPtr sel);

		// objc/runtime.h
		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static /* SEL */ IntPtr sel_registerName (/* const char* */ IntPtr name);

		// objc/runtime.h
		// Selector.GetHandle is optimized by the AOT compiler, and the current implementation only supports IntPtr, so we can't switch to NativeHandle quite yet (the AOT compiler crashes).
		public static IntPtr GetHandle (string name)
		{
			var ptr = Marshal.StringToHGlobalAnsi (name);
			try {
				return sel_registerName (ptr);
			} finally {
				Marshal.FreeHGlobal (ptr);
			}
		}

		// objc/objc.h
		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static /* BOOL */ byte sel_isMapped (/* SEL */ IntPtr sel);
	}
}
