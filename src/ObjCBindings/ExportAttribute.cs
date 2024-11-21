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

		public string? Selector { get; set; } = null;

		public T? Flags { get; set; } = default (T);

		public ArgumentSemantic ArgumentSemantic { get; set; } = ArgumentSemantic.None;

		protected ExportAttribute () { }

		public ExportAttribute (string? selector)
		{
			Selector = selector;
		}

		public ExportAttribute (string? selector, ArgumentSemantic semantic)
		{
			Selector = selector;
			ArgumentSemantic = semantic;
			Flags = default (T);
		}

		public ExportAttribute (string? selector, T? flags)
		{
			Selector = selector;
			ArgumentSemantic = ArgumentSemantic.None;
			Flags = flags;
		}

		public ExportAttribute (string? selector, ArgumentSemantic semantic, T? flags)
		{
			Selector = selector;
			ArgumentSemantic = semantic;
			Flags = flags;
		}
	}
}
