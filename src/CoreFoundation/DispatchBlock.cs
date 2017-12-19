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
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.CoreFoundation {
#if !COREBUILD
	internal class DispatchBlock {

		//
		// You must invoke ->CleanupBlock after you have transferred ownership to
		// the unmanaged code to release the resources allocated on the managed side
		//
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
#endif // !COREBUILD
}
