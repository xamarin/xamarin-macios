// Copyright 2011-2013 Xamarin Inc. All rights reserved

#if HAS_UIKIT || HAS_APPKIT

using System;
using System.Drawing;

#if HAS_APPKIT
using AppKit;
#endif
using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if HAS_UIKIT
using UIKit;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.XKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSTextStorageTest {
		[Test]
		public void InitWithString ()
		{
			using var obj = new NSTextStorage ("Hello World");
		}
	}
}

#endif // HAS_UIKIT || HAS_APPKIT
