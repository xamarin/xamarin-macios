// Copyright 2015 Xamarin Inc. All rights reserved.

#if IOS

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


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

		// Manually deal with these properties until we get BindAs working
		[iOS (11,0), NoTV, Mac (10, 11)]
		public bool? IsUserCreated { 
			get {
				return _IsUserCreated?.BoolValue;
			} set {
				_IsUserCreated = value.HasValue ? new NSNumber (value.Value) : null;
			}
		}

		[iOS (11, 0), NoTV, Mac (10, 11)]
		public bool? IsUserOwned {
			get {
				return _IsUserOwned?.BoolValue;
			} set {
				_IsUserOwned = value.HasValue ? new NSNumber (value.Value) : null;
			}
		}

		[iOS (11, 0), NoTV, Mac (10, 11)]
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
