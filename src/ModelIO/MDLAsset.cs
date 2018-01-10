#if XAMCORE_2_0 || !MONOMAC
using System;
namespace ModelIO {
	public partial class MDLAsset {
		public MDLObject this [nuint index] {
			get {
				return GetObject (index);
			}
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the overload that takes an 'MDLLightProbeIrradianceDataSource' instead.")]
		public static MDLLightProbe[] PlaceLightProbes (float density, MDLProbePlacement type, MDLLightProbeIrradianceDataSource dataSource)
		{
			return PlaceLightProbes (density, type, (IMDLLightProbeIrradianceDataSource) dataSource);
		}
#endif
	}
}
#endif