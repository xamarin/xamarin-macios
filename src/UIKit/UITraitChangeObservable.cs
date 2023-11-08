//
// UITraitChangeObservable.cs: support for IUITraitChangeObservable
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace UIKit {
	public partial interface IUITraitChangeObservable {
		static Class [] ToClasses (params Type [] traits)
		{
			if (traits is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (traits));
			var traitsClasses = new Class [traits.Length];
			for (var i = 0; i < traits.Length; i++)
				traitsClasses [i] = new Class (traits [i]);
			return traitsClasses;
		}

		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return RegisterForTraitChanges (ToClasses (traits), handler);
		}

		// Add an override with 'params', unfortunately this means reordering the parameters.
		public unsafe IUITraitChangeRegistration RegisterForTraitChanges (Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
		{
			return RegisterForTraitChanges (ToClasses (traits), handler);
		}

		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T : IUITraitDefinition
		{
			return RegisterForTraitChanges (ToClasses (typeof (T)), handler);
		}

		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T1, T2> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
		{
			return RegisterForTraitChanges (ToClasses (typeof (T1), typeof (T2)), handler);
		}

		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T1, T2, T3> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
		{
			return RegisterForTraitChanges (ToClasses (typeof (T1), typeof (T2), typeof (T3)), handler);
		}

		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T1, T2, T3, T4> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
			where T4 : IUITraitDefinition
		{
			return RegisterForTraitChanges (ToClasses (typeof (T1), typeof (T2), typeof (T3), typeof (T4)), handler);
		}

		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, NSObject target, Selector action)
		{
			return RegisterForTraitChanges (ToClasses (traits), target, action);
		}

		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, Selector action)
		{
			return RegisterForTraitChanges (ToClasses (traits), action);
		}

#if !XAMCORE_5_0
		[BindingImpl (BindingImplOptions.Optimizable)]
#if !NET
		public IUITraitChangeRegistration RegisterForTraitChanges (Class [] traits, global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			// The manual block code is somewhat annoying to implement, so at least don't do it twice (once for .NET and once for legacy Xamarin) unless we really need to.
			throw new NotImplementedException ("This API has not been implemented for legacy Xamarin. Please upgrade to .NET");
		}
#else
		public unsafe IUITraitChangeRegistration RegisterForTraitChanges (Class [] traits, [BlockProxy (typeof (ObjCRuntime.Trampolines.IUITraitChangeObservable_RegisterForTraitChanges_NIDAction))] global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			if (traits is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (traits));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));
			using var nsa_traits = NSArray.FromNSObjects (traits);
			using var block_handler = Trampolines.IUITraitChangeObservable_RegisterForTraitChanges_SDAction.CreateBlock (handler);
			return Runtime.GetINativeObject<IUITraitChangeRegistration> (NativeHandle_objc_msgSend_NativeHandle_BlockLiteral (this.Handle, Selector.GetHandle ("registerForTraitChanges:withHandler:"), nsa_traits.Handle, &block_handler), false)!;
		}
#endif

		[BindingImpl (BindingImplOptions.Optimizable)]
		public IUITraitChangeRegistration RegisterForTraitChanges (Class [] traits, NSObject target, Selector action)
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			if (traits is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (traits));
			var target__handle__ = target!.GetNonNullHandle (nameof (target));
			var action__handle__ = action!.GetNonNullHandle (nameof (action));
			using var nsa_traits = NSArray.FromNSObjects (traits);
			return Runtime.GetINativeObject<IUITraitChangeRegistration> (NativeHandle_objc_msgSend_NativeHandle_NativeHandle_NativeHandle (this.Handle, Selector.GetHandle ("registerForTraitChanges:withTarget:action:"), nsa_traits.Handle, target__handle__, action.Handle), false)!;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public IUITraitChangeRegistration RegisterForTraitChanges (Class [] traits, Selector action)
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			if (traits is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (traits));
			var action__handle__ = action!.GetNonNullHandle (nameof (action));
			using var nsa_traits = NSArray.FromNSObjects (traits);
			return Runtime.GetINativeObject<IUITraitChangeRegistration> (NativeHandle_objc_msgSend_NativeHandle_NativeHandle (this.Handle, Selector.GetHandle ("registerForTraitChanges:withAction:"), nsa_traits.Handle, action.Handle), false)!;
		}

		static Class [] ToClasses (IUITraitDefinition [] traits)
		{
			if (traits is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (traits));
			var traitsClasses = new Class [traits.Length];
			for (var i = 0; i < traits.Length; i++)
				traitsClasses [i] = new Class (traits [i].GetType ());
			return traitsClasses;
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Action<IUITraitEnvironment, UITraitCollection>)' method instead.")]
		public IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return RegisterForTraitChanges (ToClasses (traits), handler);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], NSObject, Selector)' method instead.")]
		public IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, NSObject target, Selector action)
		{
			return RegisterForTraitChanges (ToClasses (traits), target, action);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Selector)' method instead.")]
		public IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Selector action)
		{
			return RegisterForTraitChanges (ToClasses (traits), action);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static NativeHandle NativeHandle_objc_msgSend_NativeHandle_NativeHandle (IntPtr receiver, IntPtr selector, NativeHandle arg1, NativeHandle arg2);

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern unsafe static NativeHandle NativeHandle_objc_msgSend_NativeHandle_BlockLiteral (IntPtr receiver, IntPtr selector, NativeHandle arg1, BlockLiteral* arg2);

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static NativeHandle NativeHandle_objc_msgSend_NativeHandle_NativeHandle_NativeHandle (IntPtr receiver, IntPtr selector, NativeHandle arg1, NativeHandle arg2, NativeHandle arg3);
#endif // !XAMCORE_5_0
	}
}

