using System;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	public class PropertyNotFoundEventArgs : EventArgs {
		public PropertyNotFoundEventArgs (string originalProperty)
		{
			OriginalProperty = originalProperty;
		}
		public string OriginalProperty { get; init; }
	}

	public class PropertyFoundEventArgs : PropertyNotFoundEventArgs {
		public PropertyFoundEventArgs (string originalProperty, string mappedProperty)
			: base (originalProperty)
		{
			MappedProperty = mappedProperty;
		}
		public string MappedProperty { get; init; }
	}
}
