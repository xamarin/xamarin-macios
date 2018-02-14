//
// EventKitUIBundle C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace EventKitUI {
	public static class EKUIBundle {

		[iOS (11,0)]
		[DllImport (Constants.EventKitUILibrary)]
		static extern IntPtr EventKitUIBundle ();

		[iOS (11,0)]
		public static NSBundle UIBundle { get; } = Runtime.GetNSObject<NSBundle> (EventKitUIBundle ());
	}
}
#endif
