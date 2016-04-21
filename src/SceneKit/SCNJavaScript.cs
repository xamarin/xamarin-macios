//
// SCNJavaScript.cs: JSC bridge
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
using XamCore.JavaScriptCore;

namespace XamCore.SceneKit
{
#if XAMCORE_2_0 || !MONOMAC
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	public static class SCNJavaScript
	{
		[DllImport (Constants.SceneKitLibrary)]
		static extern void SCNExportJavaScriptModule (IntPtr context);

		public static void ExportModule (JSContext context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");

			SCNExportJavaScriptModule (context.Handle);
		}
	}
#endif
}
