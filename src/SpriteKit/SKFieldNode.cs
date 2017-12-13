// 
// SKFieldNode.cs: SKFieldNode class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.SpriteKit {
#if !XAMCORE_3_0 && !MONOMAC
	public partial class SKFieldNode  {
		
		[Obsolete ("Use the method 'CreateVortexField' instead.")]
		public static SKFieldNode CraeteVortexField ()
		{
			return CreateVortexField ();
		}
	}
#endif
}

