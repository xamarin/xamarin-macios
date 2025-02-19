// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.Macios.Generator.Extensions;

static class EnumExtensions {

	public static bool HasCustomMarshalDirective (this Enum self)
	{
		return self switch {
			// cast to the flags we know that could have the value and return it
			ObjCBindings.Method methodFlag => methodFlag.HasFlag (ObjCBindings.Method.CustomMarshalDirective),
			ObjCBindings.Property propertyFlag => propertyFlag.HasFlag (ObjCBindings.Property.CustomMarshalDirective),
			_ => false
		};
	}

	public static bool HasMarshalNativeExceptions (this Enum self)
	{
		return self switch {
			// cast to the flags we know that could have the value and return it
			ObjCBindings.Method methodFlag => methodFlag.HasFlag (ObjCBindings.Method.MarshalNativeExceptions),
			ObjCBindings.Property propertyFlag => propertyFlag.HasFlag (ObjCBindings.Property.MarshalNativeExceptions),
			_ => false
		};
	}
}
