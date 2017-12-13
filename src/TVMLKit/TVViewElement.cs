using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.TVMLKit {

	public class TVViewElementDispatchResult {

#if !COREBUILD
		public TVViewElementDispatchResult (bool isDispatched, bool isCancelled)
		{
			IsDispatched = isDispatched;
			IsCancelled = isCancelled;
		}

		public bool IsDispatched { get; set; }

		public bool IsCancelled { get; set; }
#endif
	}
}
