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
	public partial class VTPixelRotationProperties : DictionaryContainer {
		public VTRotation Rotation {
			get => VTRotationExtensions.GetValue (GetNSStringValue (VTPixelRotationPropertyKeys.Rotation)!);
			set => SetStringValue (VTPixelRotationPropertyKeys.Rotation, value.GetConstant ());
		}
	}
}
