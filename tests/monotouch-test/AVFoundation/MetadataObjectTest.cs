//
// Unit tests for AVMetadataObject
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__

using System;
using System.Drawing;
using System.IO;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using AVFoundation;
using CoreMedia;
#else
using MonoTouch.AVFoundation;
using MonoTouch.CoreMedia;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MetadataObjectTest {

		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);

			using (var obj = new AVMetadataFaceObject ()) {
				Assert.AreEqual (0, obj.FaceID, "FaceID");
				Assert.AreEqual (false, obj.HasRollAngle, "HasRollAngle");
				Assert.AreEqual (false, obj.HasYawAngle, "HasYawAngle");
#if XAMCORE_2_0
#if !MONOMAC // No Type property for Mac
				Assert.AreEqual (AVMetadataObjectType.Face, obj.Type, "Type");
#endif
				Assert.AreEqual (AVMetadataObject.TypeFace, obj.WeakType, "WeakType");
#else
				Assert.AreEqual (AVMetadataObject.TypeFace, obj.Type, "Type");
#endif
			}

#if !MONOMAC // iOS only
			using (var obj = new AVMetadataMachineReadableCodeObject ()) {
				Assert.IsNotNull (obj.Corners, "Corners");
				Assert.AreEqual (0, obj.Corners.Length, "Corners");
				Assert.IsNull (obj.StringValue, "StringValue");
#if XAMCORE_2_0
				Assert.AreEqual (AVMetadataObjectType.None, obj.Type, "Type");
				Assert.IsNull (obj.WeakType, "WeakType");
#else
				Assert.IsNull (obj.Type, "Type");
#endif
		}
#endif
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
