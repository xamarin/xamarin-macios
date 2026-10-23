// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !os(tvOS)
import Foundation
import WidgetKit

// Keep WidgetKit in a separate archive member so unused reload support can be stripped.
@objc(XamarinWidgetKitSwiftFunctions)
public class XamarinWidgetKitSwiftFunctions : NSObject {
    @objc(reloadTimelinesOfKind:)
    @available(iOS 14, macCatalyst 14, macOS 11, *)
    public static func ReloadTimelines(ofKind kind: String)
    {
        WidgetCenter.shared.reloadTimelines(ofKind: kind)
    }

    @objc(reloadAllTimelines)
    @available(iOS 14, macCatalyst 14, macOS 11, *)
    public static func ReloadAllTimelines()
    {
        WidgetCenter.shared.reloadAllTimelines()
    }
}
#endif
