//
// NSThread.cs: extensions for NSThread
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc

#if MONOMAC

using System;

namespace XamCore.Foundation 
{
	public partial class NSThread
	{
		class ActionThread : NSThread
		{
			NSAction action;
		
			public ActionThread (NSAction action)
			{
				this.action = action;
			}
	
			public override void Main ()
			{
				action ();
			}
		}

		public static NSThread Start (NSAction action)
		{
			if (action == null) {
				throw new ArgumentNullException ("action");
			}

			var thread = new ActionThread (action);
			thread.Start ();
			return thread;
		}
	}
}

#endif // MONOMAC
