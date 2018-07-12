
using System;
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;

public static class TypeManager {
	public static Type System_Attribute;
	public static Type System_Boolean;
	public static Type System_Byte;
	public static Type System_Delegate;
	public static Type System_Double;
	public static Type System_Float;
	public static Type System_Int16;
	public static Type System_Int32;
	public static Type System_Int64;
	public static Type System_IntPtr;
	public static Type System_Object;
	public static Type System_SByte;
	public static Type System_String;
	public static Type System_String_Array;
	public static Type System_UInt16;
	public static Type System_UInt32;
	public static Type System_UInt64;
	public static Type System_Void;

	public static Type System_nint;
	public static Type System_nuint;
	public static Type System_nfloat;

	/* fundamental */
	public static Type NSObject;
	public static Type INativeObject;

	/* objcruntime */
	public static Type BlockLiteral;
	public static Type Class;
	public static Type Protocol;
	public static Type Selector;

	public static Type Constants;

	/* Different binding types */

	public static Type DictionaryContainerType;

	public static Type ABAddressBook;
	public static Type ABPerson;
	public static Type ABRecord;
	public static Type AudioBuffers;
	public static Type AudioComponent;
	public static Type AudioUnit;
	public static Type AURenderEventEnumerator;
	public static Type AVCaptureWhiteBalanceGains;
	public static Type CATransform3D;
	public static Type CFRunLoop;
	public static Type CGAffineTransform;
	public static Type CGColor;
	public static Type CGColorSpace;
	public static Type CGContext;
	public static Type CGPDFDocument;
	public static Type CGPDFPage;
	public static Type CGGradient;
	public static Type CGImage;
	public static Type CGLayer;
	public static Type CGLContext;
	public static Type CGLPixelFormat;
	public static Type CGPath;
	public static Type CGVector;
	public static Type CLLocationCoordinate2D;
	public static Type CMAudioFormatDescription;
	public static Type CMClock;
	public static Type CMFormatDescription;
	public static Type CMSampleBuffer;
	public static Type CMTime;
	public static Type CMTimebase;
	public static Type CMTimeMapping;
	public static Type CMTimeRange;
	public static Type CMVideoFormatDescription;
	public static Type CVImageBuffer;
	public static Type CVPixelBuffer;
	public static Type CVPixelBufferPool;
	public static Type DispatchQueue;
	public static Type MidiEndpoint;
	public static Type MKCoordinateSpan;
	public static Type MTAudioProcessingTap;
	public static Type MusicSequence;
	public static Type NSNumber;
	public static Type NSRange;
	public static Type NSString;
	public static Type NSValue;
	public static Type NSZone;
	public static Type SCNMatrix4;
	public static Type SCNVector3;
	public static Type SCNVector4;
	public static Type SecAccessControl;
	public static Type SecIdentity;
	public static Type SecTrust;
	public static Type SecProtocolMetadata;
	public static Type SecProtocolOptions;
	public static Type SecTrust2;
	public static Type SecIdentity2;
	public static Type UIEdgeInsets;
	public static Type UIOffset;
	public static Type NSDirectionalEdgeInsets;

	public static Type CoreGraphics_CGPoint;
	public static Type CoreGraphics_CGRect;
	public static Type CoreGraphics_CGSize;

	static Assembly api_assembly;
	static Assembly corlib_assembly;
	static Assembly platform_assembly;
	static Assembly system_assembly;
	static Assembly binding_assembly;

	public static Assembly CorlibAssembly {
		get { return corlib_assembly; }
	}

	public static Assembly PlatformAssembly {
		get { return platform_assembly; }
		set { platform_assembly = value; }
	}

	public static Assembly SystemAssembly {
		get { return system_assembly; }
	}

	public static Assembly BindingAssembly {
		get { return binding_assembly; }
	}

