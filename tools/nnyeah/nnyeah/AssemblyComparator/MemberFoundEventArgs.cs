using System;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	public class MethodNotFoundEventArgs : EventArgs {
		public MethodNotFoundEventArgs (string originalMember)
		{
			OriginalMember = originalMember;
		}
		public string OriginalMember { get; init; }
	}

	public class MethodFoundEventArgs : MethodNotFoundEventArgs {
		public MethodFoundEventArgs (string originalMember, string mappedMember)
			: base (originalMember)
		{
			MappedMember = mappedMember;
		}
		public string MappedMember { get; init; }
	}
}
