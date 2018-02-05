//
// CategoryAttribyute.cs:
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.

using System;

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
