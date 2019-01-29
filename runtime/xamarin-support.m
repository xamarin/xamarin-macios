#import <Foundation/Foundation.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#include <iconv.h>

#include "xamarin/xamarin.h"
#include "runtime-internal.h"
#include "monotouch-support.h"

const char *
xamarin_get_locale_country_code ()
{
	// COOP: no managed memory access: any mode.
	NSLocale *locale = [NSLocale currentLocale];
	NSString *cc = [locale objectForKey: NSLocaleCountryCode];
	return strdup ([cc UTF8String]);
}

void
xamarin_log (const unsigned short *unicodeMessage)
{
	// COOP: no managed memory access: any mode.
	int length = 0;
	const unsigned short *ptr = unicodeMessage;
	while (*ptr++)
		length += sizeof (unsigned short);
	NSString *msg = [[NSString alloc] initWithBytes: unicodeMessage length: length encoding: NSUTF16LittleEndianStringEncoding];

#if TARGET_OS_WATCH && defined (__arm__) // maybe make this configurable somehow?
	const char *utf8 = [msg UTF8String];
	int len = strlen (utf8);
	fwrite (utf8, 1, len, stdout);
	if (len == 0 || utf8 [len - 1] != '\n')
		fwrite ("\n", 1, 1, stdout);
	fflush (stdout);
#else
	if (length > 4096) {
		// Write in chunks of max 4096 characters; older versions of iOS seems to have a bug where NSLog may hang with long strings (!).
		// https://github.com/xamarin/maccore/issues/1014
		const char *utf8 = [msg UTF8String];
		int len = strlen (utf8);
		const int max_size = 4096;
		while (len > 0) {
			int chunk_size = len > max_size ? max_size : len;

			// Try to not break in the middle of a line, by looking backwards for a newline
			while (chunk_size > 0 && utf8 [chunk_size] != 0 && utf8 [chunk_size] != '\n')
				chunk_size--;
			if (chunk_size == 0) {
				// No newline found, break in the middle.
				chunk_size = len > max_size ? max_size : len;
			}
			NSLog (@"%.*s", chunk_size, utf8);

			len -= chunk_size;
			utf8 += chunk_size;
		}
	} else {
		NSLog (@"%@", msg);
	}
#endif
	[msg release];
}

void*
xamarin_timezone_get_data (const char *name, int *size)
{
	// COOP: no managed memory access: any mode.
	NSTimeZone *tz = nil;
	if (name) {
		NSString *n = [[NSString alloc] initWithUTF8String: name];
		tz = [[[NSTimeZone alloc] initWithName:n] autorelease];
		[n release];
	} else {
		tz = [NSTimeZone localTimeZone];
	}
	NSData *data = [tz data];
	*size = [data length];
	void* result = malloc (*size);
	memcpy (result, data.bytes, *size);
	return result;
}

char**
xamarin_timezone_get_names (int *count)
{
	// COOP: no managed memory access: any mode.
	NSArray *array = [NSTimeZone knownTimeZoneNames];
	*count = array.count;
	char** result = (char**) malloc (sizeof (char*) * (*count));
	for (int i = 0; i < *count; i++) {
		NSString *s = [array objectAtIndex: i];
		result [i] = strdup (s.UTF8String);
	}
	return result;
}

#if !TARGET_OS_WATCH && !TARGET_OS_TV
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
	CFReadStreamRef stream = CFReadStreamCreateForHTTPRequest (kCFAllocatorDefault, message);
	
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
#endif /* !TARGET_OS_WATCH && !TARGET_OS_TV */

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

void objc_msgSend_stret (id self, SEL op, ...)
{
	PRINT ("Unimplemented objc_msgSend_stret %s", sel_getName (op));
	abort ();
}

void objc_msgSendSuper_stret (struct objc_super *super, SEL op, ...)
{
	PRINT ("Unimplemented objc_msgSendSuper_stret %s", sel_getName (op));
	abort ();
}

#endif

