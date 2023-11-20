//
// NativeAttribute.cs: hint for the generator to generate
// native int size argument for message calls
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin, Inc. All rights reserved.
//

using System;
using System.Diagnostics;

#nullable enable

namespace ObjCRuntime {
	[AttributeUsage (AttributeTargets.Enum)]
	public sealed class NativeAttribute : Attribute {
		public NativeAttribute ()
		{
		}

		// use in case where the managed name is different from the native name
		// Extrospection tests will use this to find the matching type to compare
		public NativeAttribute (string name)
		{
			NativeName = name;
		}

		public string? NativeName { get; set; }

		// methods to use to convert a managed enum value to native (and vice versa)
		// The methods should have the following signatures:
		//     public static NativeType ConvertToNative (MyEnum value) { }
		//     public static MyEnum ConvertToManaged (NativeType value) { }
		// Where <integral type> is any of [S]Byte, [U]Int16, [U]Int32, [U]Int64, n[u]int (our own versions).
		// <integral type> must be the same for both methods.
		// If one of these methods is specified, the other one must be as well.
		public string? ConvertToNative { get; set; }
		public string? ConvertToManaged { get; set; }
	}
}
