
using System;
using System.Reflection;

public class TypeManager {
	BindingTouch BindingTouch;
	Frameworks Frameworks { get { return BindingTouch.Frameworks; } }

	public Type System_Attribute;
	public Type System_Boolean;
	public Type System_Byte;
	public Type System_Char;
	public Type System_Delegate;
	public Type System_Double;
	public Type System_Float;
	public Type System_Int16;
	public Type System_Int32;
	public Type System_Int64;
	public Type System_IntPtr;
	public Type System_Object;
	public Type System_SByte;
	public Type System_String;
	public Type System_String_Array;
	public Type System_UInt16;
	public Type System_UInt32;
	public Type System_UInt64;
	public Type System_UIntPtr;
	public Type System_Void;

	public Type System_nint;
	public Type System_nuint;
	public Type System_nfloat;

	/* fundamental */
	public Type NSObject;
	public Type INativeObject;

	/* objcruntime */
	public Type BlockLiteral;
	public Type Class;
	public Type Protocol;
	public Type Selector;

	public Type Constants;

	/* Different binding types */

	public Type DictionaryContainerType;

	public Type ABAddressBook;
	public Type ABPerson;
	public Type ABRecord;
	public Type AudioBuffers;
	public Type AudioComponent;
	public Type AudioUnit;
	public Type AURenderEventEnumerator;
	public Type AVCaptureWhiteBalanceGains;
	public Type CATransform3D;
	public Type CFRunLoop;
	public Type CGAffineTransform;
	public Type CGColor;
	public Type CGColorSpace;
	public Type CGContext;
	public Type CGPDFDocument;
	public Type CGPDFPage;
	public Type CGGradient;
	public Type CGImage;
	public Type CGImageSource;
	public Type CGLayer;
	public Type CGLContext;
	public Type CGLPixelFormat;
	public Type CGPath;
	public Type CGVector;
	public Type CLLocationCoordinate2D;
	public Type CMAudioFormatDescription;
	public Type CMClock;
	public Type CMFormatDescription;
	public Type CMSampleBuffer;
	public Type CMTime;
	public Type CMTimebase;
	public Type CMTimeMapping;
	public Type CMTimeRange;
	public Type CMVideoFormatDescription;
	public Type CVImageBuffer;
	public Type CVPixelBuffer;
	public Type CVPixelBufferPool;
	public Type DispatchQueue;
	public Type DispatchData;
	public Type MidiEndpoint;
	public Type MKCoordinateSpan;
	public Type MTAudioProcessingTap;
	public Type MusicSequence;
	public Type NSNumber;
	public Type NSRange;
	public Type NSString;
	public Type NSValue;
	public Type NSZone;
	public Type SCNMatrix4;
	public Type SCNVector3;
	public Type SCNVector4;
	public Type SecAccessControl;
	public Type SecIdentity;
	public Type SecTrust;
	public Type SecProtocolMetadata;
	public Type SecProtocolOptions;
	public Type SecTrust2;
	public Type SecIdentity2;
	public Type UIEdgeInsets;
	public Type UIOffset;
	public Type NSDirectionalEdgeInsets;

	public Type CoreGraphics_CGPoint;
	public Type CoreGraphics_CGRect;
	public Type CoreGraphics_CGSize;

	Assembly api_assembly;
	Assembly corlib_assembly;
	Assembly platform_assembly;

	Type Lookup (Assembly assembly, string @namespace, string @typename, bool inexistentOK = false)
	{
		string fullname;

		if (string.IsNullOrEmpty (@namespace)) {
			fullname = @typename;
		} else {
			fullname = @namespace + "." + @typename;
		}

		var rv = assembly.GetType (fullname);
		if (rv == null && !inexistentOK)
			throw new BindingException (1052, true, fullname, assembly);
		return rv;
	}

	public Type GetUnderlyingNullableType (Type type)
	{
		if (!type.IsConstructedGenericType)
			return null;

		var gt = type.GetGenericTypeDefinition ();
		if (gt.IsNested)
			return null;

		if (gt.Namespace != "System")
			return null;

		if (gt.Name != "Nullable`1")
			return null;

		return type.GenericTypeArguments [0];
	}

	public static bool IsOutParameter (ParameterInfo pi)
	{
		return pi.IsOut;
	}

	public static Type GetUnderlyingEnumType (Type type)
	{
		return type.GetEnumUnderlyingType ();
	}

