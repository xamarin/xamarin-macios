#if XAMCORE_2_0 || !MONOMAC
using System;
using ObjCRuntime;
using Vector2i = global::OpenTK.Vector2i;

namespace ModelIO {

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	public enum  MDLNoiseTextureType {
		Vector,
		Cellular,
	}

	public partial class MDLNoiseTexture {
		[iOS (9,0), Mac (10,11, onlyOn64 : true)]
		public MDLNoiseTexture (float input, string name, Vector2i textureDimensions, MDLTextureChannelEncoding channelEncoding) : this (input, name, textureDimensions, channelEncoding, MDLNoiseTextureType.Vector)
		{
		}

		[iOS (10,2), Mac (10,12, onlyOn64 : true)]
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
#endif