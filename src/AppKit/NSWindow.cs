//
// Copyright 2010, Novell, Inc.
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
using System;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace AppKit {

	public partial class NSWindow {

		public static bool DisableReleasedWhenClosedInConstructor;

		static IntPtr selInitWithWindowRef = Selector.GetHandle ("initWithWindowRef:");

		// Do not actually export because NSObjectFlag is not exportable.
		// The Objective C method already exists. This is just to allow
		// access on the managed side via the static method.
		//[Export ("initWithWindowRef:")]
		private NSWindow (IntPtr windowRef, NSObjectFlag x) : base (NSObjectFlag.Empty)
		{
			if (IsDirectBinding) {
				Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, selInitWithWindowRef);
			} else {
				Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, selInitWithWindowRef);
			}
			if (!DisableReleasedWhenClosedInConstructor)
				ReleasedWhenClosed = false;
		}

		static public NSWindow FromWindowRef (IntPtr windowRef)
		{
			return new NSWindow (windowRef, NSObjectFlag.Empty);
		}

		public void Close ()
		{
			// Windows that do not have a WindowController use released_when_closed
			// if set to true, the call to Close will release the object, and we will
			// end up with a double free.
			//
			// If that is the case, we take a reference first, and to keep the behavior
			// we call Dispose after that.
			if (WindowController == null){
				bool released_when_closed = ReleasedWhenClosed;
				if (released_when_closed)
					CFObject.CFRetain (Handle);
				_Close ();
				if (released_when_closed)
					Dispose ();
			} else
				_Close ();
		}

		// note: if needed override the protected Get|Set methods
		public string FrameAutosaveName { 
			get { return GetFrameAutosaveName (); }
			// ignore return value (bool)
			set { SetFrameAutosaveName (value); }
		}

		public NSEvent NextEventMatchingMask (NSEventMask mask)
		{
			return NextEventMatchingMask ((uint) mask);
		}
		
		public NSEvent NextEventMatchingMask (NSEventMask mask, NSDate expiration, string mode, bool deqFlag)
		{
			return NextEventMatchingMask ((uint) mask, expiration, mode, deqFlag);
		}

		public void DiscardEventsMatchingMask (NSEventMask mask, NSEvent beforeLastEvent)
		{
			DiscardEventsMatchingMask ((uint) mask, beforeLastEvent);
		}

// NSString NSWindowDidBecomeKeyNotification;
// NSString NSWindowDidBecomeMainNotification;
// NSString NSWindowDidChangeScreenNotification;
// NSString NSWindowDidDeminiaturizeNotification;
// NSString NSWindowDidExposeNotification;
// NSString NSWindowDidMiniaturizeNotification;
// NSString NSWindowDidMoveNotification;
// NSString NSWindowDidResignKeyNotification;
// NSString NSWindowDidResignMainNotification;
// NSString NSWindowDidResizeNotification;
// NSString NSWindowDidUpdateNotification;
// NSString NSWindowWillCloseNotification;
// NSString NSWindowWillMiniaturizeNotification;
// NSString NSWindowWillMoveNotification;
// NSString NSWindowWillBeginSheetNotification;
// NSString NSWindowDidEndSheetNotification;
// NSString NSWindowDidChangeScreenProfileNotification
// NSString NSWindowWillStartLiveResizeNotification
// NSString NSWindowDidEndLiveResizeNotification

		
	}
}
