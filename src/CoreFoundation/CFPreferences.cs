//
// CFPreferences.cs: binding for CFPreferences
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin, Inc.
//
// 64-bit audit complete
//
// Note: currently only the "high level" API is bound

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using System.Runtime.Versioning;

namespace CoreFoundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class CFPreferences {
		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFPreferencesCopyAppValue (IntPtr key, IntPtr applicationId);

		public static readonly NSString? CurrentApplication;

		/*public static readonly NSString AnyApplication;
		public static readonly NSString CurrentHost;
		public static readonly NSString AnyHost;
		public static readonly NSString CurrentUser;
		public static readonly NSString AnyUser;*/

		static CFPreferences ()
		{
			var handle = Libraries.CoreFoundation.Handle;
			CurrentApplication = Dlfcn.GetStringConstant (handle, "kCFPreferencesCurrentApplication");

			/*AnyApplication = Dlfcn.GetStringConstant (handle, "kCFPreferencesAnyApplication");
			CurrentHost = Dlfcn.GetStringConstant (handle, "kCFPreferencesCurrentHost");
			AnyHost = Dlfcn.GetStringConstant (handle, "kCFPreferencesAnyHost");
			CurrentUser = Dlfcn.GetStringConstant (handle, "kCFPreferencesCurrentUser");
			AnyUser = Dlfcn.GetStringConstant (handle, "kCFPreferencesAnyUser");*/
		}

