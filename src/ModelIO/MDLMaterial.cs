#nullable enable

namespace ModelIO {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class MDLAsset {
		public MDLMaterialProperty this [nuint index] {
			get {
				return ObjectAtIndexedSubscript (index);
			}
		}

		public MDLMaterialProperty this [string name] {
			get {
				return ObjectForKeyedSubscript (name);
			}
		}
	}
}
