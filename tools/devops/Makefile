TOP=../..
include $(TOP)/Make.config

device-tests-provisioning.csx: device-tests-provisioning.csx.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's#@XCODE_XIP_NAME@#$(notdir $(XCODE_URL))#g' \
		-e 's#@XI_PACKAGE@#$(XI_PACKAGE)#g' \
		-e 's#@MONO_PACKAGE@#$(MIN_MONO_URL)#g' \
		-e 's#@MIN_SHARPIE_URL@#$(MIN_SHARPIE_URL)#g' \
		-e 's#@INCLUDE_MAC@#$(INCLUDE_MAC)#g' \
		-e 's#@INCLUDE_IOS@#$(INCLUDE_IOS)#g' \
		-e 's#@INCLUDE_TVOS@#$(INCLUDE_TVOS)#g' \
		-e 's#@INCLUDE_WATCH@#$(INCLUDE_WATCH)#g' \
		-e 's#@INCLUDE_XAMARIN_LEGACY@#$(INCLUDE_XAMARIN_LEGACY)#g' \
		$< > $@;
		@echo "Generated $@:"
		@cat $@

build-provisioning.csx: build-provisioning.csx.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's#@XCODE_XIP_NAME@#$(notdir $(XCODE_URL))#g' \
		-e 's#@MONO_PACKAGE@#$(MIN_MONO_URL)#g' \
		-e 's#@MIN_SHARPIE_URL@#$(MIN_SHARPIE_URL)#g' \
		$< > $@;
		@echo "Generated $@:"
		@cat $@

mac-tests-provisioning.csx: mac-tests-provisioning.csx.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's#@XM_PACKAGE@#$(XM_PACKAGE)#g' \
		-e 's#@MONO_PACKAGE@#$(MIN_MONO_URL)#g' \
		-e 's#@INCLUDE_MAC@#$(INCLUDE_MAC)#g' \
		-e 's#@INCLUDE_IOS@#$(INCLUDE_IOS)#g' \
		-e 's#@INCLUDE_XAMARIN_LEGACY@#$(INCLUDE_XAMARIN_LEGACY)#g' \
		$< > $@;
		@echo "Generated $@:"
		@cat $@

provision-xcode.csx: provision-xcode.csx.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's#@XCODE_XIP_NAME@#$(notdir $(XCODE_URL))#g' \
		-e 's#@XCODE_ROOT_PATH@#$(XCODE_DEVELOPER_ROOT)#g' \
		$< > $@;
		@echo "Generated $@:"
		@cat $@

LocProject.json: LocProject.json.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's#@WORKING_DIRECTORY@#$(PWD)#g' \
		$< > $(TOP)/Localize/$@;

all check: check-sh check-yaml
	@true

check-sh:
	$(Q) if ! type shellcheck >/dev/null; then echo "shellcheck is not installed. Install it like this: 'brew install shellcheck'"; exit 1; fi
	$(Q) shellcheck $(shell find . -name '*.sh')

check-yml check-yaml:
	$(Q) if ! type yamllint >/dev/null; then echo "yamllint is not installed. Install it like this: 'brew install yamllint'"; exit 1; fi
	$(Q) yamllint -c .yamllint.yml $(shell find . -name '*.yml')

print-abspath-variable:
	@echo $(VARIABLE)=$(abspath $($(VARIABLE)))

print-variable:
	@echo $(VARIABLE)=$($(VARIABLE))

print-variable-value-to-file:
	@echo $($(VARIABLE)) > "$(FILE)"

provisioning: build-provisioning.csx device-tests-provisioning.csx provision-xcode.csx
