//
// MPSStateResourceList.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using Foundation;
using Metal;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSStateResourceList {
		public static MPSStateResourceList? Create (params MTLTextureDescriptor [] descriptors)
		{
			if (descriptors is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptors));
			if (descriptors.Length > Messaging.MaxVarArgs + 1)
				throw new ArgumentException ($"Only {Messaging.MaxVarArgs + 1} parameters are currently supported.");

			var firstValue = descriptors [0].GetNonNullHandle ($"{nameof (descriptors)} [0]");
			var varArgs = new IntPtr [descriptors.Length - 1];
			for (int i = 1; i < descriptors.Length; ++i)
				varArgs [i - 1] = descriptors [i].GetNonNullHandle ($"{nameof (descriptors)}[{i}]");

			var handle = Messaging.objc_msgSend_3_vargs (
					class_ptr,
					Selector.GetHandle ("resourceListWithTextureDescriptors:"),
					firstValue,
					varArgs);

			return Runtime.GetNSObject<MPSStateResourceList> (handle);
		}

		public static MPSStateResourceList? Create (params nuint [] bufferSizes)
		{
			if (bufferSizes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bufferSizes));
			if (bufferSizes.Length > Messaging.MaxVarArgs + 1)
				throw new ArgumentException ($"Only {Messaging.MaxVarArgs + 1} parameters are currently supported.");

			var firstValue = (IntPtr) bufferSizes [0];
			var varArgs = new IntPtr [bufferSizes.Length - 1];
			Array.Copy (bufferSizes, 1, varArgs, 0, bufferSizes.Length - 1);

			var handle = Messaging.objc_msgSend_3_vargs (
					class_ptr,
					Selector.GetHandle ("resourceListWithBufferSizes:"),
					firstValue,
					varArgs);

			return Runtime.GetNSObject<MPSStateResourceList> (handle);
		}
	}
}
