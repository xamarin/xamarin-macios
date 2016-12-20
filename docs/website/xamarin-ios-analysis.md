id:{c29b69f5-08e4-4dcc-831e-7fd692ab0886}
title:Xamarin.iOS Analysis Rules

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

<h3><a name="XIA0001"/>XIA0001: DisabledLinkerRule</h3>

* **Problem:** The linker is disabled on device for the debug mode.
* **Fix:** You should try to run your code with the linker to avoid any surprises.
To set it up, go to Project > iOS Build > Linker Behavior.

<h3><a name="XIA0002"/>XIA0002: TestCloudAgentReleaseRule</h3>

* **Problem:** App builds that initialize the Test Cloud agent will be rejected by Apple when submitted, as they use private API.
* **Fix:** Add or fix the necessary #if and defines in code.

<h3><a name="XIA0003"/>XIA0003: IPADebugBuildsRule</h3>

* **Problem:** Debug configuration that uses developer signing keys should not generate an IPA as it is only needed for distribution, which now uses the Publishing Wizard.
* **Fix:** Disable IPA build in Project Options for the Debug configuration.

