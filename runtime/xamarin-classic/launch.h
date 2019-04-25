//
// launch.h: Public launch code for Xamarin.Mac.
//           This header describes the launch sequence of Xamarin.Mac apps.
//           This header is public.
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. 
//

#ifndef __XAMARIN_LAUNCH_H__
#define __XAMARIN_LAUNCH_H__

#include <Foundation/Foundation.h>

#ifdef __cplusplus
extern "C" {
#endif

/*
 * Xamarin.Mac will follow this startup logic:
 *
 * 1) If found, call the custom initialization function (xamarin_app_initialize). See later in this file how
 *    to declare this function.
 *
 * 2) (If not embedding Mono) Search for the system Mono in the following directories:
 * 		a) As specified by the MONO_RUNTIME environment variable.
 *		b) Otherwise '/Library/Frameworks/Mono.framework/Versions/Current'
 *
 * 3) Ensure the following environment variables are set:
 * 
 *  appdir = /full/path/to/theapp.app ([[NSBundle mainBundle] bundlePath])
 *  resdir = $appdir/Contents/Resources
 *  bindir = $appdir/Contents/MacOS
 *	monodir = the path to the Mono runtime (as found in point 2 above)
 *
 *  If not embedding Mono:
 *
 *		DYLD_FALLBACK_LIBRARY_PATH=$resdir/lib:$monodir/lib:$DYLD_FALLBACK_LIBRARY_PATH
 *		PKG_CONFIG_PATH=/Library/Frameworks/Mono.framework/External/pkgconfig:$resdir/lib/pkgconfig:$resdir/share/pkgconfig:$PKG_CONFIG_PATH
 *		MONO_GAC_PREFIX=$resdir:$MONO_GAC_PREFIX
 *		PATH=$bindir:$PATH
 *
 *  If embedding Mono:
 *
 *      MONO_DISABLE_SHARED_AREA=1
 *      MONO_REGISTRY_PATH=~/Library/Application Support (NSApplicationSupportDirectory/$appid)	
 *
 *  The app does not need to set these variables. The app may end up relaunching in
 *  this process (some environment variables only take effect upon initial load, in
 *  particular any DYLD_* variables).
 *
 * 3b) (If embedding Mono) Change the current directory to $appdir/Contents/Resources
 *
 * 4) Ensure that the maximum number of open files is least 1024.
 * 
 * 5) Verify the minimum Mono version. The minimum mono version is specified in:
 *		a) The bundle's Info.plist's MonoMinimumVersion field.
 *		b) The bundle's Info.plist's MonoMinVersion field.
 *		c) The minimum mono version Xamarin.Mac requires (currently 3.10)
 *
 *	  If the minimum Mono version requirement can't be met, a dialog will show an error
 *	  and the app will exit.
 *
 * 6) Find the executable. The name is:
 *		a) The bundle's Info.plist's MonoBundleExecutable field.
 *		b) Otherwise the name of the executable + ".exe"
 *
 *	  The directory is by default the bundle's Contents/MonoBundle directory, but can be changed
 *	  by calling xamarin_set_bundle_path.
 *
 *	  If the executable can't be found, a dialog will show an error and the app will exit.
 *
 * 7a) [If not embedding Mono] Don't parse any config files, leave it to the Mono runtime to load the defaults.
 *
 * 7b) [If embedding Mono] Parse $appdir/Contents/MonoBundle/machine.config and $appdir/Contents/MonoBundle/config
 *     (It is not an error to not find these files, but the app will most likely not work)
 *
 */

/*
 * To have a custom initialization method link a static library into the app,
 * and include a function called 'xamarin_app_initialize'. The method will be
 * called as described above.
 *
 * Example mmp command line:
 *     mmp --output pathToApp --name MyApp --no-root-assembly --profile=xammac --link_flags="-force_load custom-init.a" --use-system-mono --arch i386
 *
 *     "-force_load" is required, because otherwise the native linker will see that
 *     it's not used, and remove it.
 */
typedef struct xamarin_initialize_data {
	/* Input arguments */
	int size; /* The size of this structure */
	int argc;
	char **argv;
	bool is_relaunch; /* If we've been respawned or not */
	const char *basename;
	NSString *app_dir; /* The bundle directory */
	/* Output arguments */
	bool exit; // If initialization failed and the app should exit.
	int exit_code; // The exit code to return if initialization failed.
	bool requires_relaunch; // if the launcher should respawn itself (setting some environment variables require this, in particular DYLD_* variables). This is ignored if 'is_relaunch' is true.
	enum XamarinLaunchMode launch_mode;
} xamarin_initialize_data;

typedef void (*xamarin_custom_initialize_func) (xamarin_initialize_data *data);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __XAMARIN_LAUNCH_H__ */
