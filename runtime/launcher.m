// vim: set filetype=objc :

#include <dlfcn.h> // dlsym

#import <Cocoa/Cocoa.h>

#include "shared.h"
#include "product.h"
#include "xamarin/xamarin.h"
#include "xamarin/launch.h"
#include "launcher.h"
#include "runtime-internal.h"
#include "main-internal.h"

#ifdef DYNAMIC_MONO_RUNTIME
	#define DEFAULT_MONO_RUNTIME "/Library/Frameworks/Mono.framework/Versions/Current"
	static const char *mono_runtime_prefix = NULL;
	bool xamarin_enable_debug = 0;
#endif

static char original_working_directory_path [MAXPATHLEN];

extern "C" const char * const
xamarin_get_original_working_directory_path ()
{
	return original_working_directory_path;
}

static int
redirect_io (int from_fd, const char *to_path)
{
	int err;
	int fd;

	if ((fd = open (to_path, O_CREAT | O_TRUNC | O_WRONLY, 0644)) == -1)
		return -1;

	if (dup2 (fd, from_fd) == -1) {
		err = errno;
		close (fd);
		errno = err;
		return -1;
	}

	return 0;
}

static void
init_logdir (void)
{
	const char *env;
	size_t dirlen;
	char *path;

	if ((env = getenv ("MONOMAC_LOGDIR")) != NULL && *env) {
		// Redirect stdout/err to log files...
		NSError *error = nil;
		if (![[NSFileManager defaultManager] 
				createDirectoryAtPath: [NSString stringWithUTF8String: env] 
				withIntermediateDirectories: YES 
				attributes: @{ NSFilePosixPermissions: [NSNumber numberWithInt: 0755] } 
				error: &error]) {
			fprintf (stderr, PRODUCT ": Could not create log directory: %s\n", [[error description] UTF8String]);
			return;
		}

		dirlen = strlen (env);
		path = (char *) malloc (dirlen + 12);
		strcpy (path, env);

		if (path[dirlen - 1] != '/')
			path[dirlen++] = '/';

		strcpy (path + dirlen, "stdout.log");
		if (redirect_io (STDOUT_FILENO, path) == -1)
			fprintf (stderr, PRODUCT ": Could not redirect stdout to `%s': %s\n", path, strerror (errno));

		strcpy (path + dirlen, "stderr.log");
		if (redirect_io (STDERR_FILENO, path) == -1)
			fprintf (stderr, PRODUCT ": Could not redirect stderr to `%s': %s\n", path, strerror (errno));

		free (path);
	}
}

static char *
decode_qstring (unsigned char **in, unsigned char qchar)
{
	unsigned char *inptr = *in;
	unsigned char *start = *in;
	char *value, *v;
	size_t len = 0;

	while (*inptr) {
		if (*inptr == qchar)
			break;

		if (*inptr == '\\') {
			if (inptr[1] == '\0')
				break;

			inptr++;
		}

		inptr++;
		len++;
	}

	v = value = (char *) malloc (len + 1);
	while (start < inptr) {
		if (*start == '\\')
			start++;

		*v++ = (char) *start++;
	}

	*v = '\0';

	if (*inptr)
		inptr++;

	*in = inptr;

	return value;
}

typedef struct _ListNode {
	struct _ListNode *next;
	char *value;
} ListNode;

