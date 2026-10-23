// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

#nullable enable

namespace WidgetKit {

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Internal]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[iOS (14, 0), MacCatalyst (14, 0), Mac (11, 0), NoTV]
	interface XamarinWidgetKitSwiftFunctions {
		[Export ("reloadTimelinesOfKind:")]
		[Static]
		void ReloadTimelines (string kind);

		[Export ("reloadAllTimelines")]
		[Static]
		void ReloadAllTimelines ();
	}
}