	static Type Lookup (Assembly assembly, string @namespace, string @typename, bool inexistentOK = false)
	{
		string fullname;
		string nsManagerPrefix = null;

		if (assembly == platform_assembly || assembly == api_assembly)
			nsManagerPrefix = BindingTouch.NamespacePlatformPrefix;
		
		if (!string.IsNullOrEmpty (nsManagerPrefix))
			nsManagerPrefix += ".";

		if (string.IsNullOrEmpty (@namespace)) {
			fullname = nsManagerPrefix + @typename;
		} else {
			fullname = nsManagerPrefix + @namespace + "." + @typename;
		}

		var rv = assembly.GetType (fullname);
		if (rv == null && !inexistentOK)
			throw new BindingException (1052, true, "Internal error: Could not find the type {0} in the assembly {1}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", fullname, assembly);
		return rv;
	}

	public static Type GetUnderlyingNullableType (Type type)
	{
		if (!type.IsConstructedGenericType)
			return null;

		var gt = type.GetGenericTypeDefinition ();
		if (gt.Assembly != CorlibAssembly)
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

	public static void Initialize (Assembly api, Assembly corlib, Assembly platform, Assembly system, Assembly binding)
	{
		api_assembly = api;
		corlib_assembly = corlib;
		platform_assembly = platform;
		system_assembly = system;
		binding_assembly = binding;

		/* corlib */
		System_Attribute = Lookup (corlib_assembly, "System", "Attribute");
		System_Boolean = Lookup (corlib_assembly, "System", "Boolean");
		System_Byte = Lookup (corlib_assembly, "System", "Byte");
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
		System_Void = Lookup (corlib_assembly, "System", "Void");

		if (Generator.UnifiedAPI) {
			System_nint = Lookup (platform_assembly, "System", "nint");
			System_nuint = Lookup (platform_assembly, "System", "nuint");
			System_nfloat = Lookup (platform_assembly, "System", "nfloat");
		}

		/* fundamental */
		NSObject = Lookup (platform_assembly, "Foundation", "NSObject");
		INativeObject = Lookup (platform_assembly, "ObjCRuntime", "INativeObject");

		/* objcruntime */
		BlockLiteral = Lookup (platform_assembly, "ObjCRuntime", "BlockLiteral");
		Class = Lookup (platform_assembly, "ObjCRuntime", "Class");
		Protocol = Lookup (platform_assembly, "ObjCRuntime", "Protocol");
		Selector = Lookup (platform_assembly, "ObjCRuntime", "Selector");

		if (Generator.UnifiedAPI) {
			Constants = Lookup (platform_assembly, "ObjCRuntime", "Constants");
		} else {
			Constants = Lookup (platform_assembly, "", "Constants");
		}

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
		if (Frameworks.HaveCoreMidi)
			MidiEndpoint = Lookup (platform_assembly, "CoreMidi", "MidiEndpoint");
		if (Frameworks.HaveMapKit)
			MKCoordinateSpan = Lookup (platform_assembly, "MapKit", "MKCoordinateSpan", true /* isn't in XM/Classic */);
		if (Frameworks.HaveMediaToolbox)
			MTAudioProcessingTap = Lookup (platform_assembly, "MediaToolbox", "MTAudioProcessingTap");
		NSNumber = Lookup (Generator.BindThirdPartyLibrary ? platform_assembly : api_assembly, "Foundation", "NSNumber");
		NSRange = Lookup (platform_assembly, "Foundation", "NSRange");
		NSString = Lookup (platform_assembly, "Foundation", "NSString");
		NSValue = Lookup (Generator.BindThirdPartyLibrary ? platform_assembly : api_assembly, "Foundation", "NSValue");
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

		if (Generator.UnifiedAPI) {
			CoreGraphics_CGRect = Lookup (platform_assembly, "CoreGraphics", "CGRect");
			CoreGraphics_CGPoint = Lookup (platform_assembly, "CoreGraphics", "CGPoint");
			CoreGraphics_CGSize = Lookup (platform_assembly, "CoreGraphics", "CGSize");
		}
	}
}
