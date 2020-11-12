//
// CGColorConversionInfo.cs: Implements the managed CGColorConversionInfo
//
// Copyright 2016 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

#if !COREBUILD
using CoreFoundation;
using Foundation;
#endif

namespace CoreGraphics {

	// uint32_t enum -> CGColorConversionInfo.h
	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	public enum CGColorConversionInfoTransformType : uint {
		FromSpace = 0,
		ToSpace,
		ApplySpace
	}

	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	[StructLayout (LayoutKind.Sequential)]
	public struct GColorConversionInfoTriple {
		public CGColorSpace Space;
		public CGColorConversionInfoTransformType Transform;
		public CGColorRenderingIntent Intent;
	}

	// CGColorConverter.h
	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	public partial class CGColorConversionInfo : INativeObject, IDisposable {

		/* invoked by marshallers */
		internal CGColorConversionInfo (IntPtr handle)
		{
			Handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGColorConversionInfo (IntPtr handle, bool owns)
		{
			Handle = handle;
			if (!owns)
				CFObject.CFRetain (Handle);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorConversionInfoRef __nullable */ IntPtr CGColorConversionInfoCreateFromList (/* __nullable CFDictionaryRef */ IntPtr options, 
			/* CGColorSpaceRef __nullable */ IntPtr space1, CGColorConversionInfoTransformType transform1, CGColorRenderingIntent intent1,
			/* CGColorSpaceRef __nullable */ IntPtr space2, CGColorConversionInfoTransformType transform2, CGColorRenderingIntent intent2,
			/* CGColorSpaceRef __nullable */ IntPtr space3, CGColorConversionInfoTransformType transform3, CGColorRenderingIntent intent3,
			IntPtr lastSpaceMarker);

#if !MONOMAC
		// https://developer.apple.com/library/ios/documentation/Xcode/Conceptual/iPhoneOSABIReference/Articles/ARM64FunctionCallingConventions.html
		// Declare dummies until we're on the stack then the arguments
		// <quote>C language requires arguments smaller than int to be promoted before a call, but beyond that, unused bytes on the stack are not specified by this ABI</quote>
		// The 'transformX' argument is a CGColorConversionInfoTransformType, which is defined as uint (uint32_t in the header),
		// but since each parameter must be pointer-sized (to occupy the right amount of stack space),
		// we define it as nuint (and not the enum type, which is 32-bit even on 64-bit platforms).
		// Same for the 'intentX' argument (except that it's signed instead of unsigned).
		[DllImport(Constants.CoreGraphicsLibrary, EntryPoint="CGColorConversionInfoCreateFromList")]
		extern static /* CGColorConversionInfoRef __nullable */ IntPtr CGColorConversionInfoCreateFromList_arm64 (/* __nullable CFDictionaryRef */ IntPtr options,
			IntPtr space1, nuint transform1, nint intent1, // varargs starts after them
			IntPtr dummy4, IntPtr dummy5, IntPtr dummy6, IntPtr dummy7, // dummies so the rest goes to the stack
			IntPtr space2, nuint transform2, nint intent2,
			IntPtr space3, nuint transform3, nint intent3,
			IntPtr lastSpaceMarker);
#endif

		static GColorConversionInfoTriple empty = new GColorConversionInfoTriple ();


		public CGColorConversionInfo (CGColorConversionOptions options, params GColorConversionInfoTriple [] triples)
			: this (options?.Dictionary, triples)
		{
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public CGColorConversionInfo (NSDictionary options, params GColorConversionInfoTriple [] triples)
		{
			// the API won't return a valid instance if no triple is given, i.e. at least one is needed. 
			// `null` is accepted to mark the end of the list, not to make it optional
			if ((triples == null) || (triples.Length == 0))
				throw new ArgumentNullException ("triples");
			if (triples.Length > 3)
				throw new ArgumentException ("A maximum of 3 triples are supported");
			
			IntPtr o = NativeObjectHelper.GetHandle (options);
			var first = triples [0]; // there's always one
			var second = triples.Length > 1 ? triples [1] : empty; 
			var third = triples.Length > 2 ? triples [2] : empty;
#if !MONOMAC
			if (Runtime.IsARM64CallingConvention) {
				Handle = CGColorConversionInfoCreateFromList_arm64 (o, NativeObjectHelper.GetHandle (first.Space), (uint) first.Transform, (int) first.Intent,
					IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
					NativeObjectHelper.GetHandle (second.Space), (uint) second.Transform, (int) second.Intent,
					NativeObjectHelper.GetHandle (third.Space), (uint) third.Transform, (int) third.Intent,
					IntPtr.Zero);
			} else {
#endif
				Handle = CGColorConversionInfoCreateFromList (o, NativeObjectHelper.GetHandle (first.Space), first.Transform, first.Intent,
					NativeObjectHelper.GetHandle (second.Space), second.Transform, second.Intent,
					NativeObjectHelper.GetHandle (third.Space), third.Transform, third.Intent,
					IntPtr.Zero);
#if !MONOMAC
			}
#endif
			if (Handle == IntPtr.Zero)
				throw new Exception ("Failed to create CGColorConverter");
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorConversionInfoCreate (/* cg_nullable CGColorSpaceRef */ IntPtr src, /* cg_nullable CGColorSpaceRef */ IntPtr dst);

		public CGColorConversionInfo (CGColorSpace source, CGColorSpace destination)
		{
			// API accept null arguments but returns null, which we can't use
			if (source == null)
				throw new ArgumentNullException (nameof (source));
			if (destination == null)
				throw new ArgumentNullException (nameof (destination));
			Handle = CGColorConversionInfoCreate (source.Handle, destination.Handle);

			if (Handle == IntPtr.Zero)
				throw new Exception ("Failed to create CGColorConversionInfo");
		}

		[Mac (10,14,6)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
		[DllImport(Constants.CoreGraphicsLibrary)]
		static extern /* CGColorConversionInfoRef* */ IntPtr CGColorConversionInfoCreateWithOptions (/* CGColorSpaceRef* */ IntPtr src, /* CGColorSpaceRef* */ IntPtr dst, /* CFDictionaryRef _Nullable */ IntPtr options);

		[Mac (10,14,6)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
		public CGColorConversionInfo (CGColorSpace source, CGColorSpace destination, NSDictionary options)
		{
			if (source == null)
				throw new ArgumentNullException (nameof (source));
			if (destination == null)
				throw new ArgumentNullException (nameof (destination));

			Handle = CGColorConversionInfoCreateWithOptions (source.Handle, destination.Handle, options.GetHandle ());

			if (Handle == IntPtr.Zero)
				throw new Exception ("Failed to create CGColorConversionInfo");
		}

		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
		public CGColorConversionInfo (CGColorSpace source, CGColorSpace destination, CGColorConversionOptions options) :
			this (source, destination, options?.Dictionary)
		{
		}

		~CGColorConversionInfo ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle { get; private set; }

		protected virtual void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero){
				CFObject.CFRelease (Handle);
				Handle = IntPtr.Zero;
			}
		}
	}
}
