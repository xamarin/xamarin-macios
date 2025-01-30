// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct ErrorDomainData {

	public string ErrorDomain { get; }
	public string? LibraryName { get; }


	public ErrorDomainData (string domain)
	{
		ErrorDomain = domain;
	}

	public ErrorDomainData (string domain, string? libraryName)
	{
		ErrorDomain = domain;
		LibraryName = libraryName;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out ErrorDomainData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string errorDomain;
		string? libraryName = null;

		switch (count) {
		case 1:
			errorDomain = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		case 2:
			errorDomain = (string) attributeData.ConstructorArguments [0].Value!;
			libraryName = (string) attributeData.ConstructorArguments [1].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (errorDomain, libraryName);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "ErrorDomain":
				errorDomain = (string) value.Value!;
				break;
			case "LibraryName":
				libraryName = (string) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (errorDomain, libraryName);
		return true;
	}
}
