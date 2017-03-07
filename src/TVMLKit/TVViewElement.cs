using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

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
