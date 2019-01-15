//
// DispatchBlock.cs: Support for creating dispatch blocks.
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2019 Microsoft Corp.
//
//
using System;
using System.Runtime.InteropServices;
using System.Threading;
using ObjCRuntime;
using Foundation;

namespace CoreFoundation {
#if !COREBUILD

	[StructLayout (LayoutKind.Sequential)]
	[iOS (8, 0)]
	[Mac (10, 10)]
	public sealed class DispatchBlock : NativeObject {
		internal DispatchBlock (IntPtr handle, bool owns)
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
			if (action == null)
				throw new ArgumentNullException (nameof (action));
			return new DispatchBlock (action, flags);
		}

		public static DispatchBlock Create (Action action, DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
		{
			if (action == null)
				throw new ArgumentNullException (nameof (action));
			return new DispatchBlock (action, flags, qosClass, relative_priority);
		}

		public static DispatchBlock Create (DispatchBlock block, DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
		{
			if (block == null)
				throw new ArgumentNullException (nameof (block));
			return block.Create (flags, qosClass, relative_priority);
		}

		public DispatchBlock Create (DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority)
		{
			return new DispatchBlock (dispatch_block_create_with_qos_class ((nuint) (ulong) flags, qosClass, relative_priority, GetCheckedHandle ()), true);
		}

		protected override void Retain ()
		{
			Handle = BlockLiteral._Block_copy (GetCheckedHandle ());
		}

		protected override void Release ()
		{
			BlockLiteral._Block_release (GetCheckedHandle ());
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_block_create (/*DispatchBlockFlags*/ nuint flags, ref BlockLiteral block);

		// Returns a retained heap-allocated block
		[BindingImpl (BindingImplOptions.Optimizable)]
		static IntPtr create (Action action, DispatchBlockFlags flags)
		{
			if (action == null)
				throw new ArgumentNullException (nameof (action));

			BlockLiteral block_handler = new BlockLiteral ();
			try {
				block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				return dispatch_block_create ((nuint) (ulong) flags, ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_block_create_with_qos_class (/*DispatchBlockFlags*/ nuint flags, DispatchQualityOfService qosClass, int relative_priority, ref BlockLiteral dispatchBlock);

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_block_create_with_qos_class (/*DispatchBlockFlags*/ nuint flags, DispatchQualityOfService qosClass, int relative_priority, IntPtr dispatchBlock);

		// Returns a retained heap-allocated block
		[BindingImpl (BindingImplOptions.Optimizable)]
		static IntPtr create (DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority, Action action)
		{
			if (action == null)
				throw new ArgumentNullException (nameof (action));

			BlockLiteral block_handler = new BlockLiteral ();
			try {
				block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				return dispatch_block_create_with_qos_class ((nuint) (ulong) flags, qosClass, relative_priority, ref block_handler);
			} finally {
				block_handler.CleanupBlock ();
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
			if (notification == null)
				throw new ArgumentNullException (nameof (notification));
			using (var block = new DispatchBlock (notification))
				Notify (queue, block);
		}

		public void Notify (DispatchQueue queue, DispatchBlock notification)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			if (notification == null)
				throw new ArgumentNullException (nameof (notification));
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

		delegate void DispatchBlockCallback (IntPtr block);

		public static explicit operator Action (DispatchBlock block)
		{
			if (block == null)
				return null;

			unsafe {
				BlockLiteral *handle = (BlockLiteral *) block.GetCheckedHandle ();
				var del = handle->GetDelegateForBlock<DispatchBlockCallback> ();
				return new Action (() => del (block.GetCheckedHandle ()));
			}
		}

		public void Invoke ()
		{
			((Action) this) ();
		}

		//
		// You must invoke ->CleanupBlock after you have transferred ownership to
		// the unmanaged code to release the resources allocated on the managed side
		//
		[BindingImpl (BindingImplOptions.Optimizable)]
		internal static unsafe void Invoke (Action codeToRun, Action<IntPtr> invoker)
		{
			BlockLiteral *block_ptr;
			BlockLiteral block;
			block = new BlockLiteral ();
			block_ptr = &block;

			block.SetupBlockUnsafe (Trampolines.SDAction.Handler, codeToRun);
			invoker ((IntPtr) block_ptr);
			block_ptr->CleanupBlock ();
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
