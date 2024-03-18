#import <Foundation/Foundation.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#include <iconv.h>

#include "xamarin/xamarin.h"
#include "runtime-internal.h"
#include "monotouch-support.h"

void
xamarin_os_log (os_log_t logger, os_log_type_t type, const char *message)
{
	// Logging a dynamic string as-is using %{public}s is, strictly speaking,
	// an antipattern. However, there is no way to call os_log directly from
	// C#. It can only be called from Objective-C, and even then it requires
	// that the format string be compiled into the binary. This really is
	// our only option to pass a string from C# into os_log.
	if (logger == NULL)
		logger = OS_LOG_DEFAULT;

	os_log_with_type (logger, type, "%{public}s", message);
}

const char *
xamarin_get_locale_country_code ()
{
	// COOP: no managed memory access: any mode.
	NSLocale *locale = [NSLocale currentLocale];
	NSString *cc = [locale objectForKey: NSLocaleCountryCode];
	if (cc == NULL) {
		// Assume the US if the country isn't available.
		return strdup ("US");
	}
	return strdup ([cc UTF8String]);
}

void
xamarin_log (const unsigned short *unicodeMessage)
{
	// COOP: no managed memory access: any mode.
	unsigned int length = 0;
	const unsigned short *ptr = unicodeMessage;
	while (*ptr++)
		length += sizeof (unsigned short);
	NSString *msg = [[NSString alloc] initWithBytes: unicodeMessage length: length encoding: NSUTF16LittleEndianStringEncoding];

#if TARGET_OS_WATCH && defined (__arm__) // maybe make this configurable somehow?
	const char *utf8 = [msg UTF8String];
	NSUInteger len = [msg lengthOfBytesUsingEncoding:NSUTF8StringEncoding] + 1; // does not include NULL
	fwrite (utf8, 1, len, stdout);
	if (len == 0 || utf8 [len - 1] != '\n')
		fwrite ("\n", 1, 1, stdout);
	fflush (stdout);
#else
	if (length > 4096) {
		// Write in chunks of max 4096 characters; older versions of iOS seems to have a bug where NSLog may hang with long strings (!).
		// https://github.com/xamarin/maccore/issues/1014
		const char *utf8 = [msg UTF8String];
		NSUInteger len = [msg lengthOfBytesUsingEncoding:NSUTF8StringEncoding] + 1; // does not include NULL
		const size_t max_size = 4096;
		while (len > 0) {
			size_t chunk_size = len > max_size ? max_size : len;

			// Try to not break in the middle of a line, by looking backwards for a newline
			while (chunk_size > 0 && utf8 [chunk_size] != 0 && utf8 [chunk_size] != '\n')
				chunk_size--;
			if (chunk_size == 0) {
				// No newline found, break in the middle.
				chunk_size = len > max_size ? max_size : len;
			}
			NSLog (@"%.*s", (int) chunk_size, utf8);

			len -= chunk_size;
			utf8 += chunk_size;
		}
	} else {
		NSLog (@"%@", msg);
	}
#endif
	objc_release (msg);
}

// NOTE: The timezone functions are duplicated in mono, so if you're going to modify here, it would be nice
// if we modify there.
//
// See in Mono sdks/ios/runtime/runtime.m

void*
xamarin_timezone_get_data (const char *name, uint32_t *size)
{
	// COOP: no managed memory access: any mode.
	NSTimeZone *tz = nil;
	if (name) {
		NSString *n = [[NSString alloc] initWithUTF8String: name];
		tz = objc_autorelease ([[NSTimeZone alloc] initWithName:n]);
		objc_release (n);
	} else {
		tz = [NSTimeZone localTimeZone];
	}
	NSData *data = [tz data];
	*size = (uint32_t) [data length];
	void* result = malloc (*size);
	[data getBytes: result length: *size];
	return result;
}

char**
xamarin_timezone_get_names (uint32_t *count)
{
	// COOP: no managed memory access: any mode.
	NSArray *array = [NSTimeZone knownTimeZoneNames];
	*count = (uint32_t) array.count;
	char** result = (char**) malloc (sizeof (char*) * (*count));
	for (unsigned long i = 0; i < *count; i++) {
		NSString *s = [array objectAtIndex: i];
		result [i] = strdup (s.UTF8String);
	}
	return result;
}

