TOP = ../..

include $(TOP)/Make.config

MMP=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/mmp

ifdef TESTS_USE_SYSTEM
export TargetFrameworkFallbackSearchPaths=/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=/Library/Frameworks/Mono.framework/External/xbuild
export XamarinMacFrameworkRoot=/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XAMMAC_FRAMEWORK_PATH=/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
else
export TargetFrameworkFallbackSearchPaths=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XAMMAC_FRAMEWORK_PATH=$(abspath $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR))
endif

ALL_TESTS = \
	custom-bundle-name \
	embedded-mono \
	embedded-mono-profile \
	link-frameworks-1 \
	link-keep-resources-1 \
	link-keep-resources-2 \
	link-posix-1 \
	link-preserve-assembly \
	link-remove-attributes-1 \
	link-safe-1 \
	system-mono \
	system-mono-profile \

all-local:: run

CONFIG?=Debug

$(foreach test,$(ALL_TESTS),$(test)/bin): dylib/libTest.dylib
	$(QF_GEN) if ! $(SYSTEM_XIBUILD) -- $(@D)/$(@D).csproj /p:Configuration=$(CONFIG); then EC=$$?; rm -Rf $@; exit $$EC; fi

dylib/libTest.dylib: dylib/src.c
	$(QF_GEN) $(XCODE_CC) $< -o$@ -arch x86_64 -shared -L /Library/Developer/CommandLineTools/SDKs/MacOSX.sdk/usr/lib

build: $(foreach test,$(ALL_TESTS),$(test)/bin)

run run-test run-tests:
	$(MAKE) clean
	$(MAKE) build
	$(MAKE) exec

run-test-%:
	rm -Rf $*/bin
	$(MAKE) $*/bin
	$(MAKE) exec-$*

exec-system-mono-profile exec-embedded-mono-profile: export MONO_ENV_OPTIONS=--profile=log:output=output.mlpd

exec-link-frameworks-1 link-frameworks-1/bin: CONFIG=Release

exec-%:
	@echo Executing $*... | tee $*/$@.log
	@test -z $(BUILD_REVISION) || echo @MonkeyWrench: AddFile: $(CURDIR)/$*/$@.log
	@cd $* && if ! bin/$(CONFIG)/$*.app/Contents/MacOS/$* >> $@.log 2>&1; then echo "CRASHED" >> $@.log; fi
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
