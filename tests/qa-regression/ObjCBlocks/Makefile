ifeq ($(IN_TREE),1)
FRAMEWORK = ../../../_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
else
FRAMEWORK = /Library/Frameworks/Xamarin.Mac.framework/Versions/Current
endif

BMAC = $(FRAMEWORK)/bin/bmac
MMP = $(FRAMEWORK)/bin/mmp
XAMMAC = $(FRAMEWORK)/lib/mono/XamMac.dll
SYSTEM_LAUNCHER = $(FRAMEWORK)/lib/mono/XamMacLauncher

.PHONY: all run clean

all: run 

block.dylib: block.m
	clang -dynamiclib -m32 -o $@ -ObjC -framework Foundation $<

block.dll: block.dylib block.cs $(BMAC)
	$(BMAC) block.cs -out:$@ -outdir:generated -baselib:$(XAMMAC) -r:System.Drawing

app.exe: block.dll app.cs
	mcs -debug app.cs -out:$@ -r:block.dll -r:$(XAMMAC)
	cp $(XAMMAC){,.mdb} .

SystemLauncher.app: app.exe
	rm -rf $@
	mkdir -p $@/Contents/MonoBundle
	cp XamMac.dll block.dll block.dylib $@/Contents/MonoBundle
	cp app.exe $@/Contents/MonoBundle/SystemLauncher.exe
	mkdir -p $@/Contents/MacOS
	cp $(SYSTEM_LAUNCHER) $@/Contents/MacOS/SystemLauncher
	cp Info.plist $@/Contents
	chmod +x $@/Contents/MacOS/SystemLauncher

LinkedLauncher.app: app.exe
	rm -rf $@
	$(MMP) --nolink $< -name LinkedLauncher
	cp Info.plist $@/Contents
	mv $@/Contents/MonoBundle/app.exe $@/Contents/MonoBundle/LinkedLauncher.exe
	cp block.dylib $@/Contents/MonoBundle

run: SystemLauncher.app LinkedLauncher.app
	SystemLauncher.app/Contents/MacOS/SystemLauncher
	LinkedLauncher.app/Contents/MacOS/LinkedLauncher

clean:
	rm -rf *.exe *.dll *.mdb *.dylib generated *.app
