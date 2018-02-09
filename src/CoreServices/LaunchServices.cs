//
// LaunchServices.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
// NOTE: intentionally passing IntPtr.Zero to all
// 'out NSError' APIs since errors return NULL anyway,
// and the NSError objects are specified to be a
// constant error object (from the docs).
// 
// In other words, A NULL return value implies
// ApplicationNotFoundso we just drop the
// 'out NSError' parameter to make the API nicer.
//
// NOTE: only bound APIs not deprecated in 10.11
//
// NOTE: KEEP IN SYNC WITH TESTS!

#if MONOMAC

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace CoreServices
{
	[Flags]
	public enum LSRoles/*Mask*/ : uint /* always 32-bit uint */
	{
		None = 1,
		Viewer = 2,
		Editor = 4,
		Shell = 8,
		All = 0xffffffff
	}

	[Flags]
	public enum LSAcceptanceFlags : uint /* always 32-bit uint */
	{
		Default = 1,
		AllowLoginUI = 2
	}

	public enum LSResult
	{
		Success = 0,
		AppInTrash = -10660,
		ExecutableIncorrectFormat = -10661,
		AttributeNotFound = -10662,
		AttributeNotSettable = -10663,
		IncompatibleApplicationVersion = -10664,
		NoRosettaEnvironment = -10665,
		Unknown = -10810,
		NotAnApplication = -10811,
		NotInitialized = -10812,
		DataUnavailable = -10813,
		ApplicationNotFound = -10814,
		UnknownType = -10815,
		DataTooOld = -10816,
		Data = -10817,
		LaunchInProgress = -10818,
		NotRegistered = -10819,
		AppDoesNotClaimType = -10820,
		AppDoesNotSupportSchemeWarning = -10821,
		ServerCommunication = -10822,
		CannotSetInfo = -10823,
		NoRegistrationInfo = -10824,
		IncompatibleSystemVersion = -10825,
		NoLaunchPermission = -10826,
		NoExecutable = -10827,
		NoClassicEnvironment = -10828,
		MultipleSessionsNotSupported = -10829
	}

	public static class LaunchServices
	{
		#region Locating an Application

		[Mac (10, 10)]
		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyDefaultApplicationURLForURL (IntPtr inUrl, LSRoles inRole, /*out*/ IntPtr outError);

		[Mac (10, 10)]
		public static NSUrl GetDefaultApplicationUrlForUrl (NSUrl url, LSRoles roles = LSRoles.All)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			return Runtime.GetNSObject<NSUrl> (
				LSCopyDefaultApplicationURLForURL (url.Handle, roles, IntPtr.Zero)
			);
		}

		[Mac (10, 10)]
		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyDefaultApplicationURLForContentType (IntPtr inContentType, LSRoles inRole, /*out*/ IntPtr outError);

		[Mac (10, 10)]
		public static NSUrl GetDefaultApplicationUrlForContentType (string contentType, LSRoles roles = LSRoles.All)
		{
			if (contentType == null)
				throw new ArgumentNullException (nameof (contentType));

			return Runtime.GetNSObject<NSUrl> (
				LSCopyDefaultApplicationURLForContentType (new NSString (contentType).Handle, roles, IntPtr.Zero)
			);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyApplicationURLsForURL (IntPtr inUrl, LSRoles inRole);

		public static NSUrl [] GetApplicationUrlsForUrl (NSUrl url, LSRoles roles = LSRoles.All)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			return NSArray.ArrayFromHandle<NSUrl> (
				LSCopyApplicationURLsForURL (url.Handle, roles)
			);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern LSResult LSCanURLAcceptURL (IntPtr inItemUrl, IntPtr inTargetUrl,
			LSRoles inRole, LSAcceptanceFlags inFlags, out byte outAcceptsItem);

		// NOTE: intentionally inverting the status results (return bool, with an out
		// LSResult vs return LSResult with an out bool) to make the API nicer to use
		public static bool CanUrlAcceptUrl (NSUrl itemUrl, NSUrl targetUrl,
			LSRoles roles, LSAcceptanceFlags acceptanceFlags, out LSResult result)
		{
			if (itemUrl == null)
				throw new ArgumentNullException (nameof (itemUrl));
			if (targetUrl == null)
				throw new ArgumentNullException (nameof (targetUrl));

			byte acceptsItem;
			result = LSCanURLAcceptURL (itemUrl.Handle, targetUrl.Handle, roles, acceptanceFlags, out acceptsItem);
			return acceptsItem != 0;
		}

		public static bool CanUrlAcceptUrl (NSUrl itemUrl, NSUrl targetUrl,
			LSRoles roles = LSRoles.All, LSAcceptanceFlags acceptanceFlags = LSAcceptanceFlags.Default)
		{
			LSResult result;
			return CanUrlAcceptUrl (itemUrl, targetUrl, roles, acceptanceFlags, out result);
		}

		[Mac (10, 10)]
		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyApplicationURLsForBundleIdentifier (IntPtr inBundleIdentifier, /*out*/ IntPtr outError);

		[Mac (10, 10)]
		public static NSUrl [] GetApplicationUrlsForBundleIdentifier (string bundleIdentifier)
		{
			if (bundleIdentifier == null)
				throw new ArgumentNullException (nameof (bundleIdentifier));

			return NSArray.ArrayFromHandle<NSUrl> (
				LSCopyApplicationURLsForBundleIdentifier (new NSString (bundleIdentifier).Handle, IntPtr.Zero)
			);
		}

		#endregion

		#region Opening Items

		[DllImport (Constants.CoreServicesLibrary)]
		unsafe static extern LSResult LSOpenCFURLRef (IntPtr inUrl, void **outLaunchedUrl);

		public unsafe static LSResult Open (NSUrl url)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			return LSOpenCFURLRef (url.Handle, (void **)0);
		}

		public unsafe static LSResult Open (NSUrl url, out NSUrl launchedUrl)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			void *launchedUrlHandle;
			var result = LSOpenCFURLRef (url.Handle, &launchedUrlHandle);
			launchedUrl = Runtime.GetNSObject<NSUrl> (new IntPtr (launchedUrlHandle));
			return result;
		}

		#endregion

		#region Registering an Application

		[DllImport (Constants.CoreServicesLibrary)]
		static extern LSResult LSRegisterURL (IntPtr inUrl, byte inUpdate);

		public static LSResult Register (NSUrl url, bool update)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			return LSRegisterURL (url.Handle, (byte)(update ? 1 : 0));
		}

		#endregion

		#region Working with Role Handlers

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyAllRoleHandlersForContentType (IntPtr inContentType, LSRoles inRole);

		public static string [] GetAllRoleHandlersForContentType (string contentType, LSRoles roles = LSRoles.All)
		{
			if (contentType == null)
				throw new ArgumentNullException (nameof (contentType));

			return NSArray.StringArrayFromHandle (
				LSCopyAllRoleHandlersForContentType (new NSString (contentType).Handle, roles)
			);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyDefaultRoleHandlerForContentType (IntPtr inContentType, LSRoles inRole);

		public static string GetDefaultRoleHandlerForContentType (string contentType, LSRoles roles = LSRoles.All)
		{
			if (contentType == null)
				throw new ArgumentNullException (nameof (contentType));

			return (string)Runtime.GetNSObject<NSString> (
				LSCopyDefaultRoleHandlerForContentType (new NSString (contentType).Handle, roles)
			);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern LSResult LSSetDefaultRoleHandlerForContentType (IntPtr inContentType,
			LSRoles inRole, IntPtr inHandlerBundleID);

		// NOTE: intentionally swapped handlerBundleId and roles parameters for a nicer API
		public static LSResult SetDefaultRoleHandlerForContentType (string contentType, string handlerBundleId,
			LSRoles roles = LSRoles.All)
		{
			if (contentType == null)
				throw new ArgumentNullException (nameof (contentType));
			if (handlerBundleId == null)
				throw new ArgumentNullException (nameof (handlerBundleId));

			return LSSetDefaultRoleHandlerForContentType (
				new NSString (contentType).Handle,
				roles,
				new NSString (handlerBundleId).Handle
			);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyAllHandlersForURLScheme (IntPtr inUrlScheme);

		public static string [] GetAllHandlersForUrlScheme (string urlScheme)
		{
			if (urlScheme == null)
				throw new ArgumentNullException (nameof (urlScheme));

			return NSArray.StringArrayFromHandle (
				LSCopyAllHandlersForURLScheme (new NSString (urlScheme).Handle)
			);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr LSCopyDefaultHandlerForURLScheme (IntPtr inUrlScheme);

		public static string GetDefaultHandlerForUrlScheme (string urlScheme)
		{
			if (urlScheme == null)
				throw new ArgumentNullException (nameof (urlScheme));

			return (string)Runtime.GetNSObject<NSString> (
				LSCopyDefaultHandlerForURLScheme (new NSString (urlScheme).Handle)
			);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern LSResult LSSetDefaultHandlerForURLScheme (IntPtr inUrlScheme, IntPtr inHandlerBundleId);

		public static LSResult SetDefaultHandlerForUrlScheme (string urlScheme, string handlerBundleId)
		{
			if (urlScheme == null)
				throw new ArgumentNullException (nameof (urlScheme));
			if (handlerBundleId == null)
				throw new ArgumentNullException (nameof (handlerBundleId));

			return LSSetDefaultHandlerForURLScheme (
				new NSString (urlScheme).Handle,
				new NSString (handlerBundleId).Handle
			);
		}

		#endregion
	}
}

#endif // MONOMAC
