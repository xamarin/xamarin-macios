TOP=../../..
include $(TOP)/Make.config

define INTEGRATION_TEST_template

TEST_BUILD_TARGETS += $(1)/bin/Debug/$(1)Integration.dll
$(1)/bin/Debug/$(1)Integration.dll: $(wildcard *.cs */*.cs Makefile) $(1)/$(1)Integration.csproj
	$(Q) $(SYSTEM_MSBUILD) $(1)/$(1)Integration.csproj $(MSBUILD_VERBOSITY)

TEST_CLEAN_TARGETS += clean-$(1)
clean-$(1):
	$(Q) $(SYSTEM_MSBUILD) /t:Clean $(1)/$(1)Integration.csproj $(MSBUILD_VERBOSITY)
	$(Q) rm -rf Converted/$(1)

TEST_RUN_TARGETS += run-$(1)
run-$(1): $(1)/bin/Debug/$(1)Integration.dll
	$(Q) rm -rf Converted/$(1)
	$(Q) mkdir -p Converted/$(1)
	-$(Q) $(SYSTEM_DOTNET) run --project ../nnyeah/nnyeah.csproj --input=$(1)/bin/Debug/$(1)Integration.dll --output=Converted/$(1)/$(1)Integration.dll --xamarin-assembly=$(2) --microsoft-assembly=$(3)

endef

$(eval $(call INTEGRATION_TEST_template,macOS,../../../_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/mono/Xamarin.Mac/Xamarin.Mac.dll,../../../_build/Microsoft.macOS.Ref/ref/net6.0/Microsoft.macOS.dll))
$(eval $(call INTEGRATION_TEST_template,iOS,../../../_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.iOS/Xamarin.iOS.dll,../../../_build/Microsoft.iOS.Ref/ref/net6.0/Microsoft.iOS.dll))

all-local:: $(TEST_BUILD_TARGETS)
install-local:: all-local


run-tests:: $(TEST_RUN_TARGETS)

clean: $(TEST_CLEAN_TARGETS)
