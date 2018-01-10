#if !XAMCORE_2_0

// Copyright 2012 Xamarin Inc. All rights reserved.

using System;

namespace UIKit {

	public partial class UIDocumentInteractionController {

		[Obsolete ("Use DismissPreview (typo) method instead")]
		// binary compatibility (linker will remove it)
		public virtual void DimissPreview (bool animated)
		{
			DismissPreview (animated);
		}
	}
}

#endif
