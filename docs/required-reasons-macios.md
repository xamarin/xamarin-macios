# Required Reasons API usage in .NET for iOS, tvOS, and Xamarin.iOS

The tables provide lists of C# .NET APIs that call the [Required Reasons APIs][RequiredReasonAPI] organized by category. If your application, SDK or package code calls any of the APIs from these lists, declare the reasons for their use in your privacy manifest file following the guidelines specified in Apple’s documentation on [Required Reasons APIs][RequiredReasonAPI].

**Note:** The following lists are verified only for .NET for iOS, tvOS and Xamarin.iOS versions 8.0.4 and later.

### [File timestamp APIs][FileTimestampAPIs]

The following APIs either directly or indirectly access file timestamps and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryFileTimestamp` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. Refer to [File timestamp APIs][FileTimestampAPIs] for any additional relevent values to add to the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSFileManager.CreationDate](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.creationdate) | [UIDocument.FileModificationDate](https://learn.microsoft.com/dotnet/api/uikit.uidocument.filemodificationdate) | [NSDocument.FileModificationDate](https://learn.microsoft.com/dotnet/api/appkit.nsdocument.filemodificationdate) |
| [NSFileManager.ModificationDate](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.modificationdate)
| [NSFileManager.SetAttributes(NSDictionary, string, NSError)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.setattributes)
| [NSFileManager.SetAttributes(NSFileAttributes, string, NSError)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.setattributes)
| [NSFileManager.SetAttributes(NSFileAttributes, string)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.setattributes)
| [NSFileManager.CreateDirectory(string, bool, NSDictionary, NSError)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.createdictionary)
| [NSFileManager.CreateDirectory(string, bool, NSFileAttributes, NSError)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.createdictionary)
| [NSFileManager.CreateDirectory(string, bool, NSFileAttributes)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.createdictionary)
| [NSFileManager.CreateFile(string, NSData, NSDictionary)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.createfile)
| [NSFileManager.CreateFile(string, NSData, NSFileAttributes)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.createfile)
| [NSFileManager.GetAttributes(string, NSError)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.getattributes)
| [NSFileManager.GetAttributes(string)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.getattributes)
| [NSDictionary.ToFileAttributes()](https://learn.microsoft.com/dotnet/api/foundation.nsdictionary.tofileattributes)
| [NSUrl.ContentModificationDateKey](https://learn.microsoft.com/dotnet/api/foundation.nsurl.contentmodificationdatekey)
| [NSUrl.CreationDateKey](https://learn.microsoft.com/dotnet/api/foundation.nsurl.creationdatekey)

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
            <string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>...</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

Additional reason codes from [File timestamp APIs][FileTimestampAPIs] can be provided in the array following the `NSPrivacyAccessedAPITypeReasons` key.

### [System boot time APIs][SystemBootTimeAPIs]

The following APIs either directly or indirectly access the system boot time and require reasons for use. Use the string `NSPrivacyAccessedAPICategorySystemBootTime` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you only access the system boot time from the list of APIs below, then use the `35F9.1` value in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSProcessInfo.SystemUptime](https://learn.microsoft.com/dotnet/api/foundation.nsprocessinfo.systemuptime)

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
            <string>NSPrivacyAccessedAPICategorySystemBootTime</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>35F9.1</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

### [Disk space APIs][DiskSpaceAPIs]
 
The following APIs either directly or indirectly access the available disk space and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryDiskSpace` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you access the available disk space from the list of APIs below, then use [Disk space APIs][DiskSpaceAPIs] to determine the correct values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSUrl.VolumeAvailableCapacityKey](https://learn.microsoft.com/dotnet/api/foundation.nsurl.volumeavailablecapacitykey)
| [NSUrl.VolumeAvailableCapacityForImportantUsageKey](https://learn.microsoft.com/dotnet/api/foundation.nsurl.volumeavailablecapacityforimportantusagekey)
| [NSUrl.VolumeAvailableCapacityForOpportunisticUsageKey](https://learn.microsoft.com/dotnet/api/foundation.nsurl.volumeavailablecapacityforopportunisticusagekey)
| [NSUrl.VolumeTotalCapacityKey](https://learn.microsoft.com/dotnet/api/foundation.nsurl.volumetotalcapacity)
| [NSFileManager.SystemFreeSize](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.systemfreesize)
| [NSFileManager.SystemSize](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.systemsize)
| [NSFileManager.GetFileSystemAttributes(string, NSError)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.getfilesystemattributes)
| [NSFileManager.GetFileSystemAttributes(string)](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.getfilesystemattributes)

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
            <string>NSPrivacyAccessedAPICategoryDiskSpace</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>...</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

Reason codes from [Disk space APIs][DiskSpaceAPIs] need to be provided in the array following the `NSPrivacyAccessedAPITypeReasons` key.

### [Active keyboard APIs][ActiveKeyboardAPIs]

The following APIs either directly or indirectly access the list of available keyboards and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryActiveKeyboards` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you access the list of available keyboards from the list of APIs below, then use [Active keyboard APIs][ActiveKeyboardAPIs] to determine the correct values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
|  | [UITextInputMode.ActiveInputModes](https://learn.microsoft.com/dotnet/api/appkit.uitextinputmode.activeinputmodes)

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
            <string>NSPrivacyAccessedAPICategoryActiveKeyboards</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>...</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

Reason codes from [Active keyboard APIs][ActiveKeyboardAPIs] need to be provided in the array following the `NSPrivacyAccessedAPITypeReasons` key.

### [User defaults APIs][UserDefaultsAPIs]
The following APIs either directly or indirectly access user defaults and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryUserDefaults` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you only access the user defaults from the list of APIs below, then use the value `C56D.1` in the `NSPrivacyAccessedAPITypeReasons` array. Refer to [User defaults APIs][UserDefaultsAPIs] to determine the any additional relevant values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSUserDefaults](https://learn.microsoft.com/dotnet/api/foundation.nsuserdefaults) | | [NSUserDefaultsController.NSUserDefaultsController(NSUserDefaults, NSDictionary)](https://learn.microsoft.com/dotnet/api/appkit.nsuserdefaultscontroller.-ctor#appkit-nsuserdefaultscontroller-ctor(foundation-nsuserdefaults-foundation-nsdictionary))
|                | | [NSUserDefaultsController.Defaults](https://learn.microsoft.com/dotnet/api/appkit.nsuserdefaultscontroller.defaults)
|                | | [NSUserDefaultsController.SharedUserDefaultsController](https://learn.microsoft.com/dotnet/api/appkit.nsuserdefaultscontroller.shareduserdefaultscontroller)

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