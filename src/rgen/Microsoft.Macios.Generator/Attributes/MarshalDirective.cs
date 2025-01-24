// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.Macios.Generator.Attributes;

record CustomMarshalDirective (string? NativePrefix, string? NativeSuffix, string? Library);

static class ExportDataExtensions {

	public static bool ShouldMarshalNativeExceptions<T> (this ExportData<T> self) where T : Enum
		=> self switch {
			ExportData<ObjCBindings.Property> property => property.Flags.HasFlag (ObjCBindings.Property
				.CustomMarshalDirective),
			ExportData<ObjCBindings.Method> method => method.Flags.HasFlag (
				ObjCBindings.Property.CustomMarshalDirective),
			_ => false,
		};

	public static CustomMarshalDirective? ToCustomMarshalDirective<T> (this ExportData<T> self) where T : Enum
	{
		var present = self switch {
			ExportData<ObjCBindings.Property> property => property.Flags.HasFlag (ObjCBindings.Property
				.CustomMarshalDirective),
			ExportData<ObjCBindings.Method> method => method.Flags.HasFlag (
				ObjCBindings.Property.CustomMarshalDirective),
			_ => false,
		};
		if (present) {
			return new (self.NativePrefix, self.NativeSuffix, self.Library);
		}

		return null;
	}

}
