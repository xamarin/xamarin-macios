using Foundation;
using ObjCBindings;

namespace Microsoft.Macios.Bindings.Analyzer.Sample {
	// If you don't see warnings, build the Analyzers Project.

	[BindingType]
	public enum Test {
		[Field<EnumValue> ("TheSymbolNone", LibraryName = "/my/path/to/my/Manuel.framework")]
		None,
		[Field<EnumValue> ("TheSymbolMedium", LibraryName = "/my/path/to/my/Manuel.framework")]
		Medium,
		[Field<EnumValue> ("TheSymbolHigh", LibraryName = "/my/path/to/my/Manuel.framework")]
		High,
	}

	// This does not regenerate a thing
	namespace NestedExample {
		[BindingType]
		public enum Test {
			[Field<EnumValue> ("TheSymbolNone", LibraryName = "/my/path/to/my/Manuel.framework")]
			None,
			[Field<EnumValue> ("TheSymbolMedium", LibraryName = "/my/path/to/my/Manuel.framework")]
			Medium,
			[Field<EnumValue> ("TheSymbolHigh", LibraryName = "/my/path/to/my/Manuel.framework")]
			High,
			[Field<EnumValue> ("TheSymbolFoo", LibraryName = "/my/path/to/my/Manuel.framework")]
			Test,
		}
	}

}

