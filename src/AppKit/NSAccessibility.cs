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

using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit
{
	[Mac (10,10)]
	[Native]
	public enum NSAccessibilityOrientation : nint
	{
		Unknown = 0,
		Vertical = 1,
		Horizontal = 2
	}

	[Mac (10,10)]
	[Native]
	public enum NSAccessibilitySortDirection : nint
	{
		Unknown = 0,
		Ascending = 1,
		Descending = 2
	}

	[Mac (10,10)]
	[Native]
	public enum NSAccessibilityRulerMarkerType : nint
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
	public enum NSAccessibilityUnits : nint
	{
		Unknown = 0,
		Inches = 1,
		Centimeters = 2,
		Points = 3,
		Picas = 4
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
	}
#endif
}
