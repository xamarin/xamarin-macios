// Copyright 2022 Microsoft Corporation.

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreGraphics;

namespace CoreFoundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	// The name prefix suggests CoreGraphics and based on CF_DEFINES_CGAFFINETRANSFORMCOMPONENTS
	// it could be defined in CoreGraphics but documented as CoreFoundation type
	public struct CGAffineTransformComponents {
		public CGSize Scale;

		public nfloat HorizontalShear;

		public nfloat Rotation;

		public CGVector Translation;
	}
}
