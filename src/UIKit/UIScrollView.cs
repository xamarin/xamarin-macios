//
// UIScrollView.cs: Extensions to UIScrollView
//
// Authors:
//   Geoff Norton
//
// Copyright 2009, Novell, Inc.
//

#if !WATCH

using System;

namespace UIKit {
	public partial class DraggingEventArgs : EventArgs {
		public readonly static DraggingEventArgs True, False;

		static DraggingEventArgs ()
		{
			True = new DraggingEventArgs (true);
			False = new DraggingEventArgs (false);
		}
	}
}

#endif // !WATCH
