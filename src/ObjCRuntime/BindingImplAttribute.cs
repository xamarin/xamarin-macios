//
// BindingImplAttrobute.cs: Apply this to binding methods to describe them.
//

using System;
using System.Runtime.InteropServices;

namespace XamCore.ObjCRuntime {

	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
	public class BindingImplAttribute : Attribute
	{
		public BindingImplAttribute (BindingImplOptions options)
		{
			Options = options;
		}

		public BindingImplOptions Options { get; set; }
	}

	[Flags]
	public enum BindingImplOptions
	{
		GeneratedCode = 1,
		Optimizable = 2,
	}
}
