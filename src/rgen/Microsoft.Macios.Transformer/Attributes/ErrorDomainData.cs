// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly struct ErrorDomainData : IEquatable<ErrorDomainData> {
	
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
		[NotNullWhen (true)] out ErrorDomainData ? data)
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

		data = new(errorDomain, libraryName); 
		return true;
	}
	
	public bool Equals (ErrorDomainData other)
	{
		if (ErrorDomain != other.ErrorDomain)
			return false;
		return LibraryName == other.LibraryName;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is ErrorDomainData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
		=> HashCode.Combine (ErrorDomain, LibraryName);

	public static bool operator == (ErrorDomainData x, ErrorDomainData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (ErrorDomainData x, ErrorDomainData y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ ErrorDomain: '{ErrorDomain}', LibraryName: '{LibraryName}' }}";
	}
}
