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

namespace SceneKit {
#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class SCNJavaScript {
		[DllImport (Constants.SceneKitLibrary)]
		static extern void SCNExportJavaScriptModule (IntPtr context);

		public static void ExportModule (JSContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));

			SCNExportJavaScriptModule (context.Handle);
		}
	}
}

#endif
