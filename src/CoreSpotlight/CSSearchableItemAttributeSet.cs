// Copyright 2015 Xamarin Inc. All rights reserved.

#nullable enable

#if IOS || MONOMAC

using System;
using Foundation;
using ObjCRuntime;

namespace CoreSpotlight {

	public partial class CSSearchableItemAttributeSet {

		public INSSecureCoding? this [CSCustomAttributeKey key] {
			get {
				return ValueForCustomKey (key);
			}
			set {
				SetValue (value, key);
			}
		}

		// Manually deal with these properties until we get BindAs working
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public bool? IsUserCreated { 
			get {
				return _IsUserCreated?.BoolValue;
			} set {
				_IsUserCreated = value.HasValue ? new NSNumber (value.Value) : null;
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public bool? IsUserOwned {
			get {
				return _IsUserOwned?.BoolValue;
			} set {
				_IsUserOwned = value.HasValue ? new NSNumber (value.Value) : null;
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public bool? IsUserCurated { 
			get {
				return _IsUserCurated?.BoolValue;
			} set {
				_IsUserCurated = value.HasValue ? new NSNumber (value.Value) : null;
			}
		}
	}
}

#endif
