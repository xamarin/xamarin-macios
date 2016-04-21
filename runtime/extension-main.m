#include <stdlib.h>
#include "xamarin/main.h"
#include "main-internal.h"

void
xamarin_initialize_extension_main () __attribute__ ((constructor));

void
xamarin_initialize_extension_main ()
{
#ifdef EXTENSION
	xamarin_extension_main = NSExtensionMain;
#elif WATCH_EXTENSION
	xamarin_extension_main = main;
#else
	xamarin_extension_main = NULL;
#endif
}
