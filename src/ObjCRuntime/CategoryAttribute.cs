//
// CategoryAttribyute.cs:
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.

using System;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace ObjCRuntime {
	[AttributeUsage (AttributeTargets.Class)]
	public class CategoryAttribute : Attribute {
		public CategoryAttribute (Type type)
		{
			Type = type;
		}

		public Type Type { get; set; }
		public string Name { get; set; }
	}
}
