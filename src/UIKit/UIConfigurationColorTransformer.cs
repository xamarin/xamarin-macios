//
// UIConfigurationColorTransformer.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

#nullable enable
#if !WATCH

namespace UIKit {

	public static partial class UIConfigurationColorTransformer {

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static UIConfigurationColorTransformerHandler Grayscale {
			[return: DelegateProxy (typeof (SDUIConfigurationColorTransformerHandler))]
			get => NIDUIConfigurationColorTransformerHandler.Create (_Grayscale)!;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static UIConfigurationColorTransformerHandler PreferredTint {
			[return: DelegateProxy (typeof (SDUIConfigurationColorTransformerHandler))]
			get => NIDUIConfigurationColorTransformerHandler.Create (_PreferredTint)!;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static UIConfigurationColorTransformerHandler MonochromeTint {
			[return: DelegateProxy (typeof (SDUIConfigurationColorTransformerHandler))]
			get => NIDUIConfigurationColorTransformerHandler.Create (_MonochromeTint)!;
		}
	} /* class UIConfigurationColorTransformer */

	[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
	[UserDelegateType (typeof (UIConfigurationColorTransformerHandler))]
	internal delegate IntPtr DUIConfigurationColorTransformerHandler (IntPtr block, IntPtr color);

	//
	// This class bridges native block invocations that call into C#
	//
	static internal class SDUIConfigurationColorTransformerHandler {
#if !NET
		static internal readonly DUIConfigurationColorTransformerHandler Handler = Invoke;

		[MonoPInvokeCallback (typeof (DUIConfigurationColorTransformerHandler))]
#else
		[UnmanagedCallersOnly]
#endif
		static unsafe IntPtr Invoke (IntPtr block, IntPtr color)
		{
			var descriptor = (BlockLiteral*) block;
			var del = (UIConfigurationColorTransformerHandler) (descriptor->Target);
			var retval = del is null ? null : del (Runtime.GetNSObject<UIColor> (color)!);
			return retval.GetHandle ();
		}
	} /* class SDUIConfigurationColorTransformerHandler */

	internal sealed class NIDUIConfigurationColorTransformerHandler : TrampolineBlockBase {
		DUIConfigurationColorTransformerHandler invoker;

		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe NIDUIConfigurationColorTransformerHandler (BlockLiteral* block) : base (block)
		{
			invoker = block->GetDelegateForBlock<DUIConfigurationColorTransformerHandler> ();
		}

		[Preserve (Conditional = true)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static UIConfigurationColorTransformerHandler? Create (IntPtr block)
		{
			if (block == IntPtr.Zero)
				return null;
			var del = (UIConfigurationColorTransformerHandler) GetExistingManagedDelegate (block);
			return del ?? new NIDUIConfigurationColorTransformerHandler ((BlockLiteral*) block).Invoke;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		UIColor Invoke (UIColor color)
		{
			return Runtime.GetNSObject<UIColor> (invoker (BlockPointer, color.GetHandle ()))!;
		}
	} /* class NIDUIConfigurationColorTransformerHandler */
}
#endif // WATCH
