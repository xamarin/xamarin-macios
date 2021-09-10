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

namespace LocalAuthenticationEmbeddedUI {
    
    [NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
    [BaseType (typeof (NSView))]
    interface LAAuthenticationView
    {
        [Export ("initWithFrame:")]
        IntPtr Constructor (CGRect frameRect);

        [Export ("initWithContext:")]
        IntPtr Constructor (LAContext context);

        [Export ("initWithContext:controlSize:")]
        IntPtr Constructor (LAContext context, NSControlSize controlSize);

        [Export ("context")]
        LAContext Context { get; }

        [Export ("controlSize")]
        NSControlSize ControlSize { get; }
    }
}
