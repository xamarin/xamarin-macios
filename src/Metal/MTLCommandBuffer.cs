using System;

using ObjCRuntime;

#nullable enable

namespace Metal {

	public partial interface IMTLCommandBuffer {

		/// <summary>Marks the specified residency sets as part of the current command buffer execution.</summary>
		public void UseResidencySets (IMTLResidencySet [] residencySets)
		{
			NativeObjectExtensions.CallWithPointerToFirstElementAndCount (residencySets, nameof (residencySets), UseResidencySets);
		}
	}
}
