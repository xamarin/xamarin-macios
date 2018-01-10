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

namespace OpenGLES {

	[BaseType (typeof (NSObject))]
	// <quote>It is created when an EAGLContext object is initialized and disposed of when the last EAGLContext object that references it is released.</quote>
	[DisableDefaultCtor]
	interface EAGLSharegroup {

		[iOS (6,0)]
		[Export ("debugLabel")]
		[NullAllowed]
		string DebugLabel { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init now marked with NS_UNAVAILABLE
	interface EAGLContext {
		[Export ("initWithAPI:")]
		IntPtr Constructor (EAGLRenderingAPI api);

		[DesignatedInitializer]
		[Export ("initWithAPI:sharegroup:")]
		IntPtr Constructor (EAGLRenderingAPI api, EAGLSharegroup sharegroup);

		[Static, Export("setCurrentContext:")]
		bool SetCurrentContext([NullAllowed] EAGLContext context);

		[Static, Export("currentContext")]
		[NullAllowed]
		EAGLContext CurrentContext { get; }

		[Export("API")]
		EAGLRenderingAPI API { get; }

		[Export("sharegroup")]
		EAGLSharegroup ShareGroup { get; }

		[iOS (6,0)]
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

		[iOS (10,0)][TV (10,0)]
		[Internal]
		[Export ("presentRenderbuffer:atTime:")]
		bool _PresentRenderbufferAtTime (nuint target, double presentationTime);

		[iOS (10,3)][TV (10,2)]
		[Internal]
		[Export ("presentRenderbuffer:afterMinimumDuration:")]
		bool _PresentRenderbufferAfterMinimumDuration (nuint target, double duration);

		[iOS (7,1)]
		[Export ("multiThreaded")]
		bool IsMultiThreaded { [Bind ("isMultiThreaded")] get; set; }

		// IOSurface (EAGLContext)

		[iOS (11,0)]
		[TV (11,0)]
		[Export ("texImageIOSurface:target:internalFormat:width:height:format:type:plane:")]
		bool TexImage (IOSurface.IOSurface ioSurface, nuint target, nuint internalFormat, uint width, uint height, nuint format, nuint type, uint plane);
	}

	[Protocol]
	// no [Model] because "The EAGLDrawable protocol is not intended to be implemented by objects outside of the iOS."
	interface EAGLDrawable {
		[Abstract]
		[NullAllowed] // by default this property is null
		[Export ("drawableProperties", ArgumentSemantic.Copy)]
		NSDictionary DrawableProperties { get; set; }
	}
}
