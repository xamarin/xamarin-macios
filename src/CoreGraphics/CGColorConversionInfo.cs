//
// CGColorConversionInfo.cs: Implements the managed CGColorConversionInfo
//
// Copyright 2016 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

#if !COREBUILD
using CoreFoundation;
using Foundation;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
#if NET
	public struct CGColorConversionInfoTriple {
#else
	public struct GColorConversionInfoTriple {
#endif
		public CGColorSpace Space;
		public CGColorConversionInfoTransformType Transform;
		public CGColorRenderingIntent Intent;
	}

	// CGColorConverter.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class CGColorConversionInfo : NativeObject {
		[Preserve (Conditional = true)]
		internal CGColorConversionInfo (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorConversionInfoRef __nullable */ IntPtr CGColorConversionInfoCreateFromList (/* __nullable CFDictionaryRef */ IntPtr options,
			/* CGColorSpaceRef __nullable */ IntPtr space1, CGColorConversionInfoTransformType transform1, CGColorRenderingIntent intent1,
			/* CGColorSpaceRef __nullable */ IntPtr space2, CGColorConversionInfoTransformType transform2, CGColorRenderingIntent intent2,
			/* CGColorSpaceRef __nullable */ IntPtr space3, CGColorConversionInfoTransformType transform3, CGColorRenderingIntent intent3,
			IntPtr lastSpaceMarker);

		// https://developer.apple.com/library/ios/documentation/Xcode/Conceptual/iPhoneOSABIReference/Articles/ARM64FunctionCallingConventions.html
		// Declare dummies until we're on the stack then the arguments
		// <quote>C language requires arguments smaller than int to be promoted before a call, but beyond that, unused bytes on the stack are not specified by this ABI</quote>
		// The 'transformX' argument is a CGColorConversionInfoTransformType, which is defined as uint (uint32_t in the header),
		// but since each parameter must be pointer-sized (to occupy the right amount of stack space),
		// we define it as nuint (and not the enum type, which is 32-bit even on 64-bit platforms).
		// Same for the 'intentX' argument (except that it's signed instead of unsigned).
		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGColorConversionInfoCreateFromList")]
		extern static /* CGColorConversionInfoRef __nullable */ IntPtr CGColorConversionInfoCreateFromList_arm64 (/* __nullable CFDictionaryRef */ IntPtr options,
			IntPtr space1, nuint transform1, nint intent1, // varargs starts after them
			IntPtr dummy4, IntPtr dummy5, IntPtr dummy6, IntPtr dummy7, // dummies so the rest goes to the stack
			IntPtr space2, nuint transform2, nint intent2,
			IntPtr space3, nuint transform3, nint intent3,
			IntPtr lastSpaceMarker);

#if NET
		public CGColorConversionInfo (CGColorConversionOptions? options, params CGColorConversionInfoTriple [] triples)
#else
		public CGColorConversionInfo (CGColorConversionOptions? options, params GColorConversionInfoTriple [] triples)
#endif
			: this (options?.Dictionary, triples)
		{
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
#if NET
		static IntPtr Create (NSDictionary? options, params CGColorConversionInfoTriple [] triples)
#else
		static IntPtr Create (NSDictionary? options, params GColorConversionInfoTriple [] triples)
#endif
		{
			// the API won't return a valid instance if no triple is given, i.e. at least one is needed. 
			// `null` is accepted to mark the end of the list, not to make it optional
			if ((triples is null) || (triples.Length == 0))
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (triples));
			if (triples.Length > 3)
				throw new ArgumentException ("A maximum of 3 triples are supported");

			IntPtr handle;
			IntPtr o = options.GetHandle ();
			var first = triples [0]; // there's always one
#if NET
			var second = triples.Length > 1 ? triples [1] : default (CGColorConversionInfoTriple);
			var third = triples.Length > 2 ? triples [2] : default (CGColorConversionInfoTriple);
#else
			var second = triples.Length > 1 ? triples [1] : default (GColorConversionInfoTriple);
			var third = triples.Length > 2 ? triples [2] : default (GColorConversionInfoTriple);
#endif
			if (Runtime.IsARM64CallingConvention) {
				handle = CGColorConversionInfoCreateFromList_arm64 (o, first.Space.GetHandle (), (uint) first.Transform, (int) first.Intent,
					IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
					second.Space.GetHandle (), (uint) second.Transform, (int) second.Intent,
					third.Space.GetHandle (), (uint) third.Transform, (int) third.Intent,
					IntPtr.Zero);
			} else {
				handle = CGColorConversionInfoCreateFromList (o, first.Space.GetHandle (), first.Transform, first.Intent,
					second.Space.GetHandle (), second.Transform, second.Intent,
					third.Space.GetHandle (), third.Transform, third.Intent,
					IntPtr.Zero);
			}
			return handle;
		}

#if NET
		public CGColorConversionInfo (NSDictionary? options, params CGColorConversionInfoTriple [] triples)
#else
		public CGColorConversionInfo (NSDictionary? options, params GColorConversionInfoTriple [] triples)
#endif
			: base (Create (options, triples), true, verify: true)
		{

		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorConversionInfoCreate (/* cg_nullable CGColorSpaceRef */ IntPtr src, /* cg_nullable CGColorSpaceRef */ IntPtr dst);

		static IntPtr Create (CGColorSpace source, CGColorSpace destination)
		{
			// API accept null arguments but returns null, which we can't use
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (destination is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (destination));
			return CGColorConversionInfoCreate (source.Handle, destination.Handle);
		}

		public CGColorConversionInfo (CGColorSpace source, CGColorSpace destination)
			: base (Create (source, destination), true, verify: true)
		{
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorConversionInfoRef* */ IntPtr CGColorConversionInfoCreateWithOptions (/* CGColorSpaceRef* */ IntPtr src, /* CGColorSpaceRef* */ IntPtr dst, /* CFDictionaryRef _Nullable */ IntPtr options);

		static IntPtr Create (CGColorSpace source, CGColorSpace destination, NSDictionary? options)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (destination is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (destination));

			return CGColorConversionInfoCreateWithOptions (source.Handle, destination.Handle, options.GetHandle ());
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		public CGColorConversionInfo (CGColorSpace source, CGColorSpace destination, NSDictionary? options)
			: base (Create (source, destination, options), true, verify: true)
		{
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		public CGColorConversionInfo (CGColorSpace source, CGColorSpace destination, CGColorConversionOptions? options) :
			this (source, destination, options?.Dictionary)
		{
		}
	}
}
