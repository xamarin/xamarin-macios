//
// corefoundation.cs: Definitions for CoreFoundation
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

namespace CoreFoundation {

	/// <summary>A class that allows for explicit allocation and de-allocation of memory.</summary>
	[Partial]
	interface CFAllocator {

		[Internal]
		[Field ("kCFAllocatorDefault")]
		IntPtr default_ptr { get; }

		[Internal]
		[Field ("kCFAllocatorSystemDefault")]
		IntPtr system_default_ptr { get; }

		[Internal]
		[Field ("kCFAllocatorMalloc")]
		IntPtr malloc_ptr { get; }

		[Internal]
		[Field ("kCFAllocatorMallocZone")]
		IntPtr malloc_zone_ptr { get; }

		[Internal]
		[Field ("kCFAllocatorNull")]
		IntPtr null_ptr { get; }
	}

	[Partial]
	interface CFArray {

		[Internal]
		[Field ("kCFNull")]
		IntPtr /* CFNullRef */ _CFNullHandle { get; }
	}

	[Partial]
	[Internal]
	interface CFBoolean {
		[Internal]
		[Field ("kCFBooleanTrue", "CoreFoundation")]
		IntPtr TrueHandle { get; }

		[Internal]
		[Field ("kCFBooleanFalse", "CoreFoundation")]
		IntPtr FalseHandle { get; }
	}

	/// <summary>Main loop implementation for Cocoa and CocoaTouch applications.</summary>
	///     <remarks>Run loops can be executed recursively.</remarks>
	[Partial]
	interface CFRunLoop {

		/// <summary>Represents the value associated with the constant kCFRunLoopDefaultMode</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCFRunLoopDefaultMode")]
		NSString ModeDefault { get; }

		/// <summary>Represents the value associated with the constant kCFRunLoopCommonModes</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCFRunLoopCommonModes")]
		NSString ModeCommon { get; }
	}

	[Partial]
	interface DispatchData {
		[Internal]
		[Field ("_dispatch_data_destructor_free", "/usr/lib/system/libdispatch.dylib")]
		IntPtr free { get; }
	}

	/// <summary>Provides the necessary methods needed for accessing the system's global proxy configuration settings and resolving a list of proxies to use for connecting to a URL.</summary>
	[Partial]
	interface CFNetwork {

		[Field ("kCFErrorDomainCFNetwork", "CFNetwork")]
		NSString ErrorDomain { get; }
	}

	enum CFStringTransform {
		[Field ("kCFStringTransformStripCombiningMarks")]
		StripCombiningMarks,

		[Field ("kCFStringTransformToLatin")]
		ToLatin,

		[Field ("kCFStringTransformFullwidthHalfwidth")]
		FullwidthHalfwidth,

		[Field ("kCFStringTransformLatinKatakana")]
		LatinKatakana,

		[Field ("kCFStringTransformLatinHiragana")]
		LatinHiragana,

		[Field ("kCFStringTransformHiraganaKatakana")]
		HiraganaKatakana,

		[Field ("kCFStringTransformMandarinLatin")]
		MandarinLatin,

		[Field ("kCFStringTransformLatinHangul")]
		LatinHangul,

		[Field ("kCFStringTransformLatinArabic")]
		LatinArabic,

		[Field ("kCFStringTransformLatinHebrew")]
		LatinHebrew,

		[Field ("kCFStringTransformLatinThai")]
		LatinThai,

		[Field ("kCFStringTransformLatinCyrillic")]
		LatinCyrillic,

		[Field ("kCFStringTransformLatinGreek")]
		LatinGreek,

		[Field ("kCFStringTransformToXMLHex")]
		ToXmlHex,

		[Field ("kCFStringTransformToUnicodeName")]
		ToUnicodeName,

		[Field ("kCFStringTransformStripDiacritics")]
		StripDiacritics,
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum OSLogLevel : byte {
		// These values must match the os_log_type_t enum in <os/log.h>.
		Default = 0x00,
		Info = 0x01,
		Debug = 0x02,
		Error = 0x10,
		Fault = 0x11,
	}
}
