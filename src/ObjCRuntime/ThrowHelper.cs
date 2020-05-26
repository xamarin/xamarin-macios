using System;
// the linker will remove the attributes
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCRuntime {

	static class ThrowHelper {

		[DoesNotReturn]
		static internal void ThrowObjectDisposedException (object o)
		{
			throw new ObjectDisposedException (o.GetType ().ToString ());
		}
	}
}
