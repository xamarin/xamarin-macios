using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xamarin.iOS.Tasks {
	public class DataSet {
		[JsonProperty ("data")]
		public IEnumerable<DataItem> DataItems { get; set; }

		//This stores the properties we don't need to deserialize but exist, just to avoid loosing information
		[JsonExtensionData]
		IDictionary<string, JToken> JsonData;
	}
}
