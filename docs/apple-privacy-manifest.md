# Apple’s privacy manifest policy requirements
Apple is introducing new privacy policies that affect .NET applications targeting macOS, Mac Catalyst, iOS, iPadOS, and tvOS platforms on the App Stores. The policies affect how app developers are expected to disclose how they use any data collected from a users device and why certain APIs are being used. Developers will provide the details for these in a [privacy manifest file][PrivacyManifestFiles] that is included in the application bundle.

The [data collection](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_data_use_in_privacy_manifests) policies require you to declare the types of data that you collect and describe the data in a [privacy manifest files][PrivacyManifestFiles]. Apart from how to properly include the [privacy manifest files][PrivacyManifestFiles] in your .NET project, this document does not go into any detail on the data collection settings. The Apple documentation should be sufficient for declaring how your App or framework uses data it collects from users.

The [Required Reason APIs][RequiredReasonAPI] policy forces app developers to identify certain categories of APIs that their app, or frameworks use and to provide a reason for their use. This information is provided in a [privacy manifest files][PrivacyManifestFiles] along with the data collection information. This document will provide .NET application developers the information needed to provide a `PrivacyInfo.xcprivacy` file with thier .NET applications with the proper  policy settings to pass the [Required Reason APIs][RequiredReasonAPI] checks when submitting to the Apple App Stores.

The privacy manifest file (`PrivacyInfo.xcprivacy`) lists the [types of data](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_data_use_in_privacy_manifests) your .NET MAUI application, or any third-party SDKs and packages collect, and the reasons for using certain [Required Reason APIs][RequiredReasonAPI] categories.

**Important:** If the use of the [Required Reason APIs][RequiredReasonAPI] by you or third-party SDKs isn’t declared in the privacy manifest, your application might be rejected by the App Store. For more information, visit Apple’s documentation on [Required Reasons APIs][RequiredReasonAPI].

## Prepare your .NET MAUI applications for Apple’s privacy manifest policy
You must review your native code, C# code, and data collection and tracking practices to understand if Apple’s privacy manifest policy applies to you. Follow these guidelines to decide if you need to include a privacy manifest file in your product:

* If your application includes any third-party SDKs or packages, then these third-party components (if applicable) must provision their own privacy manifest files separately. **Note:** It’s your responsibility however, to make sure that the owners of these third-party components include privacy manifest files. Microsoft isn’t responsible for any third-party privacy manifest, and their data collection and tracking practices.

