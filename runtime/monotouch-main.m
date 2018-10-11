//
// main.m: Basic startup code for Mono on the iPhone
// 
// Authors:
//   Geoff Norton
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2009 Novell, Inc.
// Copyright 2011-2012 Xamarin Inc. 
//

#include <UIKit/UIKit.h>
#include <sys/time.h>
#include <zlib.h>
#include <dlfcn.h>
#include <objc/message.h>
#include <sys/mman.h>

#include "xamarin/xamarin.h"
#include "monotouch-debug.h"

#include "shared.h"
#include "product.h"
#include "runtime-internal.h"
#include "delegates.h"

#if defined(__x86_64__)
#include "../tools/mtouch/monotouch-fixes.c"
#endif

static unsigned char *
xamarin_load_aot_data (MonoAssembly *assembly, int size, gpointer user_data, void **out_handle)
{
	// COOP: This is a callback called by the AOT runtime, I believe we don't have to change the GC mode here (even though it accesses managed memory).
	*out_handle = NULL;

	char path [1024];
	char name [1024];
	MonoAssemblyName *assembly_name = mono_assembly_get_name (assembly);
	const char *aname = mono_assembly_name_get_name (assembly_name);
	xamarin_get_assembly_name_without_extension (aname, name, sizeof (name));

	// LOG (PRODUCT ": Looking for aot data for assembly '%s'.", name);

	strlcat (name, ".aotdata", sizeof (name));

	bool found = xamarin_locate_assembly_resource_for_name (assembly_name, name, path, sizeof (path));

	if (!found) {
		LOG (PRODUCT ": Could not find the aot data for %s.\n", aname)
		return NULL;
	}
	
	int fd = open (path, O_RDONLY);
	if (fd < 0) {
		LOG (PRODUCT ": Could not load the aot data for %s from %s: %s\n", aname, path, strerror (errno));
		return NULL;
	}

	void *ptr = mmap (NULL, size, PROT_READ, MAP_FILE | MAP_PRIVATE, fd, 0);
	if (ptr == MAP_FAILED) {
		LOG (PRODUCT ": Could not map the aot file for %s: %s\n", aname, strerror (errno));
		close (fd);
		return NULL;
	}
	
	close (fd);

	//LOG (PRODUCT ": Loaded aot data for %s.\n", name);

	*out_handle = ptr;

	return (unsigned char *) ptr;
}

static void
xamarin_free_aot_data (MonoAssembly *assembly, int size, gpointer user_data, void *handle)
{
	// COOP: This is a callback called by the AOT runtime, I belive we don't have to change the GC mode here.
	munmap (handle, size);
}

/*
This hook avoids the gazillion of filesystem probes we do as part of assembly loading.
*/
static MonoAssembly*
assembly_preload_hook (MonoAssemblyName *aname, char **assemblies_path, void* user_data)
{
	// COOP: This is a callback called by the AOT runtime, I belive we don't have to change the GC mode here.
	char filename [1024];
	char path [1024];
	const char *name = mono_assembly_name_get_name (aname);
	const char *culture = mono_assembly_name_get_culture (aname);

	// LOG (PRODUCT ": Looking for assembly '%s' (culture: '%s')\n", name, culture);

	int len = strlen (name);
	int has_extension = len > 3 && name [len - 4] == '.' && (!strcmp ("exe", name + (len - 3)) || !strcmp ("dll", name + (len - 3)));
	bool dual_check = false;

	// add extensions if required.
	strlcpy (filename, name, sizeof (filename));
	if (!has_extension) {	
		// Figure out if we need to append 'dll' or 'exe'
		if (xamarin_executable_name != NULL) {
			// xamarin_executable_name already has the ".exe", so only compare the rest of the filename.
			if (culture == NULL && !strncmp (xamarin_executable_name, filename, strlen (xamarin_executable_name) - 4)) {
				strlcat (filename, ".exe", sizeof (filename));
			} else {
				strlcat (filename, ".dll", sizeof (filename));
			}
		} else {
			// we need to check both :|
			dual_check = true;
			// start with .dll
			strlcat (filename, ".dll", sizeof (filename));
		}
	}

	if (culture == NULL)
		culture = "";

	bool found = xamarin_locate_assembly_resource_for_name (aname, filename, path, sizeof (path));
	if (!found && dual_check) {
		filename [strlen (filename) - 4] = 0;
		strlcat (filename, ".exe", sizeof (filename));
		found = xamarin_locate_assembly_resource_for_name (aname, filename, path, sizeof (path));
	}

	if (!found) {
		LOG (PRODUCT ": Unable to locate assembly '%s' (culture: '%s')\n", name, culture);
		return NULL;
	}

	// LOG (PRODUCT ": Found assembly '%s' (culture: '%s'): %s\n", name, culture, path);

	return mono_assembly_open (path, NULL);
}

