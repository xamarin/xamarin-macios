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

#if NET
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos12.3")]
	[SupportedOSPlatform ("maccatalyst15.4")]
#else
	[NoiOS]
	[NoTV]
	[NoWatch]
	[Mac (12,3)]
	[MacCatalyst (15,4)]
#endif
	public enum SCContentFilterOption {
		Include,
		Exclude,
	}

	public partial class SCContentFilter {

		public SCContentFilter (SCDisplay display, SCWindow [] windows, SCContentFilterOption option) : base (NSObjectFlag.Empty)
		{
			switch (option) {
			case SCContentFilterOption.Include:
				Handle = InitWithDisplayIncludingWindows (display, windows);
				break;
			case SCContentFilterOption.Exclude:
				Handle = InitWithDisplayExcludingWindows (display, windows);
				break;
			default:
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (option));
				break;
			};
		}

		public SCContentFilter (SCDisplay display, SCRunningApplication [] applications, SCWindow [] exceptingWindows, SCContentFilterOption option) : base (NSObjectFlag.Empty)
		{
			switch (option) {
			case SCContentFilterOption.Include:
				Handle = InitWithDisplayIncludingApplications (display, applications, exceptingWindows);
				break;
			case SCContentFilterOption.Exclude:
				Handle = InitWithDisplayExcludingApplications (display, applications, exceptingWindows);
				break;
			default:
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (option));
				break;
			};
		}
	}
}
