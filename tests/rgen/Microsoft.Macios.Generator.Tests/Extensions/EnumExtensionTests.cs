// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003

using System;
using Microsoft.Macios.Generator.Extensions;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class EnumExtensionTests {

	[Theory]
	[InlineData (ObjCBindings.Property.Notification, false)]
	[InlineData (ObjCBindings.Property.CustomMarshalDirective, true)]
	[InlineData (ObjCBindings.Method.Default, false)]
	[InlineData (ObjCBindings.Method.CustomMarshalDirective, true)]
	[InlineData (StringComparison.Ordinal, false)]
	public void HasCustomMarshalDirective<T> (T enumValue, bool expected) where T : Enum
		=> Assert.Equal (enumValue.HasCustomMarshalDirective (), expected);

	[Theory]
	[InlineData (ObjCBindings.Property.Notification, false)]
	public void HasMarshalNativeExceptions<T> (T enumValue, bool expected) where T : Enum
		=> Assert.Equal (enumValue.HasMarshalNativeExceptions (), expected);
}
