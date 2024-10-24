TOP=../../../..
TESTNAME=DisposeTaggedPointersTestApp
include $(TOP)/tests/common/shared-dotnet.mk

export RUNTIMEIDENTIFIER=
export RUNTIMEIDENTIFIERS=

.stamp-restore:
	$(Q) $(DOTNET) restore
	$(Q) touch $@

define Test
clean-$(2):
	$(Q) rm -f .stamp-build-$(2)
	$(Q) rm -rf bin/$(1) obj/$(1)
CLEAN_TARGETS+=clean-$(2)

.stamp-build-$(2): .stamp-restore
	$(MAKE) build CONFIG=$(1) BUILD_ARGUMENTS="/tl:off $(3)"
	$(Q) touch $$@
BUILD_TARGETS+=.stamp-build-$(2)

run-$(2): export TEST_CASE=$(1)
run-$(2): .stamp-build-$(2)
	$(MAKE) run-bare CONFIG=$(1)
RUN_TARGETS+=run-$(2)

test-$(2): run-$(2)
endef

$(eval $(call Test,Default,default))
$(eval $(call Test,DisableWithAppContext,disablewithappcontext))
$(eval $(call Test,EnableWithAppContext,enablewithappcontext))
$(eval $(call Test,DisableWithMSBuildPropertyUntrimmed,disablewithmsbuildpropertyuntrimmed,/p:DisposeTaggedPointers=false /p:_LinkMode=None))
$(eval $(call Test,EnableWithMSBuildPropertyUntrimmed,enablewithmsbuildpropertyuntrimmed,/p:DisposeTaggedPointers=true /p:_LinkMode=None))
$(eval $(call Test,DisableWithMSBuildPropertyTrimmed,disablewithmsbuildpropertytrimmed,/p:DisposeTaggedPointers=false /p:_LinkMode=SdkOnly))
$(eval $(call Test,EnableWithMSBuildPropertyTrimmed,enablewithmsbuildpropertytrimmed,/p:DisposeTaggedPointers=true /p:_LinkMode=SdkOnly))

build-all: $(BUILD_TARGETS)
clean-all:: $(CLEAN_TARGETS)
	$(Q) rm -f .stamp-* *.binlog
test-all run-tests run-all run: $(RUN_TARGETS)

.PHONY: $(TEST_CASES)
