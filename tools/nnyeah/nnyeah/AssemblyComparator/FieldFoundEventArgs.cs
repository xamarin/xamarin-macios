using System;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	public class FieldNotFoundEventArgs : EventArgs {
		public FieldNotFoundEventArgs (string originalField)
		{
			OriginalField = originalField;
		}
		public string OriginalField { get; init; }
	}

	public class FieldFoundEventArgs : FieldNotFoundEventArgs {
		public FieldFoundEventArgs (string originalField, string mappedField)
			: base (originalField)
		{
			MappedField = mappedField;
		}
		public string MappedField { get; init; }
	}
}