static char **
get_mono_env_options (int *count)
{
	const char *env = getenv ("MONO_ENV_OPTIONS");
	ListNode *list = NULL, *node, *tail = NULL;
	unsigned char *start, *inptr;
	char *value, **argv;
	int i, n = 0;
	size_t size;

	if (env == NULL) {
		*count = 0;
		return NULL;
	}

	inptr = (unsigned char *) env;

	while (*inptr) {
		while (isblank ((int) *inptr))
			inptr++;

		if (*inptr == '\0')
			break;

		start = inptr++;
		switch (*start) {
		case '\'':
		case '"':
			value = decode_qstring (&inptr, *start);
			break;
		default:
			while (*inptr && !isblank ((int) *inptr))
				inptr++;

			// Note: Mac OS X <= 10.6.8 do not have strndup()
			//value = strndup ((char *) start, (size_t) (inptr - start));
			size = (size_t) (inptr - start);
			value = (char *) malloc (size + 1);
			memcpy (value, start, size);
			value[size] = '\0';
			break;
		}

		node = (ListNode *) malloc (sizeof (ListNode));
		node->value = value;
		node->next = NULL;
		n++;

		if (tail != NULL)
			tail->next = node;
		else
			list = node;

		tail = node;
	}

	// Note: we do not want child processes to inherit this environment variable,
	// so now that we've parsed it (and each value is strdup'd), we can safely
	// unset it.
	unsetenv ("MONO_ENV_OPTIONS");
	
	*count = n;

	if (n == 0)
		return NULL;

	argv = (char **) malloc (sizeof (char *) * (n + 1));
	i = 0;

	while (list != NULL) {
		node = list->next;
		argv[i++] = list->value;
		free (list);
		list = node;
	}

	argv[i] = NULL;

	return argv;
}

static void
exit_with_message (const char *reason, const char *argv0, bool request_mono)
{
	NSString *appName = nil;
	NSDictionary *plist = [[NSBundle mainBundle] infoDictionary];
	if (plist) {
		appName = (NSString *) [plist objectForKey:(NSString *)kCFBundleNameKey];
	}
	if (!appName) {
		appName = [[NSString stringWithUTF8String: argv0] lastPathComponent];
	}

	NSAlert *alert = [[NSAlert alloc] init];
	[alert setMessageText:[NSString stringWithFormat:@"Could not launch %@", appName]];
	NSString *fmt = request_mono ? @"%s\n\nPlease download and install the latest version of Mono." : @"%s\n";
	NSString *msg = [NSString stringWithFormat:fmt, reason]; 
	[alert setInformativeText:msg];
	NSLog (@"%@", msg);
	
	if (request_mono) {
		[alert addButtonWithTitle:@"Download Mono Framework"];
		[alert addButtonWithTitle:@"Cancel"];
	} else {
		[alert addButtonWithTitle:@"OK"];
	}
	NSInteger answer = [alert runModal];
	[alert release];
	
	if (request_mono && answer == NSAlertFirstButtonReturn) {
		NSString *mono_download_url = @"http://www.go-mono.com/mono-downloads/download.html";
		CFURLRef url = CFURLCreateWithString (NULL, (CFStringRef) mono_download_url, NULL);
		LSOpenCFURLRef (url, NULL);
		CFRelease (url);
	}
	exit (1);
}

static int
check_mono_version (const char *version, const char *req_version)
{
	char *req_end, *end;
	long req_val, val;
	
	while (*req_version) {
		req_val = strtol (req_version, &req_end, 10);
		if (req_version == req_end || (*req_end && *req_end != '.')) {
			fprintf (stderr, "Bad version requirement string '%s'\n", req_end);
			return FALSE;
		}
		
		req_version = req_end;
		if (*req_version)
			req_version++;
		
		val = strtol (version, &end, 10);
		if (version == end || val < req_val)
			return FALSE;
		
		if (val > req_val)
			return TRUE;
		
		if (*req_version == '.' && *end != '.')
			return FALSE;
		
		version = end + 1;
	}
	
	return TRUE;
}

#ifdef DYNAMIC_MONO_RUNTIME
static int
push_env (const char *variable, NSString *str_value)
{
	const char *value = [str_value UTF8String];
	size_t len = strlen (value);
	const char *current;
	int rv;
	
	if ((current = getenv (variable)) && *current) {
		char *buf = (char *) malloc (len + strlen (current) + 2);
		memcpy (buf, value, len);
		buf[len] = ':';
		strcpy (buf + len + 1, current);
		rv = setenv (variable, buf, 1);
		free (buf);
	} else {
		rv = setenv (variable, value, 1);
	}
	
	return rv;
}
#endif

