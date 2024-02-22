using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class DataItem {
		[JsonProperty ("universal-type-identifier", NullValueHandling = NullValueHandling.Ignore)]
		public string UniversalTypeIdentifier { get; set; }

		[JsonProperty ("graphicsFeatureSet", NullValueHandling = NullValueHandling.Ignore)]
		public string GraphicsFeatureSet { get; set; }

		[JsonProperty ("memory", NullValueHandling = NullValueHandling.Ignore)]
		public string Memory { get; set; }

		[JsonProperty ("idiom", NullValueHandling = NullValueHandling.Ignore)]
		public string Idiom { get; set; }

		[JsonProperty ("filename", NullValueHandling = NullValueHandling.Ignore)]
		public string Filename { get; set; }

#pragma warning disable 0169 // warning CS0169: The field 'DataItem.UnsupportedData' is never used
		//This stores the Asset Catalogs properties we don't support yet, 
		//by doing this we avoid loosing any change made to the json file outside VS.
		[JsonExtensionData]
		IDictionary<string, JToken> UnsupportedData;
#pragma warning restore 0169
	}
}
