TOP = ../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

no-mmp: main.m
	clang -o$@ main.m -framework AppKit -arch i386

no-mmp.exe: Main.cs XamMac.dll libxammac.dylib
	$(SYSTEM_CSC) Main.cs -out:$@ -r:XamMac.dll

XamMac.dll: $(TOP)/src/build/mac/compat/XamMac.dll
	cp $^ $@

libxammac.dylib: $(TOP)/launcher/.libs/libxammac.dylib
	cp $^ $@

no-mmp.app: no-mmp.exe no-mmp
	rm -Rf no-mmp.app
	mkdir -p no-mmp.app/Contents/MacOS
	cp no-mmp no-mmp.app/Contents/MacOS
	mkdir -p no-mmp.app/Contents/MonoBundle
	cp no-mmp.exe* no-mmp.app/Contents/MonoBundle
	cp libxammac.dylib no-mmp.app/Contents/MonoBundle
	cp XamMac.dll* no-mmp.app/Contents/MonoBundle

run: no-mmp.app
	no-mmp.app/Contents/MacOS/no-mmp
