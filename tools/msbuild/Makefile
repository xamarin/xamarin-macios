TOP=../..
include $(TOP)/Make.config

all: MSBuild.dll MSBuild.dll.config
	
clean:
	-rm -f MSBuild.dll MSBuild.dll.config

MSBuild.dll: $(MONO_PREFIX)/lib/mono/msbuild/15.0/bin/MSBuild.dll
	$(Q) $(CP) $< $@

MSBuild.dll.config: MSBuild.dll.config.in Makefile
	$(Q) sed -e 's,@MSBUILD_EXTENSIONS_PATH@,$(abspath $(TOP))/_mac-build//Library/Frameworks/Mono.framework/External/xbuild/;$(abspath $(TOP))/_ios-build//Library/Frameworks/Mono.framework/External/xbuild/,g' -e 's,@MONO_PREFIX@,$(MONO_PREFIX),g' $< > $@

