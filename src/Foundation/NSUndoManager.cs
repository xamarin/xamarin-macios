//
// This file implements the NSUndoManager interfase
//
// Authors:
//   Paola Villarreal
//
// Copyright 2015 Xamarin Inc.
//
//

#nullable enable

using System;
using Foundation;
using ObjCRuntime;

namespace Foundation {
	public partial class NSUndoManager {
#if !NET
		public virtual void SetActionName (string actionName)
		{
			SetActionname (actionName);
		}
#endif

#if NET
		public NSRunLoopMode [] RunLoopModes {
			get {
				var modes = WeakRunLoopModes;
				if (modes is null)
					return Array.Empty<NSRunLoopMode> ();

				var array = new NSRunLoopMode [modes.Length];
				for (int n = 0; n < modes.Length; n++)
					array [n] = NSRunLoopModeExtensions.GetValue (modes [n]);
				return array;
			}
			set {
				WeakRunLoopModes = value?.GetConstants ()!;
			}
		}
#endif
	}
}
