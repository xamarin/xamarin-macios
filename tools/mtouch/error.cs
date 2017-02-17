// Copyright 2011-2012, Xamarin Inc. All rights reserved,

using System;
using System.Collections.Generic;

namespace Xamarin.Bundler {

	// Error allocation
	//
	// MT0xxx	mtouch itself, e.g. parameters, environment (e.g. missing tools)
	//					MT0000	Unexpected error - Please fill a bug report at http://bugzilla.xamarin.com
	//					MT0001  '-devname' was provided without any device-specific action
	//					MT0002	Could not parse the environment variable '{0}'.
	//					MT0003	Application name '{0}.exe' conflicts with an SDK or product assembly (.dll) name.
	//					MT0004	<unused>
	//					MT0005	<unused>
	//					MT0006	There is no devel platform at '{0}', use --platform=PLAT to specify the SDK
	//					MT0007	The root assembly '{0}' does not exist
	//					MT0008	You should provide one root assembly only, found {0} assemblies: '{1}'
	//					MT0009	Error while loading assemblies: {0}
	//					MT0010	Could not parse the command line arguments: {0}
	//					MT0011	{0} was built against a more recent runtime ({1}) than Xamarin.iOS supports
	//					MT0012	Incomplete data is provided to complete `{0}`.
	//					MT0013	<unused>
	//					MT0014	The iOS {0}.{1} SDK does not support building applications targeting {2}
	//					MT0015	Invalid ABI: {0}. Supported ABIs are: i386, x86_64,  armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, arm64 and arm64+llvm.
	//					MT0016	The option '{0}' has been deprecated.
	//					MT0017	You should provide a root assembly
	//					MT0018	Unknown command line argument: '{0}'
	//					MT0019	Only one --[log|install|kill|launch]dev or --[launch|debug]sim option can be used.
	//					MT0020	The valid options for '{0}' are '{1}'.
	//					MT0021	Cannot compile using gcc/g++ (--use-gcc) when using the static registrar (this is the default when compiling for device). Either remove the --use-gcc flag or use the dynamic registrar (--registrar:dynamic).
	//					MT0022  <unused>
	//					MT0023	Application name '{0}.exe' conflicts with another user assembly.
	//					MT0024  Could not find required file '{0}'.
	//					MT0025	No SDK version was provided. Please add --sdk=X.Y to specify which iOS SDK should be used to build your application.
	//					MT0026  Could not parse the command line argument '{0}': {1}
	//					MT0027  The options '{0}' and '{1}' are not compatible.
	//					MT0028  Cannot enable PIE (-pie) when targeting iOS 4.1 or earlier. Please disable PIE (-pie:false) or set the deployment target to at least iOS 4.2
	//					MT0029	REPL (--enable-repl) is only supported in the simulator (--sim).
	// 		Warning		MT0030	The executable name ({0}) and the app name ({1}) are different, this may prevent crash logs from getting symbolicated properly.
	//					MT0031	The command line arguments '--enable-background-fetch' and '--launch-for-background-fetch' require '--launchsim' too.
	//		Warning		MT0032	The option '--debugtrack' is ignored unless '--debug' is also specified.
	//					MT0033	A Xamarin.iOS project must reference either monotouch.dll or Xamarin.iOS.dll
	//					MT0034	Cannot reference '{0}.dll' in a {1} project - it is implicitly referenced by '{2}'.
	//					MT0035	<unused>
	//					MT0036	<unused>
	//					MT0037	<unused>
	//					MT0038	<unused>
	//					MT0040	Could not find the assembly '{0}', referenced by '{1}'.
	//					MT0041	<unused>
	//					MT0042	<unused>
	//		Warning		MT0043	The Boehm garbage collector is not supported. The SGen garbage collector has been selected instead.
	//					MT0044	--listsim is only supported with Xcode 6.0 or later.
	//					MT0045	--extension is only supported when using the iOS 8.0 (or later) SDK.
	//		Warning		MT0046	{0}.framework is unsupported in from Xcode {1} and might not work correctly
	//					MT0047	<unused>
	//		Warning		MT0048	<unused>
	//					MT0049	{0}.framework is supported only if deployment target is 8.0 or later. {0} features might not work correctly.
	//		Warning		MT0050	<unused>
	//					MT0051	Xamarin.iOS {0} requires Xcode 6.0 or later. The current Xcode version (found in {2}) is {1}.
	//					MT0052	No command specified.
	//					MT0053  <used by mmp>
	//					MT0054	Unable to canonicalize the path '{0}': {1}
	//					MT0055	The Xcode path '{0}' does not exist.
	//					MT0056	Cannot find Xcode in the default location (/Applications/Xcode.app). Please install Xcode, or pass a custom path using --sdkroot <path>.
	//					MT0057	Cannot determine the path to Xcode.app from the sdk root '{0}'. Please specify the full path to the Xcode.app bundle.
	//					MT0058	The Xcode.app '{0}' is invalid (the file '{1}' does not exist).
	//		Warning		MT0059	Could not find the currently selected Xcode on the system: {0}
	//		Warning		MT0060	Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '{0}', but that directory does not exist.
	//					MT0061	No Xcode.app specified (using --sdkroot), using the system Xcode as reported by 'xcode-select --print-path': {0}
	//					MT0062	No Xcode.app specified (using --sdkroot or 'xcode-select --print-path'), using the default Xcode instead: {0}
	//					MT0063	Cannot find the executable in the extension {0} (no CFBundleExecutable entry in its Info.plist)
	//					MT0064	<unused>
	//					MT0065	Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 (current deployment target: {0} embedded frameworks: {1})
	//					MT0066	Invalid build registrar assembly: {0}
	//					MT0067	Invalid registrar: {0}
	//					MT0068	Invalid value for target framework: {0}.
	//					MT0069	<unused, can be reused>
	//					MT0070	Invalid target framework: {0}. Valid target frameworks are: {1}.
	//					MT0071	Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.
	//					MT0072	Extensions are not supported for the platform '{0}'.
	//					MT0073	Xamarin.iOS {0} does not support a deployment target of {1} (the minimum is {2}). Please select a newer deployment target in your project's Info.plist.
	//					MT0074	Xamarin.iOS {0} does not support a minimum deployment target of {1} (the maximum is {2}). Please select an older deployment target in your project's Info.plist or upgrade to a newer version of Xamarin.iOS.
	//					MT0075	Invalid architecture '{0}' for {1} projects. Valid architectures are: {2}
	//					MT0076	No architecture specified (using the --abi argument). An architecture is required for {0} projects.
	//					MT0077	WatchOS projects must be extensions.
	//					MT0078	Incremental builds are enabled with a deployment target < 8.0 (currently {0}). This is not supported (the resulting application will not launch on iOS 9), so the deployment target will be set to 8.0.
	//		Warning		MT0079	The recommended Xcode version for Xamarin.iOS {0} is Xcode 7.0 or later. The current Xcode version (found in {2}) is {1}.
	//		Warning		MT0080  Disabling NewRefCount, --new-refcount:false, is deprecated.
	//					MT0081	The command line argument --download-crash-report also requires --download-crash-report-to.
	//					MT0082	REPL (--enable-repl) is only supported when linking is not used (--nolink).
	//					MT0083	Asm-only bitcode is not supported on watchOS. Use either --bitcode:marker or --bitcode:full.
	//					MT0084	Bitcode is not supported in the simulator. Do not pass --bitcode when building for the simulator.
	//					MT0085  No reference to '{0}' was found. It will be added automatically.
	//					MT0086  A target framework (--target-framework) must be specified when building for TVOS or WatchOS.
	//		Warning		MT0087	<unused>
	//					MT0088	The GC must be in cooperative mode for watchOS apps. Please remove the --coop:false argument to mtouch.
	//					MT0089	The option '{0}' cannot take the value '{1}' when cooperative mode is enabled for the GC.
	//					MT0091	This version of Xamarin.iOS requires the {0} {1} SDK (shipped with Xcode {2}) when the managed linker is disabled. Either upgrade Xcode, or enable the managed linker by changing the Linker behaviour to Link Framework SDKs Only.
	//					MT0092	<used by Xamarin.Launcher> The option '{0}' is required.
	//					MT0093	Could not find 'mlaunch'.
	//		Warning		MT0094	<unused> Both profiling (--profiling) and incremental builds (--fastdev) are currently not supported when building for {0}, and incremental builds have been disabled (this will be fixed in a future release).
	//					MT0095 Aot files could not be copied to the destination directory.
	//					MT0096 No reference to Xamarin.iOS.dll was found.
	//					MT0097 <used by mmp>
	//					MT0098 <used by mmp>
	//					MT0099	Internal error {0}. Please file a bug report with a test case (http://bugzilla.xamarin.com).
	//					MT0100	Invalid assembly build target: '{0}'. Please file a bug report with a test case (http://bugzilla.xamarin.com).
	//					MT0101	The assembly '*' is specified multiple times in --assembly-build-target arguments.
	//					MT0102	The assemblies '*' and '*' have the same target name ('*'), but different targets ('*' and '*').
	//					MT0103	The static object '*' contains more than one assembly ('*'), but each static object must correspond with exactly one assembly.
	//					MT0105	No assembly build target was specified for '{0}'.
	//					MT0106	The assembly build target name '{0}' is invalid: the character '{1}' is not allowed.
	//					MT0107	The assemblies '{0}' have different custom LLVM optimizations ({1}), which is not allowed when they are all compiled to a single binary.
	//					MT0108	The assembly build target '{0}' did not match any assemblies.
	//		Warning		MT0109	The assembly '{0}' was loaded from a different path than the provided path (provided path: {1}, actual path: {2}).
	//		Warning		MT0110  Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include third-party binding libraries and that compiles to bitcode.
	//		Warning		MT0111	Bitcode has been enabled because this version of Xamarin.iOS does not support building watchOS projects using LLVM without enabling bitcode.
	// MT1xxx	file copy / symlinks (project related)
	//			MT10xx	installer.cs / mtouch.cs
	//					MT1001	Could not find an application at the specified directory: {0}
	//					MT1002	<unused>
	//					MT1003	Could not kill the application '{0}'. You may have to kill the application manually.
	//					MT1004	Could not get the list of installed applications.
	//					MT1005	Could not kill the application '{0}' on the device '{1}': {2}. You may have to kill the application manually.
	//					MT1006	Could not install the application '{0}' on the device '{1}': {2}.
	//					MT1007	Failed to launch the application '{0}' on the device '{1}': {2}. You can still launch the application manually by tapping on it.
	//					MT1008	Failed to launch the simulator: {0}
	//					MT1009	Could not copy the assembly '{0}' to '{1}': {2}
	//					MT1010	Could not load the assembly '{0}': {1}
	//					MT1011	Could not add missing resource file: '{0}'
	//					MT1012	Failed to list the apps on the device '{0}': {1}
	//					MT1013  Dependency tracking error: no files to compare. Please file a bug report at http://bugzilla.xamarin.com with a test case.
	//					MT1014  Failed to re-use cached version of '{0}': {1}.
	//					MT1015	Failed to create the executable '{0}': {1}
	//					MT1016	Failed to create the NOTICE file because a directory already exists with the same name.
	//					MT1017	Failed to create the NOTICE file: {0}
	//					MT1018	Your application failed code-signing checks and could not be installed on the device '{0}'. Check your certificates, provisioning profiles, and bundle ids. Probably your device is not part of the selected provisioning profile (error: 0xe8008015).
	//					MT1019	Your application has entitlements not supported by your current provisioning profile and could not be installed on the device '{0}'. Please check the iOS Device Log for more detailed information (error: 0xe8008016).
	//					MT1020	Failed to launch the application '{0}' on the device '{1}': {2}
	//					MT1021	Could not copy the file '{0}' to '{1}': {2}
	//					MT1022	Could not copy the directory '{0}' to '{1}': {2}
	//					MT1023	Could not communicate with the device to find the application '{0}' : {1}
	//					MT1024	The application signature could not be verified on device '{0}'. Please make sure that the provisioning profile is installed and not expired (error: 0xe8008017).
	//					MT1025	Could not list the crash reports on the device {0}.
	//					MT1026	Could not download the crash report {1} from the device {0}.
	//					MT1027	Can't use Xcode 7+ to launch applications on devices with iOS {0} (Xcode 7 only supports iOS 6+).
	//					MT1028	Invalid device specification: '{0}'. Expected 'ios', 'watchos' or 'all'.
	//					MT1029  Could not find an application at the specified directory: {0}
	//					MT1030	Launching applications on device using a bundle identifier is deprecated. Please pass the full path to the bundle to launch.
	//					MT1031	Could not launch the app '{0}' on the device '{1}' because the device is locked. Please unlock the device and try again.
	//		Warning			MT1032	This application executable might be too large ({0} MB) to execute on device. If bitcode was enabled you might want to disable it for development, it is only required to submit applications to Apple.
	//					MT1033	Could not uninstall the application '{0}' from the device '{1}': {2}
	//			MT11xx	DebugService.cs
	//					MT1101	Could not start app: {0}
	//					MT1102	Could not attach to the app (to kill it): {0}
	//					MT1103	Could not detach
	//					MT1104	Failed to send packet: {0}
	//					MT1105	Unexpected response type
	//					MT1106	Could not get list of applications on the device: Request timed out.
	//					MT1107	Application failed to launch: {0}
	//					MT1108	Could not find developer tools for this {0} ({1}) device. Please ensure you are using a compatible Xcode version and then connect this device to Xcode to install the development support files.
	//					MT1109	Application failed to launch because the device is locked. Please unlock the device and try again.
	//					MT1110	Application failed to launch because of iOS security restrictions. Please ensure the developer is trusted.
	//			MT12xx	simcontroller.cs
	//					MT1201	Could not load the simulator: {0}
	//					MT1202	Invalid simulator configuration: {0}
	//					MT1203	Invalid simulator specification: {0}
	//					MT1204	Invalid simulator specification '{0}': runtime not specified.
	//					MT1205	Invalid simulator specification '{0}': device type not specified.
	//					MT1206	Could not find the simulator runtime '{0}'.
	//					MT1207	Could not find the simulator device type '{0}'.
	//					MT1208	Could not find the simulator runtime '{0}'.
	//					MT1209	Could not find the simulator device type '{0}'.
	//					MT1210	Invalid simulator specification: {0}, unknown key '{1}'
	//					MT1211	The simulator version '{0}' does not support the simulator type '{1}'
	//					MT1212	Failed to create a simulator version where type = {0} and runtime = {1}.
	//					MT1213	<unused>
	//					MT1214	<unused>
	//					MT1215	<unused>
	//					MT1216	Could not find the simulator UDID '{0}'.
	//					MT1217	Could not load the app bundle at '{0}'.
	//					MT1218	No bundle identifier found in the app at '{0}'.
	//					MT1219	Could not find the simulator for '{0}'.
	//					MT1220	Could not find the latest simulator runtime for device '{0}'.
	//					MT1221	Could not find the paired iPhone simulator for the WatchOS simulator '{0}'.
	//			MT13xx	[LinkWith]
	//					MT1301  <unused>
	//					MT1302  Could not extract the native library '{0}' from '{1}'. Please ensure the native library was properly embedded in the managed assembly (if the assembly was built using a binding project, the native library must be included in the project, and its Build Action must be 'ObjcBindingNativeLibrary').
	//					MT1303	Could not decompress the native framework '{0}' from '{1}'. Please review the build log for more information from the native 'zip' command.
	//					MT1304	The embedded framework '{0}' in {1} is invalid: it does not contain an Info.plist.
	//					MT1305	The binding library '{0}' contains a user framework ({0}), but embedded user frameworks require iOS 8.0 (the current deployment target is {0}). Please set the deployment target in the Info.plist file to at least 8.0.
	//			MT14xx	CrashReports.cs
	//					MT1400	Could not open crash report service: AFCConnectionOpen returned {0}
	//					MT1401	Could not close crash report service: AFCConnectionClose returned {0}
	//					MT1402	Could not read file info for {0}: AFCFileInfoOpen returned {1}
	//					MT1403	Could not read crash report: AFCDirectoryOpen ({0}) returned: {1}
	//					MT1404	Could not read crash report: AFCFileRefOpen ({0}) returned: {1}
	//					MT1405	Could not read crash report: AFCFileRefRead ({0}) returned: {1}
	//					MT1406	Could not list crash reports: AFCDirectoryOpen ({0}) returned: {1}
	//			MT16xx	MachO.cs
	//					MT1600	Not a Mach-O dynamic library (unknown header '0x{0}'): {1}.
	//					MT1601	Not a static library (unknown header '{0}'): {1}.
	//					MT1602	Not a Mach-O dynamic library (unknown header '0x{0}'): {1}.
	//					MT1603	Unknown format for fat entry at position {0} in {1}.
	//					MT1604	File of type {0} is not a MachO file ({1}).
	// MT2xxx	Linker
	//			MT20xx	Linker (general) errors
	//					MT2001	Could not link assemblies
	//					MT2002	Can not resolve reference: {0}
	//					MT2003	Option '{0}' will be ignored since linking is disabled
	//					MT2004	Extra linker definitions file '{0}' could not be located.
	//					MT2005	Definitions from '{0}' could not be parsed.
	//					MT2006	Can not load mscorlib.dll from: '{0}'. Please reinstall Xamarin.iOS.
	//					MT2007	** reserved Xamarin.Mac **
	//					MT2009  ** reserved Xamarin.Mac **
	//					MT2010	Unknown HttpMessageHandler `{0}`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler
	//					MT2011	Unknown TlsProvider `{0}`.  Valid values are default or appletls.
	//					MT2012  ** reserved Xamarin.Mac **
	//					MT2013	** reserved Xamarin.Mac **
	//					MT2014	** reserved Xamarin.Mac **
	//		Warning		MT2015	Invalid HttpMessageHandler `{0}` for watchOS. The only valid value is NSUrlSessionHandler.
	//		Warning		MT2016  Invalid TlsProvider `{0}` option. The only valid value `{1}` will be used.
	//					MT2017  Could not process XML description: {0}
	//					MT2018	The assembly '{0}' is referenced from two different locations: '{1}' and '{2}'.
	//					MT2019	Can not load the root assembly '{0}'.
	//					MT202x	Binding Optimizer failed processing `...`.
	//					MT203x	Removing User Resources failed processing `...`.
	//					MT204x	Default HttpMessageHandler setter failed processing `...`.
	//					MT205x	Code Remover failed processing `...`.
	//					MT206x	Sealer failed processing `...`.
	//					MT207x	Metadata Reducer failed processing `...`.
	//					MT208x	MarkNSObjects failed processing `...`.
	//					MT209x	Inliner failed processing `...`.
	// MT3xxx	AOT
	//			MT30xx	AOT (general) errors
	//					MT3001	Could not AOT the assembly '{0}'
	//					MT3002	AOT restriction: Method '{0}' must be static since it is decorated with [MonoPInvokeCallback]. See http://ios.xamarin.com/Documentation/Limitations#Reverse_Callbacks # this error message comes from the AOT compiler
	//					MT3003	Conflicting --debug and --llvm options. Soft-debugging is disabled.
	//					MT3004  Could not AOT the assembly '{0}' because it doesn't exist.
	//					MT3005  The dependency '{0}' of the assembly '{1}' was not found. Please review the project's references.
	//					MT3006  Could not compute a complete dependency map for the project. This will result in slower build times because Xamarin.iOS can't properly detect what needs to be rebuilt (and what does not need to be rebuilt). Please review previous warnings for more details.
	//		Warning		MT3007	Debug info files (*.mdb) will not be loaded when llvm is enabled.
	//					MT3008	Bitcode support requires the use of the LLVM AOT backend (--llvm)
	//					MM3009	** reserved Xamarin.Mac **
	//					MM3010	** reserved Xamarin.Mac **
	// MT4xxx	code generation
	// 			MT40xx	main.m
	//					MT4001	The main template could not be expanded to `{0}`.
	//					MT4002	Failed to compile the generated code for P/Invoke methods. Please file a bug report at http://bugzilla.xamarin.com
	//			MT41xx	registrar.m
	//					MT4101	The registrar cannot build a signature for type `{0}`.
	//					MT4102	The registrar found an invalid type `{0}` in signature for method `{2}`. Use `{1}` instead.
	//					MT4103	The registrar found an invalid type `{0}` in signature for method `{2}`: The type implements INativeObject, but does not have a constructor that takes two (IntPtr, bool) arguments
	//					MT4104	The registrar cannot marshal the return value for type `{0}` in signature for method `{1}`.
	//					MT4105	The registrar cannot marshal the parameter of type `{0}` in signature for method `{1}`.
	//					MT4106	The registrar cannot marshal the return value for structure `{0}` in signature for method `{1}`.
	//					MT4107	The registrar cannot marshal the parameter of type `{0}` in signature for method `{1}`.
	//					MT4108	The registrar cannot get the ObjectiveC type for managed type `{0}`."
	//					MT4109	Failed to compile the generated registrar code. Please file a bug report at http://bugzilla.xamarin.com
	//					MT4110	The registrar cannot marshal the out parameter of type `{0}` in signature for method `{1}`.
	//					MT4111	The registrar cannot build a signature for type `{0}' in method `{1}`.
	//					MT4112  The registrar found a generic type: {0}. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information).
	//					MT4113	he registrar found a generic method: '{0}.{1}'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information
	//					MT4114	Unexpected error in the registrar for the method '{0}.{1}' - Please file a bug report at http://bugzilla.xamarin.com
	//					MT4116	Could not register the assembly '{0}': {1}
	//					MT4117  The registrar found a signature mismatch in the method '{0}.{1}' - the selector indicates the method takes {2} parameters, while the managed method has {3} parameters.
	//					MT4118  Cannot register two managed types ('{1}' and '{2}') with the same native name ('{0}').
	//					MT4119  Could not register the selector '{0}' of the member '{1}.{2}' because the selector is already registered on a different member.
	//					MT4120	The registrar found an unknown field type '{0}' in field '{1}.{2}'. Please file a bug report at http://bugzilla.xamarin.com
	//					MT4121	Cannot use GCC/G++ to compile the generated code from the static registrar when using the Accounts framework (the header files provided by Apple used during the compilation require Clang). Either use Clang (--compiler:clang) or the dynamic registrar (--registrar:dynamic).
	//					MT4122	Cannot use the Clang compiler provided in the {0}.{1} SDK to compile the generated code from the static registrar when non-ASCII type names ('{1}') are present in the application. Either use GCC/G++ (--compiler:gcc|g++), the dynamic registrar (--registrar:dynamic) or a newer SDK.
	//					MT4123	The type of the variadic parameter in the variadic function '{0}' must be System.IntPtr.
	//					MT4124	Invalid {1} found on '{0}'. Please file a bug report at http://bugzilla.xamarin.com
	//					MT4125  The registrar found an invalid type '{0}' in signature for method '{1}': The interface must have a Protocol attribute specifying its wrapper type.
	//					MT4126  Cannot register two managed protocols ('{1}' and '{2}') with the same native name ('{0}').
	//					MT4127	Cannot register more than one interface method for the method '{0}' (which is implementing '{1}').
	//					MT4128  The registrar found an invalid generic parameter type '{0}' in the method '{1}'. The generic parameter must have an 'NSObject' constraint.
	//					MT4129	The registrar found an invalid generic return type '{0}' in the method '{1}'. The generic return type must have an 'NSObject' constraint.
	//					MT4130  The registrar cannot export static methods in generic classes ('{0}').
	//					MT4131  The registrar cannot export static properties in generic classes ('{0}.{1}').
	//					MT4132	The registrar found an invalid generic return type '{0}' in the property '{1}'. The return type must have an 'NSObject' constraint.
	//					MT4133  Cannot construct an instance of the type '{0}' from Objective-C because the type is generic. [Runtime exception]
	//					MT4134	Your application is using the '{0}' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS {2}, while you're building with the iOS {1} SDK.) Please select a newer SDK in your app's iOS Build options.
	//					MT4135	The member '{0}.{1}' has an Export attribute that doesn't specify a selector. A selector is required.
	//					MT4136	The registrar cannot marshal the parameter type '{0}' of the parameter '{1}' in the method '{2}.{3}'
	//					MT4137	<unused>
	//					MT4138	The registrar cannot marshal the property type '{0}' of the property '{1}.{2}'.
	//					MT4139	The registrar cannot marshal the property type '{0}' of the property '{1}.{2}'. Properties with the [Connect] attribute must have a property type of NSObject (or a subclass of NSObject).
	//					MT4140  The registrar found a signature mismatch in the method '{0}.{1}' - the selector indicates the variadic method takes {2} parameters, while the managed method has {3} parameters.
	//					MT4141	Cannot register the selector '{0}' on the member '{1}.{2}' because Xamarin.iOS implicitly registers this selector.
	//					MT4142	Failed to register the type '{0}'
	//					MT4143	The ObjectiveC class '{0}' could not be registered, it does not seem to derive from any known ObjectiveC class (including NSObject).
	//					MT4144	Cannot register the method '{0}.{1}' since it does not have an associated trampoline. Please file a bug report at http://bugzilla.xamarin.com
	//					MT4145	Invalid enum '{0}': enums with the [Native] attribute must have a underlying enum type of either 'long' or 'ulong'.
	//					MT4147	Detected a protocol inheriting from the JSExport protocol while using the dynamic registrar. It is not possible to export protocols to JavaScriptCore dynamically; the static registrar must be used (add '--registrar:static to the additional mtouch arguments in the project's iOS Build options to select the static registrar).
	//					MT4148	The registrar found a generic protocol: '{0}'. Exporting generic protocols is not supported.
	//					MT4149	Cannot register the method '{0}.{1}' because the type of the first parameter ('{2}') does not match the category type ('{3}').
	//					MT4150	Cannot register the type '{0}' because the Type property ('{1}') in its Category attribute does not inherit from NSObject.
	//					MT4151	Cannot register the type '{0}' because the Type property in its Category attribute isn't set.
	//					MT4152	Cannot register the type '{0}' as a category because it implements INativeObject or subclasses NSObject.
	//					MT4153	Cannot register the type '{0}' as a category because it's generic.
	//					MT4154	Cannot register the method '{0}.{1}' as a category method because it's generic.
	//					MT4155	Cannot register the method '{0}.{1}' with the selector '{2}' as a category method on '{3}' because Objective-C already has an implementation for this selector.
	//					MT4156	Cannot register two categories ('{0}' and '{1}') with the same native name ('{2}').
	//					MT4157	Cannot register the category method '{0}.{1}' because at least one parameter is required (and its type must match the category type '{2}')
	//					MT4158	Cannot register the constructor {0} in the category {1} because constructors in categories are not supported.
	//					MT4159	Cannot register the method '{0}.{1}' as a category method because category methods must be static.
	//					MT4160	Invalid character '{0}' (0x{1}) found in selector '{2}' for '{3}.{4}'.
	//					MT4161	The registrar found an unsupported structure '{0}': All fields in a structure must also be structures (field '{1}' with type '{2}' is not a structure).
	//					MT4162	The type '{0}' (used as {1} {2}) is not available in {3} {4} (it was introduced in {3} {5}){6} Please build with a newer iOS SDK (usually done by using the most recent version of Xcode.
	//					MT4163	Internal error in the registrar ({0}). Please file a bug report at http://bugzilla.xamarin.com
	//					MT4164	Cannot export the property '{0}' because its selector '{1}' is an Objective-C keyword. Please use a different name.
	//					MT4165	The registrar couldn't find the type 'System.Void' in any of the referenced assemblies.
	//					MT4166	Cannot register the method '{0}' because the signature contains a type ({1}) that isn't a reference type.
	//					MT4167	Cannot register the method '{0}' because the signature contains a generic type ({1}) with a generic argument type that isn't an NSObject subclass ({2}).
	// MT5xxx	GCC and toolchain
	//			MT51xx	compilation
	//					MT5101	Missing '{0}' compiler. Please install Xcode 'Command-Line Tools' component
	//					MT5102	Failed to assemble the file '{0}'. Please file a bug report at http://bugzilla.xamarin.com
	//					MT5103	Failed to compile the file '{0}'. Please file a bug report at http://bugzilla.xamarin.com
	//					MT5104  Could not find neither the '{0}' nor the '{1}' compiler. Please install Xcode 'Command-Line Tools' component
	//					MT5105  ** reserved Xamarin.Mac **
	//					MT5106	Could not compile the file(s) '{0}'. Please file a bug report at http://bugzilla.xamarin.com
	//			MT52xx	linking
	//					MT5201	Native linking failed. Please review the build log and the user flags provided to gcc: {0}
	//					MT5202	Native linking failed. Please review the build log.
	//					MT5203	Native linking warning: {0}
	//					MT5203-5207 is used below
	//					MT5209	Native linking error: {0}
	//					MT5210	Native linking failed, undefined symbol: {0}. Please verify that all the necessary frameworks have been referenced and native libraries are properly linked in.
	//					MT5211	Native linking failed, undefined Objective-C class: {0}. The symbol '{1}' could not be found in any of the libraries or frameworks linked with your application.
	//					MT5212	Native linking failed, duplicate symbol: {0}.
	//					MT5213	Duplicate symbol in: {0} (Location related to previous error)
	//					MT5214	Native linking failed, undefined symbol: {0}. This symbol was referenced by the managed member {1}.{2}. Please verify that all the necessary frameworks have been referenced and native libraries linked.
	//		Warning		MT5215	References to '{0}' might require additional -framework=XXX or -lXXX instructions to the native linker
	//					MT5216	Native linking failed for {0}. Please file a bug report at http://bugzilla.xamarin.com
	//			MT53xx	other tools
	//					MT5301	Missing 'strip' tool. Please install Xcode 'Command-Line Tools' component
	//					MT5302	Missing 'dsymutil' tool. Please install Xcode 'Command-Line Tools' component
	//					MT5303	Failed to generate the debug symbols (dSYM directory). Please review the build log.
	//					MT5304	Failed to strip the final binary. Please review the build log.
	//					MT5305  Missing 'lipo' tool. Please install Xcode 'Command-Line Tools' component
	//					MT5306  Failed to create the a fat library. Please review the build log.
	//					MT5307  <unused>
	//					MT5308  ** reserved Xamarin.Mac **
	//					MT5309  ** reserved Xamarin.Mac **
	//					MT5310  ** reserved Xamarin.Mac **
	// MT6xxx	mtouch internal tools
	//			MT600x	Stripper
	//					MT6001	Running version of Cecil doesn't support assembly stripping
	//					MT6002	Could not strip assembly `{0}`.
	//					MT6003  [UnauthorizedAccessException message]
	// MT7xxx	msbuild reserved
	//			MT700x	misc
	//					MT7001	Could not resolve host IPs for WiFi debugger settings.
	//					MT7002	This machine does not have any network adapters. This is required when debugging or profiling on device over WiFi.
	//					MT7003	The App Extension '*' does not contain an Info.plist.
	//					MT7004	The App Extension '*' does not specify a CFBundleIdentifier.
	//					MT7005	The App Extension '*' does not specify a CFBundleExecutable.
	//					MT7006	The App Extension '*' has an invalid CFBundleIdentifier (*), it does not begin with the main app bundle's CFBundleIdentifier (*).
	//					MT7007	The App Extension '*' has a CFBundleIdentifier (*) that ends with the illegal suffix ".key".
	//					MT7008	The App Extension '*' does not specify a CFBundleShortVersionString.
	//					MT7009	The App Extension '*' has an invalid Info.plist: it does not contain an NSExtension dictionary.
	//					MT7010	The App Extension '*' has an invalid Info.plist: the NSExtension dictionary does not contain an NSExtensionPointIdentifier value.
	//					MT7011	The WatchKit Extension '*' has an invalid Info.plist: the NSExtension dictionary does not contain an NSExtensionAttributes dictionary.
	//					MT7012	The WatchKit Extension '*' does not have exactly one watch app.
	//					MT7013	The WatchKit Extension '*' has an invalid Info.plist: UIRequiredDeviceCapabilities must contain the 'watch-companion' capability.
	//					MT7014	The Watch App '*' does not contain an Info.plist.
	//					MT7015	The Watch App '*' does not specify a CFBundleIdentifier.
	//					MT7016	The Watch App '*' has an invalid CFBundleIdentifier (*), it does not begin with the main app bundle's CFBundleIdentifier (*).
	//					MT7017	The Watch App '*' does not have a valid UIDeviceFamily value. Expected 'Watch (4)' but found '* (*)'.
	//					MT7018	The Watch App '*' does not specify a CFBundleExecutable
	//					MT7019	The Watch App '*' has an invalid WKCompanionAppBundleIdentifier value ('*'), it does not match the main app bundle's CFBundleIdentifier ('*').
	//					MT7020	The Watch App '*' has an invalid Info.plist: the WKWatchKitApp key must be present and have a value of 'true'.
	//					MT7021	The Watch App '*' has an invalid Info.plist: the LSRequiresIPhoneOS key must not be present.
	//					MT7022	The Watch App '*' does not contain a Watch Extension.
	//					MT7023	The Watch Extension '*' does not contain an Info.plist.
	//					MT7024	The Watch Extension '*' does not specify a CFBundleIdentifier.
	//					MT7025	The Watch Extension '*' does not specify a CFBundleExecutable.
	//					MT7026	The Watch Extension '*' has an invalid CFBundleIdentifier (*), it does not begin with the main app bundle's CFBundleIdentifier (*).
	//					MT7027	The Watch Extension '*' has a CFBundleIdentifier (*) that ends with the illegal suffix ".key".
	//					MT7028	The Watch Extension '*' has an invalid Info.plist: it does not contain an NSExtension dictionary.
	//					MT7029	The Watch Extension '*' has an invalid Info.plist: the NSExtensionPointIdentifier must be "com.apple.watchkit".
	//					MT7030	The Watch Extension '*' has an invalid Info.plist: the NSExtension dictionary must contain NSExtensionAttributes.
	//					MT7031	The Watch Extension '*' has an invalid Info.plist: the NSExtensionAttributes dictionary must contain a WKAppBundleIdentifier.
	//					MT7032	The WatchKit Extension '*' has an invalid Info.plist: UIRequiredDeviceCapabilities should not contain the 'watch-companion' capability.
	//					MT7033	The Watch App '*' does not contain an Info.plist.
	//					MT7034	The Watch App '*' does not specify a CFBundleIdentifier.
	//					MT7035	The Watch App '*' does not have a valid UIDeviceFamily value. Expected '*' but found '* (*)'.
	//					MT7036	The Watch App '*' does not specify a CFBundleExecutable.
	//					MT7037	The WatchKit Extension '{extensionName}' has an invalid WKAppBundleIdentifier value ('*'), it does not match the Watch App's CFBundleIdentifier ('*').
	//					MT7038	The Watch App '*' has an invalid Info.plist: the WKCompanionAppBundleIdentifier must exist and must match the main app bundle's CFBundleIdentifier.
	//					MT7039	The Watch App '*' has an invalid Info.plist: the LSRequiresIPhoneOS key must not be present.
	//					MT7040	The app bundle {AppBundlePath} does not contain an Info.plist.
	//					MT7041	Main Info.plist path does not specify a CFBundleIdentifier.
	//					MT7042	Main Info.plist path does not specify a CFBundleExecutable.
	//					MT7043	Main Info.plist path does not specify a CFBundleSupportedPlatforms.
	//					MT7044	Main Info.plist path does not specify a UIDeviceFamily.
	//					MT7045	Unrecognized Type: *.
	//					MT7046	Add: Entry, *, Incorrectly Specified.
	//					MT7047	Add: Entry, *, Contains Invalid Array Index.
	//					MT7048	Add: * Entry Already Exists.
	//					MT7049	Add: Can't Add Entry, *, to Parent.
	//					MT7050	Delete: Can't Delete Entry, *, from Parent.
	//					MT7051	Delete: Entry, *, Contains Invalid Array Index.
	//					MT7052	Delete: Entry, *, Does Not Exist.
	//					MT7053	Import: Entry, *, Incorrectly Specified.
	//					MT7054	Import: Entry, *, Contains Invalid Array Index.
	//					MT7055	Import: Error Reading File: *.
	//					MT7056	Import: Can't Add Entry, *, to Parent.
	//					MT7057	Merge: Can't Add array Entries to dict.
	//					MT7058	Merge: Specified Entry Must Be a Container.
	//					MT7059	Merge: Entry, *, Contains Invalid Array Index.
	//					MT7060	Merge: Entry, *, Does Not Exist.
	//					MT7061	Merge: Error Reading File: *.
	//					MT7062	Set: Entry, *, Incorrectly Specified.
	//					MT7063	Set: Entry, *, Contains Invalid Array Index.
	//					MT7064	Set: Entry, *, Does Not Exist.
	//					MT7065	Unknown PropertyList editor action: *.
	//					MT7066	Error loading '*': *.
	//					MT7067	Error saving '*': *.
	// MT8xxx	runtime
	//			MT800x	misc
	//					MT8001	Version mismatch between the native Xamarin.iOS runtime and monotouch.dll. Please reinstall Xamarin.iOS.
	//					MT8002	Could not find the method '{0}' in the type '{1}'.
	//					MT8003	Failed to find the closed generic method '{0}' on the type '{1}'.
	//					MT8004	Cannot create an instance of {0} for the native object 0x{1} (of type '{2}'), because another instance already exists for this native object (of type {3}).
	//					MT8005	Wrapper type '{0}' is missing its native ObjectiveC class '{1}'.
	//					MT8006	Failed to find the selector '{0}' on the type '{1}'
	//					MT8007	Cannot get the method descriptor for the selector '{0}' on the type '{1}', because the selector does not correspond to a method
	//					MT8008	The loaded version of Xamarin.iOS.dll was compiled for {0} bits, while the process is {1} bits. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8009	Unable to locate the block to delegate conversion method for the method {0}.{1}'s parameter #{2}. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8010	Native type size mismatch between Xamarin.[iOS|Mac].dll and the executing architecture. Xamarin.[iOS|Mac].dll was built for {0}-bit, while the current process is {1}-bit.
	//					MT8011	Unable to locate the delegate to block conversion attribute ([DelegateProxy]) for the return value for the method {0}.{1}. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8012	Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: DelegateType is null. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8013	Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: DelegateType ({2}) specifies a type without a 'Handler' field. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8014	Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: The DelegateType's ({2}) 'Handler' field is null. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8015	Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: The DelegateType's ({2}) 'Handler' field is not a delegate, it's a {3}. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8016	Unable to convert delegate to block for the return value for the method {0}.{1}, because the input isn't a delegate, it's a {1}. Please file a bug at http://bugzilla.xamarin.com.
	//					MT8017	** reserved Xamarin.Mac **
	//					MT8018	Internal consistency error. Please file a bug report at http://bugzilla.xamarin.com.
	//					MT8019	Could not find the assembly {0} in the loaded assemblies.
	//					MT8020	Could not find the module with MetadataToken {0} in the assembly {1}.
	//					MT8021	Unknown implicit token type: 0x{0}.
	//					MT8022	Expected the token reference 0x{0} to be a {1}, but it's a {2}. Please file a bug report at http://bugzilla.xamarin.com.
	//					MT8023	An instance object is required to construct a closed generic method for the open generic method: {0}.{1} (token reference: 0x{2}). Please file a bug report at http://bugzilla.xamarin.com.
	// MT9xxx	Licensing
	//

	public class MonoTouchException : Exception {
		
		public MonoTouchException (int code, string message, params object[] args) : 
			this (code, false, message, args)
		{
		}

		public MonoTouchException (int code, bool error, string message, params object[] args) : 
			this (code, error, null, message, args)
		{
		}

		public MonoTouchException (int code, bool error, Exception innerException, string message, params object[] args) : 
			base (String.Format (message, args), innerException)
		{
			Code = code;
			Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
		}
	
		public string FileName { get; set; }

		public int LineNumber { get; set; }

		public int Code { get; private set; }
		
		public bool Error { get; private set; }
		
		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		public override string ToString ()
		{
			if (string.IsNullOrEmpty (FileName)) {
				return String.Format ("{0} MT{1:0000}: {2}", Error ? "error" : "warning", Code, Message);
			} else {
				return String.Format ("{3}({4}): {0} MT{1:0000}: {2}", Error ? "error" : "warning", Code, Message, FileName, LineNumber);
			}
		}
	}
}
