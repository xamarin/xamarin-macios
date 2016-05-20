# Linker Regression Tests

Most of our unit tests follow the default project configuration, which is 
**"Don't Link"** for simulator builds and **"Link SDK"** for device builds.

The *linker* tests are different as they set both simulator and device
configuration to the same settings. This is the main way to test the linker
on the simulator (which is the most common test configuration for our bots)

Note that the spaces in the projects directory names are **by design** as
it ensure our tool chain can cope with them.


## dont link

* regression testing **without** using the linker

* both simulator and devices are set to "Don't link"


## link all

* regression testing using the linker on the **all** assemblies

* "Link all assemblies" for both simulator/devices


## link sdk

* regression testing using the linker on the **SDK** assemblies

* "Link SDK assemblies only" for both simulator/devices
