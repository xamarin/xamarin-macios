//
//  TemplateFx.swift
//  TemplateFx
//

import Foundation

@objc(SwiftBindTest)
public class SwiftBindTest : NSObject
{
    @objc
    public static func getString(myString: String) -> String {
        return myString  + " from swift!"
    }

    //REPLACE

}
