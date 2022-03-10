//
// SCNJavaScript.cs: JSC bridge
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !WATCH

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using JavaScriptCore;

#nullable enable

namespace SceneKit
{
#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#else
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	public static class SCNJavaScript
	{
		[DllImport (Constants.SceneKitLibrary)]
		static extern void SCNExportJavaScriptModule (IntPtr context);

		public static void ExportModule (JSContext context)
		{
			if (context == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));

			SCNExportJavaScriptModule (context.Handle);
		}
	}
}

#endif
