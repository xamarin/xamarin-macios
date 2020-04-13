## Unofficial MSBuild Project SDK support

 This is an unofficial package which introduces a Xamarin.iOS [MSBuild Project SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk).
 In some ways this is a standalone implementation / extension of MSBuild.Sdk.Extras for Xamarin.iOS.

 The `TargetFramework` element is the only required `.csproj` file element.

 ```xml
<Project Sdk="Microsoft.iOS.Sdk">
  <PropertyGroup>
    <TargetFramework>xamarinios10</TargetFramework>
  </PropertyGroup>
</Project>
 ```

Default iOS related file globbing behavior is defined in `Microsoft.iOS.Sdk.DefaultItems.props`.

# How to get started

To use the package locally without installing it:

* Create a new directory.

    ```shell
    mkdir -p ~/dotnet-ios-test
    cd ~/dotnet-ios-test
    ```

* Download this nuget for Xamarin.iOS ([Xamarin.iOS.Sdk.13.19.0-alpha.242.nupkg][1]):

    ```shell
    curl -LO https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/net5-preview/92fc89d8ba0b743e207e13a666cb9c96add38f04/6/package/Xamarin.iOS.Sdk.13.19.0-alpha.242.nupkg
    ```

* Add this `NuGet.config` to the directory:

    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
            <config>
                    <add key="repositorypath" value="packages" />
            </config>
            <packageSources>
                    <add key="Nuget Official" value="https://www.nuget.org/api/v2/" />
            </packageSources>
    </configuration>
    ```

* Install the nuget into a local feed:

    ```shell
    nuget add *.nupkg -source local-feed
    nuget sources add -Name local-feed -Source $PWD/local-feed -ConfigFile NuGet.config
    ```

* Add this `global.json` to the directory (remember to change the versions to the exact versions of the package):

    ```json
    {
            "msbuild-sdks": {
                    "Xamarin.iOS.Sdk": "13.19.0-alpha.242+92fc89d8b"
            }
    }
    ```

* Create a new project file (`myproject.csproj`):

    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <Project Sdk="Xamarin.iOS.Sdk">
      <PropertyGroup>
        <TargetFramework>xamarinios10</TargetFramework>
      </PropertyGroup>
    </Project>
    ```

* Create source code (`AppDelegate.cs`):

    ```csharp
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Foundation;
    using UIKit;
    
    public class AppDelegate : UIApplicationDelegate
    {
    	UIWindow window;
    	UIViewController dvc;
    
    	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
    	{
    		window = new UIWindow (UIScreen.MainScreen.Bounds);
    
    
    		dvc = new UIViewController ();
    		dvc.View.BackgroundColor = UIColor.Green;
    		window.RootViewController = dvc;
    		window.MakeKeyAndVisible ();
    
    		return true;
    	}
    
    	static void Main (string[] args)
    	{
    		UIApplication.Main (args, null, typeof (AppDelegate));
    	}
    }
    ```

* Create `Info.plist`:

    ```xml
    <?xml version="1.0" encoding="UTF-8"?>
    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
    <plist version="1.0">
    <dict>
    	<key>CFBundleDevelopmentRegion</key>
    	<string>English</string>
    	<key>CFBundleIdentifier</key>
    	<string>com.xamarin.myproject</string>
    	<key>CFBundleInfoDictionaryVersion</key>
    	<string>6.0</string>
    	<key>CFBundleName</key>
    	<string>MyProject</string>
    	<key>CFBundlePackageType</key>
    	<string>APPL</string>
    	<key>CFBundleSignature</key>
    	<string>????</string>
    	<key>CFBundleVersion</key>
    	<string>1.0</string>
    	<key>LSRequiresIPhoneOS</key>
    	<true/>
    	<key>UIDeviceFamily</key>
    	<array>
    		<integer>1</integer>
    		<integer>2</integer>
    	</array>
    	<key>CFBundleDisplayName</key>
    	<string>MyProject</string>
    	<key>MinimumOSVersion</key>
    	<string>11.0</string>
    </dict>
    </plist>
    ```

* Build

    ```shell
    export MD_MTOUCH_SDK_ROOT=$HOME/.nuget/packages/xamarin.ios.sdk/13.19.0-alpha.242
    dotnet build /p:Platform=iPhone
    ```

* Run (but first rename your device so that it matches my device's name)

    ```shell
    ~/.nuget/packages/xamarin.ios.sdk/13.19.0-alpha.242/tools/bin/mlaunch --installdev bin/iPhone/Debug/Device/xamarinios10/myproject.app --devname "Rolf's iPhone 7 - iOS 11.4.1"
    ~/.nuget/packages/xamarin.ios.sdk/13.19.0-alpha.242/tools/bin/mlaunch --launchdev bin/iPhone/Debug/Device/xamarinios10/myproject.app --devname "Rolf's iPhone 7 - iOS 11.4.1" --wait-for-exit
    ```

[1]: https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/net5-preview/92fc89d8ba0b743e207e13a666cb9c96add38f04/6/package/Xamarin.iOS.Sdk.13.19.0-alpha.242.nupkg
