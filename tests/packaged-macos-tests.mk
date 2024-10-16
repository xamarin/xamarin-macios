TOP=..

include $(TOP)/Make.config

export DOTNET=$(shell which dotnet)

ifeq ($(shell arch),"arm64")
IS_ARM64=1
IS_APPLE_SILICON=1
endif
ifeq ($(shell sysctl -n sysctl.proc_translated 2>/dev/null),1)
IS_ROSETTA=1
IS_APPLE_SILICON=1
endif

CONFIG?=Debug
LAUNCH_ARGUMENTS=--autostart --autoexit

include $(TOP)/scripts/run-with-timeout/fragment.mk

# Time test runs out after 5 minutes (300 seconds)
LAUNCH_WITH_TIMEOUT=$(RUN_WITH_TIMEOUT_EXEC) 300
# Some tests need a bit more time... (introspection, monotouch-test)
LAUNCH_WITH_TIMEOUT_LONGER=$(RUN_WITH_TIMEOUT_EXEC) 600

### .NET dependency projects

# We have library projects that are used in multiple test projects.
# If those test projects are built in parallel, these library projects
# might be built in multiple build processes at once, and that
# may turn into build errors when the simultaneous builds stomp
# on eachother. So here we build those library projects first, serialized,
# so that when the test projects need them, they're already built.

define DotNetDependentProject
.stamp-dotnet-dependency-$(2)-$(1): Makefile
	$$(Q) $$(MAKE) -C "$(1)/dotnet/$(2)" build
	$$(Q) touch $$@

.stamp-dotnet-dependency-$(2):: .stamp-dotnet-dependency-$(2)-$(1)
	$$(Q) touch $$@
endef
$(eval $(call DotNetDependentProject,BundledResources,macOS))
$(eval $(call DotNetDependentProject,BundledResources,MacCatalyst))
$(eval $(call DotNetDependentProject,EmbeddedResources,macOS))
$(eval $(call DotNetDependentProject,EmbeddedResources,MacCatalyst))

### .NET normal tests

define DotNetNormalTest
# macOS/.NET/x64
build-mac-dotnet-x64-$(1): .stamp-dotnet-dependency-macOS
	$$(Q) $$(MAKE) -C "$(1)/dotnet/macOS" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=osx-x64

exec-mac-dotnet-x64-$(1): $(RUN_WITH_TIMEOUT)
	@echo "ℹ️  Executing the '$(1)' test for macOS/.NET (x64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT$(3)) "./$(1)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-x64/$(2).app/Contents/MacOS/$(2)"

# macOS/.NET/arm64
build-mac-dotnet-arm64-$(1): .stamp-dotnet-dependency-macOS
	$$(Q) $$(MAKE) -C "$(1)/dotnet/macOS" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=osx-arm64

exec-mac-dotnet-arm64-$(1): $(RUN_WITH_TIMEOUT)
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(1)' test for macOS/.NET (arm64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT$(3)) "./$(1)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-arm64/$(2).app/Contents/MacOS/$(2)"
else
	@echo "⚠️  Not executing the '$(1)' test for macOS/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif

# MacCatalyst/.NET/x64
build-maccatalyst-dotnet-x64-$(1): .stamp-dotnet-dependency-MacCatalyst
	$$(Q_BUILD) $$(MAKE) -C "$(1)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-x64

exec-maccatalyst-dotnet-x64-$(1): $(RUN_WITH_TIMEOUT)
	@echo "ℹ️  Executing the '$(1)' test for Mac Catalyst/.NET (x64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT$(3)) "./$(1)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-x64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)

# MacCatalyst/.NET/arm64
build-maccatalyst-dotnet-arm64-$(1):.stamp-dotnet-dependency-MacCatalyst
	$$(Q) $$(MAKE) -C "$(1)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-arm64

exec-maccatalyst-dotnet-arm64-$(1): $(RUN_WITH_TIMEOUT)
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(1)' test for Mac Catalyst/.NET (arm64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT$(3)) "./$(1)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-arm64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)
else
	@echo "⚠️  Not executing the '$(1)' test for Mac Catalyst/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif

