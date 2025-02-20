using System.Collections.Generic;

#nullable enable

namespace Cecil.Tests {
	public partial class SetHandleTest {
		static HashSet<string> knownFailuresNobodyCallsHandleSetter = new HashSet<string> {
			"AddressBook.ABGroup::.ctor(AddressBook.ABRecord)",
			"AppKit.NSBitmapImageRep::.ctor(Foundation.NSObjectFlag,Foundation.NSObjectFlag)",
			"AppKit.NSGradient::Initialize(AppKit.NSColor[],System.Void*,AppKit.NSColorSpace)",
			"AppKit.NSImage::.ctor(Foundation.NSData,System.Boolean)",
			"AppKit.NSImage::.ctor(System.String,System.Boolean)",
			"AppKit.NSOpenGLPixelFormat::.ctor(AppKit.NSOpenGLPixelFormatAttribute[])",
			"AppKit.NSOpenGLPixelFormat::.ctor(System.Object[])",
			"AppKit.NSTextContainer::.ctor(CoreGraphics.CGSize,System.Boolean)",
			"AVFoundation.AVAudioRecorder::.ctor(Foundation.NSUrl,AVFoundation.AudioSettings,Foundation.NSError&)",
			"AVFoundation.AVAudioRecorder::.ctor(Foundation.NSUrl,AVFoundation.AVAudioFormat,Foundation.NSError&)",
			"CoreFoundation.CFMutableString::.ctor(CoreFoundation.CFString,System.IntPtr)",
			"CoreFoundation.CFMutableString::.ctor(System.String,System.IntPtr)",
			"CoreFoundation.CFSocket::Initialize(CoreFoundation.CFRunLoop,CoreFoundation.CFSocket/CreateSocket)",
			"CoreFoundation.CFString::.ctor(System.String)",
			"CoreFoundation.DispatchBlock::Retain()",
			"CoreFoundation.OSLog::.ctor(System.String,System.String)",
			"CoreGraphics.CGPattern::.ctor(CoreGraphics.CGRect,CoreGraphics.CGAffineTransform,System.Runtime.InteropServices.NFloat,System.Runtime.InteropServices.NFloat,CoreGraphics.CGPatternTiling,System.Boolean,CoreGraphics.CGPattern/DrawPattern)",
			"Foundation.NSAttributedString::.ctor(Foundation.NSData,Foundation.NSAttributedStringDataType,Foundation.NSDictionary&)",
			"Foundation.NSHttpCookie::CreateCookie(System.String,System.String,System.String,System.String,System.String,System.String,System.Nullable`1<System.Boolean>,System.Nullable`1<System.DateTime>,System.Nullable`1<System.Int32>,System.String,System.Nullable`1<System.Boolean>,System.Nullable`1<System.Int32>)",
			"Foundation.NSKeyedUnarchiver::.ctor(Foundation.NSData)",
			"Foundation.NSString::.ctor(System.String,System.Int32,System.Int32)",
			"Foundation.NSString::.ctor(System.String)",
			"Foundation.NSThread::.ctor(Foundation.NSObject,ObjCRuntime.Selector,Foundation.NSObject)",
			"Foundation.NSUrlProtectionSpace::.ctor(System.String,System.Int32,System.String,System.String,System.String,System.Boolean)",
			"Foundation.NSUrlProtectionSpace::.ctor(System.String,System.Int32,System.String,System.String,System.String)",
			"Foundation.NSUserDefaults::.ctor(System.String,Foundation.NSUserDefaultsType)",
			"Foundation.NSUuid::.ctor(System.Byte[])",
			"GameKit.GKScore::.ctor(System.String)",
			"GameplayKit.GKPath::.ctor(System.Numerics.Vector2[],System.Single,System.Boolean)",
			"GameplayKit.GKPath::.ctor(System.Numerics.Vector3[],System.Single,System.Boolean)",
			"ModelIO.MDLNoiseTexture::.ctor(System.Single,System.String,CoreGraphics.NVector2i,ModelIO.MDLTextureChannelEncoding,ModelIO.MDLNoiseTextureType)",
			"MultipeerConnectivity.MCSession::.ctor(MultipeerConnectivity.MCPeerID,Security.SecIdentity,MultipeerConnectivity.MCEncryptionPreference)",
			"MultipeerConnectivity.MCSession::.ctor(MultipeerConnectivity.MCPeerID,Security.SecIdentity,Security.SecCertificate[],MultipeerConnectivity.MCEncryptionPreference)",
			"ObjCRuntime.Runtime::RegisterNSObject(Foundation.NSObject,System.IntPtr)",
			"ScreenCaptureKit.SCContentFilter::.ctor(ScreenCaptureKit.SCDisplay,ScreenCaptureKit.SCRunningApplication[],ScreenCaptureKit.SCWindow[],ScreenCaptureKit.SCContentFilterOption)",
			"ScreenCaptureKit.SCContentFilter::.ctor(ScreenCaptureKit.SCDisplay,ScreenCaptureKit.SCWindow[],ScreenCaptureKit.SCContentFilterOption)",
			"Security.SecTrust2::.ctor(Security.SecTrust)",
			"SpriteKit.SKVideoNode::.ctor(Foundation.NSUrl)",
			"SpriteKit.SKVideoNode::.ctor(System.String)",
		};
	}
}
