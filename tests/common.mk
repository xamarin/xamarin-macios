MMP=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/mmp $(MMP_VERBOSITY)

.PRECIOUS: build/mobile-32.app build/mobile-64.app

check-guiunit-xml:
	@if test -f $(FILE); then \
		if grep 'One or more child tests had errors' $(FILE) > /dev/null; then \
			echo Test run failed; \
			exit 1; \
		fi; \
		echo Test run succeeded; \
	else \
		echo Test run crashed; \
		exit 1; \
	fi

BASE_DLLS = $(TOP)/src/build/mac/compat/XamMac.dll \
	$(TOP)/src/build/mac/mobile-32/Xamarin.Mac.dll \
	$(TOP)/src/build/mac/mobile-64/Xamarin.Mac.dll

all-local:: build/compat/$(TESTDLL) build/mobile-32/$(TESTDLL) build/mobile-64/$(TESTDLL)

clean-local::
	rm -rf build TestResult*.xml

build/%/$(TESTDLL): $(MAC_SOURCES) build/GuiUnit.exe Makefile $(BASE_DLLS)
	@mkdir -p $(dir $@)
	$(Q_CSC) $(SYSTEM_CSC) -out:$@ -t:library -debug -d:MONOMAC -d:XAMCORE_2_0 \
		-r:build/GuiUnit.exe \
		-r:$(TOP)/src/build/mac/$*/Xamarin.Mac.dll \
		$(MAC_SOURCES)

build/compat/$(TESTDLL): $(MAC_SOURCES) build/GuiUnit.exe Makefile $(BASE_DLLS)
	@mkdir -p $(dir $@)
	$(Q_CSC) $(SYSTEM_CSC) -out:$@ -t:library -debug -d:MONOMAC \
		-r:build/GuiUnit.exe \
		-r:System.Drawing \
		-r:$(TOP)/src/build/mac/compat/XamMac.dll \
		$(MAC_SOURCES)

build/compat.app: build/compat/$(TESTDLL) $(BASE_DLLS)
	@rm -Rf $@
	$(Q) DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) $(MMP) --output $(abspath build/compat.app) $(abspath build/GuiUnit.exe) -a $(TOP)/src/build/mac/compat/XamMac.dll --nolink --profile 4.5 --cache $(abspath build/compat-mmp-cache) --marshal-objectivec-exceptions=throwmanaged

build/mobile-%.app: build/mobile-%/$(TESTDLL) $(BASE_DLLS)
	@rm -Rf $@
	$(Q) DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) $(MMP) --output $(abspath build/mobile-$*.app) $(abspath build/GuiUnit.exe) -a $(TOP)/src/build/mac/mobile-$*/Xamarin.Mac.dll --nolink --profile mobile --arch $(shell test '$*' == '32' && echo i386 || echo x86_64) --cache $(abspath build/mobile-$*-mmp-cache) --marshal-objectivec-exceptions=throwmanaged

exec-compat: build/compat.app
	@rm -f TestResult-compat.xml
	build/compat.app/GuiUnit.app/Contents/MacOS/GuiUnit -noheader $(abspath build/compat/$(TESTDLL)) -xml:$(PWD)/TestResult-compat.xml
	@test -z "$(BUILD_REVISION)" || echo @"MonkeyWrench: AddFile: $(abspath TestResult-compat.xml)"
	@$(MAKE) check-guiunit-xml FILE=TestResult-compat.xml STAMP=.$@-failure.stamp

exec-mobile-%: build/mobile-%.app
	@rm -f TestResult-mobile-$*.xml .$@-failure.stamp
	build/mobile-$*.app/GuiUnit.app/Contents/MacOS/GuiUnit -noheader $(abspath build/mobile-$*/$(TESTDLL)) -xml:$(PWD)/TestResult-mobile-$*.xml
	@test -z "$(BUILD_REVISION)" || echo @"MonkeyWrench: AddFile: $(abspath TestResult-mobile-$*.xml)"
	@$(MAKE) check-guiunit-xml FILE=TestResult-mobile-$*.xml STAMP=.$@-failure.stamp

run-%:
	@rm -f .$@-failure.stamp
	@$(MAKE) exec-$* || echo "run-$* failed" >> .$@-failure.stamp
	@if test -e .$@-failure.stamp; then cat .$@-failure.stamp; rm .$@-failure.stamp; exit 1; fi

run run-test run-tests: build/compat.app build/mobile-32.app build/mobile-64.app
	@rm -f .$@-failure.stamp
	@$(MAKE) exec-compat    || echo "run-compat failed"    >> .$@-failure.stamp
	@$(MAKE) exec-mobile-32 || echo "run-mobile-32 failed" >> .$@-failure.stamp
	@$(MAKE) exec-mobile-64 || echo "run-mobile-64 failed" >> .$@-failure.stamp
	@if test -e .$@-failure.stamp; then cat .$@-failure.stamp; rm .$@-failure.stamp; exit 1; fi

build/GuiUnit.exe: $(shell find $(GUI_UNIT_PATH)/src/framework -name \*.cs -or -name \*.csproj)
	@mkdir -p build
	@$(SYSTEM_XBUILD) $(GUI_UNIT_PATH)/src/framework/GuiUnit_NET_4_5.csproj
	@cp $(GUI_UNIT_PATH)/bin/net_4_5/GuiUnit.exe $@
