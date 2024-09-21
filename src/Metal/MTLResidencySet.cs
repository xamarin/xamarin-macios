using System;

using ObjCRuntime;

#nullable enable

namespace Metal {

	public partial interface IMTLResidencySet {

		/// <summary>Adds allocations to be committed the next time <see cref="Commit" /> is called.</summary>
		/// <param name="allocations">The allocations to add.</param>
		public void AddAllocations (params IMTLAllocation [] allocations)
		{
			NativeObjectExtensions.CallWithPointerToFirstElementAndCount (allocations, nameof (allocations), AddAllocations);
		}

		/// <summary>Marks allocations to be removed the next time <see cref="Commit" /> is called.</summary>
		/// <param name="allocations">The allocations to remove.</param>
		public void RemoveAllocations (params IMTLAllocation [] allocations)
		{
			NativeObjectExtensions.CallWithPointerToFirstElementAndCount (allocations, nameof (allocations), RemoveAllocations);
		}
	}
}
