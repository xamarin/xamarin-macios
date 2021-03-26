#if IOS
using System;

using Foundation;
using ObjCRuntime;
using UIKit;

namespace MetricKit {

	public partial class MXMetricPayload {

		public virtual NSDictionary DictionaryRepresentation {
			get {
				if (UIDevice.CurrentDevice.CheckSystemVersion (14,0))
					return _DictionaryRepresentation14;
				else
					return _DictionaryRepresentation13;
			}
		}
	}
}
#endif
