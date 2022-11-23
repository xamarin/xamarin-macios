#if !WATCH && !__MACCATALYST__

using System;
using ObjCRuntime;

#nullable enable

namespace SceneKit {
	public partial class SCNRenderingOptions {
		public SCNRenderingApi? RenderingApi {
			get {
				var val = GetNUIntValue (SCNRenderingOptionsKeys.RenderingApiKey);
				if (val is not null)
					return (SCNRenderingApi) (uint) val;
				return null;
			}

			set {
				if (value.HasValue)
					SetNumberValue (SCNRenderingOptionsKeys.RenderingApiKey, (nuint) (uint) value.Value);
				else
					RemoveValue (SCNRenderingOptionsKeys.RenderingApiKey);
			}
		}
	}
}

#endif