		public static object? GetAppValue (string key)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			return GetAppValue (key, CurrentApplication);
		}

		public static object? GetAppValue (string key, string applicationId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				return GetAppValue (key, nsApplicationId);
			}
		}

		public static object? GetAppValue (string key, NSString applicationId)
		{
			if (key is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			} else if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			IntPtr valuePtr;

			using (var cfKey = new CFString (key)) {
				valuePtr = CFPreferencesCopyAppValue (cfKey.Handle, applicationId.Handle);
			}

			if (valuePtr == IntPtr.Zero) {
				return null;
			}

			using (var plist = new CFPropertyList (valuePtr, true)) {
				return plist.Value;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFPreferencesSetAppValue (IntPtr key, IntPtr value, IntPtr applicationId);

		public static void SetAppValue (string key, object value)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			SetAppValue (key, value, CurrentApplication);
		}

		public static void SetAppValue (string key, object? value, string applicationId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				SetAppValue (key, value, nsApplicationId);
			}
		}

		public static void SetAppValue (string key, object? value, NSString applicationId)
		{
			if (key is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			} else if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var cfKey = new CFString (key)) {
				if (value is null) {
					CFPreferencesSetAppValue (cfKey.Handle, IntPtr.Zero, applicationId.Handle);
					return;
				} else if (value is string) {
					using (var valueStr = new CFString ((string) value)) {
						CFPreferencesSetAppValue (cfKey.Handle, valueStr.Handle, applicationId.Handle);
					}

					return;
				} else if (value is NSString || value is CFString ||
					value is NSData || value is CFData ||
					value is NSArray || value is CFArray ||
					value is NSDictionary || value is CFDictionary ||
					value is NSNumber || value is CFBoolean) {
					CFPreferencesSetAppValue (cfKey.Handle, ((INativeObject) value).Handle, applicationId.Handle);
					return;
				}

				var nsnumber = NSNumber.FromObject (value);
				if (nsnumber is not null) {
					using (nsnumber) {
						CFPreferencesSetAppValue (cfKey.Handle, nsnumber.Handle, applicationId.Handle);
					}
					return;
				}

				throw new ArgumentException ("unsupported type: " + value.GetType (), "value");
			}
		}

		public static void RemoveAppValue (string key)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			SetAppValue (key, null, CurrentApplication);
		}

		public static void RemoveAppValue (string key, string applicationId)
		{
			SetAppValue (key, null, applicationId);
		}

		public static void RemoveAppValue (string key, NSString applicationId)
		{
			SetAppValue (key, null, applicationId);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CFPreferencesGetAppBooleanValue (IntPtr key, IntPtr applicationId,
			/*out bool*/ IntPtr keyExistsAndHasValidFormat);

		public static bool GetAppBooleanValue (string key)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			return GetAppBooleanValue (key, CurrentApplication);
		}

		public static bool GetAppBooleanValue (string key, string applicationId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				return GetAppBooleanValue (key, nsApplicationId);
			}
		}

		public static bool GetAppBooleanValue (string key, NSString applicationId)
		{
			if (key is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			} else if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var cfKey = new CFString (key)) {
				return CFPreferencesGetAppBooleanValue (cfKey.Handle, applicationId.Handle, IntPtr.Zero);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern nint CFPreferencesGetAppIntegerValue (IntPtr key, IntPtr applicationId,
			/*out bool*/ IntPtr keyExistsAndHasValidFormat);

		public static nint GetAppIntegerValue (string key)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			return GetAppIntegerValue (key, CurrentApplication);
		}

		public static nint GetAppIntegerValue (string key, string applicationId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				return GetAppIntegerValue (key, nsApplicationId);
			}
		}

		public static nint GetAppIntegerValue (string key, NSString applicationId)
		{
			if (key is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			} else if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var cfKey = new CFString (key)) {
				return CFPreferencesGetAppIntegerValue (cfKey.Handle, applicationId.Handle, IntPtr.Zero);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFPreferencesAddSuitePreferencesToApp (IntPtr applicationId, IntPtr suiteId);

		public static void AddSuitePreferencesToApp (string suiteId)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			AddSuitePreferencesToApp (CurrentApplication, suiteId);
		}

		public static void AddSuitePreferencesToApp (string applicationId, string suiteId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				AddSuitePreferencesToApp (nsApplicationId, suiteId);
			}
		}

		public static void AddSuitePreferencesToApp (NSString applicationId, string suiteId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			} else if (suiteId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (suiteId));
			}

			using (var cfSuiteId = new CFString (suiteId)) {
				CFPreferencesAddSuitePreferencesToApp (applicationId.Handle, cfSuiteId.Handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFPreferencesRemoveSuitePreferencesFromApp (IntPtr applicationId, IntPtr suiteId);

		public static void RemoveSuitePreferencesFromApp (string suiteId)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			RemoveSuitePreferencesFromApp (CurrentApplication, suiteId);
		}

		public static void RemoveSuitePreferencesFromApp (string applicationId, string suiteId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				RemoveSuitePreferencesFromApp (nsApplicationId, suiteId);
			}
		}

		public static void RemoveSuitePreferencesFromApp (NSString applicationId, string suiteId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			} else if (suiteId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (suiteId));
			}

			using (var cfSuiteId = new CFString (suiteId)) {
				CFPreferencesRemoveSuitePreferencesFromApp (applicationId.Handle, cfSuiteId.Handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CFPreferencesAppSynchronize (IntPtr applicationId);

		public static bool AppSynchronize ()
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			return AppSynchronize (CurrentApplication);
		}

		public static bool AppSynchronize (string applicationId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				return AppSynchronize (nsApplicationId);
			}
		}

		public static bool AppSynchronize (NSString applicationId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			return CFPreferencesAppSynchronize (applicationId.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CFPreferencesAppValueIsForced (IntPtr key, IntPtr applicationId);

		public static bool AppValueIsForced (string key)
		{
			if (CurrentApplication is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (CurrentApplication));
			return AppValueIsForced (key, CurrentApplication);
		}

		public static bool AppValueIsForced (string key, string applicationId)
		{
			if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var nsApplicationId = new NSString (applicationId)) {
				return AppValueIsForced (key, nsApplicationId);
			}
		}

		public static bool AppValueIsForced (string key, NSString applicationId)
		{
			if (key is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			} else if (applicationId is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationId));
			}

			using (var cfKey = new CFString (key)) {
				return CFPreferencesAppValueIsForced (cfKey.Handle, applicationId.Handle);
			}
		}
	}
}
