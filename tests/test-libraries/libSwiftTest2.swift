import Foundation
import libSwiftTest

@objc(SwiftTestClass2)
@available(iOS 15, tvOS 15, macOS 12, macCatalyst 12, *)
open class SwiftTestClass2 : SwiftTestClass {
    @objc
    public func SayHello2() -> String {
        return "Hello from Swift 2"
    }
}
