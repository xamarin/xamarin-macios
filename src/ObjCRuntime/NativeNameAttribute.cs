//
// NativeNameAttribute.cs: an attribute to declare the native name for an API
// where it doesn't match the managed name (casing differences for instance).
//
// Authors:
//   Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022 Microsoft Corp. All rights reserved.
//

using System;
using System.Diagnostics;

namespace ObjCRuntime {
	// Currently only for enums and structs, but more can be added if need be.
	// Classes already have the Register attribute that can be used to specify a native name.
	[AttributeUsage (AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false)]
	public sealed class NativeNameAttribute : Attribute {
		// use in case where the managed name is different from the native name
		public NativeNameAttribute (string name)
		{
			NativeName = name;
		}

		public string NativeName { get; set; }
	}
}
