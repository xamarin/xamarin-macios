// 
// VTProfessionalVideoWorkflow.cs: VideoTools Professional Video Workflow 
//
// Authors: 
//    Israel Soto (issoto@microsoft.com)
// 
// Copyright 2022 Microsoft Corporation.
//

#if __MACOS__

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace VideoToolbox {

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("watchos")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoiOS, NoMacCatalyst, NoWatch, NoTV]
#endif
	public static class VTProfessionalVideoWorkflow {
		[DllImport (Constants.VideoToolboxLibrary, EntryPoint = "VTRegisterProfessionalVideoWorkflowVideoDecoders")]
		public extern static void RegisterVideoDecoders ();

		[DllImport (Constants.VideoToolboxLibrary, EntryPoint = "VTRegisterProfessionalVideoWorkflowVideoEncoders")]
		public extern static void RegisterVideoEncoders ();
	}
}

#endif // __MACOS__
