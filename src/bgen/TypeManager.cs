using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

public class TypeManager {
	Frameworks? Frameworks { get; set; }

	public Type? System_Attribute {get; private set;} 
	public Type? System_Boolean {get; private set;} 
	public Type? System_Byte {get; private set;} 
	public Type? System_Char {get; private set;} 
	public Type? System_Delegate {get; private set;} 
	public Type? System_Double {get; private set;} 
	public Type? System_Float {get; private set;} 
	public Type? System_Int16 {get; private set;} 
	public Type? System_Int32 {get; private set;} 
	public Type? System_Int64 {get; private set;} 
	public Type? System_IntPtr {get; private set;} 
	public Type? System_Object {get; private set;} 
	public Type? System_SByte {get; private set;} 
	public Type? System_String {get; private set;} 
	public Type? System_String_Array {get; private set;} 
	public Type? System_UInt16 {get; private set;} 
	public Type? System_UInt32 {get; private set;} 
	public Type? System_UInt64 {get; private set;} 
	public Type? System_UIntPtr {get; private set;} 
	public Type? System_Void {get; private set;} 

	public Type? System_nint {get; private set;} 
	public Type? System_nuint {get; private set;} 
	public Type? System_nfloat {get; private set;} 

	/* fundamental */
	public Type? NSObject {get; private set;} 
	public Type? INativeObject {get; private set;} 

	/* objcruntime */
	public Type? BlockLiteral {get; private set;} 
	public Type? Class {get; private set;} 
	public Type? Protocol {get; private set;} 
	public Type? Selector {get; private set;} 

	public Type? Constants {get; private set;} 

	/* Different binding types */

	public Type? DictionaryContainerType {get; private set;} 

	public Type? ABAddressBook {get; private set;} 
	public Type? ABPerson {get; private set;} 
	public Type? ABRecord {get; private set;} 
	public Type? AudioBuffers {get; private set;} 
	public Type? AudioComponent {get; private set;} 
	public Type? AudioUnit {get; private set;} 
	public Type? AURenderEventEnumerator {get; private set;} 
	public Type? AVCaptureWhiteBalanceGains {get; private set;} 
	public Type? CATransform3D {get; private set;} 
	public Type? CFRunLoop {get; private set;} 
	public Type? CGAffineTransform {get; private set;} 
	public Type? CGColor {get; private set;} 
	public Type? CGColorSpace {get; private set;} 
	public Type? CGContext {get; private set;} 
	public Type? CGPDFDocument {get; private set;} 
	public Type? CGPDFPage {get; private set;} 
	public Type? CGGradient {get; private set;} 
	public Type? CGImage {get; private set;} 
	public Type? CGImageSource {get; private set;} 
	public Type? CGLayer {get; private set;} 
	public Type? CGLContext {get; private set;} 
	public Type? CGLPixelFormat {get; private set;} 
	public Type? CGPath {get; private set;} 
	public Type? CGVector {get; private set;} 
	public Type? CLLocationCoordinate2D {get; private set;} 
	public Type? CMAudioFormatDescription {get; private set;} 
	public Type? CMClock {get; private set;} 
	public Type? CMFormatDescription {get; private set;} 
	public Type? CMSampleBuffer {get; private set;} 
	public Type? CMTime {get; private set;} 
	public Type? CMTimebase {get; private set;} 
	public Type? CMTimeMapping {get; private set;} 
	public Type? CMTimeRange {get; private set;} 
	public Type? CMVideoFormatDescription {get; private set;} 
	public Type? CMVideoDimensions {get; private set;} 
	public Type? CVImageBuffer {get; private set;} 
	public Type? CVPixelBuffer {get; private set;} 
	public Type? CVPixelBufferPool {get; private set;} 
	public Type? DispatchQueue {get; private set;} 
	public Type? DispatchData {get; private set;} 
	public Type? MidiEndpoint {get; private set;} 
	public Type? MKCoordinateSpan {get; private set;} 
	public Type? MTAudioProcessingTap {get; private set;} 
	public Type? MusicSequence {get; private set;} 
	public Type? NSNumber {get; private set;} 
	public Type? NSRange {get; private set;} 
	public Type? NSString {get; private set;} 
	public Type? NSValue {get; private set;} 
	public Type? NSZone {get; private set;} 
	public Type? SCNMatrix4 {get; private set;} 
	public Type? SCNVector3 {get; private set;} 
	public Type? SCNVector4 {get; private set;} 
	public Type? SecAccessControl {get; private set;} 
	public Type? SecIdentity {get; private set;} 
	public Type? SecTrust {get; private set;} 
	public Type? SecProtocolMetadata {get; private set;} 
	public Type? SecProtocolOptions {get; private set;} 
	public Type? SecTrust2 {get; private set;} 
	public Type? SecIdentity2 {get; private set;} 
	public Type? UIEdgeInsets {get; private set;} 
	public Type? UIOffset {get; private set;} 
	public Type? NSDirectionalEdgeInsets {get; private set;} 

