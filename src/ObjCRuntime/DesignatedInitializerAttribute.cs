//
// DesignatedInitializerAttribute.cs:
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc.

using System;

namespace ObjCRuntime {

	// For bindings the attribute is used on interfaces, which means we must be able to decorated methods
	// not only constructors
	[AttributeUsage (AttributeTargets.Constructor | AttributeTargets.Method)]
	public class DesignatedInitializerAttribute : Attribute {
		public DesignatedInitializerAttribute ()
		{
		}
	}
}
