//
// RequiresSuperAttribute.cs:
//
// Authors:
//   Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2017 Xamarin Inc.

using System;

using Foundation;

namespace ObjCRuntime {

	// https://clang.llvm.org/docs/AttributeReference.html#objc-requires-super-clang-objc-requires-super
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public sealed class RequiresSuperAttribute : AdviceAttribute {
		public RequiresSuperAttribute ()
			: base ("Overriding this method requires a call to the overriden method.")
		{
		}
	}
}
