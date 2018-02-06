//
// IOSurface bindings
//
// Authors:
//	Miguel de Icaza  <miguel@microsoft.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

using System;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace IOSurface {

	[Static]
	[Internal]
	// See bug #59201 [iOS (10,0)]
	[iOS (11, 0)]
	[TV (11,0)]
	[Mac (10, 12)]
	interface IOSurfacePropertyKey {
		[Field ("IOSurfacePropertyAllocSizeKey")]
		NSString AllocSizeKey { get; }
		
		[Field ("IOSurfacePropertyKeyWidth")]
		NSString WidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyHeight")]
		NSString HeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyBytesPerRow")]
		NSString BytesPerRowKey { get; }
		
		[Field ("IOSurfacePropertyKeyBytesPerElement")]
		NSString BytesPerElementKey { get; }
		
		[Field ("IOSurfacePropertyKeyElementWidth")]
		NSString ElementWidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyElementHeight")]
		NSString ElementHeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyOffset")]
		NSString OffsetKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneInfo")]
		NSString PlaneInfoKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneWidth")]
		NSString PlaneWidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneHeight")]
		NSString PlaneHeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneBytesPerRow")]
		NSString PlaneBytesPerRowKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneOffset")]
		NSString PlaneOffsetKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneSize")]
		NSString PlaneSizeKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneBase")]
		NSString PlaneBaseKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneBytesPerElement")]
		NSString PlaneBytesPerElementKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneElementWidth")]
		NSString PlaneElementWidthKey { get; }
		
		[Field ("IOSurfacePropertyKeyPlaneElementHeight")]
		NSString PlaneElementHeightKey { get; }
		
		[Field ("IOSurfacePropertyKeyCacheMode")]
		NSString CacheModeKey { get; }
		
		[Field ("IOSurfacePropertyKeyPixelFormat")]
		NSString PixelFormatKey { get; }
		
		[Field ("IOSurfacePropertyKeyPixelSizeCastingAllowed")]
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

	[iOS (11,0)]
	[TV (11,0)]
	[Mac (10,12)]
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
		nint GetWidth (nuint planeIndex);
	
		[Export ("heightOfPlaneAtIndex:")]
		nint GetHeight (nuint planeIndex);
	
		[Export ("bytesPerRowOfPlaneAtIndex:")]
		nint GetBytesPerRow (nuint planeIndex);
	
		[Export ("bytesPerElementOfPlaneAtIndex:")]
		nint GetBytesPerElement (nuint planeIndex);
	
		[Export ("elementWidthOfPlaneAtIndex:")]
		nint GetElementWidth (nuint planeIndex);
	
		[Export ("elementHeightOfPlaneAtIndex:")]
		nint GetElementHeight (nuint planeIndex);
	
		[Export ("baseAddressOfPlaneAtIndex:")]
		IntPtr GetBaseAddress (nuint planeIndex);
	
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
	
		[iOS (11,0)][Mac (10,13)]
		[Internal, Export ("setPurgeable:oldState:")]
		int _SetPurgeable (IOSurfacePurgeabilityState newState, IntPtr oldStatePtr);
	}
}
