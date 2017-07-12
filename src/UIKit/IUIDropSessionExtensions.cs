//
// IUIDropSessionExtensions.cs
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

	public static class IUIDropSessionExtensions {

		public static NSProgress LoadObjects (this IUIDropSession session, Type type, Action<INSItemProviderReading []> completion) {
			return session.LoadObjects (new Class (type), completion);
		}
	}
}

#endif