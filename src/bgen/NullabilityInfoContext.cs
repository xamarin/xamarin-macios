using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#nullable enable

#if NET
namespace bgen {

	public static class NullabilityInfoExtensions {
		public static bool IsNullable (this NullabilityInfo self)
			=> self.ReadState == NullabilityState.Nullable;
	}
}
#endif
