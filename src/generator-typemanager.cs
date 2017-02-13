
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using XamCore.CoreFoundation;
using XamCore.CoreGraphics;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.Security;

using DictionaryContainerType = XamCore.Foundation.DictionaryContainer;

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

	/* attributes */
	public static Type AbstractAttribute;
	public static Type AdvancedAttribute;
	public static Type AlignAttribute;
	public static Type AppearanceAttribute;
	public static Type AsyncAttribute;
	public static Type AutoreleaseAttribute;
	public static Type AvailabilityBaseAttribute;
	public static Type BaseTypeAttribute;
	public static Type BindAttribute;
	public static Type BlockCallbackAttribute;
	public static Type CategoryAttribute;
	public static Type CCallbackAttribute;
	public static Type CheckDisposedAttribute;
	public static Type CoreImageFilterAttribute;
	public static Type CoreImageFilterPropertyAttribute;
	public static Type DebuggerBrowsableAttribute;
	public static Type DebuggerDisplayAttribute;
	public static Type DefaultValueAttribute;
	public static Type DefaultValueFromArgumentAttribute;
	public static Type DelegateApiNameAttribute;
	public static Type DelegateNameAttribute;
	public static Type DesignatedInitializerAttribute;
	public static Type DisableZeroCopyAttribute;
	public static Type DisposeAttribute;
	public static Type EditorBrowsableAttribute;
	public static Type EventArgsAttribute;
	public static Type EventNameAttribute;
	public static Type ExportAttribute;
	public static Type FactoryAttribute;
	public static Type FieldAttribute;
	public static Type FieldOffsetAttribute;
	public static Type FlagsAttribute;
	public static Type InternalAttribute;
	public static Type IsThreadStaticAttribute;
	public static Type LinkWithAttribute;
	public static Type ManualAttribute;
	public static Type MarshalAsAttribute;
	public static Type MarshalNativeExceptionsAttribute;
	public static Type ModelAttribute;
	public static Type NativeAttribute;
	public static Type NewAttribute;
	public static Type NoDefaultValueAttribute;
	public static Type NotificationAttribute;
	public static Type NotImplementedAttribute;
	public static Type NullAllowedAttribute;
	public static Type ObsoleteAttribute;
	public static Type OptionalImplementationAttribute;
	public static Type OutAttribute;
	public static Type OverrideAttribute;
	public static Type ParamArrayAttribute;
	public static Type ParamsAttribute;
	public static Type PartialAttribute;
	public static Type PlainStringAttribute;
	public static Type PostGetAttribute;
	public static Type PostSnippetAttribute;
	public static Type PreSnippetAttribute;
	public static Type ProbePresenceAttribute;
	public static Type PrologueSnippetAttribute;
	public static Type ProtectedAttribute;
	public static Type ProtocolAttribute;
	public static Type ProtocolizeAttribute;
	public static Type ProxyAttribute;
	public static Type RegisterAttribute;
	public static Type ReleaseAttribute;
	public static Type RetainAttribute;
	public static Type SealedAttribute;
	public static Type StaticAttribute;
	public static Type StrongDictionaryAttribute;
	public static Type SyntheticAttribute;
	public static Type TargetAttribute;
	public static Type ThreadSafeAttribute;
	public static Type TransientAttribute;
	public static Type UnifiedInternalAttribute;
	public static Type WrapAttribute;
	public static Type ZeroCopyStringsAttribute;

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
	public static Type UIEdgeInsets;
	public static Type UIOffset;

	public static Type CoreGraphics_CGPoint;
	public static Type CoreGraphics_CGRect;
	public static Type CoreGraphics_CGSize;

	static Assembly api_assembly;
	// static Assembly corlib_assembly;
	static Assembly platform_assembly;
	// static Assembly binding_assembly;

	static Type Lookup (Assembly assembly, string @namespace, string @typename, bool inexistentOK = false)
	{
		string fullname;
		string nsManagerPrefix = BindingTouch.NamespacePlatformPrefix;
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

	public static void Initialize (Assembly api)
	{
		api_assembly = api;
		// corlib_assembly = typeof (object).Assembly;
		platform_assembly = typeof (NSObject).Assembly;
		// binding_assembly = typeof (ProtocolizeAttribute).Assembly;

		/* corlib */
		System_Attribute = typeof (System.Attribute);
		System_Boolean = typeof (bool);
		System_Byte = typeof (byte);
		System_Delegate = typeof (System.Delegate);
		System_Double = typeof (double);
		System_Float = typeof (float);
		System_Int16 = typeof (short);
		System_Int32 = typeof (int);
		System_Int64 = typeof (long);
		System_IntPtr = typeof (System.IntPtr);
		System_Object = typeof (object);
		System_SByte = typeof (sbyte);
		System_String = typeof (string);
		System_String_Array = typeof (string[]);
		System_UInt16 = typeof (ushort);
		System_UInt32 = typeof (uint);
		System_UInt64 = typeof (ulong);
		System_Void = typeof (void);

		if (Generator.UnifiedAPI) {
			System_nint = Lookup (platform_assembly, "System", "nint");
			System_nuint = Lookup (platform_assembly, "System", "nuint");
			System_nfloat = Lookup (platform_assembly, "System", "nfloat");
		}

		/* fundamental */
		NSObject = typeof (NSObject);
		INativeObject = typeof (INativeObject);

		/* objcruntime */
		BlockLiteral = typeof (BlockLiteral);
		Class = typeof (Class);
		Protocol = typeof (Protocol);
		Selector = typeof (Selector);

		if (Generator.UnifiedAPI) {
			Constants = Lookup (platform_assembly, "ObjCRuntime", "Constants");
		} else {
			Constants = Lookup (platform_assembly, "", "Constants");
		}

		/* attributes */
		AbstractAttribute = typeof (AbstractAttribute);
		AdvancedAttribute = typeof (AdvancedAttribute);
		AlignAttribute = typeof (AlignAttribute);
		AppearanceAttribute = typeof (AppearanceAttribute);
		AsyncAttribute = typeof (AsyncAttribute);
		AutoreleaseAttribute = typeof (AutoreleaseAttribute);
		AvailabilityBaseAttribute = typeof (AvailabilityBaseAttribute);
		BaseTypeAttribute = typeof (BaseTypeAttribute);
		BindAttribute = typeof (BindAttribute);
		BlockCallbackAttribute = typeof (BlockCallbackAttribute);
		CategoryAttribute = typeof (CategoryAttribute);
		CCallbackAttribute = typeof (CCallbackAttribute);
		CheckDisposedAttribute = typeof (CheckDisposedAttribute);
		CoreImageFilterAttribute = typeof (CoreImageFilterAttribute);
		CoreImageFilterPropertyAttribute = typeof (CoreImageFilterPropertyAttribute);
		DebuggerBrowsableAttribute = typeof (DebuggerBrowsableAttribute);
		DebuggerDisplayAttribute = typeof (DebuggerDisplayAttribute);
		DefaultValueAttribute = typeof (DefaultValueAttribute);
		DefaultValueFromArgumentAttribute = typeof (DefaultValueFromArgumentAttribute);
		DelegateApiNameAttribute = typeof (DelegateApiNameAttribute);
		DelegateNameAttribute = typeof (DelegateNameAttribute);
		DesignatedInitializerAttribute = typeof (DesignatedInitializerAttribute);
		DisableZeroCopyAttribute = typeof (DisableZeroCopyAttribute);
		DisposeAttribute = typeof (DisposeAttribute);
		EditorBrowsableAttribute = typeof (System.ComponentModel.EditorBrowsableAttribute);
		EventArgsAttribute = typeof (EventArgsAttribute);
		EventNameAttribute = typeof (EventNameAttribute);
		ExportAttribute = typeof (ExportAttribute);
		FactoryAttribute = typeof (FactoryAttribute);
		FieldAttribute = typeof (FieldAttribute);
		FieldOffsetAttribute = typeof (FieldOffsetAttribute);
		FlagsAttribute = typeof (FlagsAttribute);
		InternalAttribute = typeof (InternalAttribute);
		IsThreadStaticAttribute = typeof (IsThreadStaticAttribute);
		LinkWithAttribute = typeof (LinkWithAttribute);
		ManualAttribute = typeof (ManualAttribute);
		MarshalAsAttribute = typeof (MarshalAsAttribute);
		MarshalNativeExceptionsAttribute = typeof (MarshalNativeExceptionsAttribute);
		ModelAttribute = typeof (ModelAttribute);
		NativeAttribute = typeof (NativeAttribute);
		NewAttribute = typeof (NewAttribute);
		NoDefaultValueAttribute = typeof (NoDefaultValueAttribute);
		NotificationAttribute = typeof (NotificationAttribute);
		NotImplementedAttribute = typeof (NotImplementedAttribute);
		NullAllowedAttribute = typeof (NullAllowedAttribute);
		ObsoleteAttribute = typeof (ObsoleteAttribute);
		OptionalImplementationAttribute = typeof (OptionalImplementationAttribute);
		OutAttribute = typeof (OutAttribute);
		OverrideAttribute = typeof (OverrideAttribute);
		ParamArrayAttribute = typeof (ParamArrayAttribute);
		ParamsAttribute = typeof (ParamsAttribute);
		PartialAttribute = typeof (PartialAttribute);
		PlainStringAttribute = typeof (PlainStringAttribute);
		PostGetAttribute = typeof (PostGetAttribute);
		PostSnippetAttribute = typeof (PostSnippetAttribute);
		PreSnippetAttribute = typeof (PreSnippetAttribute);
		ProbePresenceAttribute = typeof (ProbePresenceAttribute);
		PrologueSnippetAttribute = typeof (PrologueSnippetAttribute);
		ProtectedAttribute = typeof (ProtectedAttribute);
		ProtocolAttribute = typeof (ProtocolAttribute);
		ProtocolizeAttribute = typeof (ProtocolizeAttribute);
		ProxyAttribute = typeof (ProxyAttribute);
		RegisterAttribute = typeof (RegisterAttribute);
		ReleaseAttribute = typeof (ReleaseAttribute);
		RetainAttribute = typeof (RetainAttribute);
		SealedAttribute = typeof (SealedAttribute);
		StaticAttribute = typeof (StaticAttribute);
		StrongDictionaryAttribute = typeof (StrongDictionaryAttribute);
		SyntheticAttribute = typeof (SyntheticAttribute);
		TargetAttribute = typeof (TargetAttribute);
		ThreadSafeAttribute = typeof (ThreadSafeAttribute);
		TransientAttribute = typeof (TransientAttribute);
		UnifiedInternalAttribute = typeof (UnifiedInternalAttribute);
		WrapAttribute = typeof (WrapAttribute);
		ZeroCopyStringsAttribute = typeof (ZeroCopyStringsAttribute);

		/* Different binding types */

		DictionaryContainerType = typeof (DictionaryContainerType);

		if (Frameworks.HaveAddressBook) {
			ABAddressBook = Lookup (platform_assembly, "AddressBook", "ABAddressBook");
			ABPerson = Lookup (platform_assembly, "AddressBook", "ABPerson");
			ABRecord = Lookup (platform_assembly, "AddressBook", "ABRecord");
		}
		if (Frameworks.HaveAudioToolbox) {
			AudioBuffers = Lookup (platform_assembly, "AudioToolbox", "AudioBuffers");
			MusicSequence = Lookup (platform_assembly, "AudioToolbox", "MusicSequence", true /* may not be found */);
		}
		if (Frameworks.HaveAudioUnit) {
			AudioComponent = Lookup (platform_assembly, "AudioUnit", "AudioComponent");
			AudioUnit = Lookup (platform_assembly, "AudioUnit", "AudioUnit");
			AURenderEventEnumerator = Lookup (platform_assembly, "AudioUnit", "AURenderEventEnumerator");
		}
		AVCaptureWhiteBalanceGains = typeof (XamCore.AVFoundation.AVCaptureWhiteBalanceGains);
		if (Frameworks.HaveCoreAnimation)
			CATransform3D = Lookup (platform_assembly, "CoreAnimation", "CATransform3D");

		CFRunLoop = typeof (CFRunLoop);
		CGAffineTransform = typeof (CGAffineTransform);
		CGColor = typeof (CGColor);
		CGColorSpace = typeof (CGColorSpace);
		CGContext = typeof (CGContext);
		CGGradient = typeof (CGGradient);
		CGImage = typeof (CGImage);
		CGLayer = typeof (CGLayer);
		if (Frameworks.HaveOpenGL) {
			CGLContext = Lookup (platform_assembly, "OpenGL", "CGLContext");
			CGLPixelFormat = Lookup (platform_assembly, "OpenGL", "CGLPixelFormat");
		}
		CGPath = typeof (CGPath);
		CGVector = typeof (CGVector);
		if (Frameworks.HaveCoreLocation)
			CLLocationCoordinate2D = Lookup (platform_assembly, "CoreLocation", "CLLocationCoordinate2D");
		if (Frameworks.HaveCoreMedia) {
			CMAudioFormatDescription = Lookup (platform_assembly, "CoreMedia", "CMAudioFormatDescription");
			CMClock = Lookup (platform_assembly, "CoreMedia", "CMClock");
			CMFormatDescription = Lookup (platform_assembly, "CoreMedia", "CMFormatDescription");
			CMSampleBuffer = Lookup (platform_assembly, "CoreMedia", "CMSampleBuffer");
			CMTime = Lookup (platform_assembly, "CoreMedia", "CMTime");
			CMTimebase = Lookup (platform_assembly, "CoreMedia", "CMTimebase");
			CMTimeMapping = Lookup (platform_assembly, "CoreMedia", "CMVideoFormatDescription");
			CMTimeRange = Lookup (platform_assembly, "CoreMedia", "CMTimeRange");
			CMVideoFormatDescription = Lookup (platform_assembly, "CoreMedia", "CMVideoFormatDescription");
		}
		if (Frameworks.HaveCoreVideo) {
			CVImageBuffer = Lookup (platform_assembly, "CoreVideo", "CVImageBuffer");
			CVPixelBuffer = Lookup (platform_assembly, "CoreVideo", "CVPixelBuffer");
			CVPixelBufferPool = Lookup (platform_assembly, "CoreVideo", "CVPixelBufferPool");
		}
		DispatchQueue = typeof (DispatchQueue);
		if (Frameworks.HaveCoreMidi)
			MidiEndpoint = Lookup (platform_assembly, "CoreMidi", "MidiEndpoint");
		if (Frameworks.HaveMapKit)
			MKCoordinateSpan = Lookup (platform_assembly, "MapKit", "MKCoordinateSpan", true /* isn't in XM/Classic */);
		if (Frameworks.HaveMediaToolbox)
			MTAudioProcessingTap = Lookup (platform_assembly, "MediaToolbox", "MTAudioProcessingTap");
		NSNumber = Lookup (BindingTouch.BindingThirdParty ? platform_assembly : api_assembly, "Foundation", "NSNumber");
		NSRange = typeof (NSRange);
		NSString = typeof (NSString);
		NSValue = Lookup (BindingTouch.BindingThirdParty ? platform_assembly : api_assembly, "Foundation", "NSValue");
		NSZone = typeof (NSZone);
		SCNVector3 = typeof (XamCore.SceneKit.SCNVector3);
		SCNVector4 = typeof (XamCore.SceneKit.SCNVector4);
		SCNMatrix4 = typeof (XamCore.SceneKit.SCNMatrix4);
		SecAccessControl = typeof (SecAccessControl);
		SecIdentity = typeof (SecIdentity);
		SecTrust = typeof (SecTrust);
		if (Frameworks.HaveUIKit) {
			UIOffset = Lookup (platform_assembly, "UIKit", "UIOffset");
			UIEdgeInsets = Lookup (platform_assembly, "UIKit", "UIEdgeInsets");
		}

		if (Generator.UnifiedAPI) {
			CoreGraphics_CGRect = Lookup (platform_assembly, "CoreGraphics", "CGRect");
			CoreGraphics_CGPoint = Lookup (platform_assembly, "CoreGraphics", "CGPoint");
			CoreGraphics_CGSize = Lookup (platform_assembly, "CoreGraphics", "CGSize");
		}
	}
}
