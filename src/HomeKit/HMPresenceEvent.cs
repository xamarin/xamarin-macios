using System;
using XamCore.ObjCRuntime;

namespace XamCore.HomeKit {

	partial class HMPresenceEvent {

		public virtual HMPresenceType PresenceType {
			get {
				return (HMPresenceType) (HMPresenceTypeExtensions.GetValue (_PresenceType));
			}
			set {
				_PresenceType = HMPresenceTypeExtensions.GetConstant (value);
			}
		}
	}
}
