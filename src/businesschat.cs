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

using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.UIKit;
#endif

namespace XamCore.BusinessChat {
	[Mac (10,13, onlyOn64: true), iOS (11,3)]
#if MONOMAC
	[BaseType (typeof(NSControl))]
#else
	[BaseType (typeof(UIControl))]
#endif
	interface BCChatButton {
		[Mac (10,13,4, onlyOn64: true), iOS (11,3)]
		[Export ("initWithStyle:")]
		[DesignatedInitializer]
		IntPtr Constructor (BCChatButtonStyle style);
	}


	[Mac (10,13, onlyOn64: true), iOS (11,3)]
	[BaseType (typeof(NSObject))]
	interface BCChatAction {

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("openTranscript:intentParameters:")]
		void OpenTranscript (string businessIdentifier, NSDictionary<NSString, NSString> intentParameters);
	}
}
