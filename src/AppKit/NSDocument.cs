using System;
using System.Collections.Generic;

using Foundation;
using ObjCRuntime;

namespace AppKit {
		
	public partial class NSDocument {
		public delegate void DuplicateCallback (NSDocument document, bool didDuplicate);

		[Register ("__NSDocumentDuplicateCallback")]
		internal class Callback : NSObject {
			DuplicateCallback callback;
			
			public Callback (DuplicateCallback callback)
			{
				this.callback = callback;
				IsDirectBinding = false;
				DangerousRetain ();
			}
			
			[Export ("document:didDuplicate:contextInfo:")]
			void SelectorCallback (NSDocument source, bool didDuplicate, IntPtr contextInfo)
			{
				try {
					callback (source, didDuplicate);
				} finally {
					DangerousRelease ();
				}
			}
		}
		
		public void DuplicateDocument (DuplicateCallback callback)
		{
			if (callback == null) {
				_DuplicateDocument (null, null, IntPtr.Zero);
			} else {
				_DuplicateDocument (new Callback (callback), new Selector ("document:didDuplicate:contextInfo:"), IntPtr.Zero);
			}
		}
	}
}