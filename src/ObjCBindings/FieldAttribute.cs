using System;

#nullable enable

namespace ObjCBindings {

	[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class FieldAttribute<T> : Attribute where T : FieldTag {
		public FieldAttribute (string symbolName)
		{
			SymbolName = symbolName;
		}

		public FieldAttribute (string symbolName, string libraryName)
		{
			SymbolName = symbolName;
			LibraryName = libraryName;
		}

		public string SymbolName { get; set; }
		public string? LibraryName { get; set; }
	}
}
