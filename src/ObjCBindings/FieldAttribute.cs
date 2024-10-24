using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// Generic attribute that is used to mark code to be backed by a field in ObjC.
	/// </summary>
	[Experimental ("APL0003")]
	[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class FieldAttribute<T> : Attribute where T : FieldTag {

		/// <summary>
		/// Create a new FieldAttribute for the given symbol and using the namespace as its containing library.
		/// <param name="symbolName">The name of the symbol.</param>
		/// </summary>
		public FieldAttribute (string symbolName)
		{
			SymbolName = symbolName;
		}

		/// <summary>
		/// Create a new FieldAttribute for the given symbol in the provided library.
		/// <param name="symbolName">The name of the symbol.</param>
		/// <param name="libraryName">The name of the library that contains the symbol.</param>
		/// </summary>
		public FieldAttribute (string symbolName, string libraryName)
		{
			SymbolName = symbolName;
			LibraryName = libraryName;
		}

		
		/// <summary>
		/// Get/Set the symbol represented by the attribute.
		/// </summary>
		public string SymbolName { get; set; }

		/// <summary>
		/// Get/Set the library that contains the symbol..
		/// </summary>
		public string? LibraryName { get; set; }
	}
}
