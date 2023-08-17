using System;
using CoreMedia;
using Foundation;
using ObjCRuntime;

#nullable enable

#if !WATCH

namespace Cinematic {

#if NET
	[UnsupportedOSPlatform ("watchos")]
	[UnsupportedOSPlatform ("tvos17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
#else
	[NoWatch, TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
	public enum CNDecisionIdentifierType {
		Single,
		Group,
	}

	public partial class CNDecision {

		public CNDecision (CMTime time, long detectionID, bool isStrong, CNDecisionIdentifierType identifierType)
		{

			switch (identifierType) {
			case CNDecisionIdentifierType.Single:
				InitializeHandle (_InitWithSingleIdentifier (time, detectionID, isStrong), "initWithTime:detectionID:strong:");
				break;
			case CNDecisionIdentifierType.Group:
				InitializeHandle (_InitWithGroupIdentifier (time, detectionID, isStrong), "initWithTime:detectionGroupID:strong:");
				break;
			default:
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (identifierType), $"Unknown identifier type: {identifierType}");
				break;
			}
		}
	}
}
#endif
