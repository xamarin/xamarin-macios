//
// Unit tests for CGDataConsumer
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DataConsumerTest {

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerCreateWithURL (/* CFURLRef */ IntPtr url);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerCreateWithCFData (/* CFMutableDataRef */ IntPtr data);

		[Test]
		public void Create_Nullable ()
		{
			// the native API accept a nil argument but it returns nil, so we must keep our ArgumentNullException as
			// a .NET constructor can't return null (and we don't want invalid managed instances)
			Assert.That (CGDataConsumerCreateWithURL (IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "CGDataConsumerCreateWithURL");
			Assert.That (CGDataConsumerCreateWithCFData (IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "CGDataConsumerCreateWithCFData");
		}
	}
}
