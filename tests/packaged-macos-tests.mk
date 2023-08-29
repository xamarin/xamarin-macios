TOP=..

include $(TOP)/Make.config

# Env Variables to use local not system XM

export TargetFrameworkFallbackSearchPaths:=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride:=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild

ifeq ($(shell uname -a),"arm64")
IS_ARM64=1
IS_APPLE_SILICON=1
endif
ifeq ($(shell sysctl -n sysctl.proc_translated 2>/dev/null),1)
IS_ROSETTA=1
IS_APPLE_SILICON=1
endif

MACOS_VERSION:=$(shell sw_vers -productVersion)
MACOS_MAJOR_VERSION:=$(shell echo $(MACOS_VERSION) | awk -F'.' '{print $$1}')
MACOS_MINOR_VERSION:=$(shell echo $(MACOS_VERSION) | awk -F'.' '{print $$2}')
SUPPORTS_MACCATALYST:=$(shell echo '$(MACOS_MAJOR_VERSION).$(MACOS_MINOR_VERSION) >= 10.15' | bc)

CONFIG?=Debug
LAUNCH_ARGUMENTS=--autostart --autoexit

# Time test runs out after 5 minutes (300 seconds)
RUN_WITH_TIMEOUT=./run-with-timeout.csharp 300
# Some tests need a bit more time... (introspection, monotouch-test/xammac_tests)
RUN_WITH_TIMEOUT_LONGER=./run-with-timeout.csharp 600

.stamp-configure-projects-mac: Makefile xharness/xharness.exe
	$(Q) $(MAKE) .stamp-xharness-configure
	$(Q) touch $@

PACKAGES_CONFIG:=$(shell git ls-files -- '*.csproj' '*/packages.config' | sed 's/ /\\ /g')
ifdef INCLUDE_XAMARIN_LEGACY
.stamp-nuget-restore-mac: tests-mac.sln $(PACKAGES_CONFIG)
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -t -- /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore tests-mac.sln
	$(Q) touch $@
else
.stamp-nuget-restore-mac:
	$(Q) echo "Legacy Xamarin is disabled, so nothing to restore"
	$(Q) touch $@
endif

#
# dont link
#

## macOS/legacy/modern
build-mac-modern-dontlink: linker/mac/dont\ link/dont\ link-mac.csproj .stamp-nuget-restore-mac
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -- "/property:Configuration=$(CONFIG)" /r /t:Build $(XBUILD_VERBOSITY) "linker/mac/dont link/dont link-mac.csproj"

exec-mac-modern-dontlink:
	@echo "ℹ️  Executing the 'dont link' test for Xamarin.Mac (Modern profile) ℹ️"
	$(Q) "linker/mac/dont link/bin/x86/$(CONFIG)/dont link.app/Contents/MacOS/dont link"

# macOS/legacy/full
build-mac-full-dontlink: linker/mac/dont\ link/generated-projects/full/dont\ link-mac-full.csproj .stamp-nuget-restore-mac
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -- "/property:Configuration=$(CONFIG)" /r /t:Build $(XBUILD_VERBOSITY) "linker/mac/dont link/generated-projects/full/dont link-mac-full.csproj"

exec-mac-full-dontlink:
	@echo "ℹ️  Executing the 'dont link' test for Xamarin.Mac (Full profile) ℹ️"
	$(Q) $(RUN_WITH_TIMEOUT) "linker/mac/dont link/generated-projects/full/bin/x86/$(CONFIG)-full/dont link.app/Contents/MacOS/dont link"

# macOS/legacy/system
build-mac-system-dontlink: linker/mac/dont\ link/generated-projects/system/dont\ link-mac-system.csproj .stamp-nuget-restore-mac
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -- "/property:Configuration=$(CONFIG)" /r /t:Build $(XBUILD_VERBOSITY) "linker/mac/dont link/generated-projects/system/dont link-mac-system.csproj"

exec-mac-system-dontlink:
	@echo "ℹ️  Executing the 'dont link' test for Xamarin.Mac (System profile) ℹ️"
	$(Q) $(RUN_WITH_TIMEOUT) "linker/mac/dont link/generated-projects/system/bin/x86/$(CONFIG)-system/dont link.app/Contents/MacOS/dont link"

#
# introspection
#

## macOS/legacy/modern
build-mac-modern-introspection: introspection/Mac/introspection-mac.csproj .stamp-nuget-restore-mac
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -- "/property:Configuration=$(CONFIG)" /r /t:Build $(XBUILD_VERBOSITY) "introspection/Mac/introspection-mac.csproj"