#if !XAMCORE_5_0 && NET
namespace ObjCRuntime {
	using UIKit;
	static partial class Trampolines {
		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		[UserDelegateType (typeof (global::System.Action<global::UIKit.IUITraitEnvironment, global::UIKit.UITraitCollection>))]
		unsafe internal delegate void IUITraitChangeObservable_RegisterForTraitChanges_Callback (IntPtr block, NativeHandle arg1, NativeHandle arg2);

		static internal class IUITraitChangeObservable_RegisterForTraitChanges_SDAction {
			[Preserve (Conditional = true)]
			[UnmanagedCallersOnly]
			internal static unsafe void Invoke (IntPtr block, NativeHandle arg1, NativeHandle arg2)
			{
				var del = BlockLiteral.GetTarget<global::System.Action<global::UIKit.IUITraitEnvironment, global::UIKit.UITraitCollection>> (block);
				if (del is not null) {
					del (Runtime.GetINativeObject<UIKit.IUITraitEnvironment> (arg1, false)!, Runtime.GetNSObject<UITraitCollection> (arg2)!);
				}
			}
			internal static unsafe BlockLiteral CreateNullableBlock (global::System.Action<global::UIKit.IUITraitEnvironment, global::UIKit.UITraitCollection>? callback)
			{
				if (callback is null)
					return default (BlockLiteral);
				return CreateBlock (callback);
			}
			[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
			internal static unsafe BlockLiteral CreateBlock (global::System.Action<global::UIKit.IUITraitEnvironment, global::UIKit.UITraitCollection> callback)
			{
				delegate* unmanaged<IntPtr, NativeHandle, NativeHandle, void> trampoline = &Invoke;
				return new BlockLiteral (trampoline, callback, typeof (IUITraitChangeObservable_RegisterForTraitChanges_SDAction), nameof (Invoke));
			}
		}

		internal sealed class IUITraitChangeObservable_RegisterForTraitChanges_NIDAction : TrampolineBlockBase {
			IUITraitChangeObservable_RegisterForTraitChanges_Callback invoker;
			[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
			public unsafe IUITraitChangeObservable_RegisterForTraitChanges_NIDAction (BlockLiteral* block) : base (block)
			{
				invoker = block->GetDelegateForBlock<IUITraitChangeObservable_RegisterForTraitChanges_Callback> ();
			}
			[Preserve (Conditional = true)]
			[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
			public unsafe static global::System.Action<global::UIKit.IUITraitEnvironment, global::UIKit.UITraitCollection>? Create (IntPtr block)
			{
				if (block == IntPtr.Zero)
					return null;
				var del = (global::System.Action<global::UIKit.IUITraitEnvironment, global::UIKit.UITraitCollection>) GetExistingManagedDelegate (block);
				return del ?? new IUITraitChangeObservable_RegisterForTraitChanges_NIDAction ((BlockLiteral*) block).Invoke;
			}
			[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
			unsafe void Invoke (global::UIKit.IUITraitEnvironment arg1, global::UIKit.UITraitCollection arg2)
			{
				var arg1__handle__ = arg1.GetHandle ();
				var arg2__handle__ = arg2.GetHandle ();
				invoker (BlockPointer, arg1__handle__, arg2__handle__);
			}
		}
	}
}
#endif // !XAMCORE_5_0 && NET

#endif // !__WATCHOS__
