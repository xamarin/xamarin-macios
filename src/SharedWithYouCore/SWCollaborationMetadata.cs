//
// Custom methods for SWCollaborationMetadata
//
// Authors:
//	Israel Soto  <issoto@microsoft.com>
//
// Copyright 2022 Microsoft Corporation.
//
#nullable enable

using System;

using Foundation;

using ObjCRuntime;

#if !TVOS && !WATCH

namespace SharedWithYouCore {

#if NET
	[UnsupportedOSPlatform ("watchos")]
	[UnsupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos13.0")]
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst16.0")]
#else
	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
#endif
	public enum SWIdentifierType {
		Local,
		Collaboration,
	}

	public partial class SWCollaborationMetadata : NSObject {
		public SWCollaborationMetadata (string localIdentifier) : base (NSObjectFlag.Empty) =>
			InitializeHandle (_InitWithLocalIdentifier (localIdentifier), "initWithLocalIdentifier:");

#if NET
		[UnsupportedOSPlatform ("watchos")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.1")]
		[SupportedOSPlatform ("maccatalyst16.1")]
#else
		[iOS (16, 1), MacCatalyst (16, 1)]
#endif
		public SWCollaborationMetadata (string identifier, SWIdentifierType identifierType) : base (NSObjectFlag.Empty)
		{
			switch (identifierType) {
			case SWIdentifierType.Local:
				InitializeHandle (_InitWithLocalIdentifier (identifier), "initWithLocalIdentifier:");
				break;
			case SWIdentifierType.Collaboration:
				InitializeHandle (_InitWithCollaborationIdentifier (identifier), "initWithCollaborationIdentifier:");
				break;
			default:
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (identifierType), $"Unknown identifier type: {identifierType}");
				break;
			}
		}
	}
}
#endif
