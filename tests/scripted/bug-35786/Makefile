TOP=../../..
include $(TOP)/Make.config

all-local:: bug-35786

ITERATIONS=100
ALL_DLLS:=$(shell printf "libs/C%i.dll " {1..$(ITERATIONS)})

libs/C%.cs: Makefile
	@echo "public class C$* {}" >> $@

libs/C%.dll: libs/C%.cs Makefile
	$(SYSTEM_CSC) -target:library $< -out:$@

libs/All.cs: Makefile
	@echo "public class All {" > $@
	@echo "    public static void UseMe ()" >> $@
	@echo "    {" >> $@
	@printf "        System.Console.WriteLine (typeof (C%i));\n" {1..$(ITERATIONS)} >> $@
	@echo "    }" >> $@
	@echo "}" >> $@

libs/All.dll.rsp: Makefile
	@printf -- "-r:libs/C%i.dll\n" {1..$(ITERATIONS)} > $@

libs/All.dll: libs/All.cs libs/All.dll.rsp $(ALL_DLLS)
	$(SYSTEM_CSC) -target:library @$@.rsp $< -out:$@

libs/bug-35786.exe: Main.cs libs/All.dll
	$(SYSTEM_CSC) -target:exe $< -out:$@ -r:libs/All.dll -r:$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Xamarin.iOS.dll -lib:$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS

bug-35786:
	@git clean -xfdq
	@mkdir -p libs
	@$(MAKE) -j20 libs/bug-35786.exe
	@mkdir -p libs/bug-35786.app
	@# ulimit -n=64 works while testing (with the fix for this bug), so use 96 to have some buffer.
	@# we lower the ulimit here so that we don't have to create that many assemblies to test with
	@# the ulimit default on Yosemite is ulimit=256, which requires 300 assemblies to break the stripper, and 400 assemblies to break the AOT compiler.
	@# with 96 here, we only need 20 assemblies to break the stripper, and 35 to break the AOT compiler, so running the test with 100 assemblies should be more than enough.
	ulimit -n 96 && $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch --dev libs/bug-35786.app --targetver=10.3 --abi armv7,arm64 libs/bug-35786.exe --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION) -r:$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Xamarin.iOS.dll


include $(TOP)/mk/rules.mk
