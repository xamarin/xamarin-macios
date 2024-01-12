//
// Copyright 2011, Kenneth J. Pouncey
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

//
// scriptingbridge.cs: Bindings for the ScriptingBridge.Framework API
//
using System;
using AppKit;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ScriptingBridge {


	[BaseType (typeof (NSObject))]
	interface SBObject : NSCoding {

		[Export ("initWithProperties:")]
		NativeHandle Constructor (NSDictionary properties);

		[Export ("initWithData:")]
		NativeHandle Constructor (NSObject data);

		[Export ("get")]
		NSObject Get { get; }

		// part of SBObject.h include file, not in the official documentation
		[Export ("lastError")]
		NSError LastError { get; }
	}

#pragma warning disable 0618 // SBElement can only access children elements via NSMutableArray base type
	[BaseType (typeof (NSMutableArray))]
	[DisableDefaultCtor] // *** -[SBElementArray init]: should never be used.
	interface SBElementArray {
		[Export ("initWithCapacity:")]
		NativeHandle Constructor (nuint capacity);

		[Export ("objectWithName:")]
		NSObject ObjectWithName (string name);

		[Export ("objectWithID:")]
		NSObject ObjectWithID (NSObject identifier);

		[Export ("objectAtLocation:")]
		NSObject ObjectAtLocation (NSObject location);

		[Export ("arrayByApplyingSelector:")]
		NSObject [] ArrayByApplyingSelector (Selector selector);

		[Export ("arrayByApplyingSelector:withObject:")]
		NSObject [] ArrayByApplyingSelector (Selector aSelector, NSObject argument);

		[Export ("get")]
		NSObject [] Get ();
	}
#pragma warning restore 0618


	// TODO: The documentation says these are rarely used so will clean these up later
	//	interface SBObject {
	//		[Export ("initWithElementCode:properties:data:")]
	//		NSObject InitWithElementCodepropertiesdata (DescType code, NSDictionary properties, NSObject data);
	//
	//		[Export ("propertyWithCode:")]
	//		SBObject PropertyWithCode (AEKeyword code);
	//
	//		[Export ("propertyWithClass:code:")]
	//		SBObject PropertyWithClasscode (Class cls, AEKeyword code);
	//
	//		[Export ("elementArrayWithCode:")]
	//		SBElementArray ElementArrayWithCode (DescType code);
	//
	//		[Export ("sendEvent:id:parameters:...")]
	//		NSObject SendEventidparameters... (AEEventClass eventClass, AEEventID eventID, DescType firstParamCode,, );
	//
	//		[Export ("setTo:")]
	//		void SetTo (NSObject value);
	//
	//	}

	[BaseType (typeof (SBObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (SBApplicationDelegate) })]
	[DisableDefaultCtor] // An uncaught exception was raised: *** -[SBApplication init]: should never be used.
	interface SBApplication : NSCoding {
		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("initWithProcessIdentifier:")]
		NativeHandle Constructor (int /* pid_t = int */ pid);

		[Export ("initWithBundleIdentifier:")]
		NativeHandle Constructor (string ident);

		[Internal]
		[Static]
		[Export ("applicationWithBundleIdentifier:")]
		IntPtr _FromBundleIdentifier (string ident);

		[Internal]
		[Static]
		[Export ("applicationWithURL:")]
		IntPtr _FromURL (NSUrl url);

		[Internal]
		[Static]
		[Export ("applicationWithProcessIdentifier:")]
		IntPtr _FromProcessIdentifier (int /* pid_t = int */ pid);

		[Export ("classForScriptingClass:")]
		Class ClassForScripting (string className);

		[Export ("isRunning")]
		bool IsRunning { get; }

		[Export ("activate")]
		void Activate ();

		[Export ("delegate", ArgumentSemantic.Retain), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		ISBApplicationDelegate Delegate { get; set; }

		[Export ("launchFlags")]
		LSLaunchFlags LaunchFlags { get; set; }

		[Export ("sendMode")]
		AESendMode SendMode { get; set; }

		[Export ("timeout")]
		nint Timeout { get; set; }
	}

	interface ISBApplicationDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SBApplicationDelegate {
#if !NET
		[Abstract]
		[Export ("eventDidFail:withError:"), DelegateName ("SBApplicationError"), DefaultValue (null)]
		//NSObject EventDidFailwithError (const AppleEvent event, NSError error);
		NSObject EventDidFailwithError (IntPtr appleEvent, NSError error);
#else
		[Abstract]
		[Export ("eventDidFail:withError:"), DelegateName ("SBApplicationError"), DefaultValue (null)]
		NSObject EventFailed (IntPtr appleEvent, NSError error);
#endif
	}

}
