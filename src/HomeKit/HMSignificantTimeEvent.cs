using System;
using XamCore.ObjCRuntime;

namespace XamCore.HomeKit {

	partial class HMSignificantTimeEvent {

		public virtual HMSignificantEvent SignificantEvent {
			get {
				return (HMSignificantEvent) (HMSignificantEventExtensions.GetValue (WeakSignificantEvent));
			}
			set {
				WeakSignificantEvent = HMSignificantEventExtensions.GetConstant (value);
			}
		}
	}
}
