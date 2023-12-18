#if !WATCH && !TVOS

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using AudioToolbox;

#nullable enable

namespace AVFoundation {
	public partial class AVCaptureVideoPreviewLayer {

		public enum InitMode {
			WithConnection,
#if NET
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[SupportedOSPlatform ("maccatalyst")]
#endif
			WithNoConnection,
		}

		public AVCaptureVideoPreviewLayer (AVCaptureSession session, InitMode mode) : base (NSObjectFlag.Empty)
		{
			switch (mode) {
			case InitMode.WithConnection:
				InitializeHandle (InitWithConnection (session));
				break;
			case InitMode.WithNoConnection:
				InitializeHandle (InitWithNoConnection (session));
				break;
			default:
				throw new ArgumentException (nameof (mode));
			}
		}

		public AVCaptureVideoPreviewLayer (AVCaptureSession session) : this (session, InitMode.WithConnection) { }
	}
}

#endif
