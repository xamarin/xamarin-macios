// 
// VTPixelRotationProperties.cs: Strongly Typed dictionary for VTPixelRotationPropertyKeys 
//
// Authors: Israel Soto (issoto@microsoft.com)
//     
// Copyright 2022 Microsoft Corporation.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;
using CoreVideo;
using AVFoundation;

namespace VideoToolbox {
#if NET
	[SupportedOSPlatform ("macos13.0")]
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst16.0")]
	[SupportedOSPlatform ("watchos9.0")]
	[SupportedOSPlatform ("tvos16.0")]
#else
	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), Watch (9,0), TV (16,0)]
#endif
	public partial class VTPixelRotationProperties : DictionaryContainer {
		public VTRotation Rotation {
			get => VTRotationExtensions.GetValue (GetNSStringValue (VTPixelRotationPropertyKeys.Rotation)!);
			set => SetStringValue (VTPixelRotationPropertyKeys.Rotation, value.GetConstant ());
		}
	}
}
