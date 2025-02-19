// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCBindings;

namespace AppKit;

[BindingType<Class> (Class.IsThreadSafe)]
public partial class ThreadSafeAppKitPropertyTests {

	// simple value type
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst13.1")]
	[Export<Property> ("count")]
	public virtual partial nuint Count { get; }

}
