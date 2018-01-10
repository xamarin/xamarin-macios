//
// Enums.cs: enums for JavaScriptCore
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013-2014 Xamarin Inc.

using System;

namespace JavaScriptCore {
#if !MONOMAC || XAMCORE_2_0
	// untyped enum -> JSValueRef.h
	public enum JSType {
		Undefined,
		Null,
		Boolean,
		Number,
		String,
		Object
	}

	// typedef unsigned -> JSObjectRef.h
	[Flags]
	public enum JSPropertyAttributes : uint {
		None = 0,
		ReadOnly = 1 << 1,
		DontEnum = 1 << 2,
		DontDelete = 1 << 3
	}

	// typedef unsigned -> JSObjectRef.h
	[Flags]
	public enum JSClassAttributes : uint {
		None = 0,
		NoAutomaticPrototype = 1 << 1
	}
#endif
}
