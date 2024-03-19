# Required Reasons API usage in .NET MAUI and Xamarin.Forms

The tables provide lists of C# .NET APIs that call the [Required Reasons APIs][RequiredReasonAPI] organized by category. If your application, SDK or package code calls any of the APIs from these lists, declare the reasons for their use in your privacy manifest file following the guidelines specified in Apple’s documentation on [Required Reasons APIs][RequiredReasonAPI].


**Note:** The following lists are verified only for .NET MAUI versions 8.0.0 and later.

### [User defaults APIs][UserDefaultsAPIs]
The following APIs either directly or indirectly access user defaults and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryUserDefaults` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you only access the user defaults from the list of APIs below, then use the value `C56D.1` in the `NSPrivacyAccessedAPITypeReasons` array. Refer to [User defaults APIs][UserDefaultsAPIs] to determine the any additional relevant values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| API usage |
| - |
| [NSUserDefaults](https://learn.microsoft.com/dotnet/api/foundation.nsuserdefaults) | 

For example, if you use any of the APIs listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSPrivacyAccessedAPITypes</key>
    <array>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryUserDefaults</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>...</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

Reason codes from [User defaults APIs][UserDefaultsAPIs] need to be provided in the array following the `NSPrivacyAccessedAPITypeReasons` key.


[RequiredReasonAPI]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api
[PrivacyManifestFiles]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files
[C#NETAPIs]: #c-net-apis-in-net-maui
[FileTimestampAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278393
[SystemBootTimeAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278394
[DiskSpaceAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278397
[ActiveKeyboardAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278400
[UserDefaultsAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278401