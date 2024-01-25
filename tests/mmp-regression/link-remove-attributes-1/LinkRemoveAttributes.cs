// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

using AppKit;

// Test
// * application use some attributes that the linker will remove (unneeded at runtime)
// * Application is build in debug mode so the [System.Diagnostics.Debug*] attributes are kept
//
// Requirement
// * Link SDK or Link All must be enabled
// * Debug mode

// removed in release builds only
[assembly: Debuggable (DebuggableAttribute.DebuggingModes.Default)]

[assembly: Foundation.LinkerSafe]

// not removed
[assembly: AssemblyCompany ("Xamarin Inc.")]

namespace Xamarin.Mac.Linker.Test {

	//	[System.MonoDocumentationNote]
	//	[System.MonoExtension]
	//	[System.MonoLimitation]
	//	[System.MonoNotSupported]
	//	[System.MonoTODO]
	[System.Obsolete]
	//	[System.Xml.MonoFIX]

	[ObjCRuntime.Availability]
	[ObjCRuntime.Deprecated (ObjCRuntime.PlatformName.MacOSX)]
	[ObjCRuntime.Obsoleted (ObjCRuntime.PlatformName.MacOSX)]
	[ObjCRuntime.Introduced (ObjCRuntime.PlatformName.MacOSX)]
	[ObjCRuntime.Unavailable (ObjCRuntime.PlatformName.MacOSX)]
	[ObjCRuntime.ThreadSafe]

	[DebuggerDisplay ("")]
	[DebuggerNonUserCode]
	[DebuggerTypeProxy ("")]
	[DebuggerVisualizer ("")]

	// not removed
	[Guid ("20f66ee8-2ddc-4fc3-b690-82e1e43a93c7")]
	class DebugLinkRemoveAttributes {

		// will be removed - but the instance field will be kept
		[Foundation.Preserve]
		// removed in release builds only
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		XmlDocument document;

		[Foundation.Advice ("")]
		// removed in release builds only
		[DebuggerHidden]
		[DebuggerStepperBoundary]
		[DebuggerStepThrough]
		// not removed
		[LoaderOptimization (LoaderOptimization.SingleDomain)]
		static void CheckPresence (ICustomAttributeProvider provider, string typeName, bool expected)
		{
			// the linker should have removed the *attributes* - maybe not the type
			bool success = !expected;
			foreach (var ca in provider.GetCustomAttributes (false)) {
				if (ca.GetType ().FullName.StartsWith (typeName, StringComparison.Ordinal))
					success = expected;
			}
			Test.Log.WriteLine ("{0}\t{1} {2}", success ? "[PASS]" : "[FAIL]", typeName, expected ? "present" : "absent");
		}

#if DEBUG
		const bool debug = true;
#else
		const bool debug = false;
#endif

		static void Main (string [] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			try {
				var type = typeof (DebugLinkRemoveAttributes);
				CheckPresence (type, "System.ObsoleteAttribute", false);
				CheckPresence (type, "Foundation.AdviceAttribute", false);
				CheckPresence (type, "ObjCRuntime.AvailabilityAttribute", false);
				CheckPresence (type, "ObjCRuntime.ThreadSafeAttribute", false);
				CheckPresence (type, "System.Diagnostics.DebuggerDisplay", debug);
				CheckPresence (type, "System.Diagnostics.DebuggerNonUserCode", debug);
				CheckPresence (type, "System.Diagnostics.DebuggerTypeProxy", debug);
				CheckPresence (type, "System.Diagnostics.DebuggerVisualizer", debug);
				CheckPresence (type, "System.Runtime.InteropServices.GuidAttribute", true);

				var assembly = type.Assembly;
				CheckPresence (assembly, "Foundation.LinkerSafeAttribute", false);
				CheckPresence (assembly, "System.Diagnostics.DebuggableAttribute", debug);
				CheckPresence (assembly, "System.Reflection.AssemblyCompanyAttribute", true);

				var method = type.GetMethod ("CheckPresence", BindingFlags.Static | BindingFlags.NonPublic);
				CheckPresence (method, "Foundation.AdviceAttribute", false);
				CheckPresence (method, "System.Diagnostics.DebuggerHiddenAttribute", debug);
				CheckPresence (method, "System.Diagnostics.DebuggerStepperBoundaryAttribute", debug);
				CheckPresence (method, "System.Diagnostics.DebuggerStepThroughAttribute", debug);
				CheckPresence (method, "System.LoaderOptimizationAttribute", true);

				var field = type.GetField ("document", BindingFlags.Instance | BindingFlags.NonPublic);
				CheckPresence (field, "Foundation.PreserveAttribute", false);
				CheckPresence (field, "System.Diagnostics.DebuggerBrowsableAttribute", debug);
			} finally {
				Test.Terminate ();
			}
		}
	}
}
