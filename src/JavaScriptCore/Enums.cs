//
// Enums.cs: enums for JavaScriptCore
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013-2014 Xamarin Inc.

using System;

using ObjCRuntime;

namespace JavaScriptCore {
	// untyped enum -> JSValueRef.h
	/// <summary>An enumeration whose values specify JavaScript types, including <c>Object</c> and <c>undefined</c>.</summary>
	public enum JSType {
		Undefined,
		Null,
		Boolean,
		Number,
		String,
		Object,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Symbol,
	}

	// typedef unsigned -> JSObjectRef.h
	/// <summary>Enumerates attributes of properties.</summary>
	[Flags]
	public enum JSPropertyAttributes : uint {
		None = 0,
		ReadOnly = 1 << 1,
		DontEnum = 1 << 2,
		DontDelete = 1 << 3
	}

	// typedef unsigned -> JSObjectRef.h
	/// <summary>Enumerates class attributes.</summary>
	[Flags]
	public enum JSClassAttributes : uint {
		None = 0,
		NoAutomaticPrototype = 1 << 1
	}
}
