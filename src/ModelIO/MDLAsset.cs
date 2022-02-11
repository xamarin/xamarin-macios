#nullable enable

using System;
using System.Runtime.Versioning;
namespace ModelIO {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class MDLAsset {
		public MDLObject this [nuint index] {
			get {
				return GetObject (index);
			}
		}

#if !NET
		[Obsolete ("Use the overload that takes an 'MDLLightProbeIrradianceDataSource' instead.")]
		public static MDLLightProbe[] PlaceLightProbes (float density, MDLProbePlacement type, MDLLightProbeIrradianceDataSource dataSource)
		{
			return PlaceLightProbes (density, type, (IMDLLightProbeIrradianceDataSource) dataSource);
		}
#endif
	}
}
