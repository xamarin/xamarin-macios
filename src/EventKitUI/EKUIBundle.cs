//
// EventKitUIBundle C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace EventKitUI {
	[iOS (11,0)]
	public static class EKUIBundle {

		[DllImport (Constants.EventKitUILibrary)]
		static extern IntPtr EventKitUIBundle ();

		public static NSBundle UIBundle {
			get {
				return Runtime.GetNSObject<NSBundle> (EventKitUIBundle ());
			}
		}
	}
}
