//
// SCNSceneRenderer.cs
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreGraphics;

#nullable enable

namespace SceneKit {

#if !XAMCORE_3_0
	public static partial class SCNSceneRenderer_Extensions {
		public static SCNHitTestResult [] HitTest (ISCNSceneRenderer This, CGPoint thePoint, SCNHitTestOptions? options)
		{
			return This.HitTest (thePoint, options?.Dictionary);
		}
	}
#endif
}
