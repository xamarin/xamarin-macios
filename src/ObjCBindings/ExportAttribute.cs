using System;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using ObjCRuntime;
using Registrar;

#nullable enable

namespace ObjCBindings {

	[Experimental ("APL0003")]
	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
	public class ExportAttribute<T> : Attribute where T : Enum {

		/// <summary>
		/// Get/Set the selector that is expoed by the decorated method.
		/// </summary >
		public string? Selector { get; set; } = null;

		/// <summary>
		/// Get/Set the export configuration flags.
		/// </summary >
		public T? Flags { get; set; } = default (T);

		/// <summary>
		/// Get/Set the argment sematics to be used with the native method.
		///  </summary >
		public ArgumentSemantic ArgumentSemantic { get; set; } = ArgumentSemantic.None;

		/// <summary>
		/// Get/Set the native prefix to be used in the custom marshal directive.
		///
		/// The generator will only respect this value if the CustomMarshalDirective flag is set.
		/// If the flag is not set the analyzer will raise an compiling error.
		/// </summary >
		public string? NativePrefix { get; set; } = null;

		/// <summary>
		/// Get/Set the native suffix to be used in the custom marshal directive.
		///
		/// The generator will only respect this value if the CustomMarshalDirective flag is set.
		/// If the flag is not set the analyzer will raise an compiling error.
		/// </summary >
		public string? NativeSuffix { get; set; } = null;

		/// <summary>
		/// Get/Set the library to be used in the custom marshal directive.
		///
		/// The generator will only respect this value if the CustomMarshalDirective flag is set.
		/// If the flag is not set the analyzer will raise an compiling error.
		/// </summary >
		public string? Library { get; set; } = null;

		protected ExportAttribute () { }

		/// <summary>
		/// Mark a managed method as a exported selector.
		/// </summary >
		/// <param name="selector">The native selector to be exported.</param>
		public ExportAttribute (string? selector)
		{
			Selector = selector;
		}

		/// <summary>
		/// Mark a managed method as a exported selector.
		/// </summary >
		/// <param name="selector">The native selector to be exported.</param>
		/// <param name="semantic">The argument semantics to use when callinb the native selector.</param>
		public ExportAttribute (string? selector, ArgumentSemantic semantic)
		{
			Selector = selector;
			ArgumentSemantic = semantic;
			Flags = default (T);
		}

		/// <summary>
		/// Mark a managed method as a exported selector.
		/// </summary >
		/// <param name="selector">The native selector to be exported.</param>
		/// <param name="flags">The configuration flags for the managed mathod."</param>
		public ExportAttribute (string? selector, T? flags)
		{
			Selector = selector;
			ArgumentSemantic = ArgumentSemantic.None;
			Flags = flags;
		}

		/// <summary>
		/// Mark a managed method as a exported selector.
		/// </summary >
		/// <param name="selector">The native selector to be exported.</param>
		/// <param name="semantic">The argument semantics to use when callinb the native selector.</param>
		/// <param name="flags">The configuration flags for the managed mathod."</param>
		public ExportAttribute (string? selector, ArgumentSemantic semantic, T? flags)
		{
			Selector = selector;
			ArgumentSemantic = semantic;
			Flags = flags;
		}
	}
}
