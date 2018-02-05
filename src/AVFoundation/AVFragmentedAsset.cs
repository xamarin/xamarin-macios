//
// Copyright 2015 Xamarin Inc
//

using System;
using Foundation;

namespace AVFoundation {
#if MONOMAC && !XAMCORE_3_0
	public partial class AVFragmentedAsset {
		[Obsolete ("Default constructor should not be used")]
		public AVFragmentedAsset () : base (NSObjectFlag.Empty)
		{
		}
	}
#endif
}
