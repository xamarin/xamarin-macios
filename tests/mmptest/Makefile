TOP = ../..

all-local:: regression run

bin/:
	mkdir bin

regression::
ifneq ($(SKIP_REGRESSION), 1)
	$(MAKE) -C regression/
endif

bin/x86/Debug/mmptest.app:
	$(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) mmptest.csproj

run:: bin/ bin/x86/Debug/mmptest.app
	bin/x86/Debug/mmptest.app/Contents/MacOS/mmptest

clean-local::
	@rm -rf ./obj
	@rm -rf ./bin