* If your application includes the [.NET APIs][C#NETAPIs] that call certain native APIs listed in the Apple’s [Required Reason API][RequiredReasonAPI] categories, then you must assess your product for the API usage. For assessing what constitutes as part of data collection and tracking practices, refer to Apple’s documentation on [privacy manifest files][PrivacyManifestFiles]. **Note:** It’s your responsibility to assess your use of each of these APIs and declare the applicable reasons for using them.

Depending on whether you’re using [.NET for iOS or .NET MAUI to develop an application](#privacy-manifest-for-net-maui-and-net-for-ios-or-tvos-applications) or providing [ObjectiveC or Swift Binding packages](#privacy-manifest-for-binding-projects) to use with .NET MAUI applications, the requirement for providing a privacy manifest file might differ.

**Note:** The above guidelines are provided for your convenience. It’s important that you review Apple’s documentation on [privacy manifest files][PrivacyManifestFiles] before creating a privacy manifest for your project.

**Important:**
The following information is provided based on Apple's documentation as of March 2024. It is recommended that you review Apple’s documentation on [privacy manifest files][PrivacyManifestFiles] when creating a privacy manifest for your project to ensure you are using the most recent guidelines. If you find any discrepancies in the information below, please [file a bug](https://github.com/dotnet/maui/issues) and include the API in question.

## Privacy manifest for .NET MAUI and .NET for iOS or tvOS applications  
If you’re developing an application using .NET MAUI, consider the following steps:

1. Assess if your native application code uses any of the following APIs:
    * APIs listed under the [Required Reason API][RequiredReasonAPI] category.
    * The [C# .NET APIs][C#NETAPIs] in .NET MAUI framework.
1. If you meet one or both of the conditions from step 1, or if you have disabled [linking](https://learn.microsoft.com/xamarin/ios/deploy-test/linker?tabs=macos), which will retain all of the [C# .NET APIs][C#NETAPIs], then [create a privacy manifest file](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files#4284009) following the [example](#example) to add a `PrivacyInfo.xcprivacy` file to your project.

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
Binding projects fall into two categories, static framework bindings, or dynamic framework bindings. Privacy manifests should be included with the framework by [placing the privacy manifest in the framework bundle](https://developer.apple.com/documentation/bundleresources/placing_content_in_a_bundle#3875930). If placed properly, then the binding project can automatically place them in the app bundle properly so app developers do not need to provide reasons for the framework api usages. 

For a dynamic framework, the framework is added to the app bundle. The privacy policy manifest is in the location that the App Store expects it to be, and there is nothing for the app developer to do.

At this time, when binding a static framework, app developers will be required to manually merge the privacy manifest included with the static framework into the app privacy manifest. This is due to how static libraries are linked into the main app binary removing the need for the framework.

## C# .NET APIs in .NET MAUI
.NET for iOS, tvOS and .NET MAUI build on top of the .NET runtime and BCL. Each SDK's API usages are detailed in the links below. 

* [.NET Runtime and BCL API usages]()
* [.NET for iOS, tvOS and Xamarin.iOS API Usages]()
* [.NET MAUI and Xamarin.Forms API Usages]()

All .NET Apps that target devices running iOS, iPadOS, or tvOS will require a `PrivacyInfo.xcprivacy` file in the app bundle. This is due to the .NET Runtime and BCL using [Required Reasons APIs][RequiredReasonAPI] that are not removed regardless of the [linking](https://learn.microsoft.com/xamarin/ios/deploy-test/linker?tabs=macos) setting. The following three API categories and their associated reasons must be in the `PrivacyInfo.xcprivacy`. 
* NSPrivacyAccessedAPICategoryFileTimestamp - C617.1
* NSPrivacyAccessedAPICategorySystemBootTime - 35F9.1
* NSPrivacyAccessedAPICategoryDiskSpace - E174.1

Additionally, if you use the `NSUserDefaults` APIs in your app, you will need to add the `NSPrivacyAccessedAPICategoryUserDefaults` API category, with a reason code of `CA92.1`.

See the example below for detailed instructions on how to add a `PrivacyInfo.xcprivacy` file to your App.

# Example
Let's look at how you would add a Privacy Manifest file to an application that uses the following APIs:

* [NSUserDefaults](https://learn.microsoft.com/dotnet/api/foundation.nsuserdefaults) 
* [NSProcessInfo.SystemUptime](https://learn.microsoft.com/dotnet/api/foundation.nsprocessinfo.systemuptime)
* [NSFileManager.ModificationDate](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.modificationdate)

How they are used is not that important for this example, but the `why` will determine the reason code needed for the privacy manifest.

## Adding the `PrivacyInfo.xcprivacy` file to your project

The `PrivacyInfo.xcprivacy` is consider a resource when it is time to build the bundle. In accordance with [Placing Content in a Bundle](https://developer.apple.com/documentation/bundleresources/placing_content_in_a_bundle) the file is placed in the root of the Bundle. Use the proper set of instructions below for your project type:

### .NET MAUI
1. Create a new blank file named `PrivacyInfo.xcprivacy` in the `Platforms/iOS` folder in your .NET MAUI project.
1. In your favorite text editor, edit the .NET MAUI csproj project file.
1. Add the following elements to the bottom of the root  `<Project>` element:
    ```xml
    <ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">
      <BundleResource Include="Platforms\iOS\PrivacyInfo.xcprivacy" LogicalName="PrivacyInfo.xcprivacy" />
    </ItemGroup>
    ```
    This will package the file into the iOS app at the root of the bundle. 

### .NET for iOS (net?-ios) 
1. Create a new blank file named `PrivacyInfo.xcprivacy` in the `Resources` folder in your .NET for iOS project. This is all that is needed to package the file into the iOS app at the root of the bundle.

### .NET for tvOS (net?-tvos)
1. Create a new blank file named `PrivacyInfo.xcprivacy` in the root folder in your .NET for tvOS project.
1. In your favorite text editor, edit the .NET for tvOS csproj project file.
1. Add the following elements to the bottom of the root  `<Project>` element:
    ```xml
    <ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'tvos'">
      <BundleResource Include="PrivacyInfo.xcprivacy" LogicalName="PrivacyInfo.xcprivacy" />
    </ItemGroup>
    ```
    This will package the file into the tvOS app at the root of the bundle. 

### Xamarin.iOS including Xamarin.Forms
1. Create a new blank file named `PrivacyInfo.xcprivacy` in the root folder of your Xamarin.iOS project.
1. In your favorite text editor, edit the Xamarin.iOS csproj project file.
1. Locate the `<ItemGroup>` that contains other `<BundleResource>` elements and add the following element:
    ```xml
    <BundleResource Include="PrivacyInfo.xcprivacy" LogicalName="PrivacyInfo.xcprivacy" />
    ```
    This will package the file into the iOS app at the root of the bundle. 

Now that the file has been created in the proper location your project, it needs to be ppopulated with the correct settings for your app or framework. 

## Creating the `PrivacyInfo.xcprivacy` file

We will start by building the contents of the `PrivacyInfo.xcprivacy` file, and then go through each supported platform and how to properly configure your project so the file is included in the bundle properly.

Since the application is based on .NET, a privacy manifest is required. Let's walkthrough the steps needed to add a privacy manifest that declares our usages of the above three API's.

1. Open Xcode and either create new `App` project, or open an existing one.
1. In Xcode use the File->New->File menu to open the `Choose a new template for your file:` dialog box.
1. Scroll down until you find the `App Privacy` template.
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

You can add the entries for the APIs usage to the `PrivacyInfo.xcproject` as follows: 

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
1. Since the .NET runtime and BCL include APIs from the [File timestamp][FileTimestampAPIs], [System boot time][SystemBootTimeAPIs], [Disk space][DiskSpaceAPIs] API categories, add the following to the `NSPrivacyAccessedAPITypes` array:
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
1. For the [NSProcessInfo.SystemUptime](https://learn.microsoft.com/dotnet/api/foundation.nsprocessinfo.systemuptime), a reason code of `35F9.1` is needed since that is the only reason code available. But since the .NET runtime and BCL already included that category and reason, there is nothing additional to add.

1. For the [NSFileManager.ModificationDate](https://learn.microsoft.com/dotnet/api/foundation.nsfilemanager.modificationdate), a reason code of `C617.1` is needed since the modification dates are stored as a hash using [NSUserDefaults](https://learn.microsoft.com/dotnet/api/foundation.nsuserdefaults) but not displayed to the user. Again, the .NET runtime and BCL requirements have already satisfied this category and reason so no additional changes are needed.

1. For the [NSUserDefaults](https://learn.microsoft.com/dotnet/api/foundation.nsuserdefaults), a reason code of `CA92.1` is provided since the data accessed is only accessible to the app itself. Add the following element to the `NSPrivacyAccessedAPITypes` array:
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

Once added to your project, the `PrivacyInfo.xcprivacy` file will need to be updated if there are any additional API usages from additional categories or additional reasons for usage. This will include adding a NuGet package or Binding project that calls into any of Apple’s [Required Reason APIs][RequiredReasonAPI]. It is ultimately your responsibility to provide an accurate `PrivacyInfo.xcprivacy` file, failing to do so may result in the App Store rejecting your submission.

[RequiredReasonAPI]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api
[PrivacyManifestFiles]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files
[C#NETAPIs]: #c-net-apis-in-net-maui
[FileTimestampAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278393
[SystemBootTimeAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278394
[DiskSpaceAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278397
[ActiveKeyboardAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278400
[UserDefaultsAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278401
