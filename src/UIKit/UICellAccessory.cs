//
// UICellAccessory.cs
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

	public partial class UICellAccessory {

		[Watch (7,0), TV (14,0), iOS (14,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern IntPtr UICellAccessoryPositionBeforeAccessoryOfClass (IntPtr accessoryCls);

		[Watch (7,0), TV (14,0), iOS (14,0)]
		[return: DelegateProxy (typeof (SDUICellAccessoryPosition))]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static UICellAccessoryPosition GetPositionBeforeAccessory (Class accessoryClass)
		{
			if (accessoryClass == null)
				throw new ArgumentNullException ("accessoryClass");
			var ret = UICellAccessoryPositionBeforeAccessoryOfClass (accessoryClass.Handle);
			return NIDUICellAccessoryPosition.Create (ret)!;
		}
		
		[Watch (7,0), TV (14,0), iOS (14,0)]
		[return: DelegateProxy (typeof (SDUICellAccessoryPosition))]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static UICellAccessoryPosition GetPositionBeforeAccessory (Type accessoryType) => GetPositionBeforeAccessory (new Class (accessoryType));

		[Watch (7,0), TV (14,0), iOS (14,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern IntPtr UICellAccessoryPositionAfterAccessoryOfClass (IntPtr accessoryCls);

		[Watch (7,0), TV (14,0), iOS (14,0)]
		[return: DelegateProxy (typeof (SDUICellAccessoryPosition))]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static UICellAccessoryPosition GetPositionAfterAccessory (Class accessoryClass)
		{
			if (accessoryClass == null)
				throw new ArgumentNullException ("accessoryClass");
			var ret = UICellAccessoryPositionAfterAccessoryOfClass (accessoryClass.Handle);
			return NIDUICellAccessoryPosition.Create (ret)!;
		}

		[Watch (7,0), TV (14,0), iOS (14,0)]
		[return: DelegateProxy (typeof (SDUICellAccessoryPosition))]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static UICellAccessoryPosition GetPositionAfterAccessory (Type accessoryType) => GetPositionAfterAccessory (new Class (accessoryType));
	} /* class UICellAccessory */

	[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
	[UserDelegateType (typeof (UICellAccessoryPosition))]
	internal delegate nuint DUICellAccessoryPosition (IntPtr block, IntPtr accessories);
	
	//
	// This class bridges native block invocations that call into C#
	//
	static internal class SDUICellAccessoryPosition {
		static internal readonly DUICellAccessoryPosition Handler = Invoke;
		
		[MonoPInvokeCallback (typeof (DUICellAccessoryPosition))]
		static unsafe nuint Invoke (IntPtr block, IntPtr accessories) {
			var descriptor = (BlockLiteral *) block;
			var del = (UICellAccessoryPosition) (descriptor->Target);
			nuint retval = del (NSArray.ArrayFromHandle<UICellAccessory> (accessories));
			return retval;
		}
	} /* class SDUICellAccessoryPosition */

	internal sealed class NIDUICellAccessoryPosition : TrampolineBlockBase {
		DUICellAccessoryPosition invoker;
		
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe NIDUICellAccessoryPosition (BlockLiteral *block) : base (block)
		{
			invoker = block->GetDelegateForBlock<DUICellAccessoryPosition> ();
		}
		
		[Preserve (Conditional = true)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static UICellAccessoryPosition? Create (IntPtr block)
		{
			if (block == IntPtr.Zero)
				return null;
			var del = (UICellAccessoryPosition) GetExistingManagedDelegate (block);
			return del ?? new NIDUICellAccessoryPosition ((BlockLiteral *) block).Invoke;
		}
		
		[BindingImpl (BindingImplOptions.Optimizable)]
		unsafe nuint Invoke (UICellAccessory [] accessories)
		{
			var nsa_accessories = accessories == null ? null : NSArray.FromNSObjects (accessories);
			
			var ret = invoker (BlockPointer, nsa_accessories == null ? IntPtr.Zero : nsa_accessories.Handle);
			if (nsa_accessories != null)
				nsa_accessories.Dispose ();
			
			return ret;
		}
	} /* class NIDUICellAccessoryPosition */
}
#endif // WATCH
