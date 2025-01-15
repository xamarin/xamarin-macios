// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator;

public static class Diagnostics {
	/// <summary>
	/// An unexpected error occurred while processing '{0}'. Please fill a bug report at https://github.com/xamarin/xamarin-macios/issues/new.
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0000 = new (
		"RBI0000",
		new LocalizableResourceString (nameof (Resources.RBI0000Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0000MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0000Description), Resources.ResourceManager,
			typeof (Resources))
	);
}
