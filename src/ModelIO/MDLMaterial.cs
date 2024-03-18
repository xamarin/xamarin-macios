using System;

#nullable enable

namespace ModelIO {
	public partial class MDLMaterial {
		public MDLMaterialProperty? this [nuint index] {
			get {
				return ObjectAtIndexedSubscript (index);
			}
		}

		public MDLMaterialProperty? this [string name] {
			get {
				return ObjectForKeyedSubscript (name);
			}
		}
	}
}
