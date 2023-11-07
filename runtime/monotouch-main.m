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

#include <TargetConditionals.h>
#if !TARGET_OS_OSX
#include <UIKit/UIKit.h>
#endif
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

#if defined(__x86_64__) && !DOTNET
#include "../tools/mtouch/monotouch-fixes.c"
#endif

static char original_working_directory_path [MAXPATHLEN];

const char * const
xamarin_get_original_working_directory_path ()
{
	return original_working_directory_path;
}

#if !defined (CORECLR_RUNTIME)
unsigned char *
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

	void *ptr = mmap (NULL, (size_t) size, PROT_READ, MAP_FILE | MAP_PRIVATE, fd, 0);
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

void
xamarin_free_aot_data (MonoAssembly *assembly, int size, gpointer user_data, void *handle)
{
	// COOP: This is a callback called by the AOT runtime, I belive we don't have to change the GC mode here.
	munmap (handle, (size_t) size);
}

/*
This hook avoids the gazillion of filesystem probes we do as part of assembly loading.
*/
MonoAssembly*
xamarin_assembly_preload_hook (MonoAssemblyName *aname, char **assemblies_path, void* user_data)
{
	// COOP: This is a callback called by the AOT runtime, I belive we don't have to change the GC mode here.
	char filename [1024];
	char path [1024];
	const char *name = mono_assembly_name_get_name (aname);
	const char *culture = mono_assembly_name_get_culture (aname);

	// LOG (PRODUCT ": Looking for assembly '%s' (culture: '%s')\n", name, culture);

	size_t len = strnlen (name, sizeof (filename));
	if (len == sizeof (filename))
		return NULL;
	int has_extension = len > 3 && name [len - 4] == '.' && (!strcmp ("exe", name + (len - 3)) || !strcmp ("dll", name + (len - 3)));
	bool dual_check = false;

	// add extensions if required.
	strlcpy (filename, name, sizeof (filename));
	if (!has_extension) {	
		// Figure out if we need to append 'dll' or 'exe'
		if (xamarin_executable_name != NULL) {
			// xamarin_executable_name already has the ".exe", so only compare the rest of the filename.
			size_t exe_len = strnlen (name, PATH_MAX);
			if (exe_len == PATH_MAX)
				return NULL;
			if (culture == NULL && !strncmp (xamarin_executable_name, filename, exe_len - 4)) {
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
		size_t flen = strnlen (filename, sizeof (filename));
		if (flen == sizeof (filename))
			return NULL;
		filename [flen - 4] = 0;
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
#endif // !defined (CORECLR_RUNTIME)

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
#if !defined (CORECLR_RUNTIME)
extern void mono_gc_init_finalizer_thread (void);
#endif
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
#elif !TARGET_OS_OSX
		[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(memoryWarning:) name:UIApplicationDidReceiveMemoryWarningNotification object:nil];
#endif
	}

	return self;
}

- (void) start
{
	// COOP: ?
#if defined (__arm__) || defined(__aarch64__)
#if !defined (CORECLR_RUNTIME)
	MONO_ENTER_GC_UNSAFE;
	mono_gc_init_finalizer_thread ();
	MONO_EXIT_GC_UNSAFE;
#endif
#endif
}

- (void) memoryWarning: (NSNotification *) sender
{
	// COOP: ?
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	xamarin_gc_collect (&exception_gchandle);
	xamarin_process_managed_exception_gchandle (exception_gchandle);
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
	int managed_argc = 0;

#if defined(__x86_64__) && !DOTNET
	patch_sigaction ();
#endif

	xamarin_launch_mode = launch_mode;

	memset (managed_argv, 0, sizeof (char*) * (unsigned long) (argc + 2));

#if !(TARGET_OS_OSX || TARGET_OS_MACCATALYST)
	managed_argv [managed_argc++] = "monotouch";
#endif

	DEBUG_LAUNCH_TIME_PRINT ("Main entered");

	xamarin_setup ();
	DEBUG_LAUNCH_TIME_PRINT ("MonoTouch setup time");

	MonoAssembly *assembly;
	GCHandle exception_gchandle = NULL;

	// Get the original working directory, and store it somewhere.
	if (getcwd (original_working_directory_path, sizeof (original_working_directory_path)) == NULL)
		original_working_directory_path [0] = '\0';

	// For legacy Xamarin.Mac, we used to chdir to $appdir/Contents/Resources (I'm not sure where this comes from, earliest commit I could find was this: https://github.com/xamarin/maccore/commit/20045dd7f85cb038cea673a9281bb6131711069c)
	// For mobile platforms, we chdir to $appdir
	// In .NET, we always chdir to $appdir, so that we're consistent
	const char *c_bundle_path = xamarin_get_app_bundle_path ();
	chdir (c_bundle_path);

	setenv ("DYLD_BIND_AT_LAUNCH", "1", 1);

#if TARGET_OS_WATCH
	// watchOS can raise signals just fine...
	// we might want to move this inside mono at some point.
	signal (SIGPIPE, SIG_IGN);
#endif

	xamarin_bridge_setup ();

	DEBUG_LAUNCH_TIME_PRINT ("Spin-up time");

	{
		/*
		 * Command line arguments for mobile targets (iOS / tvOS / watchOS):
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
		 *
		 * For desktop targets (macOS / Mac Catalyst) we pass all the command
		 * line arguments directly to the managed Main method.
		 *
		 */
		int i = 0;
		for (i = 0; i < argc; i++) {
#if TARGET_OS_OSX || TARGET_OS_MACCATALYST
			managed_argv [managed_argc++] = argv [i];
#else
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
					name = strndup (arg, (size_t) (value - arg));
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
					monotouch_set_monodevelop_port ((int) strtol (value, NULL, 10));
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
						// arguments comes from mtouch (and developer), i.e. a trusted source
						setenv (k, v, 1);
					}
					free (k);
				} else {
					PRINT ("MonoTouch: --%s requires an argument.", name);
				}
			}
			
			free (name);
#endif // TARGET_OS_OSX || TARGET_OS_MACCATALYST
		}
	}