exec-mac-modern-introspection:
	@echo "ℹ️  Executing the 'introspection' test for Xamarin.Mac (Modern profile) ℹ️"
	$(Q) $(RUN_WITH_TIMEOUT) introspection/Mac/bin/x86/$(CONFIG)/introspection.app/Contents/MacOS/introspection

#
# xammac tests
#

## macOS/legacy/modern
build-mac-modern-xammac_tests: xammac_tests/xammac_tests.csproj .stamp-nuget-restore-mac
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -- "/property:Configuration=$(CONFIG)" /r /t:Build $(XBUILD_VERBOSITY) "xammac_tests/xammac_tests.csproj"

exec-mac-modern-xammac_tests:
	@echo "ℹ️  Executing the 'xammac' test for Xamarin.Mac (Modern profile) ℹ️"
	$(Q) $(RUN_WITH_TIMEOUT_LONGER) xammac_tests/bin/x86/$(CONFIG)/xammac_tests.app/Contents/MacOS/xammac_tests

#
# link all
#

## macOS/legacy/modern
build-mac-modern-linkall: linker/mac/link\ all/link\ all-mac.csproj .stamp-nuget-restore-mac
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -- "/property:Configuration=$(CONFIG)" /r /t:Build $(XBUILD_VERBOSITY) "linker/mac/link all/link all-mac.csproj"

exec-mac-modern-linkall:
	@echo "ℹ️  Executing the 'link all' test for Xamarin.Mac (Modern profile) ℹ️"
	$(Q) $(RUN_WITH_TIMEOUT) "./linker/mac/link all/bin/x86/$(CONFIG)/link all.app/Contents/MacOS/link all"

#
# link sdk
#

## macOS/legacy/modern
build-mac-modern-linksdk: linker/mac/link\ sdk/link\ sdk-mac.csproj .stamp-nuget-restore-mac
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -- "/property:Configuration=$(CONFIG)" /r /t:Build $(XBUILD_VERBOSITY) "linker/mac/link sdk/link sdk-mac.csproj"

exec-mac-modern-linksdk:
	@echo "ℹ️  Executing the 'link sdk' test for Xamarin.Mac (Modern profile) ℹ️"
	$(Q) $(RUN_WITH_TIMEOUT) "./linker/mac/link sdk/bin/x86/$(CONFIG)/link sdk.app/Contents/MacOS/link sdk"

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

exec-mac-dotnet-x64-$(1):
ifdef ENABLE_DOTNET
	@echo "ℹ️  Executing the '$(1)' test for macOS/.NET (x64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT$(3)) "./$(1)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-x64/$(2).app/Contents/MacOS/$(2)"
else
	$(Q) echo "Not executing $@, because .NET is not enabled"
endif

# macOS/.NET/arm64
build-mac-dotnet-arm64-$(1): .stamp-dotnet-dependency-macOS
	$$(Q) $$(MAKE) -C "$(1)/dotnet/macOS" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=osx-arm64

exec-mac-dotnet-arm64-$(1):
ifdef ENABLE_DOTNET
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(1)' test for macOS/.NET (arm64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT$(3)) "./$(1)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-arm64/$(2).app/Contents/MacOS/$(2)"
else
	@echo "⚠️  Not executing the '$(1)' test for macOS/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif
else
	$(Q) echo "Not executing $@, because .NET is not enabled"
endif

# MacCatalyst/.NET/x64
build-maccatalyst-dotnet-x64-$(1): .stamp-dotnet-dependency-MacCatalyst
	$$(Q_BUILD) $$(MAKE) -C "$(1)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-x64

exec-maccatalyst-dotnet-x64-$(1):
ifdef ENABLE_DOTNET
ifeq ($(SUPPORTS_MACCATALYST),1)
	@echo "ℹ️  Executing the '$(1)' test for Mac Catalyst/.NET (x64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT$(3)) "./$(1)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-x64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)
else
	@echo "⚠️  Not executing the '$(1)' test for Mac Catalyst/.NET (x64) - macOS version $(MACOS_VERSION) is too old ⚠️"
endif
else
	$(Q) echo "Not executing $@, because .NET is not enabled"
endif

# MacCatalyst/.NET/arm64
build-maccatalyst-dotnet-arm64-$(1):.stamp-dotnet-dependency-MacCatalyst
	$$(Q) $$(MAKE) -C "$(1)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-arm64

