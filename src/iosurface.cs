//
// IOSurface bindings
//
// Authors:
//	Miguel de Icaza  <miguel@microsoft.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.IOSurface {

	[Static]
	interface IOSurfacePropertyKey {
		[Field ("IOSurfacePropertyAllocSizeKey")]
		[iOS (10,0), Mac (10,12)]
		NSString AllocSizeKey { get; }
		
		[Field ("IOSurfacePropertyKeyWidth")]
		[iOS (10,0), Mac (10,12)]
		NSString WidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyHeight")]
		[iOS (10,0), Mac (10,12)]
		NSString HeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyBytesPerRow")]
		[iOS (10,0), Mac (10,12)]
		NSString BytesPerRowKey { get; }
		
		[Field ("IOSurfacePropertyKeyBytesPerElement")]
		[iOS (10,0), Mac (10,12)]
		NSString BytesPerElementKey { get; }
		
		[Field ("IOSurfacePropertyKeyElementWidth")]
		[iOS (10,0), Mac (10,12)]
		NSString ElementWidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyElementHeight")]
		[iOS (10,0), Mac (10,12)]
		NSString ElementHeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyOffset")]
		[iOS (10,0), Mac (10,12)]
		NSString OffsetKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneInfo")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneInfoKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneWidth")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneWidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneHeight")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneHeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneBytesPerRow")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneBytesPerRowKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneOffset")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneOffsetKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneSize")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneSizeKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneBase")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneBaseKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneBytesPerElement")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneBytesPerElementKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneElementWidth")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneElementWidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneElementHeight")]
		[iOS (10,0), Mac (10,12)]
		NSString PlaneElementHeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyCacheMode")]
		[iOS (10,0), Mac (10,12)]
		NSString CacheModeKey { get; }
		
		[Field ("IOSurfacePropertyKeyPixelFormat")]
		[iOS (10,0), Mac (10,12)]
		NSString PixelFormatKey { get; }
		
		[Field ("IOSurfacePropertyKeyPixelSizeCastingAllowed")]
		[iOS (10,0), Mac (10,12)]
		NSString PixelSizeCastingAllowedKey { get; } 
	}

	[StrongDictionary ("IOSurfacePropertyKey")]
	partial interface IOSurfaceOptions {
		nint AllocSize { get; set; }
		nint Width { get; set; }
		nint Height { get; set; }
		nint BytesPerRow { get; set; }
		nint BytesPerElement { get; set; }
		nint ElementWidth { get; set; }
		nint ElementHeight { get; set; }
		nint Offset { get; set; }
		NSDictionary [] PlaneInfo { get; set; }
		nint PlaneWidth { get; set; }
		nint PlaneHeight { get; set; }
		nint PlaneBytesPerRow { get; set; }
		nint PlaneOffset { get; set; }
		nint PlaneSize { get; set; }
		nint PlaneBase { get; set; }
		nint PlaneBytesPerElement { get; set; }
		nint PlaneElementWidth { get; set; }
		nint PlaneElementHeight { get; set; }
		IOSurfaceMemoryMap CacheMode { get; set; }
		uint PixelFormat { get; set; }
		bool PixelSizeCastingAllowed { get; set; }
	}

	// @interface IOSurface : NSObject <NSSecureCoding>
	[iOS (10,0), Mac(10,12)]
	[BaseType (typeof(NSObject))]
	interface IOSurface : NSSecureCoding
	{
		[Internal, Export ("initWithProperties:")]
		IntPtr Constructor (NSDictionary properties);

		[Wrap ("this (properties == null ? null : properties.Dictionary)")]
		IntPtr Constructor (IOSurfaceOptions properties);
	
		[Internal, Export ("lockWithOptions:seed:")]
		int _Lock (IOSurfaceLockOptions options, IntPtr seedPtr);
	
		[Internal, Export ("unlockWithOptions:seed:")]
		int _Unlock (IOSurfaceLockOptions options, IntPtr seed);
	
		[Export ("allocationSize")]
		nint AllocationSize { get; }
	
		[Export ("width")]
		nint Width { get; }
	
		[Export ("height")]
		nint Height { get; }
	
		[Export ("baseAddress")]
		IntPtr BaseAddress { get; }
	
		[Export ("pixelFormat")]
		uint PixelFormat { get; }
	
		[Export ("bytesPerRow")]
		nint BytesPerRow { get; }
	
		[Export ("bytesPerElement")]
		nint BytesPerElement { get; }
	
		[Export ("elementWidth")]
		nint ElementWidth { get; }
	
		[Export ("elementHeight")]
		nint ElementHeight { get; }
	
		[Export ("seed")]
		uint Seed { get; }
	
		[Export ("planeCount")]
		nuint PlaneCount { get; }
	
		[Export ("widthOfPlaneAtIndex:")]
		nint WidthOfPlane (nuint planeIndex);
	
		[Export ("heightOfPlaneAtIndex:")]
		nint HeightOfPlane (nuint planeIndex);
	
		[Export ("bytesPerRowOfPlaneAtIndex:")]
		nint BytesPerRowOfPlane (nuint planeIndex);
	
		[Export ("bytesPerElementOfPlaneAtIndex:")]
		nint BytesPerElementOfPlane (nuint planeIndex);
	
		[Export ("elementWidthOfPlaneAtIndex:")]
		nint ElementWidthOfPlane (nuint planeIndex);
	
		[Export ("elementHeightOfPlaneAtIndex:")]
		nint ElementHeightOfPlane (nuint planeIndex);
	
		[Export ("baseAddressOfPlaneAtIndex:")]
		IntPtr BaseAddressOfPlane (nuint planeIndex);
	
		[Export ("setAttachment:forKey:")]
		void SetAttachment (NSObject anObject, NSString key);
	
		[Export ("attachmentForKey:")]
		[return: NullAllowed]
		NSObject GetAttachment (NSString key);
	
		[Export ("removeAttachmentForKey:")]
		void RemoveAttachment (NSString key);
	
		[NullAllowed, Export ("allAttachments")]
		NSDictionary<NSString, NSObject> AllAttachments { get; set; }
	
		[Export ("removeAllAttachments")]
		void RemoveAllAttachments ();
	
		[Export ("inUse")]
		bool InUse { [Bind ("isInUse")] get; }
	
		[Export ("incrementUseCount")]
		void IncrementUseCount ();
	
		[Export ("decrementUseCount")]
		void DecrementUseCount ();
	
		[Export ("localUseCount")]
		int LocalUseCount { get; }
	
		[Export ("allowsPixelSizeCasting")]
		bool AllowsPixelSizeCasting { get; }
	
		[iOS (11,0)]
		[Internal, Export ("setPurgeable:oldState:")]
		int _SetPurgeable (IOSurfacePurgeabilityState newState, IntPtr oldStatePtr);
	}
}

#endif // XAMCORE_2_0