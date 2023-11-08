// 
// UITraitOverrides.cs: support for UITraitOverrides
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

#if !__WATCHOS__

using System;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace UIKit {
	public partial interface IUITraitOverrides {
		[BindingImpl (BindingImplOptions.Optimizable)]
		public sealed bool ContainsTrait<T> () where T : IUITraitDefinition
		{
			return ContainsTrait (typeof (T));
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public sealed void RemoveTrait<T> () where T : IUITraitDefinition
		{
			RemoveTrait (typeof (T));
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public sealed bool ContainsTrait (Type trait)
		{
			return ContainsTrait (new Class (trait));
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public sealed void RemoveTrait (Type trait)
		{
			RemoveTrait (new Class (trait));
		}

#if !XAMCORE_5_0
		[BindingImpl (BindingImplOptions.Optimizable)]
		public sealed bool ContainsTrait (Class trait)
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			var trait__handle__ = trait!.GetNonNullHandle (nameof (trait));
#if NET
			var ret = global::ObjCRuntime.Messaging.bool_objc_msgSend_NativeHandle (this.Handle, Selector.GetHandle ("containsTrait:"), trait__handle__);
#else
			var ret = global::ObjCRuntime.Messaging.bool_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("containsTrait:"), trait__handle__);
#endif
			return ret != 0;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public sealed void RemoveTrait (Class trait)
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			var trait__handle__ = trait!.GetNonNullHandle (nameof (trait));
#if NET
			global::ObjCRuntime.Messaging.void_objc_msgSend_NativeHandle (this.Handle, Selector.GetHandle ("removeTrait:"), trait__handle__);
#else
			global::ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("removeTrait:"), trait__handle__);
#endif
		}
#endif // !XAMCORE_5_0
	}
}

#endif // !__WATCHOS__
