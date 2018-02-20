---
id: c29b69f5-08e4-4dcc-831e-7fd692ab0886
title: Xamarin.iOS Analysis Rules
dateupdated: 2018-02-15
---

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

Xamarin.iOS analysis is a set of rules that check your project settings to help you determine if better/more optimized settings are available.

Run the analysis rules as often as possible to find possible improvements early on and save development time.

To run the rules, in Visual Studio for Mac's menu, select **Project > Run Code Analysis**.

> ⚠️ **NOTE:** Xamarin.iOS analysis only runs on your currently selected configuration. We highly recommend running the tool for debug **and** release configurations.

### <a name="XIA0001"/>XIA0001: DisabledLinkerRule

- **Problem:** The linker is disabled on device for the debug mode.
- **Fix:** You should try to run your code with the linker to avoid any surprises.
To set it up, go to Project > iOS Build > Linker Behavior.

### <a name="XIA0002"/>XIA0002: TestCloudAgentReleaseRule

- **Problem:** App builds that initialize the Test Cloud agent will be rejected by Apple when submitted, as they use private API.
- **Fix:** Add or fix the necessary #if and defines in code.

### <a name="XIA0003"/>XIA0003: IPADebugBuildsRule

- **Problem:** Debug configuration that uses developer signing keys should not generate an IPA as it is only needed for distribution, which now uses the Publishing Wizard.
- **Fix:** Disable IPA build in Project Options for the Debug configuration.

### <a name="XIA0004"/>XIA0004: Missing64BitSupportRule

- **Problem:** The supported architecture for "release | device" isn't 64 bit compatible, missing ARM64. This is a problem as Apple does not accept 32 bits only iOS apps in the AppStore.
- **Fix:** Double click on your iOS project, go to Build > iOS Build and change the supported architectures so it has ARM64.

### <a name="XIA0005"/>XIA0005: Float32Rule

- **Problem:** Not using the float32 option (--aot-options=-O=float32) leads to hefty performance cost, especially on mobile, where double precision math is measurably slower. Note that .NET uses double precision internally, even for float, so enabling this option affects precision and, possibly, compatibility.
- **Fix:** Double click on your iOS project, go to Build > iOS Build and uncheck the "Perform all 32-bit float operations as 64-bit float".

### <a name="XIA0006"/>XIA0006: HttpClientAvoidManaged

- **Problem:** We recommend using the native HttpClient handler instead of the managed one for better performance, smaller executable size, and to easily support the newer standards.
- **Fix:** Double click on your iOS project, go to Build > iOS Build and change the HttpClient implementation to either NSUrlSession (iOS 7+) or CFNetwork to support version preceding iOS 7.

### <a name="XIA0007"/>XIA0007: UseLLVMRule

- **Problem:** For Release|iPhone configuration we recommend enabling the LLVM compiler which generates code that is faster to execute at the expense of build time.
- **Fix:** Double click on your iOS project, go to Build > iOS Build and for Release|iPhone, check the LLVM optimizing compiler option.

