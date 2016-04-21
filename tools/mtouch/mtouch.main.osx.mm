#include <stdlib.h>
#include <string.h>
#include "../../runtime/xamarin/xamarin.h"

extern "C" {
int  xamarin_app_initialize ();
void mono_mkbundle_init ();
int  xammac_setup ();
}

static void
disable_gdb_backtrace ()
{
	char *mdbg = getenv ("MONO_DEBUG");
	if (mdbg && *mdbg) {
		int len = strlen (mdbg);
		char *var = (char *) malloc (len + strlen (",no-gdb-backtrace") + 1);
		strncpy (var, mdbg, len);
		strcpy (var + len, ",no-gdb-backtrace");
		setenv ("MONO_DEBUG", var, 1);
		free (var);
	} else {
		setenv ("MONO_DEBUG", "no-gdb-backtrace", 1);
	}
}

int
xamarin_app_initialize ()
{
	disable_gdb_backtrace ();
	xamarin_set_is_debug (false);
	return 0;
}

int
xammac_setup ()
{
	xamarin_set_is_mkbundle (true);
	mono_mkbundle_init ();
	return 0;
}