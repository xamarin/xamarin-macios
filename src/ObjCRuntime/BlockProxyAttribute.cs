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
	[AttributeUsage (AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class BlockProxyAttribute : Attribute {
		public BlockProxyAttribute (Type t) { Type = t; }
		/// <summary>The type that is used to proxy an Objective-C block into this managed parameter.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public Type Type { get; set; }
	}

	[AttributeUsage (AttributeTargets.ReturnValue, AllowMultiple = false)]
	public sealed class DelegateProxyAttribute : Attribute {
		public DelegateProxyAttribute (Type delegateType)
		{
			DelegateType = delegateType;
		}
		/// <summary>The delegate type that is used to proxy managed delegates into Objective-C blocks.</summary>
		///         <value>The delegate type that is used to proxy managed delegates into Objective-C blocks.</value>
		///         <remarks>
		///         </remarks>
		public Type DelegateType { get; set; }
	}
}