exec-maccatalyst-dotnet-arm64-$(1):
ifdef ENABLE_DOTNET
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(1)' test for Mac Catalyst/.NET (arm64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT$(3)) "./$(1)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-arm64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)
else
	@echo "⚠️  Not executing the '$(1)' test for Mac Catalyst/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif
else
	$(Q) echo "Not executing $@, because .NET is not enabled"
endif

build-$(1): .stamp-nuget-restore-mac
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_XAMARIN_LEGACY
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-legacy-$(1)                   || echo "build-legacy-$(1) failed"                   >> ".$$@-failure.stamp"
endif
ifdef ENABLE_DOTNET
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-x64-$(1)           || echo "build-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-arm64-$(1)         || echo "build-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-x64-$(1)   || echo "build-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-arm64-$(1) || echo "build-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
endif
	$$(Q) if test -e ".$$@-failure.stamp"; then cat ".$$@-failure.stamp"; rm ".$$@-failure.stamp"; exit 1; fi

exec-$(1):
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_XAMARIN_LEGACY
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-legacy-$(1)                   || echo "exec-legacy-$(1) failed"                   >> ".$$@-failure.stamp"
endif
ifdef ENABLE_DOTNET
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-x64-$(1)           || echo "exec-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-arm64-$(1)         || echo "exec-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-x64-$(1)   || echo "exec-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-arm64-$(1) || echo "exec-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
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

exec-mac-dotnet-x64-$(1):
	@echo "ℹ️  Executing the '$(2)' test for macOS/.NET (x64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-x64/$(2).app/Contents/MacOS/$(2)"

# macOS/.NET/arm64
build-mac-dotnet-arm64-$(1): .stamp-dotnet-dependency-macOS
	$$(Q) $$(MAKE) -C "linker/ios/$(2)/dotnet/macOS" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=osx-arm64

exec-mac-dotnet-arm64-$(1):
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(2)' test for macOS/.NET (arm64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/macOS/bin/$(CONFIG)/$(DOTNET_TFM)-macos/osx-arm64/$(2).app/Contents/MacOS/$(2)"
else
	@echo "⚠️  Not executing the '$(2)' test for macOS/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif

# MacCatalyst/.NET/x64
build-maccatalyst-dotnet-x64-$(1): .stamp-dotnet-dependency-MacCatalyst
	$$(Q_BUILD) $$(MAKE) -C "linker/ios/$(2)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-x64

exec-maccatalyst-dotnet-x64-$(1):
ifeq ($(SUPPORTS_MACCATALYST),1)
	@echo "ℹ️  Executing the '$(2)' test for Mac Catalyst/.NET (x64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-x64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)
else
	@echo "⚠️  Not executing the '$(2)' test for Mac Catalyst/.NET (x64) - macOS version $(MACOS_VERSION) is too old ⚠️"
endif

# MacCatalyst/.NET/arm64
build-maccatalyst-dotnet-arm64-$(1): .stamp-dotnet-dependency-MacCatalyst
	$$(Q) $$(MAKE) -C "linker/ios/$(2)/dotnet/MacCatalyst" build BUILD_ARGUMENTS=/p:RuntimeIdentifier=maccatalyst-arm64

exec-maccatalyst-dotnet-arm64-$(1):
ifeq ($(IS_APPLE_SILICON),1)
	@echo "ℹ️  Executing the '$(2)' test for Mac Catalyst/.NET (arm64) ℹ️"
	$$(Q) $(RUN_WITH_TIMEOUT) "./linker/ios/$(2)/dotnet/MacCatalyst/bin/$(CONFIG)/$(DOTNET_TFM)-maccatalyst/maccatalyst-arm64/$(2).app/Contents/MacOS/$(2)" $(LAUNCH_ARGUMENTS)
else
	@echo "⚠️  Not executing the '$(2)' test for Mac Catalyst/.NET (arm64) - not executing on Apple Silicon ⚠️"
endif

build-$(1): .stamp-nuget-restore-mac
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_XAMARIN_LEGACY
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-legacy-$(1)                   || echo "build-legacy-$(1) failed"                   >> ".$$@-failure.stamp"
endif
ifdef ENABLE_DOTNET
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-x64-$(1)           || echo "build-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-mac-dotnet-arm64-$(1)         || echo "build-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-x64-$(1)   || echo "build-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk build-maccatalyst-dotnet-arm64-$(1) || echo "build-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
endif
	$$(Q) if test -e ".$$@-failure.stamp"; then cat ".$$@-failure.stamp"; rm ".$$@-failure.stamp"; exit 1; fi

