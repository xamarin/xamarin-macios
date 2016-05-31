//
// Unit tests for PixelFormatType
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreVideo;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreVideo;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PixelFormatTypeTest {
		
		string FourCC (int value)
		{
			return new string (new char [] { 
				(char) (byte) (value >> 24), 
				(char) (byte) (value >> 16), 
				(char) (byte) (value >> 8), 
				(char) (byte) value });
		}
		
		[Test]
		public void FourCC ()
		{
			Assert.That (FourCC ((int) CVPixelFormatType.OneComponent8), Is.EqualTo ("L008"), "OneComponent8");
			Assert.That (FourCC ((int) CVPixelFormatType.TwoComponent8), Is.EqualTo ("2C08"), "TwoComponent8");
		}
	}
}

#endif // !__WATCHOS__
