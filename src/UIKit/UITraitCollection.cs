// 
// UITraitCollection.cs: support for UITraitCollection
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;

namespace UIKit {
#if !WATCH
	public partial class UITraitCollection {
#if !XAMCORE_4_0
		[Obsolete ("Please use the static method instead.")]
		public UITraitCollection FromPreferredContentSizeCategory (UIContentSizeCategory category)
		{
			return FromPreferredContentSizeCategory (category.GetConstant ());
		}
#endif

		public static UITraitCollection FromPreferredContentSizeCategory (UIContentSizeCategory category)
		{
			return FromPreferredContentSizeCategory (category.GetConstant ());
		}
	}
#endif
}
