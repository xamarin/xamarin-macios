# Apple’s privacy manifest policy requirements
Apple is introducing a privacy policy for including [privacy manifest files][PrivacyManifestFiles] in new and updated applications targeted for iOS, iPadOS, tvOS, and visionOS platforms on the App Store.

The privacy manifest file (`PrivacyInfo.xcprivacy`) lists the [types of data](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_data_use_in_privacy_manifests) your .NET MAUI applications, or any third-party SDKs and packages collect, and the reasons for using certain [Required Reason APIs][RequiredReasonAPI] categories.

**Important:** If the use of the [Required Reason APIs][RequiredReasonAPI] by you or third-party SDKs isn’t declared in the privacy manifest, your application might be rejected by the App Store. For more information, visit Apple’s documentation on [Required Reasons APIs][RequiredReasonAPI].

## Prepare your .NET MAUI applications for Apple’s privacy manifest policy
You must review your native code, C# code, and data collection and tracking practices to understand if Apple’s privacy manifest policy applies to you. Follow these guidelines to decide if you need to include a privacy manifest file in your product:

* If your application includes any third-party SDKs or packages, then these third-party components (if applicable) must provision their own privacy manifest files separately. **Note:** It’s your responsibility however, to make sure that the owners of these third-party components include privacy manifest files. Microsoft isn’t responsible for any third-party privacy manifest, and their data collection and tracking practices.

