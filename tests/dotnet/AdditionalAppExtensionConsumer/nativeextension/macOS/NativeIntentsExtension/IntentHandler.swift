//
//  IntentHandler.swift
//  NativeIntentsExtension
//
//  Created by Rolf Bjarne Kvinge on 30/8/22.
//

import Intents

class IntentHandler: INExtension {
    
    override func handler(for intent: INIntent) -> Any {
        // This is the default implementation.  If you want different objects to handle different intents,
        // you can override this and return the handler you want for that particular intent.
        
        return self
    }
    
}
