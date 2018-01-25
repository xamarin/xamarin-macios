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

namespace ObjCRuntime
{
	[AttributeUsage (AttributeTargets.Enum)]
	public sealed class NativeAttribute : Attribute
	{
		public NativeAttribute ()
		{
		}

		// use in case where the managed name is different from the native name
		// Extrospection tests will use this to find the matching type to compare
		public NativeAttribute (string name)
		{
			NativeName = name;
		}

		public string NativeName { get; set; }
	}
}