exec-$(1):
	$$(Q) rm -f ".$$@-failure.stamp"
ifdef INCLUDE_XAMARIN_LEGACY
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-legacy-$(1)                   || echo "exec-legacy-$(1) failed"                   >> ".$$@-failure.stamp"
endif
ifdef ENABLE_DOTNET
ifdef INCLUDE_MAC
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-x64-$(1)           || echo "exec-mac-dotnet-x64-$(1) failed"           >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-mac-dotnet-arm64-$(1)         || echo "exec-mac-dotnet-arm64-$(1) failed"         >> ".$$@-failure.stamp"
endif
ifdef INCLUDE_MACCATALYST
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-x64-$(1)   || echo "exec-maccatalyst-dotnet-x64-$(1) failed"   >> ".$$@-failure.stamp"
	$$(Q) $$(MAKE) -f packaged-macos-tests.mk exec-maccatalyst-dotnet-arm64-$(1) || echo "exec-maccatalyst-dotnet-arm64-$(1) failed" >> ".$$@-failure.stamp"
endif
endif
	$$(Q) if test -e ".$$@-failure.stamp"; then cat ".$$@-failure.stamp"; rm ".$$@-failure.stamp"; exit 1; fi
endef

$(eval $(call DotNetLinkerTest,dontlink,dont link))
$(eval $(call DotNetLinkerTest,linksdk,link sdk))
$(eval $(call DotNetLinkerTest,linkall,link all))

# Container targets that run multiple test projects

# build targets

build-legacy-dontlink: .stamp-nuget-restore-mac
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk build-mac-modern-dontlink               || echo "build-mac-modern-dontlink failed"               >> ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk build-mac-full-dontlink                 || echo "build-mac-full-dontlink failed"                 >> ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk build-mac-system-dontlink               || echo "build-mac-system-dontlink failed"               >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi

build-legacy-introspection: .stamp-nuget-restore-mac
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk build-mac-modern-introspection               || echo "build-mac-modern-introspection failed"               >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi

build-xammac_tests: .stamp-nuget-restore-mac
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk build-mac-modern-xammac_tests || echo "build-mac-modern-xammac_tests failed" >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi

build-legacy-monotouch-test: ;
	# nothing to do here

build-legacy-linkall: .stamp-nuget-restore-mac
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk build-mac-modern-linkall               || echo "build-mac-modern-link all failed"              >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi

build-legacy-linksdk: .stamp-nuget-restore-mac
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk build-mac-modern-linksdk               || echo "build-mac-modern-linksdk failed"               >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi

# execution targets

exec-legacy-dontlink:
ifdef INCLUDE_XAMARIN_LEGACY
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk exec-mac-modern-dontlink       || echo "exec-mac-modern-dont link failed"       >> ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk exec-mac-full-dontlink         || echo "exec-mac-full-dont link failed"         >> ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk exec-mac-system-dontlink       || echo "exec-mac-system-dont link failed"       >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi
else
	$(Q) echo "Not executing $@, because legacy Xamarin is not enabled"
endif

exec-legacy-introspection:
ifdef INCLUDE_XAMARIN_LEGACY
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk exec-mac-modern-introspection               || echo "exec-mac-modern-introspection failed"               >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi
else
	$(Q) echo "Not executing $@, because legacy Xamarin is not enabled"
endif

exec-xammac_tests:
ifdef INCLUDE_XAMARIN_LEGACY
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk exec-mac-modern-xammac_tests || echo "exec-mac-modern-xammac_tests failed" >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi
else
	$(Q) echo "Not executing $@, because legacy Xamarin is not enabled"
endif

exec-legacy-monotouch-test: ;
	# nothing to do here

exec-legacy-linkall:
ifdef INCLUDE_XAMARIN_LEGACY
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk exec-mac-modern-linkall || echo "exec-mac-modern-linkall failed"               >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi
else
	$(Q) echo "Not executing $@, because legacy Xamarin is not enabled"
endif

exec-legacy-linksdk:
ifdef INCLUDE_XAMARIN_LEGACY
	$(Q) rm -f ".$@-failure.stamp"
	$(Q) $(MAKE) -f packaged-macos-tests.mk exec-mac-modern-linksdk || echo "exec-mac-modern-link sdk failed"              >> ".$@-failure.stamp"
	$(Q) if test -e ".$@-failure.stamp"; then cat ".$@-failure.stamp"; rm ".$@-failure.stamp"; exit 1; fi
else
	$(Q) echo "Not executing $@, because legacy Xamarin is not enabled"
endif
