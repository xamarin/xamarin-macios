#if os(macOS)
import AppKit
#endif
import Foundation
import StoreKit
#if !os(macOS)
import UIKit
#endif

@objc(XamarinSwiftFunctions)
public class XamarinSwiftFunctions : NSObject {
#if os(macOS)
    @MainActor
    @objc(requestReview:)
    @available(macOS 13, *)
    public static func StoreKit_RequestReview(scene: NSViewController)
    {
        AppStore.requestReview(in: scene)
    }
#elseif !os(tvOS)
    @MainActor
    @objc(requestReview:)
    @available(iOS 16, macCatalyst 16, *)
    public static func StoreKit_RequestReview(scene: UIWindowScene)
    {
        AppStore.requestReview(in: scene)
    }
#endif
}
