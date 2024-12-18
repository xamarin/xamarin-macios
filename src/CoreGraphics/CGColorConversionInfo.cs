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
			/* CGColorSpaceRef __nullable */ IntPtr space1, /* CGColorConversionInfoTransformType */ IntPtr transform1, /* CGColorRenderingIntent */ IntPtr intent1,
			/* CGColorSpaceRef __nullable */ IntPtr space2, /* CGColorConversionInfoTransformType */ IntPtr transform2, /* CGColorRenderingIntent */ IntPtr intent2,
			/* CGColorSpaceRef __nullable */ IntPtr space3, /* CGColorConversionInfoTransformType */ IntPtr transform3, /* CGColorRenderingIntent */ IntPtr intent3,
			IntPtr placeholder11, IntPtr placeholder12, IntPtr placeholder13, IntPtr placeholder14, IntPtr placeholder15, IntPtr placeholder16, IntPtr placeholder17, IntPtr placeholder18, IntPtr placeholder19);

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

			if ((triples.Length - 1) * 3 > Messaging.MaxVarArgs)
				throw new ArgumentException ($"A maximum of {1 + Messaging.MaxVarArgs / 3} triples are supported");

			var varArgs = new IntPtr [(triples.Length - 1) * 3];
			for (var i = 1; i < triples.Length; i++) {
				varArgs [(i - 1) * 3] = triples [i].Space.GetHandle ();
				varArgs [(i - 1) * 3 + 1] = (IntPtr) triples [i].Transform;
				varArgs [(i - 1) * 3 + 2] = (IntPtr) triples [i].Intent;
			}

			// The first triple is not a varargs, but the subsequent ones are
			var first = triples [0];
			return Messaging.CallVariadicFunction4 (
					CGColorConversionInfoCreateFromList,
					options.GetHandle (),
					first.Space.GetHandle (),
					(IntPtr) first.Transform,
					(IntPtr) first.Intent,
					varArgs);
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
#endif
		public CGColorConversionInfo (CGColorSpace source, CGColorSpace destination, CGColorConversionOptions? options) :
			this (source, destination, options?.Dictionary)
		{
		}
	}
}
