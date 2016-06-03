TOP = ../..
include $(TOP)/Make.config

export XBUILD_FRAMEWORK_FOLDERS_PATH=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPath=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XAMMAC_FRAMEWORK_PATH=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

all-local:: regression run

bin/:
	mkdir bin

regression::
ifneq ($(SKIP_REGRESSION), 1)
	$(MAKE) -C regression/
endif

bin/x86/Debug/mmptest.app:
	xbuild mmptest.csproj

run:: bin/ bin/x86/Debug/mmptest.app
	bin/x86/Debug/mmptest.app/Contents/MacOS/mmptest

clean-local::
	@rm -rf ./obj
	@rm -rf ./bin