#ifdef DEBUG_LAUNCH_TIME
uint64_t startDate = 0;
uint64_t date = 0;
void debug_launch_time_print (const char *msg)
{
	uint64_t unow;
	struct timeval now;

	gettimeofday (&now, NULL);
	unow = now.tv_sec * 1000000ULL + now.tv_usec;

	if (startDate == 0) {
		startDate = unow;
		date = startDate;
	}

	PRINT ("%s: %llu us Total: %llu us", msg, unow - date, unow - startDate);

	date = unow;
}
#else
inline void debug_launch_time_print (const char *msg)
{
}
#endif

/*
 * This class will listen for memory warnings and when received, force
 * a full garbage collection.
 * 
 * On device it will also delay the creation of the finalizer thread for 5 seconds
 * in release builds.
 */

#if defined (__arm__) || defined(__aarch64__)
extern void mono_gc_init_finalizer_thread (void);
#endif

@interface XamarinGCSupport : NSObject {
}
- (id) init;
- (void) start;
@end

@implementation XamarinGCSupport
- (id) init
{
	if (self = [super init]) {
#if defined (__arm__) || defined(__aarch64__)
		[self start];
#endif
#if TARGET_OS_WATCH
		// I haven't found a way to listen for memory warnings on watchOS.
		// fprintf (stderr, "Need to listen for memory warnings on the watch\n");
#else
		[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(memoryWarning:) name:UIApplicationDidReceiveMemoryWarningNotification object:nil];
#endif
	}

	return self;
}

- (void) start
{
	// COOP: ?
#if defined (__arm__) || defined(__aarch64__)
	mono_gc_init_finalizer_thread ();
#endif
}

- (void) memoryWarning: (NSNotification *) sender
{
	// COOP: ?
	mono_gc_collect (mono_gc_max_generation ());
}

@end

/*
 * The main method
 */

