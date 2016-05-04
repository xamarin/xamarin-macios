//
// Test the generated API selectors against typos or non-existing cases
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;

using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif

namespace Introspection {
	
	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class MacSignatureTest : ApiSignatureTest {
		
		static MacSignatureTest ()
		{
			Runtime.RegisterAssembly (typeof (NSObject).Assembly);
		}

		public MacSignatureTest ()
		{
			ContinueOnFailure = true;
			//LogProgress = true;
		}

		protected override bool Skip (Type type, MethodBase method, string selector)
		{
			switch (type.Namespace) {
			case "MonoMac.GameKit":
			case "GameKit":
			case "MonoMac.StoreKit":
			case "StoreKit":
				if (!Mac.CheckSystemVersion (10, 8))
					return true;
				break;
			case "MonoMac.SceneKit":
			case "SceneKit":
				// SceneKit is half-baked at best, and they broke compatibility significantly
				// from Mountain Lion to Mavericks, so just ignore all problems there.
				if (!Mac.CheckSystemVersion (10, 9))
					return true;
				break;
			}
			
			// Bug 20232 - This is a hack.
			if (selector == "geometryElementWithData:primitiveType:primitiveCount:bytesPerIndex:")
				return true;

			switch (type.Name) {
			case "AVPlayerItemOutput":
			case "AVPlayerItemVideoOutput":
			case "NSSharingService":
			case "NSSharingServicePicker":
			case "NSUserNotification":
			case "NSUserNotificationCenter":
			case "NSUuid":
				return !Mac.CheckSystemVersion (10, 8);

			case "CALayer":
			case "CAEmitterLayer":
			case "CAGradientLayer":
			case "CAOpenGLLayer":
			case "CAReplicatorLayer":
			case "CAScrollLayer":
			case "CAShapeLayer":
			case "CATextLayer":
			case "CATiledLayer":
			case "CATransformLayer":
			case "QTCaptureLayer":
			case "QTMovieLayer":
			case "QCCompositionLayer":
			case "AVCaptureVideoPreviewLayer":
			case "AVPlayerLayer":
			case "SCNLayer":
				switch (selector) {
				// CAGradientLayer 'instance MonoMac.CoreAnimation.CAConstraint[] get_Constraints()' selector: constraints == @?@:
				case "constraints":
				// CAGradientLayer 'instance Void set_Constraints(MonoMac.CoreAnimation.CAConstraint[])' selector: setConstraints: == v@:@?
				case "setConstraints:":
					return true;
				}
				break;
			case "SCNSkinner":
				return !Mac.CheckSystemVersion (10, 9);
			case "NSGradient":
				switch (selector) {
				case "initWithColorsAndLocations:": // variable length parameters ...
				case "initWithColors:atLocations:colorSpace:": // void * -> nfloat [] (internal)
					return true;
				}
				break;
			case "AVAudioIONode":
			case "AVAudioUnit":
				switch (selector) {
				case "audioUnit": // ^{ComponentInstanceRecord=[1l], tested to work in an apitest
					return true;
				}
				break;
			}
			return base.Skip (type, method, selector);
		}

		protected override int Size (Type t, bool simd = false)
		{
			switch (t.FullName) {
			case "GameKit.GKGameCenterViewControllerState":
			case "AppKit.NSOpenGLContextParameter":
				// NSOpenGLContextParameter and GKGameCenterViewControllerState are anonymous enums in 10.9, but an NSInteger in 10.10.
				if (IntPtr.Size == 8 && !Mac.CheckSystemVersion (10, 10))
					return 4;
				break;
			}

			return base.Size (t, simd);
		}

		protected override bool IsValidStruct (Type type, string structName)
		{
			switch (structName) {
			case "_NSPoint":
#if XAMCORE_2_0
				return type.FullName == "CoreGraphics.CGPoint";
#else
				return type.FullName == "System.Drawing.PointF";
#endif
			case "_NSRect":
#if XAMCORE_2_0
				return type.FullName == "CoreGraphics.CGRect";
#else
				return type.FullName == "System.Drawing.RectangleF";
#endif
			case "_NSSize":
#if XAMCORE_2_0
				return type.FullName == "CoreGraphics.CGSize";
#else
				return type.FullName == "System.Drawing.SizeF";
#endif
			case "_SCNVector3":
				return type.Name == "SCNVector3";
			case "_SCNVector4":
				return type.Name == "SCNVector4";
			// CIImage 'static MonoMac.CoreImage.CIImage FromImageBuffer(MonoMac.CoreVideo.CVImageBuffer)' selector: imageWithCVImageBuffer: == @12@0:4^{__CVBuffer=}8
			// AVAssetWriterInputPixelBufferAdaptor 'instance Boolean AppendPixelBufferWithPresentationTime(MonoMac.CoreVideo.CVPixelBuffer, CMTime)' selector: appendPixelBuffer:withPresentationTime: == c36@0:4^{__CVBuffer=}8{?=qiIq}12
			case "__CVBuffer":
				return type.Name == "CVImageBuffer" || type.Name == "CVPixelBuffer";;
			case "CATransform3D":
				return type.Name == "CATransform3D" || type.Name == "SCNMatrix4";
			case "SCNVector4":
				return type.Name == "SCNVector4" || type.Name == "SCNQuaternion"; // "SCNQuaternion is a SCNVector 3, then a nfloat, so same structure
			}
			return base.IsValidStruct (type, structName);
		}

		// only handle exception here (to return true) otherwise call base to deal with it
		protected override bool Check (string encodedType, Type type)
		{
			switch (encodedType) {
			case "^{OpaqueSecTrustRef=}":
				// On 10.7 and 10.8:
				// [FAIL] Signature failure in MonoMac.Foundation.NSUrlCredential initWithTrust: Parameter 'trust' (#1) is encoded as '^{OpaqueSecTrustRef=}' and bound as 'MonoMac.Security.SecTrust'
				return type.Name == "SecTrust" || type.FullName == "System.IntPtr";
			}
			return base.Check (encodedType, type);
		}

		// only handle exception here (to return true) otherwise call base to deal with it
		// `caller` is provided to make it easier to detect "special" cases
		protected override bool Check (char encodedType, Type type)
		{
			switch (encodedType) {
			case 'i':
				switch (type.FullName) {
				case "System.nuint":
				case "System.UInt32":
					// sign-ness mis-binding, not critical
					// Signature failure in MonoMac.AppKit.NSViewController presentViewController:asPopoverRelativeToRect:ofView:preferredEdge:behavior: Parameter 'preferredEdge' (#4) is encoded as 'i' and bound as 'System.UInt32'
					return true;
				case "GameKit.GKGameCenterViewControllerState":
				case "AppKit.NSOpenGLContextParameter":
					// NSOpenGLContextParameter and GKGameCenterViewControllerState are anonymous enums in 10.9, but an NSInteger in 10.10.
					if (IntPtr.Size == 8 && !Mac.CheckSystemVersion (10, 10))
						return true;
					break;
				}
				break;
			// unsigned 32 bits
			case 'I':
				switch (type.FullName) {
				case "System.Int32":
					// sign-ness mis-binding, several of them (not critical)
					// NSActionCell 'instance Int32 get_MnemonicLocation()' selector: mnemonicLocation == I8@0:4
					// NSPathCell 'instance Int32 get_MnemonicLocation()' selector: mnemonicLocation == I8@0:4
					// SCNText 'instance Void InsertMaterial(MonoMac.SceneKit.SCNMaterial, Int32)' selector: insertMaterial:atIndex: == v16@0:4@8I12
					return true;
				}
				break;
			// unsigned 32 bits
			case 'L':
				switch (type.FullName) {
				// sign-ness mis-binding (not critical) e.g.
				// CAMediaTimingFunction 'instance Void GetControlPointAtIndex(Int32, IntPtr)' selector: getControlPointAtIndex:values: == v16@0:4L8[2f]12
				case "System.Int32":
					return true;
				}
				break;
			// unsigned 64 bits
			case 'Q':
				switch (type.FullName) {
				// sign-ness mis-binding (not critical) e.g.
				// NSEvent 'instance Int64 get_UniqueID()' selector: uniqueID == Q8@0:4
				case "System.Int64":
					return true;
				}
				break;
			}
			return base.Check (encodedType, type);
		}
	}
}
