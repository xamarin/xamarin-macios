//
// UIDragDropSessionExtensions.cs
//
// Authors:
//   Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft
//

#if !TVOS && !WATCH

using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.Foundation;

namespace XamCore.UIKit {

	public static class UIDragDropSessionExtensions {

		public static NSProgress LoadObjects<T> (this IUIDropSession session, Action<T []> completion) where T: NSObject, INSItemProviderReading
		{
			return session.LoadObjects (new Class (typeof (T)), (v) =>
			{
				var arr = v as T[];
				if (arr == null && v != null) {
					arr = new T [v.Length];
					for (int i = 0; i < arr.Length; i++) {
						if (v [i] != null)
							arr [i] = Runtime.ConstructNSObject<T> (v [i].Handle);
					}
				}

				completion (arr);
			});
		}

		public static bool CanLoadObjects (this IUIDragDropSession session, Type type)
		{
			return session.CanLoadObjects (new Class (type));
		}
	}
}

#endif