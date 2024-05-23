import Foundation

@objc(SwiftTestClass)
public class SwiftTestClass : NSObject {
    @objc
    public func SayHello() -> String {
        return "Hello from Swift"
    }
}
