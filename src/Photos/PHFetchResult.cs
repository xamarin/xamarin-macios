using ObjCRuntime;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Photos
{
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class PHFetchResult : IEnumerable<NSObject>
	{
		public NSObject this[nint index] {
			get { return _ObjectAtIndexedSubscript (index); }
		}

		public IEnumerator<NSObject> GetEnumerator ()
		{
			nint len = Count;

			for (nint i = 0; i < len; i++)
				yield return this [i];
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			nint len = Count;

			for (nint i = 0; i < len; i++)
				yield return this [i];
		}

		public T [] ObjectsAt<T> (NSIndexSet indexes) where T : NSObject
		{
			var nsarr = _ObjectsAt (indexes);
			return NSArray.ArrayFromHandle<T> (nsarr);
		}
	}
}
