#nullable enable
using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace Foundation {

#if !NET
	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
#else
	[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos10.15")]
#endif
	public partial class NSOrderedCollectionDifference
	{

		NSObject[] Insertions => NSArray.ArrayFromHandle<NSObject> (_Insertions);

		NSObject[] Removals => NSArray.ArrayFromHandle<NSObject> (_Removals);

	}
}
