//
// This file implements the NSUndoManager interfase
//
// Authors:
//   Paola Villarreal
//
// Copyright 2015 Xamarin Inc.
//
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Foundation {
	public partial class NSUndoManager {
		public virtual void SetActionName (string actionName) {
			SetActionname (actionName);
		}
	}
}