	public Type? CoreGraphics_CGPoint {get; private set;} 
	public Type? CoreGraphics_CGRect {get; private set;} 
	public Type? CoreGraphics_CGSize {get; private set;} 

	Assembly? apiAssembly;
	Assembly? corlibAssembly;
	Assembly? platformAssembly;
	
	Dictionary<Type, string>? nsnumberReturnMap;
	public Dictionary<Type, string> NSNumberReturnMap {
		get {
			if (nsnumberReturnMap is not null) 
				return nsnumberReturnMap;
			Tuple<Type?, string> [] typeMap = {
				new ( System_Boolean, ".BoolValue" ),
				new ( System_Byte, ".ByteValue" ),
				new ( System_Double, ".DoubleValue" ),
				new ( System_Float, ".FloatValue" ),
				new ( System_Int16, ".Int16Value" ),
				new ( System_Int32, ".Int32Value" ),
				new ( System_Int64, ".Int64Value" ),
				new ( System_SByte, ".SByteValue" ),
				new ( System_UInt16, ".UInt16Value" ),
				new ( System_UInt32, ".UInt32Value" ),
				new ( System_UInt64, ".UInt64Value" ),
				new (System_nfloat, ".NFloatValue"),
				new (System_nint, ".NIntValue"),
				new (System_nuint, ".NUIntValue"),
			};
			nsnumberReturnMap = new();
			foreach (var tuple in typeMap) {
				if (tuple.Item1 is not null)
					nsnumberReturnMap[tuple.Item1] = tuple.Item2;
			}
			return nsnumberReturnMap;
		}
	}

	Dictionary<Type, string>? nsvalueReturnMap;
	public Dictionary<Type, string> NSValueReturnMap {
		get {
			if (nsvalueReturnMap != null)
				return nsvalueReturnMap;
			Tuple<Type?, string> [] general = {
				new (CGAffineTransform, ".CGAffineTransformValue" ),
				new (NSRange, ".RangeValue" ),
				new (CGVector, ".CGVectorValue" ),
				new (SCNMatrix4, ".SCNMatrix4Value" ),
				new (CLLocationCoordinate2D, ".CoordinateValue" ),
				new (SCNVector3, ".Vector3Value" ),
				new (SCNVector4, ".Vector4Value" ),
				new (CoreGraphics_CGPoint, ".CGPointValue"),
				new (CoreGraphics_CGRect, ".CGRectValue"),
				new (CoreGraphics_CGSize, ".CGSizeValue"),
				new (MKCoordinateSpan, ".CoordinateSpanValue"),
			};

			Tuple<Type?, string> [] uiKitMap = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks?.HaveUIKit == true)
				uiKitMap = new Tuple<Type?, string> [] {
					new (UIEdgeInsets, ".UIEdgeInsetsValue"),
					new (UIOffset, ".UIOffsetValue"),
					new (NSDirectionalEdgeInsets, ".DirectionalEdgeInsetsValue"),
				};

			Tuple<Type?, string> [] coreMedia = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks?.HaveCoreMedia == true)
				uiKitMap = new Tuple<Type?, string> [] {
					new (CMTimeRange, ".CMTimeRangeValue"),
					new (CMTime, ".CMTimeValue"),
					new (CMTimeMapping, ".CMTimeMappingValue"),
					new (CMVideoDimensions, ".CMVideoDimensionsValue"),
				};

