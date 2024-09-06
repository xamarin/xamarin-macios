using System;

using Foundation;
using ObjCRuntime;

namespace NS {
	unsafe delegate byte* MyCallback (sbyte* a, short* b, ushort* c, int* d, uint* e, long* f, ulong* g, float* h, double* i);

	[BaseType (typeof (NSObject))]
	interface Widget {
		[Export ("foo")]
		[NullAllowed]
		MyCallback Foo { get; set; }
	}
}
