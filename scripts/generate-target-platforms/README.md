# generate-target-platforms

This script takes the $(TOP)/builds/Versions-<platform>.plist document as input, and generates the `Microsoft.<platform>.Sdk.SupportedTargetPlatforms.props` file as output, where we:

* List all the supported OS versions for a given platform (`SdkSupportedTargetPlatformVersion`).
* State the minimum OS version we support (`MinSupportedOSPlatformVersion`).
