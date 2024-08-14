using System;

using ObjCRuntime;

#nullable enable

namespace Metal {

	public partial interface IMTLCommandQueue {

		/// <summary>Marks the specified residency sets as part of the current command buffer execution.</summary>
		/// <param name="residencySets">The residency sets to mark.</param>
		public void AddResidencySets (params IMTLResidencySet [] residencySets)
		{
			NativeObjectExtensions.CallWithPointerToFirstElementAndCount (residencySets, nameof (residencySets), AddResidencySets);
		}

		/// <summary>Removes the specified residency sets from the current command buffer execution.</summary>
		/// <param name="residencySets">The residency sets to mark.</param>
		public void RemoveResidencySets (params IMTLResidencySet [] residencySets)
		{
			NativeObjectExtensions.CallWithPointerToFirstElementAndCount (residencySets, nameof (residencySets), RemoveResidencySets);
		}
	}
}