int
xamarin_main (int argc, char *argv[], enum XamarinLaunchMode launch_mode)
{
	// COOP: ?
	// + 1 for the initial "monotouch" +1 for the final NULL = +2.
	// This is not an exact number (it will most likely be lower, since there
	// are other arguments besides --app-arg), but it's a guaranteed and bound
	// upper limit.
	const char *managed_argv [argc + 2];
	int managed_argc = 1;

#if defined(__x86_64__)
	patch_sigaction ();
#endif

	xamarin_launch_mode = launch_mode;

	memset (managed_argv, 0, sizeof (char*) * (argc + 2));
	managed_argv [0] = "monotouch";

	DEBUG_LAUNCH_TIME_PRINT ("Main entered");

	xamarin_setup ();
	DEBUG_LAUNCH_TIME_PRINT ("MonoTouch setup time");

	MonoAssembly *assembly;
	guint32 exception_gchandle = 0;
	
	const char *c_bundle_path = xamarin_get_bundle_path ();

	chdir (c_bundle_path);
	setenv ("MONO_PATH", c_bundle_path, 1);

	setenv ("MONO_XMLSERIALIZER_THS", "no", 1);
	setenv ("DYLD_BIND_AT_LAUNCH", "1", 1);
	setenv ("MONO_REFLECTION_SERIALIZER", "yes", 1);

#if TARGET_OS_WATCH
	// watchOS can raise signals just fine...
	// we might want to move this inside mono at some point.
	signal (SIGPIPE, SIG_IGN);
#endif

#if TARGET_OS_WATCH || TARGET_OS_TV
	mini_parse_debug_option ("explicit-null-checks");
#endif
	// see http://bugzilla.xamarin.com/show_bug.cgi?id=820
	// take this line out once the bug is fixed
	mini_parse_debug_option ("no-gdb-backtrace");

	DEBUG_LAUNCH_TIME_PRINT ("Spin-up time");

	{
		/*
		 * Command line arguments:
		 * -debugtrack: [Simulator only]
		 *         If we should track zombie NSObjects and aggressively poke the GC to collect
		 *         every second.
		 * -monodevelop-port=<port>
		 *         The port MonoDevelop is listening on (or we should listen on).
		 *         Overrides whatever any configuration file says.
		 * -debug: 
		 *         Enables debugging (it is enabled by default, but maybe one day in the future
		 *         we can disable it by default so that when the user clicks on the app on the
		 *         device while *not* debugging he doesn't have to wait 2 extra seconds for it
		 *         to start up).
		 * -connection-mode=[wifi|usb|none]:
		 *         Select how to connect (or not) to MonoDevelop. No need to rebuild the
		 *         app anymore when switching between wifi and usb debugging since this
		 *         option overrides whatever the configuration files says. Setting 'none'
		 *         when not debugging or profiling saves 2s on startup (since the app
		 *         won't try to connect to MonoDevelop). If not set the current default is
		 *         to check the configuration files (some time in the future this will be
		 *         changed, so that it defaults to 'none'. This way there will be no 
		 *         2s delay on startup when running it manually by clicking on the icon).
		 * -app-arg=<argument>:
		 *         <argument> will be passed to the app as a command line argument. Can be
		 *         specified multiple times.
		 * -setenv=<key>=<value>
		 *         Set the environment variable <key> to the value <value>
		 */
		int i = 0;
		for (i = 0; i < argc; i++) {
			char *arg = argv [i];
			char *name;
			char *value;
			
			if (arg [0] == '-') {
				arg++;
				if (arg [0] == '-')
					arg++;
			} else if (arg [0] == '/') {
				arg++;
			} else {
				continue;
			}
			
			value = arg;
			name = NULL;
			
			while (*++value) {
				if (*value == '=' || *value == ':') {
					name = strndup (arg, value - arg);
					value++;
					break;
				}
			}
			
			if (name == NULL)
				name = strdup (arg);
			
			if (*value == 0)
				value = NULL;

#ifdef DEBUG
			if (!strcmp (name, "debugtrack")) {
				xamarin_gc_pump = true;
			} else if (!strcmp (name, "monodevelop-port")) {
				if (!value && argc > i + 1)
					value = argv [++i];
				if (value) {
					monotouch_set_monodevelop_port (strtol (value, NULL, 10));
				} else {
					PRINT ("MonoTouch: --%s requires an argument.", name);
				}
			} else if (!strcmp (name, "connection-mode")) {
				if (!value && argc > i + 1)
					value = argv [++i];
				if (value) {
					monotouch_set_connection_mode (value);
				} else {
					PRINT ("MonoTouch: --%s requires an argument.", name);
				}
			} 
#endif /* DEBUG */

			if (!strcmp (name, "app-arg")) {
				if (!value && argc > i + 1)
					value = argv [++i];
				if (value) {
					managed_argv [managed_argc++] = value;
				} else {
					PRINT ("MonoTouch: --%s requires an argument.", name);
				}
			} else if (!strcmp (name, "setenv")) {
				if (!value && argc > i + 1)
					value = argv [++i];
				if (value) {
					char *k = strdup (value);
					char *v = strchr (k, '=');
					if (v) {
						*v = 0;
						v++;
						LOG ("MonoTouch: Setting %s=%s", k, v);
						setenv (k, v, 1);
					}
					free (k);
				} else {
					PRINT ("MonoTouch: --%s requires an argument.", name);
				}
			}
			
			free (name);
		}
	}

#ifdef DEBUG
	initialize_cocoa_threads (monotouch_configure_debugging);
#else
	initialize_cocoa_threads (NULL);
#endif

#if defined (__arm__) || defined(__aarch64__)
	xamarin_register_modules ();
#endif
	DEBUG_LAUNCH_TIME_PRINT ("\tAOT register time");

#ifdef DEBUG
	monotouch_start_debugging ();
	DEBUG_LAUNCH_TIME_PRINT ("\tDebug init time");
#endif
	
	if (xamarin_init_mono_debug)
		mono_debug_init (MONO_DEBUG_FORMAT_MONO);
	
	mono_install_assembly_preload_hook (assembly_preload_hook, NULL);
	mono_install_load_aot_data_hook (xamarin_load_aot_data, xamarin_free_aot_data, NULL);

#ifdef DEBUG
	monotouch_start_profiling ();
	DEBUG_LAUNCH_TIME_PRINT ("\tProfiler config time");
#endif

	mono_set_signal_chaining (TRUE);
	mono_set_crash_chaining (TRUE);
	mono_install_unhandled_exception_hook (xamarin_unhandled_exception_handler, NULL);
	mono_install_ftnptr_eh_callback (xamarin_ftnptr_exception_handler);

	mono_jit_init_version ("MonoTouch", "mobile");
	/*
	  As part of mono initialization a preload hook is added that overrides ours, so we need to re-instate it here.
	  This is wasteful, but there's no way to manipulate the preload hook list except by adding to it.
	*/
	mono_install_assembly_preload_hook (assembly_preload_hook, NULL);
	DEBUG_LAUNCH_TIME_PRINT ("\tJIT init time");

	xamarin_initialize ();
	DEBUG_LAUNCH_TIME_PRINT ("\tmonotouch init time");

#if defined (__arm__) || defined(__aarch64__)
	xamarin_register_assemblies ();
	assembly = xamarin_open_and_register (xamarin_executable_name, &exception_gchandle);
	if (exception_gchandle != 0)
		xamarin_process_managed_exception_gchandle (exception_gchandle);
#else
	if (xamarin_executable_name) {
		assembly = xamarin_open_and_register (xamarin_executable_name, &exception_gchandle);
		if (exception_gchandle != 0)
			xamarin_process_managed_exception_gchandle (exception_gchandle);
	} else {
		const char *last_slash = strrchr (argv [0], '/');
		const char *basename = last_slash ? last_slash + 1 : argv [0];
		char *aname = xamarin_strdup_printf ("%s.exe", basename);

		assembly = xamarin_open_and_register (aname, &exception_gchandle);
		xamarin_free (aname);

		if (exception_gchandle != 0)
			xamarin_process_managed_exception_gchandle (exception_gchandle);
	}

	if (xamarin_supports_dynamic_registration) {
		xamarin_register_entry_assembly (mono_assembly_get_object (mono_domain_get (), assembly), &exception_gchandle);
		if (exception_gchandle != 0)
			xamarin_process_managed_exception_gchandle (exception_gchandle);
	}
#endif

	DEBUG_LAUNCH_TIME_PRINT ("\tAssembly register time");

	[[[XamarinGCSupport alloc] init] autorelease];

	DEBUG_LAUNCH_TIME_PRINT ("\tGC defer time");

	DEBUG_LAUNCH_TIME_PRINT ("Total initialization time");

	int rv = 0;
	switch (launch_mode) {
	case XamarinLaunchModeExtension:
		char base_dir [1024];
		char config_file_name [1024];

		snprintf (base_dir, sizeof (base_dir), "%s/" ARCH_SUBDIR, xamarin_get_bundle_path ());
		snprintf (config_file_name, sizeof (config_file_name), "%s/%s.config", base_dir, xamarin_executable_name); // xamarin_executable_name should never be NULL for extensions.

		mono_domain_set_config (mono_domain_get (), base_dir, config_file_name);

		MONO_ENTER_GC_SAFE;
		rv = xamarin_extension_main (argc, argv);
		MONO_EXIT_GC_SAFE;
		break;
	case XamarinLaunchModeApp:
		rv = mono_jit_exec (mono_domain_get (), assembly, managed_argc, managed_argv);
		break;
	case XamarinLaunchModeEmbedded:
		// do nothing
		break;
	default:
		xamarin_assertion_message ("Invalid launch mode: %i.", launch_mode);
		break;
	}
	
	return rv;
}
