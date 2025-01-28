// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly struct AsyncData : IEquatable<AsyncData> {
	/// <summary>
	/// Diff the constructor used in the bindings.
	/// </summary>
	internal enum ConstructorType {
		ResultType,
		MethodName
	}

	public string? ResultType { get; init; } // this in the attr is a type, but we do not care for the transformation
	public string? MethodName { get; init; }
	public string? ResultTypeName { get; init; }
	public string? PostNonResultSnippet { get; init; }

	public AsyncData () { }

	public AsyncData (string resultType, ConstructorType constructorType)
	{
		if (constructorType == ConstructorType.ResultType)
			ResultType = resultType;
		else
			MethodName = resultType;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out AsyncData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		ConstructorType constructorType = ConstructorType.MethodName;
		string? resultType = null;
		string? resultTypeName = null;
		string? methodName = null;
		string? postNonResultSnippet = null;

		switch (count) {
		case 0:
			break;
		case 1:
			// we have to diff constructors that take a single parameter, either a string or a type
			if (attributeData.ConstructorArguments [0].Value! is string methodNameValue) {
				constructorType = ConstructorType.MethodName;
				methodName = methodNameValue;
			} else {
				constructorType = ConstructorType.ResultType;
				resultType = ((INamedTypeSymbol) attributeData.ConstructorArguments [0].Value!).ToDisplayString ();
			}
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			if (constructorType == ConstructorType.ResultType)
				data = new (resultType!, ConstructorType.ResultType);
			else
				data = new (methodName!, ConstructorType.MethodName);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "ResultType":
				resultType = ((INamedTypeSymbol) value.Value!).ToDisplayString ();
				break;
			case "MethodName":
				methodName = (string) value.Value!;
				break;
			case "ResultTypeName":
				resultTypeName = (string) value.Value!;
				break;
			case "PostNonResultSnippet":
				postNonResultSnippet = (string) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		if (count == 0) {
			// use the default constructor and use the init properties
			data = new () {
				ResultType = resultType,
				MethodName = methodName,
				ResultTypeName = resultTypeName,
				PostNonResultSnippet = postNonResultSnippet
			};
			return true;
		}

		switch (constructorType) {
		case ConstructorType.MethodName:
			data = new (methodName!, ConstructorType.MethodName) {
				ResultType = resultType,
				ResultTypeName = resultTypeName,
				PostNonResultSnippet = postNonResultSnippet
			};
			break;
		case ConstructorType.ResultType:
			data = new (resultType!, ConstructorType.ResultType) {
				MethodName = methodName,
				ResultTypeName = resultTypeName,
				PostNonResultSnippet = postNonResultSnippet
			};
			break;
		}

		return false;
	}

	public bool Equals (AsyncData other)
	{
		if (ResultType != other.ResultType)
			return false;
		if (MethodName != other.MethodName)
			return false;
		if (ResultTypeName != other.ResultTypeName)
			return false;
		return PostNonResultSnippet == other.PostNonResultSnippet;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is AsyncData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
		=> HashCode.Combine (ResultType, MethodName, ResultTypeName, PostNonResultSnippet);

	public static bool operator == (AsyncData x, AsyncData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (AsyncData x, AsyncData y)
	{
		return !(x == y);
	}

	public override string ToString ()
		=> $"{{ ResultType: '{ResultType ?? "null"}', MethodName: '{MethodName ?? "null"}', ResultTypeName: '{ResultTypeName ?? "null"}', PostNonResultSnippet: '{PostNonResultSnippet ?? "null"}' }}";
}
