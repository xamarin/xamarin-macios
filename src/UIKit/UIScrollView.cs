//
// UIScrollView.cs: Extensions to UIScrollView
//
// Authors:
//   Geoff Norton
//
// Copyright 2009, Novell, Inc.
//

using System;

namespace UIKit {
	public partial class DraggingEventArgs : EventArgs {
		public readonly static DraggingEventArgs True;
		public readonly static DraggingEventArgs False;

		static DraggingEventArgs ()
		{
			True = new DraggingEventArgs (true);
			False = new DraggingEventArgs (false);
		}
	}
}