//
// Returns the geopolitical region ID of the local timezone.

char *
xamarin_timezone_get_local_name ()
{
	NSTimeZone *tz = nil;
	tz = [NSTimeZone localTimeZone];
	NSString *name = [tz name];
	return (name != nil) ? strdup ([name UTF8String]) : strdup ("Local");
}

#if !TARGET_OS_WATCH && !TARGET_OS_TV && !(TARGET_OS_MACCATALYST && defined (DOTNET)) && !TARGET_OS_OSX
void
xamarin_start_wwan (const char *uri)
{
	// COOP: no managed memory access: any mode.
#if defined(__i386__) || defined (__x86_64__)
	return;
#else
	unsigned char buf[1];
	CFStringRef host = CFStringCreateWithCString (kCFAllocatorDefault, uri, kCFStringEncodingUTF8);
	CFStringRef get = CFStringCreateWithCString (kCFAllocatorDefault, "GET", kCFStringEncodingUTF8);
	CFURLRef url = CFURLCreateWithString (kCFAllocatorDefault, host, nil);
	
	CFHTTPMessageRef message = CFHTTPMessageCreateRequest (kCFAllocatorDefault, get, url, kCFHTTPVersion1_1);
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
	CFReadStreamRef stream = CFReadStreamCreateForHTTPRequest (kCFAllocatorDefault, message);
#pragma clang diagnostic pop
	
	CFReadStreamScheduleWithRunLoop (stream, CFRunLoopGetCurrent (), kCFRunLoopCommonModes);
	
	if (CFReadStreamOpen (stream)) {
		// CFStreamStatus status = CFReadStreamGetStatus (stream);
		// PRINT ("CFStreamStatus %i", status);
		// note: some earlier iOS7 beta returned 1 (Opening) instead of 2 (Open) - a bit more time was needed or
		// CFReadStreamRead blocks (and never return)
		CFReadStreamRead (stream, buf, 1);
	}
	// that will remove it from the runloop (so we do it even if open failed)
	CFReadStreamClose (stream);
	
	CFRelease (stream);
	CFRelease (host);
	CFRelease (get);
	CFRelease (url);
	CFRelease (message);
#endif
}
#endif /* !TARGET_OS_WATCH && !TARGET_OS_TV && !TARGET_OS_MACCATALYST */

#if defined (MONOTOUCH)
// called from mono-extensions/mcs/class/corlib/System/Environment.iOS.cs
const char *
xamarin_GetFolderPath (int folder)
{
	// COOP: no managed memory access: any mode.
	// NSUInteger-based enum (and we do not want corlib exposed to 32/64 bits differences)
	NSSearchPathDirectory dd = (NSSearchPathDirectory) folder;
	NSURL *url = [[[NSFileManager defaultManager] URLsForDirectory:dd inDomains:NSUserDomainMask] lastObject];
	NSString *path = [url path];
	return strdup ([path UTF8String]);
}
#endif /* defined (MONOTOUCH) */

#if defined (__arm64__)

// there are no objc_msgSend[Super]_stret functions on ARM64 but the managed
// code might have (e.g. linker is off) references to the symbol, which makes
// it impossible to disable dlsym and, for example, run dontlink on devices
// https://bugzilla.xamarin.com/show_bug.cgi?id=36569#c4

void objc_msgSend_stret (void)
{
	PRINT ("Unimplemented objc_msgSend_stret");
	abort ();
}

void objc_msgSendSuper_stret (void)
{
	PRINT ("Unimplemented objc_msgSendSuper_stret");
	abort ();
}

#endif

#ifdef MONOMAC
// <quote>Do not hard-code this parameter as a C string.</quote>
// works on iOS (where we don't need it) and crash on macOS
const char *
xamarin_encode_CGAffineTransform ()
{
    // COOP: no managed memory access: any mode.
    return @encode (CGAffineTransform);
}
#endif