#ifdef DEBUG
	xamarin_initialize_cocoa_threads (monotouch_configure_debugging);
#else
	xamarin_initialize_cocoa_threads (NULL);
#endif

#if DOTNET
	xamarin_vm_initialize ();
#endif
	xamarin_bridge_initialize ();

	xamarin_initialize ();
	DEBUG_LAUNCH_TIME_PRINT ("\tmonotouch init time");

	if (xamarin_register_assemblies != NULL)
		xamarin_register_assemblies ();

#if !defined (NATIVEAOT)
	if (xamarin_executable_name) {
		assembly = xamarin_open_and_register (xamarin_executable_name, &exception_gchandle);
		xamarin_process_fatal_exception_gchandle (exception_gchandle, "An exception occurred while opening the main executable");
	} else {
		const char *last_slash = strrchr (argv [0], '/');
		const char *basename = last_slash ? last_slash + 1 : argv [0];
		char *aname = xamarin_strdup_printf ("%s.exe", basename);

		assembly = xamarin_open_and_register (aname, &exception_gchandle);
		xamarin_free (aname);
		xamarin_process_fatal_exception_gchandle (exception_gchandle, "An exception occurred while opening an assembly");
	}

	if (xamarin_supports_dynamic_registration) {
		MonoReflectionAssembly *rassembly = mono_assembly_get_object (mono_domain_get (), assembly);
		xamarin_register_entry_assembly (rassembly, &exception_gchandle);
		xamarin_mono_object_release (&rassembly);
		xamarin_process_fatal_exception_gchandle (exception_gchandle, "An exception occurred while opening the entry assembly");
	}
#else
	assembly = NULL;
	(void)exception_gchandle;
#endif // !defined (NATIVEAOT)

	DEBUG_LAUNCH_TIME_PRINT ("\tAssembly register time");

	[[[XamarinGCSupport alloc] init] autorelease];

	DEBUG_LAUNCH_TIME_PRINT ("\tGC defer time");

	DEBUG_LAUNCH_TIME_PRINT ("Total initialization time");

	int rv = 0;
	switch (launch_mode) {
	case XamarinLaunchModeExtension:
#if !DOTNET
		// It doesn't look like calling mono_domain_set_config is needed in .NET,
		// it's covered by the call to xamarin_bridge_vm_initialize.
		char base_dir [1024];
		char config_file_name [1024];

		snprintf (base_dir, sizeof (base_dir), "%s/" ARCH_SUBDIR, xamarin_get_bundle_path ());
		snprintf (config_file_name, sizeof (config_file_name), "%s/%s.config", base_dir, xamarin_executable_name); // xamarin_executable_name should never be NULL for extensions.

		mono_domain_set_config (mono_domain_get (), base_dir, config_file_name);
#endif

		rv = xamarin_extension_main (argc, argv);
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

	xamarin_mono_object_release (&assembly);
	
	xamarin_release_static_dictionaries ();

	xamarin_bridge_shutdown ();

	return rv;
}
