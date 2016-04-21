// 
// CADisplayLink: Support for CADisplayLink
//
// Authors:
//   Timothy Risi.
//     
// Copyright 2014 Xamarin Inc
//
#if !MONOMAC
using System;

using XamCore.Foundation; 
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;
using XamCore.CoreFoundation;
using XamCore.CoreText;

namespace XamCore.CoreAnimation {
	public partial class CADisplayLink {
		public void AddToRunLoop (NSRunLoop runloop, NSRunLoopMode mode)
		{
			AddToRunLoop (runloop, NSRunLoop.FromEnum (mode));
		}
	}
}
#endif