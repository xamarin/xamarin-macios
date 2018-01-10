//
// Attribute to link delegates to their marshallers
//
// Copyright 2013 Xamarin Inc
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace ObjCRuntime {
	[AttributeUsage (AttributeTargets.Parameter, AllowMultiple=false)]
#if XAMCORE_2_0
	sealed
#endif
	public class BlockProxyAttribute : Attribute {
		public BlockProxyAttribute (Type t) { Type = t; }
		public Type Type { get; set; }
	}

	[AttributeUsage (AttributeTargets.ReturnValue, AllowMultiple=false)]
	public sealed class DelegateProxyAttribute : Attribute {
		public DelegateProxyAttribute (Type delegateType)
		{
			DelegateType = delegateType;
		}
		public Type DelegateType { get; set; }
	}
}
