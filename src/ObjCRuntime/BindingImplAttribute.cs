//
// BindingImplAttrobute.cs: Apply this to binding methods to describe them.
//

using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {

	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
	public class BindingImplAttribute : Attribute {
		public BindingImplAttribute (BindingImplOptions options)
		{
			Options = options;
		}

		/// <summary>The binding implementation options.</summary>
		///         <value>The binding implementation options.</value>
		///         <remarks>
		///         </remarks>
		public BindingImplOptions Options { get; set; }
	}

	[Flags]
	public enum BindingImplOptions {
		/// <summary>If the method contains generated code.</summary>
		GeneratedCode = 1,
		/// <summary>If the method contains code that can be optimized by the Xamarin.iOS build process.</summary>
		Optimizable = 2,
	}
}
