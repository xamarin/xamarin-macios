TOP=../../..
include $(TOP)/Make.config

define INTEGRATION_TEST_template

TEST_BUILD_TARGETS += $(1)/bin/Debug/$(1)Integration.dll
$(1)/bin/Debug/$(1)Integration.dll: $(wildcard *.cs */*.cs Makefile) $(1)/$(1)Integration.csproj
	$(Q) $(SYSTEM_MSBUILD) $(1)/$(1)Integration.csproj $(MSBUILD_VERBOSITY)

TEST_CLEAN_TARGETS += clean-$(1)
clean-$(1):
	$(Q) $(SYSTEM_MSBUILD) /t:Clean $(1)/$(1)Integration.csproj $(MSBUILD_VERBOSITY)

TEST_RUN_TARGETS += run-$(1)
run-$(1): $(1)/bin/Debug/$(1)Integration.dll
	-$(Q) $(SYSTEM_DOTNET) run --project ../nnyeah/nnyeah.csproj --input=$(1)/bin/Debug/$(1)Integration.dll --output=Converted/$(1)Integration.dll

endef

$(eval $(call INTEGRATION_TEST_template,macOS))
$(eval $(call INTEGRATION_TEST_template,iOS))
$(eval $(call INTEGRATION_TEST_template,tvOS))

all-local:: $(TEST_BUILD_TARGETS)
install-local:: all-local

prep-tests:
	$(Q) rm -rf converted
	$(Q) mkdir -p converted	

run-tests:: prep-tests $(TEST_RUN_TARGETS)

clean: $(TEST_CLEAN_TARGETS)
