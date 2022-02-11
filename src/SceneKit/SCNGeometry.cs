//
// SCNGeometry.cs: extensions to SCNGeometry
//
// Authors:
//   MIguel de Icaza (miguel@xamarin.com)
//
// Copyright Xamarin Inc
//
using System;

using CoreGraphics;
using Foundation;
using System.Runtime.Versioning;

#nullable enable

namespace SceneKit {
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class SCNGeometry {
#if !XAMCORE_3_0
		[Obsolete ("Use the 'Create (SCNGeometrySource[], SCNGeometryElement[])' method instead, as it has a strongly typed return.")]
		public static NSObject FromSources (SCNGeometrySource [] sources, SCNGeometryElement [] elements)
		{
			return Create (sources, elements);
		}
#endif
	}
}