static void
update_environment (xamarin_initialize_data *data)
{
	if (xamarin_get_is_mkbundle ())
		return;

	// 3) Ensure the following environment variables are set: [...]
	NSString *res_dir;
	NSString *monobundle_dir;

	if (data->launch_mode == XamarinLaunchModeEmbedded) {
		monobundle_dir = [data->app_dir stringByAppendingPathComponent: @"Versions/Current/MonoBundle"];
		res_dir = [data->app_dir stringByAppendingPathComponent: @"Versions/Current/Resources"];
	} else {
		monobundle_dir = [data->app_dir stringByAppendingPathComponent: @"Contents/MonoBundle"];
		res_dir = [data->app_dir stringByAppendingPathComponent: @"Contents/Resources"];
	}

#ifdef DYNAMIC_MONO_RUNTIME
	NSString *bin_dir = [data->app_dir stringByAppendingPathComponent: @"Contents/MacOS"];
	
	push_env ("DYLD_FALLBACK_LIBRARY_PATH", [NSString stringWithFormat: @"%s/lib:/usr/local/lib:/lib:/usr/lib", getenv ("HOME")]);
	push_env ("DYLD_FALLBACK_LIBRARY_PATH", [res_dir stringByAppendingPathComponent: @"/lib"]);
	push_env ("DYLD_FALLBACK_LIBRARY_PATH", [[NSString stringWithUTF8String: mono_runtime_prefix] stringByAppendingPathComponent: @"/lib"]);
	
	/* Mono "External" directory */
	push_env ("PKG_CONFIG_PATH", @"/Library/Frameworks/Mono.framework/External/pkgconfig");
	/* Enable the use of stuff bundled into the app bundle */
	push_env ("PKG_CONFIG_PATH", [res_dir stringByAppendingPathComponent: @"/lib/pkgconfig"]);
	push_env ("PKG_CONFIG_PATH", [res_dir stringByAppendingPathComponent: @"/share/pkgconfig"]);
	
	
	push_env ("MONO_GAC_PREFIX", res_dir);
	push_env ("PATH", bin_dir);
	
	data->requires_relaunch = true;
#else
	// disable /dev/shm since Apple refuse applications that uses it in the Mac App Store
	setenv ("MONO_DISABLE_SHARED_AREA", "", 1);
	unsetenv ("MONO_PATH");
	mono_set_assemblies_path (xamarin_get_bundle_path ());

	// 3b) (If embedding Mono) Change the current directory to $appdir/Contents/Resources
	chdir ([res_dir UTF8String]);

	// Set up environment variables that need to point to ~/Library/Application Support (registry path)
	NSFileManager *mgr = [NSFileManager defaultManager];
	NSArray *appSupportDirectories = [mgr URLsForDirectory:NSApplicationSupportDirectory inDomains:NSSystemDomainMask];
	
	if ([appSupportDirectories count] > 0) {
		NSString *appBundleID = [[NSBundle mainBundle] bundleIdentifier];
		NSURL *appSupport = [appSupportDirectories objectAtIndex: 0];
		if (appSupport != nil && appBundleID != nil) {
			NSURL *appDirectory = [appSupport URLByAppendingPathComponent:appBundleID isDirectory: YES];
			setenv ("MONO_REGISTRY_PATH", [[appDirectory path] UTF8String], 1);
		}
	}
#endif
	if (xamarin_disable_lldb_attach) {
		// Unfortunately the only place to set debug_options.no_gdb_backtrace is in mini_parse_debug_option
		// So route through MONO_DEBUG
		setenv ("MONO_DEBUG", "no-gdb-backtrace", 0);
	}

#ifndef DYNAMIC_MONO_RUNTIME
	setenv ("MONO_CFG_DIR", [monobundle_dir UTF8String], 0);
#endif
}

