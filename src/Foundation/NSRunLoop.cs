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

namespace Foundation {
	
	public partial class NSRunLoop {
#if !XAMCORE_2_0
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

		[Advice ("Use 'AddTimer (NSTimer, NSRunLoopMode)'.")]
		public void AddTimer (NSTimer timer, string forMode)
		{
			AddTimer (timer, GetRealMode (forMode));
		}

		[Advice ("Use 'LimitDateForMode (NSRunLoopMode)' instead.")]
		public NSDate LimitDateForMode (string mode)
		{
			return LimitDateForMode (GetRealMode (mode));
		}

		[Advice ("Use 'AcceptInputForMode (NSRunLoopMode, NSDate)'.")]
		public void AcceptInputForMode (string mode, NSDate limitDate)
		{
			AcceptInputForMode (GetRealMode (mode), limitDate);
		}
#endif
		
		public void Stop ()
		{
			GetCFRunLoop ().Stop ();
		}

		public void WakeUp ()
		{
			GetCFRunLoop ().WakeUp ();
		}
	}

	static public partial class NSRunLoopModeExtensions {

		// this is a less common pattern so it's not automatically generated
		public static NSString[] GetConstants (this NSRunLoopMode[] self)
		{
			if (self == null)
				throw new ArgumentNullException (nameof (self));
			
			var array = new NSString [self.Length];
			for (int n = 0; n < self.Length; n++)
				array [n] = self [n].GetConstant ();
			return array;
		}
	}
}