using System;

using ObjCRuntime;

#nullable enable

namespace Metal {

	public partial interface IMTLResidencySet {

		/// <summary>Adds allocations to be committed the next time <see cref="Commit" /> is called.</summary>
		public void AddAllocations (IMTLAllocation [] allocations)
		{
			NativeObjectExtensions.CallWithPointerToFirstElementAndCount (allocations, nameof (allocations), AddAllocations);
		}

		/// <summary>Marks allocations to be removed the next time <see cref="Commit" /> is called.</summary>
		public void RemoveAllocations (IMTLAllocation [] allocations)
		{
			NativeObjectExtensions.CallWithPointerToFirstElementAndCount (allocations, nameof (allocations), RemoveAllocations);
		}
	}
}