static void
app_initialize (xamarin_initialize_data *data)
{
	// The launch code here is publicly documented in xamarin/launch.h
	// The numbers in the comments refer to numbers in xamarin/launch.h.

	xamarin_launch_mode = data->launch_mode;

#ifndef SYSTEM_LAUNCHER
	if (xammac_setup ()) {
		data->exit_code = -1;
		return;
	}
#endif

	bool mkbundle = xamarin_get_is_mkbundle ();

	NSBundle *bundle;

	if (data->launch_mode == XamarinLaunchModeEmbedded) {
		bundle = [NSBundle bundleForClass: [XamarinAssociatedObject class]];
	} else {
		bundle = [NSBundle mainBundle];
	}
	data->app_dir = [bundle bundlePath]; // this is good until the autorelease pool releases the bundlePath string.

	// 1) If found, call the custom initialization function (xamarin_custom_initialize)
	xamarin_custom_initialize_func init = (xamarin_custom_initialize_func) dlsym (RTLD_MAIN_ONLY, "xamarin_app_initialize");
	if (init != NULL)
		init (data);

#ifdef DYNAMIC_MONO_RUNTIME
	// 2) (If not embedding Mono) Search for the system Mono in the following directories:
	if (!(mono_runtime_prefix = getenv ("MONO_RUNTIME")))
		mono_runtime_prefix = DEFAULT_MONO_RUNTIME;
#endif

	// 3) Ensure the following environment variables are set: [...]
	if (!data->is_relaunch) {
		update_environment (data);

		if (data->requires_relaunch)
			return;
	}

	data->app_dir = NULL; // Make sure nobody ends up using this.

	// 4) Ensure that the maximum number of open files is least 1024.
	struct rlimit limit;
	if (getrlimit (RLIMIT_NOFILE, &limit) == 0 && limit.rlim_cur < 1024) {
		limit.rlim_cur = MIN (limit.rlim_max, 1024);
		setrlimit (RLIMIT_NOFILE, &limit);
	}

	// 5) Verify the minimum Mono version. The minimum mono version is specified in: [...]
	NSDictionary *plist = [[NSBundle mainBundle] infoDictionary];
	NSString *minVersion = NULL;
	if (plist != NULL) {
		minVersion = (NSString *) [plist objectForKey:@"MonoMinimumVersion"];
		if (minVersion == NULL)
			minVersion = (NSString *) [plist objectForKey:@"MonoMinVersion"];
	} else {
		fprintf (stderr, PRODUCT ": Could not load Info.plist from the bundle.");
	}
	
	if (!minVersion) {
		// This must be kept in sync with mmp's minimum mono version (in driver.cs)
		minVersion = @"4.2.0";
	}

	char *mono_version;
#ifdef DYNAMIC_MONO_RUNTIME
	const char *err = xamarin_initialize_dynamic_runtime (mono_runtime_prefix);
	if (err) {
		mono_version = xamarin_get_mono_runtime_build_info ();
		if (mono_version && !check_mono_version (mono_version, [minVersion UTF8String])) {
			exit_with_message ([[NSString stringWithFormat:@"This application requires the Mono framework version %@ or newer.", minVersion] UTF8String], data->basename, true);
		} else {
			exit_with_message (err, data->basename, true);
		}
	}
#endif

	mono_version = mono_get_runtime_build_info ();
	if (!check_mono_version (mono_version, [minVersion UTF8String]))
		exit_with_message ([[NSString stringWithFormat:@"This application requires the Mono framework version %@ or newer.", minVersion] UTF8String], data->basename, true);

	// 6) Find the executable. The name is: [...]
	if (data->launch_mode == XamarinLaunchModeApp) {
		NSString *exeName = NULL;
		NSString *exePath;
		if (plist != NULL)
			exeName = (NSString *) [plist objectForKey:@"MonoBundleExecutable"];
		else
			fprintf (stderr, PRODUCT ": Could not find Info.plist in the bundle.\n");

		if (exeName == NULL)
			exeName = [[NSString stringWithUTF8String: data->basename] stringByAppendingString: @".exe"];

		if (mkbundle) {
			exePath = exeName;
		} else {
			exePath = [[[NSString stringWithUTF8String: xamarin_get_bundle_path ()] stringByAppendingString: @"/"] stringByAppendingString: exeName];

			if (!xamarin_file_exists ([exePath UTF8String]))
				exit_with_message ([[NSString stringWithFormat:@"Could not find the executable '%@'\n\nFull path: %@", exeName, exePath] UTF8String], data->basename, false);
		}
		xamarin_entry_assembly_path = strdup ([exePath UTF8String]);
	} else {
		NSString *dllName = [[NSString stringWithUTF8String: data->basename] stringByAppendingString: @".dll"];
		NSString *dllPath = [[[NSString stringWithUTF8String: xamarin_get_bundle_path ()] stringByAppendingString: @"/"] stringByAppendingString: dllName];
		if (!xamarin_file_exists ([dllPath UTF8String]))
				exit_with_message ([[NSString stringWithFormat:@"Could not find the extension library '%@'\n\nFull path: %@", dllName, dllPath] UTF8String], data->basename, false);

		xamarin_entry_assembly_path = strdup ([dllPath UTF8String]);
	}

	// 7a) [If not embedding] Parse the system Mono's config file ($monodir/etc/mono/config).
	// 7b) [If embedding] Parse $appdir/Contents/MonoBundle/machine.config and $appdir/Contents/MonoBundle/config
	if (!mkbundle) {
		NSString *config_path = nil;
		NSString *machine_config_path = nil;
		NSError *error = nil;
		
#ifndef DYNAMIC_MONO_RUNTIME
		config_path = [[NSString stringWithUTF8String: xamarin_get_bundle_path ()] stringByAppendingPathComponent: @"config"];
		machine_config_path = [[NSString stringWithUTF8String: xamarin_get_bundle_path ()] stringByAppendingPathComponent: @"machine.config"];
		mono_set_dirs (xamarin_get_bundle_path (), xamarin_get_bundle_path ());
#else
		mono_set_dirs (NULL, NULL);
#endif

		if (machine_config_path != nil) {
			NSString *config = [NSString stringWithContentsOfFile: machine_config_path encoding: NSUTF8StringEncoding error: &error];
			if (config != nil) {
				mono_register_machine_config (strdup ([config UTF8String]));
			} else {
				// fprintf (stderr, PRODUCT ": Could not load machine.config: %s\n", [machine_config_path UTF8String]);
			}
		}

		if (config_path != nil) {
			NSString *config = [NSString stringWithContentsOfFile: config_path encoding: NSUTF8StringEncoding error: &error];
			if (config != nil) {
				mono_config_parse_memory ((char *) [config UTF8String]);
			} else {
				// fprintf (stderr, PRODUCT ": Could not load config: %s\n", [config_path UTF8String]);
			}
		}
	}

	/* other non-documented stuff... */	

	initialize_cocoa_threads (NULL);
	init_logdir ();
}

