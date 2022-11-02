//
// BusinessChat bindings
//
// Authors:
//     Manuel de la Peña <mandel@microsoft.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//
using System;
using System.ComponentModel;

using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
using UIControl = AppKit.NSControl;
#else
using UIKit;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace BusinessChat {

	[Mac (10,13,4), iOS (11,3)]
	[BaseType (typeof(UIControl))]
	[DisableDefaultCtor]
	interface BCChatButton {
		[Export ("initWithStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (BCChatButtonStyle style);
	}


	[Mac (10,13,4), iOS (11,3)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BCChatAction {

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("openTranscript:intentParameters:")]
		void OpenTranscript (string businessIdentifier, NSDictionary<NSString, NSString> intentParameters);
	}
}
