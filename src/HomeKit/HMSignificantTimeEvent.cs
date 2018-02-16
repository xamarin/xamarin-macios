using System;
using ObjCRuntime;

namespace HomeKit {

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
