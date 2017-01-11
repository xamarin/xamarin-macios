// copied from MonoTouch mtouch code
// Copyright 2011-2013, Xamarin Inc. All rights reserved,

using System;
using System.Collections.Generic;

namespace Xamarin.Bundler {
	
	// Error allocation
	// 	as far as possible the error # is shared with MonoTouch
	//
	// MM0xxx	mmp itself, e.g. parameters, environment (e.g. missing tools)
	//					MM0000	Unexpected error - Please file a bug report at http://bugzilla.xamarin.com
	//					MM0001	This version of Xamarin.Mac requires Mono {0} (the current Mono version is {1}). Please update the Mono.framework from http://mono-project.com/Downloads
	//					MM0003	Application name '{0}.exe' conflicts with an SDK or product assembly (.dll) name.
	//					MM0007	The root assembly '{0}' does not exist
	//					MM0008	You should provide one root assembly only, found {0} assemblies: '{1}'
	//					MM0010	Could not parse the command line arguments: {0}
	//					MM0013	<unused>
	//					MM0016	The option '{0}' has been deprecated.
	//					MM0017	You should provide a root assembly
	//					MM0018	Unknown command line argument: '{0}'
	//					MM0020	The valid options for '{0}' are '{1}'.
	//					MT0023	Application name '{0}.exe' conflicts with another user assembly.
	//					MM0026  Could not parse the command line argument '{0}': {1}
	//					MM0043	The Boehm garbage collector is not supported. The SGen garbage collector has been selected instead.
	//					MM0050	You cannot provide a root assembly if --no-root-assembly is passed.
	//					MM0051	An output directory (--output) is required if --no-root-assembly is passed.
	//					MM0053	Cannot disable new refcount with the Unified API.
	//					MM0056	Cannot find Xcode in any of our default locations. Please install Xcode, or pass a custom path using --sdkroot=<path>
	//					MM0059	Could not find the currently selected Xcode on the system: {0};
	//					MM0060	Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '{0}', but that directory does not exist.
	//					MM0068	Invalid value for target framework: {0}.
	//					MM0079	Internal Error - No executable was copied into the app bundle. Please contact 'support@xamarin.com'
	//			Warning		MT0080  Disabling NewRefCount, --new-refcount:false, is deprecated.
	//					MM0088	** Reserved mtouch **
	//					MM0089	** Reserved mtouch **
	//					MM0097	machine.config file '{0}' can not be found.
	//					MM0098	AOT compilation is only available on Unified
	// MM1xxx	file copy / symlinks (project related)
	//			MM14xx	Product assemblies
	//					MM1401	The required '{0}' assembly is missing from the references
	//					MM1402	The assembly '{0}' is not compatible with this tool
	//					MM1403	{0} '{1}' could not be found. Target framework '{0}' is unusable to package the application.
	//					MM1404	Target framework '{0}' is invalid.
	//					MM1405	useFullXamMacFramework must always target framework .NET 4.5, not '{0}' which is invalid
	//					MM1406	Target framework '{0}' is invalid when targetting Xamarin.Mac 4.5 .NET framwork.
	//			MM15xx	Assembly gathering (not requiring linker) errors
	//					MM1501	Can not resolve reference: {0}
	//			MT16xx	MachO.cs
	//					MM1600	Not a Mach-O dynamic library (unknown header '0x{0}'): {1}.
	//					MM1601	Not a static library (unknown header '{0}'): {1}.
	//					MM1602	Not a Mach-O dynamic library (unknown header '0x{0}'): {1}.
	//					MM1603	Unknown format for fat entry at position {0} in {1}.
	//					MM1604	File of type {0} is not a MachO file ({1}).
	// MM2xxx	Linker
	//			MM20xx	Linker (general) errors
	//					MM2001	Could not link assemblies
	//					MM2002	Can not resolve reference: {0}
	//					MM2003	Option '{0}' will be ignored since linking is disabled
	//					MM2004	Extra linker definitions file '{0}' could not be located.
	//					MM2005	Definitions from '{0}' could not be parsed.
	//					MM2006	Native library '{0}' was referenced but could not be found.
	//					MM2007  Xamarin.Mac Unified API against a full .NET profile does not support linking. Pass the -nolink flag.
	//					MM2009	  Referenced by {0}.{1}     ** This message is related to MM2006 **
	//					MM2010	Unknown HttpMessageHandler `{0}`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler
	//					MM2011  Unknown TLSProvider `{0}.  Valid values are default or appletls
	//					MM2012   Only first {0} of {1} "Referenced by" warnings shown. ** This message related to 2009 **
	//				Warning	MM2013	Failed to resolve the reference to "{0}", referenced in "{1}". The app will not include the referenced assembly, and may fail at runtime.
	//				Warning	MM2014	Xamarin.Mac Extensions do not support linking. Request for linking will be ignored.
	//					MM2015	*** Reserved mtouch ***
	//				Warning	MM2016  Invalid TlsProvider `{0}` option. The only valid value `{1}` will be used.
	//					MM202x	Binding Optimizer failed processing `...`.
	// MT3xxx	AOT
	//			MT30xx	AOT (general) errors
	//					MT3001	Could not AOT the assembly '{0}'
	//					MM3002	** reserved mtouch **
	//					MM3003	** reserved mtouch **
	//					MM3004	** reserved mtouch **
	//					MM3005	** reserved mtouch **
	//					MM3006	** reserved mtouch **
	//					MM3007	** reserved mtouch **
	//					MM3008	** reserved mtouch **
	//					MM3009	AOT of '{0}' was requested but was not found
	//					MM3010	Exclusion of AOT of '{0}' was requested but was not found
	// MM4xxx	code generation
	// 			MM40xx	driver.m
	//					MM4001	The main template could not be expansed to `{0}`.
	// MM5xxx	GCC and toolchain
	//			MM51xx	compilation
	//					MM5101  Missing '{0}' compiler. Please install Xcode 'Command-Line Tools' component.
	//					MM5102  ** reserved mtouch **
	//					MM5103	Failed to compile. Error code - {0}. Please file a bug report at http://bugzilla.xamarin.com
	//					MM5104  ** reserved mtouch **
	//			MM52xx	linking
	//					MM5202	Mono.framework MDK is missing. Please install the MDK for your Mono.framework version from http://mono-project.com/Downloads
	//					MM5203  Can't find libxammac.a, likely because of a corrupted Xamarin.Mac installation. Please reinstall Xamarin.Mac.
	//					MM5204  Invalid architecture. x86_64 is only supported with the mobile profile.
	//					MM5205  Invalid architecture '{0}'. Valid architectures are i386 and x86_64 (when --profile=mobile).
	//					MM5206	** reserved mtouch **
	//					MM5207	** reserved mtouch **
	//					MM5208	** reserved mtouch **
	//					MM5209	** reserved mtouch **
	//					MM5210	** reserved mtouch **
	//					MM5211	** reserved mtouch **
	//					MM5212	** reserved mtouch **
	//					MM5213	** reserved mtouch **
	//					MM5214	** reserved mtouch **
	//					MM5215	** reserved mtouch **
	//			MM53xx	other tools
	//					MM5301  pkg-config could not be found. Please install the Mono.framework from http://mono-project.com/Downloads
	//					MM5302	** reserved mtouch **
	//					MM5303	** reserved mtouch **
	//					MM5304	** reserved mtouch **
	//					MM5305	Missing 'otool' tool. Please install Xcode 'Command-Line Tools' component
	//					MM5306	Missing dependencies. Please install Xcode 'Command-Line Tools' component
	//					MM5308  Xcode license agreement may not have been accepted.  Please launch Xcode.
	//					MM5309	Native linking failed with error code 1.  Check build log for details.
	//					MT5310  install_name_tool failed with an error code '{0}'. Check build log for details. 
	// MM6xxx	mmp internal tools
	// MM7xxx	reserved
	// MM8xxx	runtime
	//			MM800x	misc
	//					MM8000 - MM8016 ** reserved mtouch **
	//					MM8017	The Boehm garbage collector is not supported. Please use SGen instead.
	// MM9xxx	Licensing

	public class MonoMacException : Exception {
		
		public MonoMacException (int code, string message, params object[] args) : 
			this (code, false, message, args)
		{
		}

		public MonoMacException (int code, bool error, string message, params object[] args) : 
			this (code, error, null, message, args)
		{
		}

		public MonoMacException (int code, bool error, Exception innerException, string message, params object[] args) : 
			base (String.Format (message, args), innerException)
		{
			Code = code;
			Error = error;
		}

		public string FileName { get; set; }

		public int LineNumber { get; set; }

		public int Code { get; private set; }
		
		public bool Error { get; private set; }
		
		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		public override string ToString ()
		{
			if (string.IsNullOrEmpty (FileName)) {
				return String.Format ("{0} MM{1:0000}: {2}", Error ? "error" : "warning", Code, Message);
			} else {
				return String.Format ("{3}({4}): {0} MM{1:0000}: {2}", Error ? "error" : "warning", Code, Message, FileName, LineNumber);
			}
		}
	}
}
