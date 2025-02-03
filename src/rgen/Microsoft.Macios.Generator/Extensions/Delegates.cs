// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

/// <summary>
/// Delegate to try to parse a value struct from an attribute.
/// </summary>
/// <typeparam name="T">The type of the returned struct.</typeparam>
delegate bool TryParseDelegate<T> (AttributeData attributeData, [NotNullWhen (true)] out T? value)
	where T : struct;

/// <summary>
/// Delegate that returns a list of attribute names.
/// </summary>
delegate string? GetAttributeNames ();
