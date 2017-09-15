using System;
using XamCore.ObjCRuntime;

namespace XamCore.HomeKit {

	partial class HMMutableSignificantTimeEvent {

		public virtual HMSignificantEvent SignificantEvent {
			get {
				return (HMSignificantEvent) (HMSignificantEventExtensions.GetValue (_SignificantEvent));
			}
			set {
				_SignificantEvent = HMSignificantEventExtensions.GetConstant (value);
			}
		}
	}
}
