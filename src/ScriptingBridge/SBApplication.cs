#nullable enable

using System;
using System.Reflection;

using AppKit;

using Foundation;

using ObjCRuntime;

namespace ScriptingBridge {
	public partial class SBApplication {
		public static SBApplication? GetApplication (string ident) => Runtime.GetNSObject<SBApplication> (_FromBundleIdentifier (ident));

		public static T? GetApplication<T> (string ident) where T : SBApplication => Runtime.GetINativeObject<T> (_FromBundleIdentifier (ident), forced_type: true, owns: false);

		public static SBApplication? GetApplication (NSUrl url) => Runtime.GetNSObject<SBApplication> (_FromURL (url));

		public static T? GetApplication<T> (NSUrl url) where T : SBApplication => Runtime.GetINativeObject<T> (_FromURL (url), forced_type: true, owns: false);

		public static SBApplication? GetApplication (int pid) => Runtime.GetNSObject<SBApplication> (_FromProcessIdentifier (pid));

		public static T? GetApplication<T> (int pid) where T : SBApplication => Runtime.GetINativeObject<T> (_FromProcessIdentifier (pid), forced_type: true, owns: false);

#if !NET
		[Obsolete ("Use 'GetApplication' instead.")]
		public static SBApplication? FromBundleIdentifier (string ident) => Runtime.GetNSObject<SBApplication> (_FromBundleIdentifier (ident));

		[Obsolete ("Use 'GetApplication' instead.")]
		public static T? FromBundleIdentifier<T> (string ident) where T : SBApplication => Runtime.GetINativeObject<T> (_FromBundleIdentifier (ident), forced_type: true, owns: false);

		[Obsolete ("Use 'GetApplication' instead.")]
		public static SBApplication? FromURL (NSUrl url) => Runtime.GetNSObject<SBApplication> (_FromURL (url));

		[Obsolete ("Use 'GetApplication' instead.")]
		public static T? FromURL<T> (NSUrl url) where T : SBApplication => Runtime.GetINativeObject<T> (_FromURL (url), forced_type: true, owns: false);

		[Obsolete ("Use 'GetApplication' instead.")]
		public static SBApplication? FromProcessIdentifier (int pid) => Runtime.GetNSObject<SBApplication> (_FromProcessIdentifier (pid));

		[Obsolete ("Use 'GetApplication' instead.")]
		public static T? FromProcessIdentifier<T> (int pid) where T : SBApplication => Runtime.GetINativeObject<T> (_FromProcessIdentifier (pid), forced_type: true, owns: false);
#endif
	}
}
