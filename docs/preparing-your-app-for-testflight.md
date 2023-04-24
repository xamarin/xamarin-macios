# Preparing your App for TestFlight

Publishing to the App Store requires some preparation. This guide should help you get started with using Transporter and the App Connect site to upload your app to the App Store for internal testing via TestFlight.

There is some preparation involved with preparing your Apple Developer account for publishing, such as preparing an App Identifier and a Provisioning Profile. The MAUI documentation has a great guide for that: https://learn.microsoft.com/en-us/dotnet/maui/mac-catalyst/deployment/publish-app-store?view=net-maui-7.0

Also, your project will need to have configuration values set in the .csproj file, the Info.plist file, and the Entitlements.plist file.

## Preparing your Project File (.csproj) for App Store submission

Before you can submit your app to the App Store, you need to make sure that it is correctly configured. This includes:

- Setting the build to reference the correct `$(Entitlements.plist)` in your project.
- Setting the correct `$(ApplicationId)` in your project.
- Setting the correct `$(ApplicationVersion)` in your project.
- Setting the correct `$(CodesignKey)` in your project.
- Setting the correct `$(PackagingSigningKey)` in your project.
- Setting the correct `$(CodesignEntitlements)` in your project.
- Setting the correct `$(CodesignProvision)` in your project.

### Info.plist

The `$(InfoPlist)` is the path to the `Info.plist` file that is used to configure your app. The `Info.plist` file is used by the build system to set the `INFOPLIST_FILE` in the `Info.plist` file. The `INFOPLIST_FILE` is used by the build system to configure your app.

The `$(Category)` is the category of your app. It is used by the App Store to group your app with other apps. It is also used by the App Store to determine which apps are shown in the "New Apps We Love" section of the App Store. The `$(Category)` is set in the project file, and it is used by the build system to set the `Category` in the `Info.plist` file. The `Category` is used by the App Store to identify your app.

The `$(ApplicationId)` is the bundle identifier for your app. It is used by the App Store to identify your app. The `$(ApplicationId)` is set in the project file, and it is used by the build system to set the `CFBundleIdentifier` in the `Info.plist` file. The `CFBundleIdentifier` is used by the App Store to identify your app.

The `$(ApplicationVersion)` is the version of your app. It is used by the App Store to identify your app. The `$(ApplicationVersion)` is set in the project file, and it is used by the build system to set the `CFBundleShortVersionString` in the `Info.plist` file. The `CFBundleShortVersionString` is used by the App Store to identify your app.

### Entitlements.plist

The `$(CodesignEntitlements)` is the path to the `Entitlements.plist` file that is used to sign your app. The `Entitlements.plist` file is used by the build system to set the `CODE_SIGN_ENTITLEMENTS` in the `Info.plist` file. The `CODE_SIGN_ENTITLEMENTS` is used by the build system to sign your app. 

## Building your App for Publishing

Once you've set all the correct values, you can build your app for publishing. You can build your app for publishing very easily by using the dotnet build command. Here is an example for building a MacCatalyst app with .NET 7.0:

`dotnet build -f:net7.0-maccatalyst -c:Release`

## App Store Connect

App Store Connect is the web site that you use to manage your apps on the App Store. You can use App Store Connect to upload your app to the App Store, and you can use App Store Connect to manage your app once it is on the App Store.

### Creating an App in App Store Connect

Before you can upload your app to the App Store, you need to create an app on App Store Connect. You can create an app on App Store Connect by following these steps:

1. Go to [https://appstoreconnect.apple.com](https://appstoreconnect.apple.com).
2. Click on "My Apps".
3. Click on the "+" button.
4. Enter the name of your app.
5. Select the platform for your app.
6. Select the primary language for your app.
7. Select the category for your app. Note: It is important that the primary category you select here will be the one 
8. Click on "Create".

# Uploading your App using Transporter

Once you have created an app on App Store Connect, you can upload your app to the App Store. You can upload your app to the App Store by following these steps:

1. Go to the Mac App Store and download the Transporter app.
2. Open the Transporter app.
3. Click on "Add".
4. Click on "Add file".
5. Select the `.ipa` or `.pkg` file that you want to upload.
6. Click on "Open".
7. Click on "Upload".

# Setting up an Internal Test Flight

Once you have uploaded your app to the App Store, you can set up an internal Test Flight. You can set up an internal Test Flight by following these steps:

1. Go to [https://appstoreconnect.apple.com](https://appstoreconnect.apple.com).
2. Click on "My Apps".
3. Click on the app that you want to set up an internal Test Flight for.
4. Click on "TestFlight".
5. Click on "Create Beta Group".
6. Enter the name of the internal Test Flight.
7. Click on "Create".

Note: Careful to add testers to the group directly, and not to an individual build. There seems to be an issue with Apple incorrectly assuming that the XCode version was a beta build and will refuse to add testers to the build.

# Adding Testers to an Internal Test Flight

Once you have set up an internal Test Flight, you can add testers to the internal Test Flight. You can add testers to the internal Test Flight by following these steps:

1. Go to [https://appstoreconnect.apple.com](https://appstoreconnect.apple.com).
2. Click on "My Apps".
3. Click on the app that you want to add testers to.
4. Click on "TestFlight".
5. Click on the internal Test Flight that you want to add testers to.
6. Click on "Add Testers".
7. Click on "Add by Email".
8. Enter the email address of the tester.
9. Click on "Add".

# Adding a Build to an Internal Test Flight

Once you have added testers to an internal Test Flight, you can add a build to the internal Test Flight. You can add a build to the internal Test Flight by following these steps:

1. Go to [https://appstoreconnect.apple.com](https://appstoreconnect.apple.com).
2. Click on "My Apps".
3. Click on the app that you want to add a build to.
4. Click on "TestFlight".
5. Click on the internal Test Flight that you want to add a build to.
6. Click on "Builds".
7. Click on "Add Build".
8. Select the build that you want to add.
9. Click on "Add".

# Distributing an Internal Test Flight

Once you have added a build to an internal Test Flight, you can distribute the internal Test Flight. You can distribute the internal Test Flight by following these steps:

1. Go to [https://appstoreconnect.apple.com](https://appstoreconnect.apple.com).
2. Click on "My Apps".
3. Click on the app that you want to distribute.
4. Click on "TestFlight".
5. Click on the internal Test Flight that you want to distribute.
6. Click on "Distribute External Testers".
7. Click on "Distribute".

# Downloading an Internal Test Flight

Once you have distributed an internal Test Flight, you can download the internal Test Flight. You can download the internal Test Flight by following these steps:

1. Download the "testflight" app from the Mac App Store.
2. Open the "testflight" app.
3. Click on "Sign In".
4. Enter your Apple ID.
5. Enter your password.
6. Click on "Sign In".
7. Click on the app that you want to download.
8. Click on "Download".

# Submitting new builds to Internal Test Flight

Fortunately, the process of submitting new builds to an internal Test Flight is very simple. You can submit new builds to an internal Test Flight by following these steps:

1. Increment the build number in your project by changing the Bundle Version in the Info.plist file.
2. Build your app.
3. Upload your app to the App Store via Transporter.
4. Open the "testflight" app. You should see an Update option for your app.
