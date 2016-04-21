// Copyright 2015 Xamarin Inc. All rights reserved.

#if IOS

using System;
using XamCore.Foundation;

namespace XamCore.CoreSpotlight {

	public partial class CSSearchableItemAttributeSet {

		public INSSecureCoding this [CSCustomAttributeKey key] {
			get {
				return ValueForCustomKey (key);
			}
			set {
				SetValue (value, key);
			}
		}
	}
}

#endif
