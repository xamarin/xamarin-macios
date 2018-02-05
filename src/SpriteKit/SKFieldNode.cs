// 
// SKFieldNode.cs: SKFieldNode class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;

namespace SpriteKit {
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

