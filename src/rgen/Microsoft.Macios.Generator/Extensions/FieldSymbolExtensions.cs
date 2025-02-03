// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

static partial class FieldSymbolExtensions {

	/// <summary>
	/// Generic method to retrieve the FieldAttribute on an IFieldSymbol and parse it. The method is generic to allow
	/// to parse different types of FieldAttribute depending on the API that was used to mark the field. Old APIs
	/// (xamarin bgen API) and the new API (ObjCBindings rgen API) use different FieldAttribute.
	/// </summary>
	/// <param name="fieldSymbol">The field whose attribute data we want to parse.</param>
	/// <param name="attributeName">The name of the attribute to parse. This name is different depending on the API used.</param>
	/// <param name="tryParseDelegate">The delegate to use to parse the attribute data.</param>
	/// <typeparam name="T">The type of the struct that will contain the data of the attribute.</typeparam>
	/// <returns>Ture if the attribute was found and could be parsed.</returns>
	public static T? GetFieldData<T> (IFieldSymbol fieldSymbol, string attributeName, TryParseDelegate<T> tryParseDelegate) where T : struct
	{
		var attributes = fieldSymbol.GetAttributeData ();
		if (attributes.Count == 0)
			return null;

		// Get all the FieldAttribute, parse it and add the data to the result
		if (!attributes.TryGetValue (attributeName, out var fieldAttrDataList) ||
			fieldAttrDataList.Count != 1)
			return null;

		var fieldAttrData = fieldAttrDataList [0];
		var fieldSyntax = fieldAttrData.ApplicationSyntaxReference?.GetSyntax ();
		if (fieldSyntax is null)
			return null;

		if (tryParseDelegate (fieldAttrData, out var fieldData))
			return fieldData.Value;

		return null;
	}

}
