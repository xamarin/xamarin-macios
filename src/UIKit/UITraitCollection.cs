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

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {
#if !WATCH
	public partial class UITraitCollection {

#if !NET
		[Obsolete ("Please use the static 'Create' method instead.")]
		public UITraitCollection FromPreferredContentSizeCategory (UIContentSizeCategory category)
			=> Create (category);
#endif

		public static UITraitCollection Create (UIContentSizeCategory category)
			=> FromPreferredContentSizeCategory (category.GetConstant ());
	}
#endif
}