			Tuple<Type?, string> [] animation = Array.Empty<Tuple<Type?, string>> ();
			if (Frameworks?.HaveCoreAnimation == true)
				animation = new Tuple<Type?, string> [] {
					new (CATransform3D, ".CATransform3DValue"),
				};

			nsvalueReturnMap = new();
			foreach (var typeMap in new [] {general, uiKitMap, coreMedia, animation}) {
				foreach (var tuple in typeMap) {
					if (tuple.Item1 is not null)
						nsvalueReturnMap[tuple.Item1] = tuple.Item2;
				}
			}
			return nsvalueReturnMap;
		}
	}


	Type? Lookup (Assembly assembly, string @namespace, string typename, bool inexistentOK = false)
	{
		string fullname;

		if (string.IsNullOrEmpty (@namespace)) {
			fullname = typename;
		} else {
			fullname = @namespace + "." + typename;
		}

		var rv = assembly.GetType (fullname);
		if (rv is null && !inexistentOK)
			throw new BindingException (1052, true, fullname, assembly);
		return rv;
	}

	public Type? GetUnderlyingNullableType (Type type)
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

	public void Initialize (BindingTouch bindingTouch, Assembly api, Assembly corlib, Assembly platform)
	{
		if (bindingTouch.universe is null)
			throw ErrorHelper.CreateError (4,  bindingTouch.CurrentPlatform );
		if (bindingTouch.Frameworks is null)
			throw ErrorHelper.CreateError (3,  bindingTouch.CurrentPlatform );
		
		Frameworks = bindingTouch.Frameworks;

		apiAssembly = api;
		corlibAssembly = corlib;
		platformAssembly = platform;

		/* corlib */
		System_Attribute = Lookup (corlibAssembly, "System", "Attribute");
		System_Boolean = Lookup (corlibAssembly, "System", "Boolean");
		System_Byte = Lookup (corlibAssembly, "System", "Byte");
		System_Char = Lookup (corlibAssembly, "System", "Char");
		System_Delegate = Lookup (corlibAssembly, "System", "Delegate");
		System_Double = Lookup (corlibAssembly, "System", "Double");
		System_Float = Lookup (corlibAssembly, "System", "Single");
		System_Int16 = Lookup (corlibAssembly, "System", "Int16");
		System_Int32 = Lookup (corlibAssembly, "System", "Int32");
		System_Int64 = Lookup (corlibAssembly, "System", "Int64");
		System_IntPtr = Lookup (corlibAssembly, "System", "IntPtr");
		System_Object = Lookup (corlibAssembly, "System", "Object");
		System_SByte = Lookup (corlibAssembly, "System", "SByte");
		System_String = Lookup (corlibAssembly, "System", "String");
		System_String_Array = Lookup (corlibAssembly, "System", "String")?.MakeArrayType ();
		System_UInt16 = Lookup (corlibAssembly, "System", "UInt16");
		System_UInt32 = Lookup (corlibAssembly, "System", "UInt32");
		System_UInt64 = Lookup (corlibAssembly, "System", "UInt64");
		System_UIntPtr = Lookup (corlibAssembly, "System", "UIntPtr");
		System_Void = Lookup (corlibAssembly, "System", "Void");

#if NET
		System_nint = Lookup (corlibAssembly, "System", "IntPtr");
		System_nuint = Lookup (corlibAssembly, "System", "UIntPtr");
		var interopAssembly = binding_touch.universe.LoadFromAssemblyName ("System.Runtime.InteropServices");
		System_nfloat = Lookup (interopAssembly, "System.Runtime.InteropServices", "NFloat");
#else
		System_nint = Lookup (platformAssembly, "System", "nint");
		System_nuint = Lookup (platformAssembly, "System", "nuint");
		System_nfloat = Lookup (platformAssembly, "System", "nfloat");
#endif

		/* fundamental */
		NSObject = Lookup (platformAssembly, "Foundation", "NSObject");
		INativeObject = Lookup (platformAssembly, "ObjCRuntime", "INativeObject");

		/* objcruntime */
		BlockLiteral = Lookup (platformAssembly, "ObjCRuntime", "BlockLiteral");
		Class = Lookup (platformAssembly, "ObjCRuntime", "Class");
		Protocol = Lookup (platformAssembly, "ObjCRuntime", "Protocol");
		Selector = Lookup (platformAssembly, "ObjCRuntime", "Selector");

		Constants = Lookup (platformAssembly, "ObjCRuntime", "Constants");

		/* Different binding types */

		DictionaryContainerType = Lookup (platformAssembly, "Foundation", "DictionaryContainer");

		if (Frameworks.HaveAddressBook) {
			ABAddressBook = Lookup (platformAssembly, "AddressBook", "ABAddressBook");
			ABPerson = Lookup (platformAssembly, "AddressBook", "ABPerson");
			ABRecord = Lookup (platformAssembly, "AddressBook", "ABRecord");
		}
		// misplaced API, it's really in CoreAudio (now available everywhere)
		AudioBuffers = Lookup (platformAssembly, "AudioToolbox", "AudioBuffers");
		if (Frameworks.HaveAudioToolbox) {
			MusicSequence = Lookup (platformAssembly, "AudioToolbox", "MusicSequence", true /* may not be found */);
		}
		if (Frameworks.HaveAudioUnit) {
			AudioComponent = Lookup (platformAssembly, "AudioUnit", "AudioComponent");
			AudioUnit = Lookup (platformAssembly, "AudioUnit", "AudioUnit");
			AURenderEventEnumerator = Lookup (platformAssembly, "AudioUnit", "AURenderEventEnumerator");
		}
		AVCaptureWhiteBalanceGains = Lookup (platformAssembly, "AVFoundation", "AVCaptureWhiteBalanceGains");
		if (Frameworks.HaveCoreAnimation)
			CATransform3D = Lookup (platformAssembly, "CoreAnimation", "CATransform3D");

		CFRunLoop = Lookup (platformAssembly, "CoreFoundation", "CFRunLoop");
		CGAffineTransform = Lookup (platformAssembly, "CoreGraphics", "CGAffineTransform");
		CGColor = Lookup (platformAssembly, "CoreGraphics", "CGColor");
		CGColorSpace = Lookup (platformAssembly, "CoreGraphics", "CGColorSpace");
		CGContext = Lookup (platformAssembly, "CoreGraphics", "CGContext");
		CGPDFDocument = Lookup (platformAssembly, "CoreGraphics", "CGPDFDocument");
		CGPDFPage = Lookup (platformAssembly, "CoreGraphics", "CGPDFPage");
		CGGradient = Lookup (platformAssembly, "CoreGraphics", "CGGradient");
		CGImage = Lookup (platformAssembly, "CoreGraphics", "CGImage");
		CGImageSource = Lookup (platformAssembly, "ImageIO", "CGImageSource");
		CGLayer = Lookup (platformAssembly, "CoreGraphics", "CGLayer");
		if (Frameworks.HaveOpenGL) {
			CGLContext = Lookup (platformAssembly, "OpenGL", "CGLContext");
			CGLPixelFormat = Lookup (platformAssembly, "OpenGL", "CGLPixelFormat");
		}
		CGPath = Lookup (platformAssembly, "CoreGraphics", "CGPath");
		CGVector = Lookup (platformAssembly, "CoreGraphics", "CGVector");
		if (Frameworks.HaveCoreLocation)
			CLLocationCoordinate2D = Lookup (platformAssembly, "CoreLocation", "CLLocationCoordinate2D");
		if (Frameworks.HaveCoreMedia) {
			CMAudioFormatDescription = Lookup (platformAssembly, "CoreMedia", "CMAudioFormatDescription");
			CMClock = Lookup (platformAssembly, "CoreMedia", "CMClock");
			CMFormatDescription = Lookup (platformAssembly, "CoreMedia", "CMFormatDescription");
			CMSampleBuffer = Lookup (platformAssembly, "CoreMedia", "CMSampleBuffer");
			CMTime = Lookup (platformAssembly, "CoreMedia", "CMTime");
			CMTimebase = Lookup (platformAssembly, "CoreMedia", "CMTimebase");
			CMTimeMapping = Lookup (platformAssembly, "CoreMedia", "CMTimeMapping");
			CMTimeRange = Lookup (platformAssembly, "CoreMedia", "CMTimeRange");
			CMVideoFormatDescription = Lookup (platformAssembly, "CoreMedia", "CMVideoFormatDescription");
			CMVideoDimensions = Lookup (platformAssembly, "CoreMedia", "CMVideoDimensions");
		}
		if (Frameworks.HaveCoreVideo) {
			CVImageBuffer = Lookup (platformAssembly, "CoreVideo", "CVImageBuffer");
			CVPixelBuffer = Lookup (platformAssembly, "CoreVideo", "CVPixelBuffer");
			CVPixelBufferPool = Lookup (platformAssembly, "CoreVideo", "CVPixelBufferPool");
		}
		DispatchQueue = Lookup (platformAssembly, "CoreFoundation", "DispatchQueue");
		DispatchData = Lookup (platformAssembly, "CoreFoundation", "DispatchData");
		if (Frameworks.HaveCoreMidi)
			MidiEndpoint = Lookup (platformAssembly, "CoreMidi", "MidiEndpoint");
		if (Frameworks.HaveMapKit)
			MKCoordinateSpan = Lookup (platformAssembly, "MapKit", "MKCoordinateSpan", true /* isn't in XM/Classic */);
		if (Frameworks.HaveMediaToolbox)
			MTAudioProcessingTap = Lookup (platformAssembly, "MediaToolbox", "MTAudioProcessingTap");
		NSNumber = Lookup (bindingTouch.BindThirdPartyLibrary ? platformAssembly : apiAssembly, "Foundation", "NSNumber");
		NSRange = Lookup (platformAssembly, "Foundation", "NSRange");
		NSString = Lookup (platformAssembly, "Foundation", "NSString");
		NSValue = Lookup (bindingTouch.BindThirdPartyLibrary ? platformAssembly : apiAssembly, "Foundation", "NSValue");
		NSZone = Lookup (platformAssembly, "Foundation", "NSZone");
		SCNVector3 = Lookup (platformAssembly, "SceneKit", "SCNVector3");
		SCNVector4 = Lookup (platformAssembly, "SceneKit", "SCNVector4");
		SCNMatrix4 = Lookup (platformAssembly, "SceneKit", "SCNMatrix4");
		SecAccessControl = Lookup (platformAssembly, "Security", "SecAccessControl");
		SecIdentity = Lookup (platformAssembly, "Security", "SecIdentity");
		SecTrust = Lookup (platformAssembly, "Security", "SecTrust");
		SecProtocolOptions = Lookup (platformAssembly, "Security", "SecProtocolOptions");
		SecProtocolMetadata = Lookup (platformAssembly, "Security", "SecProtocolMetadata");
		SecTrust2 = Lookup (platformAssembly, "Security", "SecTrust2");
		SecIdentity2 = Lookup (platformAssembly, "Security", "SecIdentity2");
		if (Frameworks.HaveUIKit) {
			UIOffset = Lookup (platformAssembly, "UIKit", "UIOffset");
			UIEdgeInsets = Lookup (platformAssembly, "UIKit", "UIEdgeInsets");
			NSDirectionalEdgeInsets = Lookup (platformAssembly, "UIKit", "NSDirectionalEdgeInsets");
		}

		CoreGraphics_CGRect = Lookup (platformAssembly, "CoreGraphics", "CGRect");
		CoreGraphics_CGPoint = Lookup (platformAssembly, "CoreGraphics", "CGPoint");
		CoreGraphics_CGSize = Lookup (platformAssembly, "CoreGraphics", "CGSize");
	}
}