* If your application includes the [C# .NET APIs][C#NETAPIs] that call certain APIs listed in the Apple’s [Required Reason API][RequiredReasonAPI] categories, then you must assess your product for the API usage. For assessing what constitutes as part of data collection and tracking practices, refer to Apple’s documentation on [privacy manifest files][PrivacyManifestFiles]. **Note:** It’s your responsibility to assess your use of each of these APIs and declare the applicable reasons for using them.

Depending on whether you’re using [.NET for iOS or .NET MAUI to develop an application](#privacy-manifest-for-net-maui-and-net-for-ios-or-tvos-applications) or providing [ObjectiveC or Swift Binding packages](#privacy-manifest-for-binding-projects) to use with .NET MAUI applications, the requirement for providing a privacy manifest file might differ.

**Note:** The above guidelines are provided for your convenience. It’s important that you review Apple’s documentation on [privacy manifest files][PrivacyManifestFiles] before creating a privacy manifest for your project.

**Important:**
The following information is proivided based on Apple's documentation as of March 2024. It is recommended that you review Apple’s documentation on [privacy manifest files][PrivacyManifestFiles] when creating a privacy manifest for your project to ensure you are using the most recent guidelines. If you find any discrepancies in the information below, please [file a bug](https://github.com/dotnet/maui/issues) and include the API in question.

## Privacy manifest for .NET MAUI and .NET for iOS or tvOS applications  
If you’re developing an application using .NET MAUI, consider the following steps:

1. Assess if your native application code uses any of the following APIs:
    * APIs listed under the [Required Reason API][RequiredReasonAPI] category.
    * The [C# .NET APIs][C#NETAPIs] in .NET MAUI framework.
1. If you meet one or both of the conditions from step 1, or if you have disabled [linking](https://learn.microsoft.com/en-us/xamarin/ios/deploy-test/linker?tabs=macos), which will retain all of the [C# .NET APIs][C#NETAPIs], then [create a privacy manifest file](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files#4284009) following the [example](#an-example) to add a `PrivacyInfo.xcprivacy` file to your project.

1. In the privacy manifest file, declare the approved reasons for using the [Required Reasons APIs][RequiredReasonAPI] or [C# .NET APIs][C#NETAPIs], as applicable.

**Important:** If you don’t declare the reasons for the use of APIs, your application might be rejected by the App Store.

Verify if your native application code collects any type of data [categorized by Apple](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_data_use_in_privacy_manifests#4250555) and declare those data types in the privacy manifest file as applicable. Any third-party SDKs or packages used in your application must include their own separate manifest files to declare data collection and the use of any [Required Reasons APIs][RequiredReasonAPI] with approved reasons.

### Notes:

* It’s your responsibility to check the accuracy of the privacy manifest within the .NET MAUI app and if any third-party components included in your .NET MAUI project require any declarations in you privacy manifest. It’s recommended that you search these third-party components for any references to a privacy manifest declaration.

* If you’re developing an application using .NET MAUI as a library, check if your native application code collects any of the following information outside of the .NET MAUI project:
    * [Data collection](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_data_use_in_privacy_manifests)
    * [Required Reasons APIs][RequiredReasonAPI]

  If it does, then you must declare their usage in the privacy manifest file.

## Privacy manifest for Binding projects
If you are Binding project owner, and you are binding a xcframework, then the xcframework provider will need to include the `PrivacyInfo.xcprivacy` file as part of the xcframework. Otherwise, there are two options, provide documentation for package consumers to create the `PrivacyInfo.xcprivacy` file properly or change the bindings to bind an xcframework that has the `PrivacyInfo.xcprivacy` file included. It is currently not possible for Binding project authors to include a `PrivacyInfo.xcprivacy` file outside an xcframework that will be recognized by Apple when submitting an app.

## C# .NET APIs in .NET MAUI
There are three layers that could potentially use any of the [Required Reasons APIs][RequiredReasonAPI]:

* .NET runtime and Base Class libraries (BCL)
* .NET for iOS SDK
* .NET MAUI SDK

The .NET runtime and BCL use and ship as part of your app bundle APIs in the following categories:
* [File timestamp APIs][FileTimestampAPIs]
* [System boot time APIs][SystemBootTimeAPIs]
* [Disk space APIs][DiskSpaceAPIs]

Therefore, all .NET applications must include the above categories in `PrivacyInfo.xcprivacy` file. See the [example](#an-example) for the format and reason codes needed.

.NET for iOS and .NET MAUI build on top of the .NET runtime and BCL and include additional API usages as defined in the following tables. The tables provide lists of C# .NET APIs that call the [Required Reasons APIs][RequiredReasonAPI] organized by category. If your application, SDK or package code calls any of the APIs from these lists, declare the reasons for their use in your privacy manifest file following the guidelines specified in Apple’s documentation on [Required Reasons APIs][RequiredReasonAPI].

**Note:** The following lists are verified only for .NET MAUI versions 8.0.0 and later.

### [File timestamp APIs][FileTimestampAPIs]

The following APIs either directly or indirectly access file timestamps and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryFileTimestamp` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. Refer to [File timestamp APIs][FileTimestampAPIs] for any additional relevent values to add to the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSFileManager.CreationDate](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.creationdate) | [UIDocument.FileModificationDate](https://learn.microsoft.com/en-us/dotnet/api/uikit.uidocument.filemodificationdate) | [NSDocument.FileModificationDate](https://learn.microsoft.com/en-us/dotnet/api/appkit.nsdocument.filemodificationdate) |
| [NSFileManager.ModificationDate](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.modificationdate)
| [NSFileManager.SetAttributes(NSDictionary, string, NSError)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.setattributes)
| [NSFileManager.SetAttributes(NSFileAttributes, string, NSError)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.setattributes)
| [NSFileManager.SetAttributes(NSFileAttributes, string)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.setattributes)
| [NSFileManager.CreateDirectory(string, bool, NSDictionary, NSError)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.createdictionary)
| [NSFileManager.CreateDirectory(string, bool, NSFileAttributes, NSError)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.createdictionary)
| [NSFileManager.CreateDirectory(string, bool, NSFileAttributes)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.createdictionary)
| [NSFileManager.CreateFile(string, NSData, NSDictionary)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.createfile)
| [NSFileManager.CreateFile(string, NSData, NSFileAttributes)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.createfile)
| [NSFileManager.GetAttributes(string, NSError)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.getattributes)
| [NSFileManager.GetAttributes(string)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.getattributes)
| [NSDictionary.ToFileAttributes()](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsdictionary.tofileattributes)
| [NSUrl.ContentModificationDateKey](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsurl.contentmodificationdatekey)
| [NSUrl.CreationDateKey](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsurl.creationdatekey)

For example, if you use any of the API's listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:
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

The following APIs either directly or indirectly access the system boot time and require reasons for use. Use the string `NSPrivacyAccessedAPICategorySystemBootTime` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you only access the system boot time from the list of API's below, then use the `35F9.1` value in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSProcessInfo.SystemUptime](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsprocessinfo.systemuptime)

For example, if you use any of the API's listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:

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
 
The following APIs either directly or indirectly access the available disk space and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryDiskSpace` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you access the available disk space from the list of API's below, then use [Disk space APIs][DiskSpaceAPIs] to determine the correct values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSUrl.VolumeAvailableCapacityKey](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsurl.volumeavailablecapacitykey)
| [NSUrl.VolumeAvailableCapacityForImportantUsageKey](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsurl.volumeavailablecapacityforimportantusagekey)
| [NSUrl.VolumeAvailableCapacityForOpportunisticUsageKey](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsurl.volumeavailablecapacityforopportunisticusagekey)
| [NSUrl.VolumeTotalCapacityKey](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsurl.volumetotalcapacity)
| [NSFileManager.SystemFreeSize](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.systemfreesize)
| [NSFileManager.SystemSize](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.systemsize)
| [NSFileManager.GetFileSystemAttributes(string, NSError)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.getfilesystemattributes)
| [NSFileManager.GetFileSystemAttributes(string)](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.getfilesystemattributes)

For example, if you use any of the API's listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:

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

The following APIs either directly or indirectly access the list of available keyboards and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryActiveKeyboards ` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you access the list of available keyboards from the list of API's below, then use [Active keyboard APIs][ActiveKeyboardAPIs] to determine the correct values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
|  | [UITextInputMode.ActiveInputModes](https://learn.microsoft.com/en-us/dotnet/api/appkit.uitextinputmode.activeinputmodes)

For example, if you use any of the API's listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:

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
The following APIs either directly or indirectly access user defaults and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryUserDefaults ` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you only access the user defaults from the list of API's below, then use the value `C56D.1` in the `NSPrivacyAccessedAPITypeReasons` array. Refer to [User defaults APIs][UserDefaultsAPIs] to determine the any additional relevant values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| Foundation APIs | UIKit APIs | AppKit APIs |
| - | - | - |
| [NSUserDefaults](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsuserdefaults) | | [NSUserDefaultsController.NSUserDefaultsController(NSUserDefaults, NSDictionary)](https://learn.microsoft.com/en-us/dotnet/api/appkit.nsuserdefaultscontroller.-ctor#appkit-nsuserdefaultscontroller-ctor(foundation-nsuserdefaults-foundation-nsdictionary))
|                | | [NSUserDefaultsController.Defaults](https://learn.microsoft.com/en-us/dotnet/api/appkit.nsuserdefaultscontroller.defaults)
|                | | [NSUserDefaultsController.SharedUserDefaultsController](https://learn.microsoft.com/en-us/dotnet/api/appkit.nsuserdefaultscontroller.shareduserdefaultscontroller)

For example, if you use any of the API's listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:

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

# Example
Let's look at how you would add a Privacy Manifest file to an application that uses the following API's:

* [NSUserDefaults](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsuserdefaults) 
* [NSProcessInfo.SystemUptime](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsprocessinfo.systemuptime)
* [NSFileManager.ModificationDate](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.modificationdate)

How they are used is not that important for this example, but the `why` will determine the reason code needed for the privacy manifest.
## Creating the `PrivacyInfo.xcprivacy` file

We will start by building the contents of the `PrivacyInfo.xcprivacy` file, and then go through each supported platform and how to properly configure your project so the file in included in the bundle properly.

Since the application is based on .NET, a privacy manifest is required. Let's walkthrough the steps needed to add a privacy manifest that declares our usages of the above three API's.

1. Open Xcode and either create new `App` project, or open an existing one.
1. In Xcode use the File->New->File menu to open the `Choose a new template for your file:` dialog box.
1. Scroll dowen until you find the `App Privacy` template.
1. Select the `App Privacy` template and then click `Next`
1. In the `Save As` dialog, leave the filename as `PrivacyInfo` as that is the required name for the file.
1. Click `Create` and close Xcode.
1. Use Finder to copy the `PrivacyInfo.xcprivacy` file from the Xcode project to your documents folder for now.
1. The Xcode project is no longer needed and can be deleted.

You should now have a file named `PrivacyInfo.xcprivacy` under your documents folder with contents similar to:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<!--
... omitted for brevity
-->
<plist version="1.0">
<dict/>
</plist>
```
Use your favorite text editor, like [Visual Studio Code](https://aka.ms/vscode), to open the file for editing.

You can add the entries for the API's usage to the `PrivacyInfo.xcproject` as follows: 

1. Edit the `PrivacyInfo.xcprivacy` file to appear as follows:
    ```xml
    <?xml version="1.0" encoding="UTF-8"?>
    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
    <!--
    ... omitted for brevity
    -->
    <plist version="1.0">
    <dict>
        <key>NSPrivacyAccessedAPITypes</key>
        <array>
        </array>
    </dict>
    </plist>
    ```
    This adds the `NSPrivacyAccessAPITypes` key where each category usage will be added.
1. Since the .NET runtime and BCL include API's from the [File timestamp][FileTimestampAPIs], [System boot time][SystemBootTimeAPIs], [Disk space][DiskSpaceAPIs] API categories, add the following to the `NSPrivacyAccessedAPITypes` array:
    ```xml
    <dict>
        <key>NSPrivacyAccessedAPIType</key>
        <string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
        <key>NSPrivacyAccessedAPITypeReasons</key>
        <array>
            <string>C617.1</string>
        </array>
    </dict>
    <dict>
        <key>NSPrivacyAccessedAPIType</key>
        <string>NSPrivacyAccessedAPICategorySystemBootTime</string>
        <key>NSPrivacyAccessedAPITypeReasons</key>
        <array>
            <string>35F9.1</string>
        </array>
    </dict>
    <dict>
        <key>NSPrivacyAccessedAPIType</key>
        <string>NSPrivacyAccessedAPICategoryDiskSpace</string>
        <key>NSPrivacyAccessedAPITypeReasons</key>
        <array>
            <string>E174.1</string>
        </array>
    </dict>
    ```
1. For the [NSProcessInfo.SystemUptime](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsprocessinfo.systemuptime), a reason code of `35F9.1` is needed since that is the only reason code available. But since the .NET runtime and BCL already included that category and reason, there is nothing additional to add.

1. For the [NSFileManager.ModificationDate](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsfilemanager.modificationdate), a reason code of `C617.1` is needed since the modification dates are stored as a hash using [NSUserDefaults](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsuserdefaults) but not displayed to the user. Again, the .NET runtime and BCL requirements have already satisfied this category and reason so no additional changes are needed.

1. For the [NSUserDefaults](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsuserdefaults), a reason code of `CA92.1` is provided since the data accessed is only accessible to the app itself. Add the followinbg element to the `NSPrivacyAccessedAPITypes` array:
    ```xml
    <dict>
        <key>NSPrivacyAccessedAPIType</key>
        <string>NSPrivacyAccessedAPICategoryUserDefaults</string>
        <key>NSPrivacyAccessedAPITypeReasons</key>
        <array>
            <string>CA92.1</string>
        </array>
    </dict>
    ```

The complete `PrivacyInfo.xcprivacy` should now look similar to:
```xml
<!-- 
    ... comments and headers omitted for brevity 
-->
<plist version="1.0">
<dict>
    <key>NSPrivacyAccessedAPITypes</key>
    <array>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>C617.1</string>
            </array>
        </dict>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategorySystemBootTime</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>35F9.1</string>
            </array>
        </dict>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryDiskSpace</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>E174.1</string>
            </array>
        </dict>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryUserDefaults</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>CA92.1</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

Now that the file has been created, it needs to be placed properly when building the app. The `PrivacyInfo.xcprivacy` is consider a resource when it is time to build the bundle. In accordance with [Placing Content ina Bundle](https://developer.apple.com/documentation/bundleresources/placing_content_in_a_bundle) the file is placed in the root of the Bundle. Use the proper set of instructions below for your ptoject type:

## .NET MAUI
1. Copy the `PrivacyInfo.xcprivacy` from your documents folder to the `Platforms/iOS` folder in your .NET MAUI project.
1. In your favorite text editor, edit the .NET MAUI csproj project file.
1. Add the following elements to the bottom of the root  `<Project>` element:
    ```xml
    <ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">
      <BundleResource Include="Platforms\iOS\PrivacyInfo.xcprivacy" LogicalName="PrivacyInfo.xcprivacy" />
    </ItemGroup>
    ```
    This will package the file into the iOS app at the root of the bundle. 

## .NET for iOS (net?-ios) 
1. Copy the `PrivacyInfo.xcprivacy` from your documents folder to the `Resources` folder in your .NET for iOS project. This is all that is needed to package the file into the iOS app at the root of the bundle.

## .NET for tvOS (net?-tvos)
1. Copy the `PrivacyInfo.xcprivacy` from your documents folder to the root folder of your .NET for tvOS project.
1. In your favorite text editor, edit the .NET for tvOS csproj project file.
1. Add the following elements to the bottom of the root  `<Project>` element:
    ```xml
    <ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'tvos'">
      <BundleResource Include="PrivacyInfo.xcprivacy" LogicalName="PrivacyInfo.xcprivacy" />
    </ItemGroup>
    ```
    This will package the file into the tvOS app at the root of the bundle. 

## Xamarin iOS including Xamarin.Forms
1. Copy the `PrivacyInfo.xcprivacy` from your documents folder to the root folder of your Xamarin.iOS project.
1. In your favorite text editor, edit the Xamarin.iOS csproj project file.
1. Locate the `<ItemGroup>` that contains other `<BundleResource>` elements and add the following element:
    ```xml
    <BundleResource Include="PrivacyInfo.xcprivacy" LogicalName="PrivacyInfo.xcprivacy" />
    ```
    This will package the file into the iOS app at the root of the bundle. 


Once added to your project, the `PrivacyInfo.xcprivacy` file will need to be updated if there are any additional API usages from additional categories or additional reasons for usage. This will include adding a NuGet package or Binding project that calls into any of Apple’s [Required Reason APIs][RequiredReasonAPI]. It is ultimately yuour responsibility to provide an accurate `PrivacyInfo.xcprivacy` file, failing to do so may result in the App Store rejecting your submission.

[RequiredReasonAPI]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api
[PrivacyManifestFiles]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files
[C#NETAPIs]: #c-net-apis-in-net-maui
[FileTimestampAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278393
[SystemBootTimeAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278394
[DiskSpaceAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278397
[ActiveKeyboardAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278400
[UserDefaultsAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278401