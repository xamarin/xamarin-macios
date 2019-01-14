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

	[StructLayout (LayoutKind.Sequential)]
	public class DispatchBlock : IDisposable {
		IntPtr blockHandle;

		internal DispatchBlock (IntPtr handle)
		{
			this.blockHandle = handle;
		}
				
		[DllImport (Messaging.LIBOBJC_DYLIB)]
		unsafe internal static extern IntPtr _Block_copy (BlockLiteral *block_literal);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		unsafe internal static extern void _Block_release (IntPtr blockHandle);
		
		public DispatchBlock Create (Action action)
		{
			unsafe {
			        BlockLiteral block_handler = new BlockLiteral ();
			        BlockLiteral *block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				DispatchBlock ret = new DispatchBlock (_Block_copy (block_ptr_handler));
				block_handler.CleanupBlock ();
				return ret;
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_block_create (DispatchBlockFlags flags, IntPtr block);
		
		public DispatchBlock Create (Action action, DispatchBlockFlags flags)
		{
			unsafe {
			        BlockLiteral block_handler = new BlockLiteral ();
			        BlockLiteral *block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				var ret =  _Block_copy (block_ptr_handler);
				block_handler.CleanupBlock ();
				return new DispatchBlock (dispatch_block_create (flags, ret));
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_block_create_with_qos_class (DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority, IntPtr dispatchBlock);

		public DispatchBlock CreateWithQos (DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority, Action action)
		{
			unsafe {
			        BlockLiteral block_handler = new BlockLiteral ();
			        BlockLiteral *block_ptr_handler = &block_handler;
			        block_handler.SetupBlockUnsafe (BlockStaticDispatchClass.static_dispatch_block, action);
				var ret = _Block_copy (block_ptr_handler);
				block_handler.CleanupBlock ();
				return new DispatchBlock (dispatch_block_create_with_qos_class (flags, qosClass, relative_priority, ret));
			}
		}

		public DispatchBlock CreateWithQos (DispatchBlockFlags flags, DispatchQualityOfService qosClass, int relative_priority, DispatchBlock dispatchBlock)
		{
			if (dispatchBlock == null)
				throw new ArgumentNullException (nameof (dispatchBlock));
			return new DispatchBlock (dispatch_block_create_with_qos_class (flags, qosClass, relative_priority, dispatchBlock.blockHandle));
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_block_cancel (IntPtr block);
		
		public void Cancel ()
		{
			dispatch_block_cancel (blockHandle);
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_block_notify (IntPtr block, IntPtr queue, IntPtr notification);
		
		public void Notify (DispatchQueue queue, DispatchBlock notification)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));
			if (notification == null)
				throw new ArgumentNullException (nameof (notification));
			dispatch_block_notify (blockHandle, queue.Handle, notification.blockHandle);
		}
		
		[DllImport (Constants.libcLibrary)]
		extern static long dispatch_block_testcancel(IntPtr block);

		public long TestCancel ()
		{
			return dispatch_block_testcancel (blockHandle);
		}

		[DllImport (Constants.libcLibrary)]
		extern static long dispatch_block_wait (IntPtr block, DispatchTime time);

		public void Wait (DispatchTime time)
		{
			dispatch_block_wait (blockHandle, time);
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

		~DispatchBlock ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
		        GC.SuppressFinalize(this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (blockHandle != IntPtr.Zero){
				_Block_release (blockHandle);
				blockHandle = IntPtr.Zero;
			}
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