	public void Initialize (BindingTouch binding_touch, Assembly api, Assembly corlib, Assembly platform)
	{
		BindingTouch = binding_touch;

		api_assembly = api;
		corlib_assembly = corlib;
		platform_assembly = platform;

		/* corlib */
		System_Attribute = Lookup (corlib_assembly, "System", "Attribute");
		System_Boolean = Lookup (corlib_assembly, "System", "Boolean");
		System_Byte = Lookup (corlib_assembly, "System", "Byte");
		System_Char = Lookup (corlib_assembly, "System", "Char");
		System_Delegate = Lookup (corlib_assembly, "System", "Delegate");
		System_Double = Lookup (corlib_assembly, "System", "Double");
		System_Float = Lookup (corlib_assembly, "System", "Single");
		System_Int16 = Lookup (corlib_assembly, "System", "Int16");
		System_Int32 = Lookup (corlib_assembly, "System", "Int32");
		System_Int64 = Lookup (corlib_assembly, "System", "Int64");
		System_IntPtr = Lookup (corlib_assembly, "System", "IntPtr");
		System_Object = Lookup (corlib_assembly, "System", "Object");
		System_SByte = Lookup (corlib_assembly, "System", "SByte");
		System_String = Lookup (corlib_assembly, "System", "String");
		System_String_Array = Lookup (corlib_assembly, "System", "String").MakeArrayType ();
		System_UInt16 = Lookup (corlib_assembly, "System", "UInt16");
		System_UInt32 = Lookup (corlib_assembly, "System", "UInt32");
		System_UInt64 = Lookup (corlib_assembly, "System", "UInt64");
		System_UIntPtr = Lookup (corlib_assembly, "System", "UIntPtr");
		System_Void = Lookup (corlib_assembly, "System", "Void");

#if NET
		System_nint = Lookup (corlib_assembly, "System", "IntPtr");
		System_nuint = Lookup (corlib_assembly, "System", "UIntPtr");
		var interop_assembly = binding_touch.universe.LoadFromAssemblyName ("System.Runtime.InteropServices");
		System_nfloat = Lookup (interop_assembly, "System.Runtime.InteropServices", "NFloat");
#else
		System_nint = Lookup (platform_assembly, "System", "nint");
		System_nuint = Lookup (platform_assembly, "System", "nuint");
		System_nfloat = Lookup (platform_assembly, "System", "nfloat");
#endif

		/* fundamental */
		NSObject = Lookup (platform_assembly, "Foundation", "NSObject");
		INativeObject = Lookup (platform_assembly, "ObjCRuntime", "INativeObject");

		/* objcruntime */
		BlockLiteral = Lookup (platform_assembly, "ObjCRuntime", "BlockLiteral");
		Class = Lookup (platform_assembly, "ObjCRuntime", "Class");
		Protocol = Lookup (platform_assembly, "ObjCRuntime", "Protocol");
		Selector = Lookup (platform_assembly, "ObjCRuntime", "Selector");

		Constants = Lookup (platform_assembly, "ObjCRuntime", "Constants");

		/* Different binding types */

		DictionaryContainerType = Lookup (platform_assembly, "Foundation", "DictionaryContainer");

		if (Frameworks.HaveAddressBook) {
			ABAddressBook = Lookup (platform_assembly, "AddressBook", "ABAddressBook");
			ABPerson = Lookup (platform_assembly, "AddressBook", "ABPerson");
			ABRecord = Lookup (platform_assembly, "AddressBook", "ABRecord");
		}
		// misplaced API, it's really in CoreAudio (now available everywhere)
		AudioBuffers = Lookup (platform_assembly, "AudioToolbox", "AudioBuffers");
		if (Frameworks.HaveAudioToolbox) {
			MusicSequence = Lookup (platform_assembly, "AudioToolbox", "MusicSequence", true /* may not be found */);
		}
		if (Frameworks.HaveAudioUnit) {
			AudioComponent = Lookup (platform_assembly, "AudioUnit", "AudioComponent");
			AudioUnit = Lookup (platform_assembly, "AudioUnit", "AudioUnit");
			AURenderEventEnumerator = Lookup (platform_assembly, "AudioUnit", "AURenderEventEnumerator");
		}
		AVCaptureWhiteBalanceGains = Lookup (platform_assembly, "AVFoundation", "AVCaptureWhiteBalanceGains");
		if (Frameworks.HaveCoreAnimation)
			CATransform3D = Lookup (platform_assembly, "CoreAnimation", "CATransform3D");

		CFRunLoop = Lookup (platform_assembly, "CoreFoundation", "CFRunLoop");
		CGAffineTransform = Lookup (platform_assembly, "CoreGraphics", "CGAffineTransform");
		CGColor = Lookup (platform_assembly, "CoreGraphics", "CGColor");
		CGColorSpace = Lookup (platform_assembly, "CoreGraphics", "CGColorSpace");
		CGContext = Lookup (platform_assembly, "CoreGraphics", "CGContext");
		CGPDFDocument = Lookup (platform_assembly, "CoreGraphics", "CGPDFDocument");
		CGPDFPage = Lookup (platform_assembly, "CoreGraphics", "CGPDFPage");
		CGGradient = Lookup (platform_assembly, "CoreGraphics", "CGGradient");
		CGImage = Lookup (platform_assembly, "CoreGraphics", "CGImage");
		CGImageSource = Lookup (platform_assembly, "ImageIO", "CGImageSource");
		CGLayer = Lookup (platform_assembly, "CoreGraphics", "CGLayer");
		if (Frameworks.HaveOpenGL) {
			CGLContext = Lookup (platform_assembly, "OpenGL", "CGLContext");
			CGLPixelFormat = Lookup (platform_assembly, "OpenGL", "CGLPixelFormat");
		}
		CGPath = Lookup (platform_assembly, "CoreGraphics", "CGPath");
		CGVector = Lookup (platform_assembly, "CoreGraphics", "CGVector");
		if (Frameworks.HaveCoreLocation)
			CLLocationCoordinate2D = Lookup (platform_assembly, "CoreLocation", "CLLocationCoordinate2D");
		if (Frameworks.HaveCoreMedia) {
			CMAudioFormatDescription = Lookup (platform_assembly, "CoreMedia", "CMAudioFormatDescription");
			CMClock = Lookup (platform_assembly, "CoreMedia", "CMClock");
			CMFormatDescription = Lookup (platform_assembly, "CoreMedia", "CMFormatDescription");
			CMSampleBuffer = Lookup (platform_assembly, "CoreMedia", "CMSampleBuffer");
			CMTime = Lookup (platform_assembly, "CoreMedia", "CMTime");
			CMTimebase = Lookup (platform_assembly, "CoreMedia", "CMTimebase");
			CMTimeMapping = Lookup (platform_assembly, "CoreMedia", "CMTimeMapping");
			CMTimeRange = Lookup (platform_assembly, "CoreMedia", "CMTimeRange");
			CMVideoFormatDescription = Lookup (platform_assembly, "CoreMedia", "CMVideoFormatDescription");
		}
		if (Frameworks.HaveCoreVideo) {
			CVImageBuffer = Lookup (platform_assembly, "CoreVideo", "CVImageBuffer");
			CVPixelBuffer = Lookup (platform_assembly, "CoreVideo", "CVPixelBuffer");
			CVPixelBufferPool = Lookup (platform_assembly, "CoreVideo", "CVPixelBufferPool");
		}
		DispatchQueue = Lookup (platform_assembly, "CoreFoundation", "DispatchQueue");
		DispatchData = Lookup (platform_assembly, "CoreFoundation", "DispatchData");
		if (Frameworks.HaveCoreMidi)
			MidiEndpoint = Lookup (platform_assembly, "CoreMidi", "MidiEndpoint");
		if (Frameworks.HaveMapKit)
			MKCoordinateSpan = Lookup (platform_assembly, "MapKit", "MKCoordinateSpan", true /* isn't in XM/Classic */);
		if (Frameworks.HaveMediaToolbox)
			MTAudioProcessingTap = Lookup (platform_assembly, "MediaToolbox", "MTAudioProcessingTap");
		NSNumber = Lookup (binding_touch.BindThirdPartyLibrary ? platform_assembly : api_assembly, "Foundation", "NSNumber");
		NSRange = Lookup (platform_assembly, "Foundation", "NSRange");
		NSString = Lookup (platform_assembly, "Foundation", "NSString");
		NSValue = Lookup (binding_touch.BindThirdPartyLibrary ? platform_assembly : api_assembly, "Foundation", "NSValue");
		NSZone = Lookup (platform_assembly, "Foundation", "NSZone");
		SCNVector3 = Lookup (platform_assembly, "SceneKit", "SCNVector3");
		SCNVector4 = Lookup (platform_assembly, "SceneKit", "SCNVector4");
		SCNMatrix4 = Lookup (platform_assembly, "SceneKit", "SCNMatrix4");
		SecAccessControl = Lookup (platform_assembly, "Security", "SecAccessControl");
		SecIdentity = Lookup (platform_assembly, "Security", "SecIdentity");
		SecTrust = Lookup (platform_assembly, "Security", "SecTrust");
		SecProtocolOptions = Lookup (platform_assembly, "Security", "SecProtocolOptions");
		SecProtocolMetadata = Lookup (platform_assembly, "Security", "SecProtocolMetadata");
		SecTrust2 = Lookup (platform_assembly, "Security", "SecTrust2");
		SecIdentity2 = Lookup (platform_assembly, "Security", "SecIdentity2");
		if (Frameworks.HaveUIKit) {
			UIOffset = Lookup (platform_assembly, "UIKit", "UIOffset");
			UIEdgeInsets = Lookup (platform_assembly, "UIKit", "UIEdgeInsets");
			NSDirectionalEdgeInsets = Lookup (platform_assembly, "UIKit", "NSDirectionalEdgeInsets");
		}

		CoreGraphics_CGRect = Lookup (platform_assembly, "CoreGraphics", "CGRect");
		CoreGraphics_CGPoint = Lookup (platform_assembly, "CoreGraphics", "CGPoint");
		CoreGraphics_CGSize = Lookup (platform_assembly, "CoreGraphics", "CGSize");
	}
}
