#if canImport(AppKit)
import AppKit
#endif
import Foundation
import StoreKit
#if canImport(UIKit)
import UIKit
#endif

#if os(macOS)
public typealias PlatformScene = NSViewController
#elseif !os(tvOS)
@available(iOS 13, macCatalyst 13.1, *)
public typealias PlatformScene = UIWindowScene
#endif


@objc(XamarinSwiftFunctions)
public class XamarinSwiftFunctions : NSObject {
#if !os(tvOS)
    @MainActor
    @objc(requestReview:)
    @available(iOS 16, macCatalyst 16, macOS 13, *)
    public static func StoreKit_RequestReview(scene: PlatformScene)
    {
        AppStore.requestReview(in: scene)
    }
#endif
}