#define __XAMARIN_MAC_RELAUNCH_APP__ "__XAMARIN_MAC_RELAUNCH_APP__"

static void
run_application_init (xamarin_initialize_data *data)
{
	if (!xamarin_file_exists (xamarin_entry_assembly_path))
		exit_with_message ([[NSString stringWithFormat:@"Could not find the assembly '%s'", xamarin_entry_assembly_path] UTF8String], data->basename, false);

	// Make sure any output from mono isn't lost when launching extensions,
	// etc, by installing the log callbacks early (xamarin_initialize will
	// also do this, but if something goes wrong before we reach
	// xamarin_initialize when running as an extension, the output will be
	// lost).
	xamarin_install_log_callbacks ();

	mono_jit_init (xamarin_entry_assembly_path);

	MonoAssembly *assembly = xamarin_open_assembly ("Xamarin.Mac.dll");
	if (!assembly)
		xamarin_assertion_message ("Failed to load %s.", "Xamarin.Mac.dll");

	MonoImage *image = mono_assembly_get_image (assembly);

	MonoClass *app_class = mono_class_from_name (image, "AppKit", "NSApplication");
	if (!app_class)
		xamarin_assertion_message ("Fatal error: failed to load the NSApplication class");

	MonoMethod *initialize = mono_class_get_method_from_name (app_class, "Init", 0);
	if (!initialize)
		xamarin_assertion_message ("Fatal error: failed to load the NSApplication.Init method");

	mono_runtime_invoke (initialize, NULL, NULL, NULL);
}

