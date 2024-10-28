using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Extensions;

using ExportedConstructor = ExportedMember<IMethodSymbol, ExportData<Constructor>>;
using ExportedField = ExportedMember<IPropertySymbol, FieldData<Field>>;
using ExportedMethod = ExportedMember<IMethodSymbol, ExportData<Method>>;
using ExportedProperty = ExportedMember<IPropertySymbol, ExportData<Property>>;

readonly struct ExportedMembers {

	public ImmutableArray<ExportedConstructor> Constructors { get; }
	public ImmutableArray<ExportedField> Fields { get; }
	public ImmutableArray<ExportedMethod> Methods { get; }
	public ImmutableArray<ExportedProperty> Properties { get; }

	public ExportedMembers ()
	{
		Constructors = [];
		Fields = [];
		Methods = [];
		Properties = [];
	}

	public ExportedMembers (ImmutableArray<ExportedConstructor> ctrs,
		ImmutableArray<ExportedField> fields,
		ImmutableArray<ExportedMethod> methods,
		ImmutableArray<ExportedProperty> props)
	{
		Constructors = ctrs;
		Fields = fields;
		Methods = methods;
		Properties = props;
	}

}
