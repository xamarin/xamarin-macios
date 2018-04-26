TOP = ../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

MMP=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/mmp
BMAC=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/bmac

all-local:: build/compat/test.app build/mobile-32/test.app build/mobile-64/test.app

exec-compat: build/compat/test.app
	./build/compat/test.app/Contents/MacOS/test | grep Passed

exec-mobile-%: build/mobile-%/test.app
	./build/mobile-$*/test.app/Contents/MacOS/test | grep Passed

run run-test run-tests: build/compat/test.app build/mobile-32/test.app build/mobile-64/test.app
	@rm -f .$@-failure.stamp
	@$(MAKE) exec-compat    || echo "run-compat failed"    >> .$@-failure.stamp
	@$(MAKE) exec-mobile-32 || echo "run-mobile-32 failed" >> .$@-failure.stamp
	@$(MAKE) exec-mobile-64 || echo "run-mobile-64 failed" >> .$@-failure.stamp
	@if test -e .$@-failure.stamp; then cat .$@-failure.stamp; rm .$@-failure.stamp; exit 1; fi

build/compat/test.app: build/compat/finder.dll
	rm -Rf $@
	$(SYSTEM_CSC) "/out:build/compat/test.exe" "/r:build/compat/finder.dll" /r:$(TOP)/src/build/mac/compat/XamMac.dll /t:exe "test.cs" 
	$(MMP) -nolink "-minos=10.7" -o "build/compat" -n "test" --profile "4.5" -a $(TOP)/src/build/mac/compat/XamMac.dll -a "build/compat/finder.dll" "build/compat/test.exe"

build/compat/finder.dll:
	$(BMAC) --out=build/compat/finder.dll --outdir=build/compat/ -r:/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/System.Drawing.dll -baselib:$(TOP)/src/build/mac/compat/XamMac.dll finder.cs

build/mobile-%/test.app: build/mobile-%/finder.dll
	rm -Rf $@
	$(SYSTEM_CSC) "/out:build/mobile-$*/test.exe" "/r:build/mobile-$*/finder.dll" /r:$(TOP)/src/build/mac/mobile-$*/Xamarin.Mac.dll /t:exe "test.cs" -define:XAMCORE_2_0
	$(MMP) -nolink "-minos=10.7" -o "build/mobile-$*" -n "test" --profile "4.5" -a $(TOP)/src/build/mac/mobile-$*/Xamarin.Mac.dll -a "build/mobile-$*/finder.dll" "build/mobile-$*/test.exe" --profile mobile --arch $(shell test '$*' == '32' && echo i386 || echo x86_64)

build/mobile-%/finder.dll:
	$(BMAC) --unified-mobile-profile --out=build/mobile-$*/finder.dll --outdir=build/mobile-$*/ -baselib:$(TOP)/src/build/mac/mobile-$*/Xamarin.Mac.dll -d=XAMCORE_2_0 finder.cs

clean-local::
	@rm -rf build/
