//
// CGColorConverter.cs: Implements the managed CGColorConverter
//
// Author: Vincent Dondain
//
// Copyright 2016 Xamarin Inc.
//

#if !MONOMAC

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;

#if !COREBUILD
using XamCore.CoreFoundation;
using XamCore.Foundation;
#endif

namespace XamCore.CoreGraphics {

	// uint32_t enum -> CGColorConverter.h
	[TV (9,2)]
	[iOS (9,3)]
	public enum CGColorConverterTransformType : uint {
		FromSpace,
		ToSpace,
		ApplySpace
	}

	[TV (9,2)]
	[iOS (9,3)]
	[StructLayout (LayoutKind.Sequential)]
	public struct CGColorConverterTriple {
		public CGColorSpace Space;
		public CGColorConverterTransformType Transform;
		public CGColorRenderingIntent Intent;
	}

	// CGColorConverter.h
	[TV (9,2)]
	[iOS (9,3)]
	public class CGColorConverter : INativeObject, IDisposable
	{
		IntPtr handle;

		/* invoked by marshallers */
		internal CGColorConverter (IntPtr handle)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGColorConverter (IntPtr handle, bool owns)
		{
			this.handle = handle;
			// if (!owns)
			// 	CGColorConverterRetain (this.handle);
			// FIXME: ^ Apple does not provide this API (it might not be reference counted)
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorConverterCreate (/* __nullable CFDictionaryRef */ IntPtr options, 
			/* CGColorSpaceRef __nullable */ IntPtr space1, CGColorConverterTransformType transform1, CGColorRenderingIntent intent1,
			/* CGColorSpaceRef __nullable */ IntPtr space2, CGColorConverterTransformType transform2, CGColorRenderingIntent intent2,
			/* CGColorSpaceRef __nullable */ IntPtr space3, CGColorConverterTransformType transform3, CGColorRenderingIntent intent3,
			IntPtr lastSpaceMarker);

		// https://developer.apple.com/library/ios/documentation/Xcode/Conceptual/iPhoneOSABIReference/Articles/ARM64FunctionCallingConventions.html
		// Declare dummies until we're on the stack then the arguments
		// <quote>C language requires arguments smaller than int to be promoted before a call, but beyond that, unused bytes on the stack are not specified by this ABI</quote>
		[DllImport(Constants.CoreGraphicsLibrary, EntryPoint="CGColorConverterCreate")]
		extern static IntPtr CGColorConverterCreate_arm64 (/* __nullable CFDictionaryRef */ IntPtr options,
			IntPtr space1, long transform1, long intent1, // varargs starts after them
			IntPtr dummy4, IntPtr dummy5, IntPtr dummy6, IntPtr dummy7, // dummies so the rest goes to the stack
			IntPtr space2, long transform2, long intent2,
			IntPtr space3, long transform3, long intent3,
			IntPtr lastSpaceMarker);

		static CGColorConverterTriple empty = new CGColorConverterTriple ();

		public CGColorConverter (NSDictionary options, params CGColorConverterTriple [] triples)
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
			if ((IntPtr.Size == 8) && (Runtime.Arch == Arch.DEVICE)) {
				handle = CGColorConverterCreate_arm64 (o, NativeObjectHelper.GetHandle (first.Space), (long) first.Transform, (long) first.Intent,
					IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
					NativeObjectHelper.GetHandle (second.Space), (long) second.Transform, (long) second.Intent,
					NativeObjectHelper.GetHandle (third.Space), (long) third.Transform, (long) third.Intent,
					IntPtr.Zero);
			} else {
				handle = CGColorConverterCreate (o, NativeObjectHelper.GetHandle (first.Space), first.Transform, first.Intent,
					NativeObjectHelper.GetHandle (second.Space), second.Transform, second.Intent,
					NativeObjectHelper.GetHandle (third.Space), third.Transform, third.Intent,
					IntPtr.Zero);
			}

			if (handle == IntPtr.Zero)
				throw new Exception ("Failed to create CGColorConverter");
		}

		// Added in 9.3 but it only works on simulator (not devices). Fixed in iOS 10 beta 1
		// https://trello.com/c/Rwko9Wef/37-24734681-cgcolorconvertercreatesimple-is-missing-for-device-builds
		[iOS (10,0)]
		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorConverterCreateSimple (/* __nullable CGColorSpaceRef */ IntPtr from, /* __nullable CGColorSpaceRef */ IntPtr to);

		[iOS (10,0)]
		public CGColorConverter (CGColorSpace from, CGColorSpace to)
		{
			// API accept null arguments but returns null, which we can't use
			if (from == null)
				throw new ArgumentNullException (nameof (from));
			if (from == to)
				throw new ArgumentNullException (nameof (to));
			handle = CGColorConverterCreateSimple (from.Handle, to.Handle);

			if (handle == IntPtr.Zero)
				throw new Exception ("Failed to create CGColorConverter");
		}

		~CGColorConverter ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorConverterRelease (/* CGColorConverterRef */ IntPtr converter);

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGColorConverterRelease (handle);
				handle = IntPtr.Zero;
			}
		}
	}
}

#endif // !MONOMAC

