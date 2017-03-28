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
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;
using XamCore.UIKit;
using System;

namespace XamCore.OpenGLES {

	[BaseType (typeof (NSObject))]
	// <quote>It is created when an EAGLContext object is initialized and disposed of when the last EAGLContext object that references it is released.</quote>
	[DisableDefaultCtor]
	interface EAGLSharegroup {

		[iOS (6,0)]
		[Export ("debugLabel")]
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
		EAGLContext CurrentContext { get; }

		[Export("API")]
		EAGLRenderingAPI API { get; }

		[Export("sharegroup")]
		EAGLSharegroup ShareGroup { get; }

		[iOS (6,0)]
		[Export ("debugLabel")]
		string DebugLabel { get; set; }

		//
		// These are from @interface EAGLContext (EAGLContextDrawableAdditions)
		//

		[Export ("renderbufferStorage:fromDrawable:")]
		bool RenderBufferStorage (nuint target, [NullAllowed] XamCore.CoreAnimation.CAEAGLLayer drawable);

		[Export ("presentRenderbuffer:")]
		bool PresentRenderBuffer (nuint target);

		[Internal]
		[Export ("presentRenderbuffer:atTime:")]
		bool _PresentRenderbufferAtTime (nuint target, double presentationTime);

		[Internal]
		[Export ("presentRenderbuffer:afterMinimumDuration:")]
		bool _PresentRenderbufferAfterMinimumDuration (nuint target, double duration);

		[Since (7,1)]
		[Export ("multiThreaded")]
		bool IsMultiThreaded { [Bind ("isMultiThreaded")] get; set; }
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
