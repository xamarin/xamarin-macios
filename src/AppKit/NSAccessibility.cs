//
// NSAccessibility.cs
//
// Author:
//  Chris Hamons <chris.hamons@xamarin.com>
//
// Copyright 2016 Xamarin Inc. (http://xamarin.com)

#if !__MACCATALYST__

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using CoreFoundation;

using CoreGraphics;

using Foundation;

using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AppKit {
	public partial interface INSAccessibility { }

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public partial class NSAccessibility {
#if !COREBUILD
#if NET
		[SupportedOSPlatform ("macos")]
#endif
		[DllImport (Constants.AppKitLibrary)]
		static extern CGRect NSAccessibilityFrameInView (NativeHandle parentView, CGRect frame);

		public static CGRect GetFrameInView (NSView parentView, CGRect frame)
		{
			return NSAccessibilityFrameInView (parentView.GetHandle (), frame);
		}

#if NET
		[SupportedOSPlatform ("macos")]
#endif
		[DllImport (Constants.AppKitLibrary)]
		static extern CGPoint NSAccessibilityPointInView (NativeHandle parentView, CGPoint point);

		public static CGPoint GetPointInView (NSView parentView, CGPoint point)
		{
			return NSAccessibilityPointInView (parentView.GetHandle (), point);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern void NSAccessibilityPostNotificationWithUserInfo (IntPtr element, IntPtr notification, IntPtr userInfo);

		public static void PostNotification (NSObject element, NSString notification, NSDictionary? userInfo)
		{
			if (element is null)
				throw new ArgumentNullException ("element");

			if (notification is null)
				throw new ArgumentNullException ("notification");

			var userInfoHandle = userInfo.GetHandle ();

			NSAccessibilityPostNotificationWithUserInfo (element.Handle, notification.Handle, userInfoHandle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern void NSAccessibilityPostNotification (IntPtr element, IntPtr notification);

		public static void PostNotification (NSObject element, NSString notification)
		{
			if (element is null)
				throw new ArgumentNullException ("element");

			if (notification is null)
				throw new ArgumentNullException ("notification");

			NSAccessibilityPostNotification (element.Handle, notification.Handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityRoleDescription (IntPtr role, IntPtr subrole);

		public static string? GetRoleDescription (NSString role, NSString? subrole)
		{
			if (role is null)
				throw new ArgumentNullException ("role");

			var subroleHandle = subrole.GetHandle ();

			IntPtr handle = NSAccessibilityRoleDescription (role.Handle, subroleHandle);
			return CFString.FromHandle (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityRoleDescriptionForUIElement (IntPtr element);

		public static string? GetRoleDescription (NSObject element)
		{
			if (element is null)
				throw new ArgumentNullException ("element");

			IntPtr handle = NSAccessibilityRoleDescriptionForUIElement (element.Handle);
			return CFString.FromHandle (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityActionDescription (IntPtr action);

		public static string? GetActionDescription (NSString action)
		{
			if (action is null)
				throw new ArgumentNullException ("action");

			IntPtr handle = NSAccessibilityActionDescription (action.Handle);
			return CFString.FromHandle (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredAncestor (IntPtr element);

		public static NSObject? GetUnignoredAncestor (NSObject element)
		{
			if (element is null)
				throw new ArgumentNullException ("element");

			var handle = NSAccessibilityUnignoredAncestor (element.Handle);
			return Runtime.GetNSObject (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredDescendant (IntPtr element);

		public static NSObject? GetUnignoredDescendant (NSObject element)
		{
			if (element is null)
				throw new ArgumentNullException ("element");

			var handle = NSAccessibilityUnignoredDescendant (element.Handle);

			return Runtime.GetNSObject (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredChildren (IntPtr originalChildren);

		public static NSObject []? GetUnignoredChildren (NSArray originalChildren)
		{
			if (originalChildren is null)
				throw new ArgumentNullException ("originalChildren");

			var handle = NSAccessibilityUnignoredChildren (originalChildren.Handle);

			return NSArray.ArrayFromHandle<NSObject> (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredChildrenForOnlyChild (IntPtr originalChild);

		public static NSObject []? GetUnignoredChildren (NSObject originalChild)
		{
			if (originalChild is null)
				throw new ArgumentNullException ("originalChild");

			var handle = NSAccessibilityUnignoredChildrenForOnlyChild (originalChild.Handle);

			return NSArray.ArrayFromHandle<NSObject> (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool NSAccessibilitySetMayContainProtectedContent ([MarshalAs (UnmanagedType.I1)] bool flag);

		public static bool SetMayContainProtectedContent (bool flag)
		{
			return NSAccessibilitySetMayContainProtectedContent (flag);
		}
#endif
	}
}
#endif // !__MACCATALYST__
