// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using Foundation;
using ObjCRuntime;

namespace NS {
	[BaseType (typeof (NSObject))]
	interface PriorityClass {
		[OverloadResolutionPriority (-1)]
		[Export ("oldMethod")]
		void OldMethod ();
	}
}
