//
// DispatchBlock.cs: Support for creating dispatch blocks.
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2015 Xamarin Inc
//
//
using System;
using System.Runtime.InteropServices;
using System.Threading;
using ObjCRuntime;
using Foundation;

namespace CoreFoundation {
#if !COREBUILD

	public enum QosClass : uint {
		UserInteractive = 0x21,
		UserInitiated = 0x19,
		Default = 0x15,
		Utility = 0x11,
		Background = 0x09,
		Unspecified = 0
	}

	[StructLayout (LayoutKind.Sequential)]
	internal struct DispatchBlock {
		IntPtr block;

		public DispatchBlock (IntPtr handle)
		{
			this.block = handle;
		}
				
		[DllImport (Messaging.LIBOBJC_DYLIB)]
		unsafe internal static extern DispatchBlock _Block_copy (BlockLiteral *block_literal);
		
		public DispatchBlock Create (Action action)
		{
			unsafe {
			        BlockLiteral block_handler = new BlockLiteral ();
			        BlockLiteral *block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				DispatchBlock ret = _Block_copy (block_ptr_handler);
				block_handler.CleanupBlock ();
				return ret;
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static DispatchBlock dispatch_block_create (DispatchBlockFlags flags, DispatchBlock block);
		
		public DispatchBlock Create (Action action, DispatchBlockFlags flags)
		{
			unsafe {
			        BlockLiteral block_handler = new BlockLiteral ();
			        BlockLiteral *block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				DispatchBlock ret = _Block_copy (block_ptr_handler);
				block_handler.CleanupBlock ();
				return dispatch_block_create (flags, ret);
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static DispatchBlock dispatch_block_create_with_qos_class (DispatchBlockFlags flags, QosClass qosClass, int relative_priority, DispatchBlock dispatchBlock);

		public DispatchBlock CreateWithQos (DispatchBlockFlags flags, QosClass qosClass, int relative_priority, Action action)
		{
			unsafe {
			        BlockLiteral block_handler = new BlockLiteral ();
			        BlockLiteral *block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				DispatchBlock ret = _Block_copy (block_ptr_handler);
				block_handler.CleanupBlock ();
				return dispatch_block_create_with_qos_class (flags, qosClass, relative_priority, ret);
			}
		}

		public DispatchBlock CreateWithQos (DispatchBlockFlags flags, QosClass qosClass, int relative_priority, DispatchBlock dispatchBlock)
		{
			return dispatch_block_create_with_qos_class (flags, qosClass, relative_priority, dispatchBlock);
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_block_cancel (DispatchBlock block);
		
		public void Cancel ()
		{
			dispatch_block_cancel (this);
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_block_notify (DispatchBlock block, IntPtr queue, DispatchBlock notification);
		
		public void Notify (DispatchQueue queue, DispatchBlock notification)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			
			dispatch_block_notify (this, queue.Handle, notification);
		}
		
		[DllImport (Constants.libcLibrary)]
		extern static long dispatch_block_testcancel(DispatchBlock block);

		public long TestCancel ()
		{
			return dispatch_block_testcancel (this);
		}

		[DllImport (Constants.libcLibrary)]
		extern static long dispatch_block_wait (DispatchBlock block, DispatchTime time);

		public void Wait (DispatchTime time)
		{
			dispatch_block_wait (this, time);
		}
		
		//
		// You must invoke ->CleanupBlock after you have transferred ownership to
		// the unmanaged code to release the resources allocated on the managed side
		//
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static unsafe void Invoke (Action codeToRun, Action<IntPtr> invoker)
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
	public enum DispatchBlockFlags : ulong {
		Barrier = 1,
		Detached = 2,
		AssignCurrent = 4,
		NoQosClass = 8,
		InheritQosClass = 16,
		EnforceQosClass = 32
	}
#endif // !COREBUILD
}
