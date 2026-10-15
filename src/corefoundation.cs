//
// corefoundation.cs: Definitions for CoreFoundation
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

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

		[Internal]
		[Field ("kCFTypeArrayCallBacks", SymbolAddress = true)]
		IntPtr _CFTypeArrayCallbacks { get; }
	}

	[Internal]
	[Partial]
	interface CFDictionary {
		[Internal]
		[Field ("kCFTypeDictionaryKeyCallBacks", SymbolAddress = true)]
		IntPtr _KeyCallbacks { get; }

		[Internal]
		[Field ("kCFTypeDictionaryValueCallBacks", SymbolAddress = true)]
		IntPtr _ValueCallbacks { get; }
	}

	[Static, Partial]
	interface CFErrorDomain {
#if !XAMCORE_5_0
		[Internal]
		[Field ("kCFErrorDomainCocoa")]
		NSString _Cocoa { get; }

		[Internal]
		[Field ("kCFErrorDomainMach")]
		NSString _Mach { get; }

		[Internal]
		[Field ("kCFErrorDomainOSStatus")]
		NSString _OSStatus { get; }

		[Internal]
		[Field ("kCFErrorDomainPOSIX")]
		NSString _Posix { get; }
#endif
#if XAMCORE_5_0
		/// <summary>Identifies errors reported by Cocoa APIs.</summary>
		[Field ("kCFErrorDomainCocoa")]
		[NullAllowed]
		NSString Cocoa { get; }

		/// <summary>Identifies errors reported by Mach APIs.</summary>
		[Field ("kCFErrorDomainMach")]
		[NullAllowed]
		NSString Mach { get; }

		/// <summary>Identifies errors reported using OSStatus values.</summary>
		[Field ("kCFErrorDomainOSStatus")]
		[NullAllowed]
		NSString OSStatus { get; }

		/// <summary>Identifies errors reported using POSIX error codes.</summary>
		[Field ("kCFErrorDomainPOSIX")]
		[NullAllowed]
		NSString Posix { get; }
#endif
	}

	[Static, Partial]
	interface CFExceptionDataKey {
#if !XAMCORE_5_0
		[Internal]
		[Field ("kCFErrorDescriptionKey")]
		NSString _Description { get; }

		[Internal]
		[Field ("kCFErrorLocalizedDescriptionKey")]
		NSString _LocalizedDescription { get; }

		[Internal]
		[Field ("kCFErrorLocalizedFailureReasonKey")]
		NSString _LocalizedFailureReason { get; }

		[Internal]
		[Field ("kCFErrorLocalizedRecoverySuggestionKey")]
		NSString _LocalizedRecoverySuggestion { get; }

		[Internal]
		[Field ("kCFErrorUnderlyingErrorKey")]
		NSString _UnderlyingError { get; }
#endif
#if XAMCORE_5_0
		/// <summary>Gets the key for an error's nonlocalized description.</summary>
		[Field ("kCFErrorDescriptionKey")]
		[NullAllowed]
		NSString Description { get; }

		/// <summary>Gets the key for an error's localized description.</summary>
		[Field ("kCFErrorLocalizedDescriptionKey")]
		[NullAllowed]
		NSString LocalizedDescription { get; }

		/// <summary>Gets the key for an error's localized failure reason.</summary>
		[Field ("kCFErrorLocalizedFailureReasonKey")]
		[NullAllowed]
		NSString LocalizedFailureReason { get; }

		/// <summary>Gets the key for an error's localized recovery suggestion.</summary>
		[Field ("kCFErrorLocalizedRecoverySuggestionKey")]
		[NullAllowed]
		NSString LocalizedRecoverySuggestion { get; }

		/// <summary>Gets the key for an underlying error.</summary>
		[Field ("kCFErrorUnderlyingErrorKey")]
		[NullAllowed]
		NSString UnderlyingError { get; }
#endif
	}

	[Static, Partial]
	interface CFPreferences {
#if !XAMCORE_5_0
		[Internal]
		[Field ("kCFPreferencesCurrentApplication")]
		NSString _CurrentApplication { get; }
#endif
#if XAMCORE_5_0
		/// <summary>The current application.</summary>
		/// <remarks>
		///   <para>Use this value to read or write preferences for the calling application.</para>
		/// </remarks>
		[Field ("kCFPreferencesCurrentApplication")]
		[NullAllowed]
		NSString CurrentApplication { get; }
#endif
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCFErrorDomainCFNetwork", "CFNetwork")]
		NSString ErrorDomain { get; }
	}

	enum CFStringTransform {
		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformStripCombiningMarks")]
		StripCombiningMarks,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformToLatin")]
		ToLatin,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformFullwidthHalfwidth")]
		FullwidthHalfwidth,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinKatakana")]
		LatinKatakana,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinHiragana")]
		LatinHiragana,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformHiraganaKatakana")]
		HiraganaKatakana,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformMandarinLatin")]
		MandarinLatin,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinHangul")]
		LatinHangul,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinArabic")]
		LatinArabic,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinHebrew")]
		LatinHebrew,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinThai")]
		LatinThai,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinCyrillic")]
		LatinCyrillic,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformLatinGreek")]
		LatinGreek,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformToXMLHex")]
		ToXmlHex,

		/// <summary>To be added.</summary>
		[Field ("kCFStringTransformToUnicodeName")]
		ToUnicodeName,

		/// <summary>To be added.</summary>
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

	[Native]
	public enum CFByteOrder : long {
		Unknown,
		LittleEndian,
		BigEndian,
	}

	// The CFProxy + friends types should really be in CFNetwork
	[Partial]
	interface CFProxy {
		[Field ("kCFProxyAutoConfigurationHTTPResponseKey", "CFNetwork")]
		NSString AutoConfigurationHttpResponseKey { get; }

		[Field ("kCFProxyAutoConfigurationJavaScriptKey", "CFNetwork")]
		NSString AutoConfigurationJavaScriptKey { get; }

		[Field ("kCFProxyAutoConfigurationURLKey", "CFNetwork")]
		NSString AutoConfigurationUrlKey { get; }

		[Field ("kCFProxyHostNameKey", "CFNetwork")]
		NSString HostNameKey { get; }

		[Field ("kCFProxyPasswordKey", "CFNetwork")]
		NSString PasswordKey { get; }

		[Field ("kCFProxyPortNumberKey", "CFNetwork")]
		NSString PortNumberKey { get; }

		[Field ("kCFProxyTypeKey", "CFNetwork")]
		NSString TypeKey { get; }

		[Field ("kCFProxyUsernameKey", "CFNetwork")]
		NSString UsernameKey { get; }
	}

	// The CFProxy + friends types should really be in CFNetwork
	/// <summary>An enum of <see cref="CoreFoundation.CFProxy" /> proxy types.</summary>
	enum CFProxyType {
		/// <summary>No proxy should be used.</summary>
		[DefaultEnumValue]
		[Field ("kCFProxyTypeNone", "CFNetwork")]
		None,
		/// <summary>An autoconfiguration url.</summary>
		[Field ("kCFProxyTypeAutoConfigurationURL", "CFNetwork")]
		AutoConfigurationUrl,
		/// <summary>Proxy types available from the system.</summary>
		[Field ("kCFProxyTypeAutoConfigurationJavaScript", "CFNetwork")]
		AutoConfigurationJavaScript,
		/// <summary>An FTP proxy.</summary>
		[Field ("kCFProxyTypeFTP", "CFNetwork")]
		FTP,
		/// <summary>An HTTP proxy.</summary>
		[Field ("kCFProxyTypeHTTP", "CFNetwork")]
		HTTP,
		/// <summary>An HTTPS proxy.</summary>
		[Field ("kCFProxyTypeHTTPS", "CFNetwork")]
		HTTPS,
		/// <summary>A SOCKS proxy.</summary>
		[Field ("kCFProxyTypeSOCKS", "CFNetwork")]
		SOCKS,
	}
}
