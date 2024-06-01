#if !__MACCATALYST__

using System;

using Foundation;
using ObjCRuntime;

#nullable enable

#if NET
namespace AppKit {
	public partial interface INSPasteboardReading {
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static T? CreateInstance<T> (NSObject propertyList, NSPasteboardType type) where T: NSObject, INSPasteboardReading
		{
			return CreateInstance<T> (propertyList, type.GetConstant ()!);
		}
	}
}
#endif // NET
#endif // !__MACCATALYST__
