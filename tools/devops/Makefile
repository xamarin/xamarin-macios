TOP=../..
include $(TOP)/Make.config

xcode-deps.csx: xcode-deps.csx.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's#@XCODE_VERSION@#$(XCODE_VERSION)#g' \
		-e 's#@XI_PACKAGE@#$(XI_PACKAGE)#g' \
		$< > $@;
