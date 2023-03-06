//
// DispatchBlock.cs: Support for creating dispatch blocks.
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2019 Microsoft Corp.
//
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Threading;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {
#if !COREBUILD

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public sealed class DispatchBlock : NativeObject {
		[Preserve (Conditional = true)]
		internal DispatchBlock (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public DispatchBlock (Action action, DispatchBlockFlags flags = DispatchBlockFlags.None)
			: base (create (action, flags), true)
		{
		}

		public DispatchBlock (Action action, DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
			: base (create (flags, qosClass, relative_priority, action), true)
		{
		}

		public DispatchBlock (DispatchBlock dispatchBlock, DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
			: base (dispatch_block_create_with_qos_class ((nuint) (ulong) flags, qosClass, relative_priority, Runtime.ThrowOnNull (dispatchBlock, nameof (dispatchBlock)).GetCheckedHandle ()), true)
		{
		}

		public static DispatchBlock Create (Action action, DispatchBlockFlags flags = DispatchBlockFlags.None)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));
			return new DispatchBlock (action, flags);
		}

		public static DispatchBlock Create (Action action, DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));
			return new DispatchBlock (action, flags, qosClass, relative_priority);
		}

		public static DispatchBlock Create (DispatchBlock block, DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
		{
			if (block is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (block));
			return block.Create (flags, qosClass, relative_priority);
		}

		public DispatchBlock Create (DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
		{
			return new DispatchBlock (dispatch_block_create_with_qos_class ((nuint) (ulong) flags, qosClass, relative_priority, GetCheckedHandle ()), true);
		}

		protected internal override void Retain ()
		{
			Handle = BlockLiteral._Block_copy (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			BlockLiteral._Block_release (GetCheckedHandle ());
		}

		[DllImport (Constants.libcLibrary)]
		unsafe extern static IntPtr dispatch_block_create (/*DispatchBlockFlags*/ nuint flags, BlockLiteral* block);

		// Returns a retained heap-allocated block
		[BindingImpl (BindingImplOptions.Optimizable)]
		static IntPtr create (Action action, DispatchBlockFlags flags)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));

			unsafe {
				using var block = BlockStaticDispatchClass.CreateBlock (action);
				return dispatch_block_create ((nuint) (ulong) flags, &block);
			}
		}

		[DllImport (Constants.libcLibrary)]
		unsafe extern static IntPtr dispatch_block_create_with_qos_class (/*DispatchBlockFlags*/ nuint flags, DispatchQualityOfService qosClass, int relative_priority, BlockLiteral* dispatchBlock);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_block_create_with_qos_class (/*DispatchBlockFlags*/ nuint flags, DispatchQualityOfService qosClass, int relative_priority, IntPtr dispatchBlock);

		// Returns a retained heap-allocated block
		[BindingImpl (BindingImplOptions.Optimizable)]
		static IntPtr create (DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority, Action action)
		{
			if (action is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (action));

			unsafe {
				using var block = BlockStaticDispatchClass.CreateBlock (action);
				return dispatch_block_create_with_qos_class ((nuint) (ulong) flags, qosClass, relative_priority, &block);
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_block_cancel (IntPtr block);

		public void Cancel ()
		{
			dispatch_block_cancel (GetCheckedHandle ());
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_block_notify (IntPtr block, IntPtr queue, IntPtr notification);

		public void Notify (DispatchQueue queue, Action notification)
		{
			if (notification is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (notification));
			using (var block = new DispatchBlock (notification))
				Notify (queue, block);
		}

		public void Notify (DispatchQueue queue, DispatchBlock notification)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));
			if (notification is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (notification));
			dispatch_block_notify (GetCheckedHandle (), queue.GetCheckedHandle (), notification.GetCheckedHandle ());
		}

		[DllImport (Constants.libcLibrary)]
		extern static nint dispatch_block_testcancel (IntPtr block);

		public nint TestCancel ()
		{
			return dispatch_block_testcancel (GetCheckedHandle ());
		}

		public bool Cancelled {
			get { return TestCancel () != 0; }
		}

		[DllImport (Constants.libcLibrary)]
		extern static nint dispatch_block_wait (IntPtr block, DispatchTime time);

		public nint Wait (DispatchTime time)
		{
			return dispatch_block_wait (GetCheckedHandle (), time);
		}

		public nint Wait (TimeSpan timeout)
		{
			return Wait (new DispatchTime (DispatchTime.Now, timeout));
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void DispatchBlockCallback (IntPtr block);

		public static explicit operator Action? (DispatchBlock block)
		{
			if (block is null)
				return null;

			unsafe {
				var handle = (BlockLiteral*) (IntPtr) block.GetCheckedHandle ();
				var del = handle->GetDelegateForBlock<DispatchBlockCallback> ();
				return new Action (() => del ((IntPtr) block.GetCheckedHandle ()));
			}
		}

		public void Invoke ()
		{
			((Action) this!) ();
		}
	}

	[Flags]
	[Native]
	public enum DispatchBlockFlags : ulong {
		None,
		Barrier = 1,
		Detached = 2,
		AssignCurrent = 4,
		NoQosClass = 8,
		InheritQosClass = 16,
		EnforceQosClass = 32,
	}
#endif // !COREBUILD
}