int xamarin_main (int argc, char **argv, enum XamarinLaunchMode launch_mode)
{
	xamarin_initialize_data data = { 0 };

	if (getcwd (original_working_directory_path, sizeof (original_working_directory_path)) == NULL)
		original_working_directory_path [0] = '\0';

	@autoreleasepool {
		data.size = sizeof (data);
		data.argc = argc;
		data.argv = argv;
		data.launch_mode = launch_mode;
		// basename = Path.GetFileName (argv [0])
		if (!(data.basename = strrchr (argv [0], '/'))) {
			data.basename = argv [0];
		} else {
			data.basename++;
		}

		data.is_relaunch = getenv (__XAMARIN_MAC_RELAUNCH_APP__) != NULL;
		if (data.is_relaunch)
			unsetenv (__XAMARIN_MAC_RELAUNCH_APP__);

		app_initialize (&data);

		if (data.exit)
			return data.exit_code;

		if (data.requires_relaunch) {
			setenv (__XAMARIN_MAC_RELAUNCH_APP__, "1", 1);
			return execv (argv [0], argv);
		}
	}

	int rv = 0;

	@autoreleasepool {
		int env_argc = 0;
		char **env_argv = get_mono_env_options (&env_argc);
		int new_argc = env_argc + 2 /* --debug executable */ + argc ;
		if (xamarin_mac_hybrid_aot)
			new_argc += 1;
		if (xamarin_mac_modern)
			new_argc += 1;

		char **new_argv = (char **) malloc (sizeof (char *) * (new_argc + 1 /* null terminated */));
		const char **ptr = (const char **) new_argv;
		// binary
		*ptr++ = argv [0];
		// inject MONO_ENV_OPTIONS
		for (int i = 0; i < env_argc; i++)
			*ptr++ = env_argv [i];

		if (xamarin_debug_mode) {
			*ptr++ = "--debug";
		} else {
			new_argc--;
		}

		if (xamarin_mac_hybrid_aot)
			*ptr++ = "--hybrid-aot";
		if (xamarin_mac_modern)
			*ptr++ = "--runtime=mobile";

		// executable assembly
		*ptr++ = xamarin_entry_assembly_path;

		// the rest
		for (int i = 1; i < argc; i++)
			*ptr++ = argv [i];
		*ptr = NULL;

		switch (launch_mode) {
		case XamarinLaunchModeExtension: {
			run_application_init (&data);

			void * libExtensionHandle = dlopen ("/usr/lib/libextension.dylib", RTLD_LAZY);
			if (libExtensionHandle == nil)
				exit_with_message ("Unable to load libextension.dylib", data.basename, false);

			typedef int (*extension_main)(int argc, char * argv[]);

			extension_main extensionMain = (extension_main) dlsym (libExtensionHandle, "NSExtensionMain");

			if (extensionMain == nil)
				exit_with_message ("Unable to load NSExtensionMain", data.basename, false);

			rv = (*extensionMain) (new_argc, new_argv);
			dlclose (libExtensionHandle);
			break;
		}
		case XamarinLaunchModeApp:
			rv = mono_main (new_argc, new_argv);
			break;
		case XamarinLaunchModeEmbedded:
			run_application_init (&data);
			break;
		default:
			xamarin_assertion_message ("Invalid launch mode: %i.", launch_mode);
			break;
		}

		free (new_argv);
	}

	return rv;
}

int main (int argc, char **argv)
{
	return xamarin_main (argc, argv, XamarinLaunchModeApp);
}

int xamarin_mac_extension_main (int argc, char **argv)
{
	return xamarin_main (argc, argv, XamarinLaunchModeExtension);
}
