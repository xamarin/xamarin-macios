using System;
using ObjCRuntime;

#if NET
using Vector2i = global::CoreGraphics.NVector2i;
#else
using Vector2i = global::OpenTK.Vector2i;
#endif

#nullable enable

namespace ModelIO {

	public partial class MDLNoiseTexture {

		public MDLNoiseTexture (float input, string name, Vector2i textureDimensions, MDLTextureChannelEncoding channelEncoding) : this (input, name, textureDimensions, channelEncoding, MDLNoiseTextureType.Vector)
		{
		}

#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (10,2)]
		[Mac (10,12)]
#endif
		public MDLNoiseTexture (float input, string name, Vector2i textureDimensions, MDLTextureChannelEncoding channelEncoding, MDLNoiseTextureType type)
		{
			// two different `init*` would share the same C# signature
			switch (type) {
			case MDLNoiseTextureType.Vector:
				Handle = InitVectorNoiseWithSmoothness (input, name, textureDimensions, channelEncoding);
				break;
			case MDLNoiseTextureType.Cellular:
				Handle = InitCellularNoiseWithFrequency (input, name, textureDimensions, channelEncoding);
				break;
			default:
				throw new ArgumentException ("type");
			}
		}
	}
}
