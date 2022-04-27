using System;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	public class TypeNotFoundEventArgs : EventArgs {
		public TypeNotFoundEventArgs (string originalType)
		{
			OriginalType = originalType;
		}
		public string OriginalType { get; init; }
	}

	public class TypeFoundEventArgs : TypeNotFoundEventArgs {
		public TypeFoundEventArgs (string originalType, string mappedType)
			: base (originalType)
		{
			MappedType = mappedType;
		}

		public string MappedType { get; init; }
	}
}
