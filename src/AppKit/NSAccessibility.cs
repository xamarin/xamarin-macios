//
// NSAccessibility.cs
//
// Author:
//  Chris Hamons <chris.hamons@xamarin.com>
//
// Copyright 2016 Xamarin Inc. (http://xamarin.com)


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace AppKit
{
	[Mac (10,10)]
	[Native]
	public enum NSAccessibilityOrientation : long
	{
		Unknown = 0,
		Vertical = 1,
		Horizontal = 2
	}

	[Mac (10,10)]
	[Native]
	public enum NSAccessibilitySortDirection : long
	{
		Unknown = 0,
		Ascending = 1,
		Descending = 2
	}

	[Mac (10,10)]
	[Native]
	public enum NSAccessibilityRulerMarkerType : long
	{
		Unknown = 0,
		TabStopLeft = 1,
		TabStopRight = 2,
		TabStopCenter = 3,
		TabStopDecimal = 4,
		IndentHead = 5,
		IndentTail = 6,
		IndentFirstLine = 7
	}

	[Mac (10,10)]
	[Native]
	public enum NSAccessibilityUnits : long
	{
		Unknown = 0,
		Inches = 1,
		Centimeters = 2,
		Points = 3,
		Picas = 4
	}

	[Mac (10,9)]
	[Native]
	public enum NSAccessibilityPriorityLevel : long
	{
		Low = 10,
		Medium = 50,
		High = 90
	}

#if !COREBUILD
	public partial class NSAccessibility
	{
		[Mac (10,10)]
		[DllImport (Constants.AppKitLibrary)]
		static extern CGRect NSAccessibilityFrameInView (NSView parentView, CGRect frame);

		public static CGRect GetFrameInView (NSView parentView, CGRect frame)
		{
			return NSAccessibilityFrameInView (parentView, frame);
		}

		[Mac (10,10)]
		[DllImport (Constants.AppKitLibrary)]
		static extern CGPoint NSAccessibilityPointInView (NSView parentView, CGPoint point);

		public static CGPoint GetPointInView (NSView parentView, CGPoint point)
		{
			return NSAccessibilityPointInView (parentView, point);
		}

		[Mac (10,7)]
		[DllImport (Constants.AppKitLibrary)]
		static extern void NSAccessibilityPostNotificationWithUserInfo (IntPtr element, IntPtr notification, IntPtr userInfo);

		public static void PostNotification (NSObject element, NSString notification, NSDictionary userInfo)
		{
			if (element == null)
				throw new ArgumentNullException ("element");

			if (notification == null)
				throw new ArgumentNullException ("notification");

			IntPtr userInfoHandle;
			if (userInfo == null)
				userInfoHandle = IntPtr.Zero;
			else
				userInfoHandle = userInfo.Handle;

			NSAccessibilityPostNotificationWithUserInfo (element.Handle, notification.Handle, userInfoHandle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern void NSAccessibilityPostNotification (IntPtr element, IntPtr notification);

		public static void PostNotification (NSObject element, NSString notification)
		{
			if (element == null)
				throw new ArgumentNullException ("element");

			if (notification == null)
				throw new ArgumentNullException ("notification");

			NSAccessibilityPostNotification (element.Handle, notification.Handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityRoleDescription (IntPtr role, IntPtr subrole);

		public static string GetRoleDescription (NSString role, NSString subrole)
		{
			if (role == null)
				throw new ArgumentNullException ("role");

			IntPtr subroleHandle;
			if (subrole == null)
				subroleHandle = IntPtr.Zero;
			else
				subroleHandle = subrole.Handle;

			IntPtr handle = NSAccessibilityRoleDescription (role.Handle, subroleHandle);
			return NSString.FromHandle (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityRoleDescriptionForUIElement (IntPtr element);

		public static string GetRoleDescription (NSObject element)
		{
			if (element == null)
				throw new ArgumentNullException ("element");

			IntPtr handle = NSAccessibilityRoleDescriptionForUIElement (element.Handle);
			return NSString.FromHandle (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityActionDescription (IntPtr action);

		public static string GetActionDescription (NSString action)
		{
			if (action == null)
				throw new ArgumentNullException ("action");

			IntPtr handle = NSAccessibilityActionDescription (action.Handle);
			return NSString.FromHandle (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredAncestor (IntPtr element);

		public static NSObject GetUnignoredAncestor (NSObject element)
		{
			if (element == null)
				throw new ArgumentNullException ("element");

			var handle = NSAccessibilityUnignoredAncestor (element.Handle);
			return Runtime.GetNSObject (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredDescendant (IntPtr element);

		public static NSObject GetUnignoredDescendant (NSObject element)
		{
			if (element == null)
				throw new ArgumentNullException ("element");

			var handle = NSAccessibilityUnignoredDescendant (element.Handle);

			return Runtime.GetNSObject (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredChildren (IntPtr originalChildren);

		public static NSObject[] GetUnignoredChildren (NSArray originalChildren)
		{
			if (originalChildren == null)
				throw new ArgumentNullException ("originalChildren");

			var handle = NSAccessibilityUnignoredChildren (originalChildren.Handle);

			return NSArray.ArrayFromHandle<NSObject> (handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		static extern IntPtr NSAccessibilityUnignoredChildrenForOnlyChild (IntPtr originalChild);

		public static NSObject[] GetUnignoredChildren (NSObject originalChild)
		{
			if (originalChild == null)
				throw new ArgumentNullException ("originalChild");

			var handle = NSAccessibilityUnignoredChildrenForOnlyChild (originalChild.Handle);

			return NSArray.ArrayFromHandle<NSObject> (handle);
		}

		[Mac (10, 9)]
		[DllImport (Constants.AppKitLibrary)]
		static extern bool NSAccessibilitySetMayContainProtectedContent (bool flag);

		public static bool SetMayContainProtectedContent (bool flag)
		{
			return NSAccessibilitySetMayContainProtectedContent (flag);
		}
	}
#endif
}
