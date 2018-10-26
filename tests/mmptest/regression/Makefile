TOP = ../../..

include $(TOP)/Make.config

MMP=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/mmp

export MD_APPLE_SDK_ROOT=$(shell dirname `dirname $(XCODE94_DEVELOPER_ROOT)`)
export TargetFrameworkFallbackSearchPaths=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current


TESTS_4_0 = \
	link-gdiplus-1 \
	link-gdiplus-2 \
	link-posix-1 \
	link-posix-2 \
	link-system.web-icalls \
	link-keep-resources-1 \
	link-preserve-assembly \
	link-uithread-1 \
	link-uithread-2 \
	link-safe-1 \
	link-frameworks-1 \
	link-aes-1 \
	link-blocks-1 \
	embedded-mono \
	system-mono \
	embedded-mono-profile \
	system-mono-profile \

DISABLED_TESTS_MONO_4_2 =  \
	link-data-contract-1 \
	link-webclient-2 \
	link-xml-serialization \

DISABLED_TESTS_MONO_4_4 =  \
	link-webclient-1 \
	link-webclient-3 \
	link-remove-attributes-1

TESTS_UNIFIED = \
	custom-bundle-name

TESTS_4_5 = \
	link-callfilepath-45 \

DISABLED_4_5_TESTS_MONO_4_2 = \
	link-httpclient-async \

ALL_TESTS = \
	$(TESTS_UNIFIED) \
	$(TESTS_4_0) \
	$(TESTS_4_5)

all-local:: run

embedded-mono/bin system-mono/bin embedded-mono-profile/bin system-mono-profile/bin $(foreach test,$(TESTS_UNIFIED),$(test)/bin): dylib/libTest.dylib
	DEVELOPER_DIR=$(XCODE94_DEVELOPER_ROOT) XAMMAC_FRAMEWORK_PATH=$(abspath $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)) $(SYSTEM_XBUILD)  $(@D)/$(@D).csproj

%/bin: dylib/libTest.dylib
	DEVELOPER_DIR=$(XCODE94_DEVELOPER_ROOT) XAMMAC_FRAMEWORK_PATH=$(abspath $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)) $(MDTOOL) build $*/$*.csproj

dylib/libTest.dylib: dylib/src.c
	clang $< -o$@ -arch i386 -arch x86_64 -shared

build: $(foreach test,$(ALL_TESTS),$(test)/bin)

run run-test run-tests:
	$(MAKE) clean
	$(MAKE) build
	$(MAKE) exec

exec-system-mono-profile exec-embedded-mono-profile: export MONO_ENV_OPTIONS=--profile=log:output=output.mlpd

exec-%:
	@echo Executing $*... | tee $*/$@.log
	@test -z $(BUILD_REVISION) || echo @MonkeyWrench: AddFile: $(CURDIR)/$*/$@.log
	@cd $* && if ! bin/Debug/$*.app/Contents/MacOS/$* >> $@.log 2>&1; then echo "CRASHED" >> $@.log; fi
	@if grep -q FAIL $*/exec-$*.log; then echo "Test $* failed"; fi
	@if grep -q CRASHED $*/exec-$*.log; then echo "Test $* crashed"; fi
	@echo Executed $* | tee -a $*/$@.log
	@echo "" >> $*/$@.log

exec: $(foreach test,$(ALL_TESTS),exec-$(test))
	@cat */exec-*.log > tests.log
	@cat tests.log
	@if grep -q CRASHED tests.log; then \
		echo Test run crashed; \
		exit 1; \
	elif grep -q FAIL tests.log; then \
		echo Test run failed; \
		exit 1; \
	else \
		echo Test run succeeded; \
	fi

clean:
	@rm -f tests.log
	@for i in $(ALL_TESTS); do \
		rm -rf $(PWD)/$$i/{bin,obj}/; \
	done
