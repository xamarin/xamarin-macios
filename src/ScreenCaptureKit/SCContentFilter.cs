//
// SCContentFilter.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace ScreenCaptureKit {
	/// <summary>This enum describes how to interpret some arguments when creating <see cref="SCContentFilter" /> instances.</summary>
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos12.3")]
	[SupportedOSPlatform ("maccatalyst18.2")]
	public enum SCContentFilterOption {
		/// <summary>The specified windows or applications are included in the filter.</summary>
		Include,
		/// <summary>The specified windows or applications are excluded from the filter.</summary>
		Exclude,
	}

	public partial class SCContentFilter {

		/// <summary>Create a new <see cref="SCContentFilter" /> to capture the contents of the specified display, including or excluding specific windows.</summary>
		/// <param name="display">The display to capture.</param>
		/// <param name="windows">The windows to include or exclude.</param>
		/// <param name="option">Whether windows specified in the <paramref name="windows" /> parameter are to be included or excluded.</param>
		public SCContentFilter (SCDisplay display, SCWindow [] windows, SCContentFilterOption option) : base (NSObjectFlag.Empty)
		{
			switch (option) {
			case SCContentFilterOption.Include:
				Handle = _InitWithDisplayIncludingWindows (display, windows);
				break;
			case SCContentFilterOption.Exclude:
				Handle = _InitWithDisplayExcludingWindows (display, windows);
				break;
			default:
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (option), $"Unknown option {option}");
				break;
			};
		}

		/// <summary>Create a new <see cref="SCContentFilter" /> to capture the contents of the specified display, including or excluding specific apps.</summary>
		/// <param name="display">The display to capture.</param>
		/// <param name="applications">The applications to include or exclude.</param>
		/// <param name="exceptingWindows">Any windows that are an exception to the condition to include or include windows.</param>
		/// <param name="option">Whether applications specified in the <paramref name="applications" /> parameter are to be included or excluded.</param>
		public SCContentFilter (SCDisplay display, SCRunningApplication [] applications, SCWindow [] exceptingWindows, SCContentFilterOption option) : base (NSObjectFlag.Empty)
		{
			switch (option) {
			case SCContentFilterOption.Include:
				Handle = _InitWithDisplayIncludingApplications (display, applications, exceptingWindows);
				break;
			case SCContentFilterOption.Exclude:
				Handle = _InitWithDisplayExcludingApplications (display, applications, exceptingWindows);
				break;
			default:
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (option), $"Unknown option {option}");
				break;
			};
		}
	}
}
