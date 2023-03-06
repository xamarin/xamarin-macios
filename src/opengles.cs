//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2015 Xamarin Inc.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace OpenGLES {

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	// <quote>It is created when an EAGLContext object is initialized and disposed of when the last EAGLContext object that references it is released.</quote>
	[DisableDefaultCtor]
	interface EAGLSharegroup {

		[Export ("debugLabel")]
		[NullAllowed]
		string DebugLabel { get; set; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init now marked with NS_UNAVAILABLE
	interface EAGLContext {
		[Export ("initWithAPI:")]
		NativeHandle Constructor (EAGLRenderingAPI api);

		[DesignatedInitializer]
		[Export ("initWithAPI:sharegroup:")]
		NativeHandle Constructor (EAGLRenderingAPI api, EAGLSharegroup sharegroup);

		[Static, Export ("setCurrentContext:")]
		bool SetCurrentContext ([NullAllowed] EAGLContext context);

		[Static, Export ("currentContext")]
		[NullAllowed]
		EAGLContext CurrentContext { get; }

		[Export ("API")]
		EAGLRenderingAPI API { get; }

		[Export ("sharegroup")]
		EAGLSharegroup ShareGroup { get; }

		[Export ("debugLabel")]
		[NullAllowed]
		string DebugLabel { get; set; }

		//
		// These are from @interface EAGLContext (EAGLContextDrawableAdditions)
		//

		[Export ("renderbufferStorage:fromDrawable:")]
		bool RenderBufferStorage (nuint target, [NullAllowed] CoreAnimation.CAEAGLLayer drawable);

		[Export ("presentRenderbuffer:")]
		bool PresentRenderBuffer (nuint target);

		[Internal]
		[Export ("presentRenderbuffer:atTime:")]
		bool _PresentRenderbufferAtTime (nuint target, double presentationTime);

		[Internal]
		[Export ("presentRenderbuffer:afterMinimumDuration:")]
		bool _PresentRenderbufferAfterMinimumDuration (nuint target, double duration);

		[Export ("multiThreaded")]
		bool IsMultiThreaded { [Bind ("isMultiThreaded")] get; set; }

		// IOSurface (EAGLContext)

		[Export ("texImageIOSurface:target:internalFormat:width:height:format:type:plane:")]
		bool TexImage (IOSurface.IOSurface ioSurface, nuint target, nuint internalFormat, uint width, uint height, nuint format, nuint type, uint plane);
	}

	[NoMac]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Protocol]
	// no [Model] because "The EAGLDrawable protocol is not intended to be implemented by objects outside of the iOS."
	interface EAGLDrawable {
		[Abstract]
		[NullAllowed] // by default this property is null
		[Export ("drawableProperties", ArgumentSemantic.Copy)]
		NSDictionary DrawableProperties { get; set; }
	}
}
