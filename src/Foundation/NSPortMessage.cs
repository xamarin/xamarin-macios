// Copyright 2014 Xamarin Inc. All rights reserved.

#if !MONOMAC && !XAMCORE_2_0
using System;

namespace MonoTouch.Foundation {

	public partial class NSPortMessage {
		// Apple will reject iOS application using those selectors - as they only exists in OSX
		// However we exposed them in monotouch.dll so we provide stubs for binary compatibility

		[Obsolete ("Only available on OSX")]
		public virtual uint MsgId { 
			get { throw new NotSupportedException (); }
			set { throw new NotSupportedException (); }
		}

		[Obsolete ("Only available on OSX")]
		public virtual NSPort ReceivePort {
			get { throw new NotSupportedException (); }
		}

		[Obsolete ("Only available on OSX")]
		public virtual NSPort SendPort {
			get { throw new NotSupportedException (); }
		}

		[Obsolete ("Only available on OSX")]
		public virtual bool SendBefore (NSDate date)
		{
			throw new NotSupportedException ();
		}
	}
}

#endif