build-$(1):
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-x64-$(1)           || echo "build-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-arm64-$(1)         || echo "build-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-x64-$(1)   || echo "build-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-arm64-$(1) || echo "build-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
	$$(Q) if test -e ".$$@-failure.stamp"; then cat ".$$@-failure.stamp"; rm ".$$@-failure.stamp"; exit 1; fi

exec-$(1):
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-x64-$(1)           || echo "exec-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-arm64-$(1)         || echo "exec-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-x64-$(1)   || echo "exec-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-arm64-$(1) || echo "exec-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
	$$(Q) if test -e ".$$@-failure.stamp"; then cat ".$$@-failure.stamp"; rm ".$$@-failure.stamp"; exit 1; fi
endef

$(eval $(call DotNetNormalTest,monotouch-test,monotouchtest,_LONGER))
$(eval $(call DotNetNormalTest,introspection,introspection,_LONGER))

### .NET linker tests

define DotNetLinkerTest
# macOS/.NET/x64
build-mac-dotnet-x64-$(1): .stamp-dotnet-dependency-macOS
	$$(Q_BUILD) $$(MAKE) -C "linker/ios/$(2)/dotnet/macOS" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=osx-x64

exec-mac-dotnet-x64-$(1): $(RUN_WITH_TIMEOUT)
	@echo "ℹ️  Executing the '$(2)' test for macOS/.NET (x64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-x64/$(2).app/Contents/MacOS/$(2)"

# macOS/.NET/arm64
build-mac-dotnet-arm64-$(1): .stamp-dotnet-dependency-macOS
	$$(Q) $$(MAKE) -C "linker/ios/$(2)/dotnet/macOS" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=osx-arm64

exec-mac-dotnet-arm64-$(1): $(RUN_WITH_TIMEOUT)
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(2)' test for macOS/.NET (arm64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-arm64/$(2).app/Contents/MacOS/$(2)"
else
	@echo "⚠️  Not executing the '$(2)' test for macOS/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif

# MacCatalyst/.NET/x64
build-maccatalyst-dotnet-x64-$(1): .stamp-dotnet-dependency-MacCatalyst
	$$(Q_BUILD) $$(MAKE) -C "linker/ios/$(2)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-x64

exec-maccatalyst-dotnet-x64-$(1): $(RUN_WITH_TIMEOUT)
	@echo "ℹ️  Executing the '$(2)' test for Mac Catalyst/.NET (x64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-x64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)

# MacCatalyst/.NET/arm64
build-maccatalyst-dotnet-arm64-$(1): .stamp-dotnet-dependency-MacCatalyst
	$$(Q) $$(MAKE) -C "linker/ios/$(2)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-arm64

exec-maccatalyst-dotnet-arm64-$(1): $(RUN_WITH_TIMEOUT)
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(2)' test for Mac Catalyst/.NET (arm64) ℹ️"
	$$(Q) $(LAUNCH_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-arm64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)
else
	@echo "⚠️  Not executing the '$(2)' test for Mac Catalyst/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif

build-$(1):
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-x64-$(1)           || echo "build-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-arm64-$(1)         || echo "build-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-x64-$(1)   || echo "build-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-arm64-$(1) || echo "build-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
	$$(Q) if test -e ".$$@-failure.stamp"; then cat ".$$@-failure.stamp"; rm ".$$@-failure.stamp"; exit 1; fi

exec-$(1):
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-x64-$(1)           || echo "exec-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-arm64-$(1)         || echo "exec-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-x64-$(1)   || echo "exec-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-arm64-$(1) || echo "exec-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
	$$(Q) if test -e ".$$@-failure.stamp"; then cat ".$$@-failure.stamp"; rm ".$$@-failure.stamp"; exit 1; fi
endef

$(eval $(call DotNetLinkerTest,dontlink,dont link))
$(eval $(call DotNetLinkerTest,linksdk,link sdk))
$(eval $(call DotNetLinkerTest,linkall,link all))
