//
// SystemLog.cs: Bindings to the kqueue(3) API from Apple
//
// Authors:
//  Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012, Xamarin Inc.
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

#nullable enable

#if MONOMAC

using System;
using CoreFoundation;
using ObjCRuntime;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Darwin {

	[StructLayout (LayoutKind.Sequential)]
	public struct TimeSpec {
		public nint Seconds;
		public nint NanoSeconds;
	}
	
	[StructLayout (LayoutKind.Sequential)]
	public struct KernelEvent {
		public IntPtr /* uintptr_tr */ Ident;
		public EventFilter /* int16_t */ Filter;
		public EventFlags /* uint16_t */ Flags;
#if NET
		public FilterFlags /* uint32_t */ FilterFlags;
#else
		public uint /* uint32_t */ FilterFlags;
#endif
		public IntPtr /* uintptr_t */ Data;
		public IntPtr /* void */ UserData;
	}

	[Flags]
	public enum EventFlags : ushort {
		Add         = 0x0001,
		Delete      = 0x0002,
		Enable      = 0x0004,
		Disable     = 0x0008,
		OneShot     = 0x0010,
		Clear       = 0x0020,
		Receipt     = 0x0040,
		Dispatch    = 0x0080,

		Flag0       = 0x1000,
		Flag1       = 0x2000,
		SystemFlags = unchecked (0xf000),
			
		// Return values.
		EOF         = 0x8000,
		Error       = 0x4000,
	}
	
	public enum EventFilter : short {
		Read = -1,
		Write = -2,
		Aio = -3,
		Vnode = -4,
		Proc = -5,
		Signal = -6,
		Timer = -7,
		MachPort = -8,
		FS = -9,
		User = -10,
		VM = -11
	}
	
	[Flags]
	public enum FilterFlags : uint {
		ReadPoll          = EventFlags.Flag0,
		ReadOutOfBand     = EventFlags.Flag1,
		ReadLowWaterMark  = 0x00000001,

		WriteLowWaterMark = ReadLowWaterMark,

		NoteTrigger       = 0x01000000,
		NoteFFNop         = 0x00000000,
		NoteFFAnd         = 0x40000000,
		NoteFFOr          = 0x80000000,
		NoteFFCopy        = 0xc0000000,
		NoteFFCtrlMask    = 0xc0000000,
		NoteFFlagsMask    = 0x00ffffff,
			          
		VNodeDelete       = 0x00000001,
		VNodeWrite        = 0x00000002,
		VNodeExtend       = 0x00000004,
		VNodeAttrib       = 0x00000008,
		VNodeLink         = 0x00000010,
		VNodeRename       = 0x00000020,
		VNodeRevoke       = 0x00000040,
		VNodeNone         = 0x00000080,
			          
		ProcExit          = 0x80000000,
		ProcFork          = 0x40000000,
		ProcExec          = 0x20000000,
		ProcReap          = 0x10000000,
		ProcSignal        = 0x08000000,
		ProcExitStatus    = 0x04000000,
		ProcResourceEnd   = 0x02000000,

		// iOS only
		ProcAppactive     = 0x00800000,
		ProcAppBackground = 0x00400000,
		ProcAppNonUI      = 0x00200000,
		ProcAppInactive   = 0x00100000,
		ProcAppAllStates  = 0x00f00000,

		// Masks
		ProcPDataMask     = 0x000fffff,
		ProcControlMask   = 0xfff00000,

		VMPressure        = 0x80000000,
		VMPressureTerminate = 0x40000000,
		VMPressureSuddenTerminate = 0x20000000,
		VMError           = 0x10000000,

		TimerSeconds      =    0x00000001,
		TimerMicroSeconds =   0x00000002,
		TimerNanoSeconds  =   0x00000004,
		TimerAbsolute     =   0x00000008,
	}

	public class KernelQueue : IDisposable, INativeObject {
		int handle;

		public NativeHandle Handle { get { return (NativeHandle) (IntPtr) handle; } }

		[DllImport (Constants.SystemLibrary)]
		extern static int /* int */ kqueue ();

		public KernelQueue ()
		{
			handle = kqueue ();
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~KernelQueue ()
		{
			Dispose (false);
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != -1){
				DispatchSource.VnodeMonitor.close (handle);
				handle = -1;
			}
		}

		[DllImport (Constants.SystemLibrary)]
		unsafe extern static int /* int */ kevent (int kq, KernelEvent *changeList, int /* int */ nChanges, KernelEvent *eventList, int /* int */ nEvents, IntPtr timeout);
		
		[DllImport (Constants.SystemLibrary)]
		unsafe extern static int /* int */ kevent (int kq, KernelEvent *changeList, int /* int */ nChanges, KernelEvent *eventList, int /* int */ nEvents, ref TimeSpec timeout);

		public int KEvent (KernelEvent[] changeList, KernelEvent[] eventList, TimeSpan? timeout = null)
		{
			if (changeList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (changeList));

			if (eventList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (eventList));

			if (changeList.Length < 1)
				throw new ArgumentOutOfRangeException ("eventList must contain at least one element", nameof (eventList));

			if (eventList.Length < 1)
				throw new ArgumentOutOfRangeException ("changeList must contain at least one element", nameof (changeList));

			return KEvent (changeList, changeList.Length, eventList, eventList.Length, ToTimespec (timeout));
		}

		public unsafe int KEvent (KernelEvent[] changeList, int nChanges, KernelEvent[] eventList, int nEvents, TimeSpec? timeout = null)
		{
			if (changeList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (changeList));

			if (eventList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (eventList));

			if (changeList.Length < 1)
				throw new ArgumentOutOfRangeException ("eventList must contain at least one element", nameof (eventList));

			if (eventList.Length < 1)
				throw new ArgumentOutOfRangeException ("changeList must contain at least one element", nameof (changeList));

			if (changeList.Length < nChanges)
				throw new ArgumentOutOfRangeException ("nChanges is larger than the number of elements in changeList", nameof (nChanges));

			if (eventList.Length < nEvents)
				throw new ArgumentOutOfRangeException ("nEvents is larger than the number of elements in eventList", nameof (nEvents));

			unsafe {
				fixed (KernelEvent *cp = changeList)
					fixed (KernelEvent *ep = eventList) {
						if (timeout is null) {
							return kevent (handle, cp, nChanges, ep, nEvents, IntPtr.Zero);
						} else {
							TimeSpec ts = timeout.Value;
							return kevent (handle, cp, nChanges, ep, nEvents, ref ts);
						}
					}
			}
		}

		static TimeSpec? ToTimespec (TimeSpan? ts)
		{
			if (ts is null)
				return null;

			var rv = new TimeSpec ();
			rv.Seconds = (nint) ts.Value.TotalSeconds;
			rv.NanoSeconds = (nint) (ts.Value.Milliseconds * 1000000L);
			return rv;
		}

// Don't worry about nullability for !NET
#nullable disable
#if !NET
		[Obsolete ("Use any of the overloads that return an int to get how many events were returned from kevent.")]
		public bool KEvent (KernelEvent [] changeList, int nChanges, KernelEvent [] eventList, int nEvents, ref TimeSpec timeOut)
		{
			if (changeList is not null && changeList.Length < nChanges)
				throw new ArgumentException ("nChanges is larger than the number of elements in changeList");

			if (eventList is not null && eventList.Length < nEvents)
				throw new ArgumentException ("nChanges is larger than the number of elements in changeList");
			
			unsafe {
				fixed (KernelEvent *cp = changeList)
					fixed (KernelEvent *ep = eventList)
						return kevent (handle, cp, nChanges, ep, nEvents, ref timeOut) != -1;
			}
		}

		[Obsolete ("Use any of the overloads that return an int to get how many events were returned from kevent.")]
		public bool KEvent (KernelEvent [] changeList, int nChanges, KernelEvent [] eventList, int nEvents)
		{
			if (changeList is not null && changeList.Length < nChanges)
				throw new ArgumentException ("nChanges is larger than the number of elements in changeList");

			if (eventList is not null && eventList.Length < nEvents)
				throw new ArgumentException ("nChanges is larger than the number of elements in changeList");
			
			unsafe {
				fixed (KernelEvent *cp = changeList)
					fixed (KernelEvent *ep = eventList)
						return kevent (handle, cp, nChanges, ep, nEvents, IntPtr.Zero) != -1;
			}
		}

		[Obsolete ("Use any of the overloads that return an int to get how many events were returned from kevent.")]
		public bool KEvent (KernelEvent [] changeList, KernelEvent [] eventList, ref TimeSpec timeOut)
		{
			unsafe {
				fixed (KernelEvent *cp = changeList)
					fixed (KernelEvent *ep = eventList)
						return kevent (handle, cp, changeList?.Length ?? 0, ep, eventList?.Length ?? 0, ref timeOut) != -1;
			}
		}
#endif
#nullable enable

#if NET
		public int KEvent (KernelEvent [] changeList, KernelEvent [] eventList)
#else
		[Obsolete ("Use any of the overloads that return an int to get how many events were returned from kevent.")]
		public bool KEvent (KernelEvent [] changeList, KernelEvent [] eventList)
#endif
		{
			unsafe {
				fixed (KernelEvent *cp = changeList)
					fixed (KernelEvent *ep = eventList)
#if NET
						return kevent (handle, cp, changeList?.Length ?? 0, ep, eventList?.Length ?? 0, IntPtr.Zero);
#else
						return kevent (handle, cp, changeList?.Length ?? 0, ep, eventList?.Length ?? 0, IntPtr.Zero) != -1;
#endif
			}
		}
	}
}

#endif
