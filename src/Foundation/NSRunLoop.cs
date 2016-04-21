// Copyright 2011, 2012 Xamarin Inc
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
using System.Runtime.InteropServices;

namespace XamCore.Foundation {
	// This is a C# utility enum without a corresponding native enum, it can stay as 'int'.
	public enum NSRunLoopMode {
		Default,
		Common,
#if MONOMAC
		ConnectionReply = 2,
		ModalPanel,
		EventTracking,
#elif !WATCH
		// iOS-specific Enums start in 100 to avoid conflicting with future extensions to MonoMac
		UITracking = 100,
#endif

		// If it is not part of these enumerations
		Other = 1000
	}
	
	public partial class NSRunLoop {
		static NSString GetRealMode (string mode)
		{
			if (mode == NSDefaultRunLoopMode)
				return NSDefaultRunLoopMode;
			else if (mode == NSRunLoopCommonModes)
				return NSRunLoopCommonModes;
#if IOS || TVOS
			else if (mode == UITrackingRunLoopMode)
				return UITrackingRunLoopMode;
#endif
			else
				return new NSString (mode);
		}

		internal static NSString FromEnum (NSRunLoopMode mode)
		{
			switch (mode){
			case NSRunLoopMode.Common:
				return NSRunLoopCommonModes;
#if MONOMAC
			case NSRunLoopMode.ConnectionReply:
				return NSRunLoopConnectionReplyMode;
			case NSRunLoopMode.ModalPanel:
				return NSRunLoopModalPanelMode;
			case NSRunLoopMode.EventTracking:
				return NSRunLoopEventTracking;
#elif !WATCH
			case NSRunLoopMode.UITracking:
				return UITrackingRunLoopMode;
#endif
	
			default:
			case NSRunLoopMode.Default:
				return NSDefaultRunLoopMode;
			}
		}
		
#if !XAMCORE_2_0
		[Advice ("Use AddTimer (NSTimer, NSRunLoopMode)")]
		public void AddTimer (NSTimer timer, string forMode)
		{
			AddTimer (timer, GetRealMode (forMode));
		}
#endif

		public void AddTimer (NSTimer timer, NSRunLoopMode forMode)
		{
			AddTimer (timer, FromEnum (forMode));
		}

#if !XAMCORE_2_0
		[Advice ("Use LimitDateForMode (NSRunLoopMode) instead")]
		public NSDate LimitDateForMode (string mode)
		{
			return LimitDateForMode (GetRealMode (mode));
		}
#endif

		public NSDate LimitDateForMode (NSRunLoopMode mode)
		{
			return LimitDateForMode (FromEnum (mode));
		}
		
#if !XAMCORE_2_0
		[Advice ("Use AcceptInputForMode (NSRunLoopMode, NSDate)")]
		public void AcceptInputForMode (string mode, NSDate limitDate)
		{
			AcceptInputForMode (GetRealMode (mode), limitDate);
		}
#endif
		
		public void AcceptInputForMode (NSRunLoopMode mode, NSDate limitDate)
		{
			AcceptInputForMode (FromEnum (mode), limitDate);
		}

		public void Stop ()
		{
			GetCFRunLoop ().Stop ();
		}

		public void WakeUp ()
		{
			GetCFRunLoop ().WakeUp ();
		}

		public bool RunUntil (NSRunLoopMode mode, NSDate limitDate)
		{
			return RunUntil (FromEnum (mode), limitDate);
		}

		public NSRunLoopMode CurrentRunLoopMode {
			get {
				var mode = CurrentMode;

				if (mode == NSDefaultRunLoopMode)
					return NSRunLoopMode.Default;
				if (mode == NSRunLoopCommonModes)
					return NSRunLoopMode.Common;
#if MONOMAC
				if (mode == NSRunLoopConnectionReplyMode)
					return NSRunLoopMode.ConnectionReply;
				if (mode == NSRunLoopModalPanelMode)
					return NSRunLoopMode.ModalPanel;
				if (mode == NSRunLoopEventTracking)
					return NSRunLoopMode.EventTracking;
#elif !WATCH
				if (mode == UITrackingRunLoopMode)
					return NSRunLoopMode.UITracking;
#endif
				return NSRunLoopMode.Other;
			}
		}
	}
}