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
using XamCore.Foundation;

namespace XamCore.UIKit {

	public static class UIDragDropSessionExtensions {

		public static NSProgress LoadObjects (this IUIDropSession session, Type type, Action<INSItemProviderReading []> completion)
		{
			return session.LoadObjects (new Class (type), completion);
		}

		public static bool CanLoadObjects (this IUIDragDropSession session, Type type)
		{
			return session.CanLoadObjects (new Class (type));
		}
	}
}

#endif