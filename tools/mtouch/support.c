#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <unistd.h>

#include <sys/types.h>
#include <sys/ptrace.h>
#include <sys/syslimits.h>

#include <mach-o/dyld.h>

#ifdef __cplusplus
extern "C" {
#endif
char *get_mtouch_full_path ();
#ifdef __cplusplus
}
#endif

// we cannot use Assembly.Location once bundled and there's an issue with
// Environment.GetCommandLineArgs which seems to return the current directory
// changing argv[0] or image_name introduce other problems
char*
get_mtouch_full_path ()
{
	char exe [PATH_MAX];
	char real [PATH_MAX];
	uint32_t path_len = PATH_MAX;
	
	// We can't use a DllImport for _NSGetExecutablePath
	// because it's a private symbol:
	// $ nm -m /usr/lib/dyld |grep _NSGetExecutablePath
	// 00007fff5fc0a380 (__TEXT,__text) non-external (was a private external) __NSGetExecutablePath
	if (_NSGetExecutablePath (exe, &path_len) != 0)
		return NULL;
	if (!realpath (exe, real))
		return NULL;
	return strdup (real);
}
