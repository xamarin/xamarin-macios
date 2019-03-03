#include <stdlib.h>
#include "xamarin/main.h"
#include "main-internal.h"

#ifdef __cplusplus
extern "C" {
#endif
void
xamarin_initialize_extension_main () __attribute__ ((constructor));
#ifdef __cplusplus
}
#endif

void
xamarin_initialize_extension_main ()
{
#ifdef TV_EXTENSION
	xamarin_extension_main = TVExtensionMain;
#elif EXTENSION
	xamarin_extension_main = NSExtensionMain;
#elif WATCH_EXTENSION
	xamarin_extension_main = main;
#else
	xamarin_extension_main = NULL;
#endif
}
