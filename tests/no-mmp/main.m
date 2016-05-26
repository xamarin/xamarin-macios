//gcc -m32 monostub.m -o monostub -framework AppKit

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <sys/time.h>
#include <sys/resource.h>
#include <unistd.h>
#include <dlfcn.h>
#include <errno.h>
#include <ctype.h>

#import <Cocoa/Cocoa.h>

#define MONO_LIB_PATH(lib) "/Library/Frameworks/Mono.framework/Versions/Current/lib/"lib

typedef int (* mono_main) (int argc, char **argv);

void *libmono;

int main (int argc, char **argv)
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	libmono = dlopen (MONO_LIB_PATH ("libmono-2.0.dylib"), RTLD_LAZY);
	
	if (libmono == NULL) {
		fprintf (stderr, "Failed to load libmono-2.0.dylib: %s\n", dlerror ());
		abort ();
	}
	
	mono_main _mono_main = (mono_main) dlsym (libmono, "mono_main");
	if (!_mono_main) {
		fprintf (stderr, "Could not load mono_main(): %s\n", dlerror ());
		abort ();
	}
	
	[pool drain];
	
	NSString *appDir = [[NSBundle mainBundle] bundlePath];
	char *argv2 [2];
	argv2 [0] = argv [0];
	argv2 [1] = [[appDir stringByAppendingPathComponent:@"Contents/MonoBundle/no-mmp.exe"] UTF8String];

	return _mono_main (2, argv2);
}