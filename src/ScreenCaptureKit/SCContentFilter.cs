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
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoiOS]
	[NoTV]
	[NoWatch]
	[Mac (12, 3)]
	[NoMacCatalyst]
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
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (option), $"Unknown option {option}");
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
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (option), $"Unknown option {option}");
				break;
			};
		}
	}
}
