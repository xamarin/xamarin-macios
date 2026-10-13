// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Foundation;

namespace InvalidSelector {
	[BaseType (typeof (NSObject))]
	interface InvalidSelectorType {
		[Export ("invalid(selector")]
		string InvalidSelector { get; set; }
	}
}
