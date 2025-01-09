// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnumInternal {
	[Field<EnumValue> ("None", "__Internal")]
	None,
	[Field<EnumValue> ("Medium", "__Internal")]
	Medium,
	[Field<EnumValue> ("High", "__Internal")]
	High,
}
