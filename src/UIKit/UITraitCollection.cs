// 
// UITraitCollection.cs: support for UITraitCollection
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;

using ObjCRuntime;
using Foundation;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {
	public partial class UITraitCollection {
		public static UITraitCollection Create (UIContentSizeCategory category)
			=> FromPreferredContentSizeCategory (category.GetConstant ());

#if !XAMCORE_5_0
		[Obsolete ("Use the overload that takes a 'UITraitMutations' parameter instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public virtual UITraitCollection GetTraitCollectionByModifyingTraits (Func<IUIMutableTraits> mutations)
		{
			// there's nothing useful this method can do.
			throw new NotSupportedException ($"Use the overload that takes a 'UITraitMutations' parameter instead.");
		}
#endif
	}
}
