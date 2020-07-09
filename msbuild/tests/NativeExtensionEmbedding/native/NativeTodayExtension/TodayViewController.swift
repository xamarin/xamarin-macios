//
//  TodayViewController.swift
//  NativeTodayExtension
//
//  Created by Chris Hamons on 7/7/20.
//  Copyright © 2020 Chris Hamons. All rights reserved.
//

import UIKit
import NotificationCenter

class TodayViewController: UIViewController, NCWidgetProviding {
    override func viewDidLoad() {
        super.viewDidLoad()
    }
        
    func widgetPerformUpdate(completionHandler: (@escaping (NCUpdateResult) -> Void)) {
        completionHandler(NCUpdateResult.newData)
    }
}
