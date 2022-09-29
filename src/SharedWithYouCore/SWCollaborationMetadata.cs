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
using CoreGraphics;
using CoreFoundation;

#if NET
	[UnsupportedOSPlatform ("watchos")]
	[UnsupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos13.0")]
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst16.0")]
#else
	[NoWatch, NoTV, Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
#endif
namespace SharedWithYouCore {
	public enum SWIdentifierType {
		Local,
		Collaboration,
	}

	public partial class SWCollaborationMetadata {
		public SWCollaborationMetadata (string localIdentifier) : base (NSObjectFlag.Empty) =>
			InitializeHandle (_InitWithLocalIdentifier (localIdentifier), "initWithLocalIdentifier:");

#if NET
		[SupportedOSPlatform ("ios16.1")]
		[SupportedOSPlatform ("maccatalyst16.1")]
#else
		[iOS (16,1), MacCatalyst (16,1)]
#endif  
		public SWCollaborationMetadata (string identifier, SWIdentifierType identifierType) : base (NSObjectFlag.Empty) => identifierType switch
		{
			SWIdentifierType.Local => InitializeHandle (_InitWithLocalIdentifier (identifier), "initWithLocalIdentifier:");
			SWIdentifierType.Collaboration => InitializeHandle (_InitWithCollaborationIdentifier (identifier), "initWithCollaborationIdentifier:");
			_ => ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (identifierType), $"Unknown identifier type: {identifierType}");
		}
	}
}
