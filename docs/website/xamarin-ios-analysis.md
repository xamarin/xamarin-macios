---
id: C29B69F5-08E4-4DCC-831E-7FD692AB0886
title: "Xamarin.iOS Analysis Rules"
dateupdated: 2017-06-26
---

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

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

- **Problem:** Not using the float32 option (--aot-options=-O=float32) leads to hefty performance cost, specially on mobile, where double precision math is measurably slower. Note that .NET uses double precision internally, even for float, so enabling this option affects precision and, possibly, compatibility.
- **Fix:** Double click on your iOS project, go to Build > iOS Build and uncheck the "Perform all 32-bit float operations as 64-bit float".
