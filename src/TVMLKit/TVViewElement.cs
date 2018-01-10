using System;
using Foundation;
using ObjCRuntime;

namespace TVMLKit {

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
