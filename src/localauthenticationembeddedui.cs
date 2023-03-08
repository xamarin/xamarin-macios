//
// LocalAuthenticationEmbeddedUI C# bindings
//
// Authors:
//	Rachel Kang  <rachelkang@microsoft.com>
//
// Copyright 2021 Microsoft Corporation All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;
using AppKit;
using CoreGraphics;
using LocalAuthentication;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace LocalAuthenticationEmbeddedUI {

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12, 0)]
	[BaseType (typeof (NSView))]
	interface LAAuthenticationView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frameRect);

		[Export ("initWithContext:")]
		NativeHandle Constructor (LAContext context);

		[Export ("initWithContext:controlSize:")]
		NativeHandle Constructor (LAContext context, NSControlSize controlSize);

		[Export ("context")]
		LAContext Context { get; }

		[Export ("controlSize")]
		NSControlSize ControlSize { get; }
	}
}
