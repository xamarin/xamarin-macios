TOP = ../..

all-local:: regression run

bin/:
	mkdir bin

bin/SimpleClass.dylib: bin/
	@clang -dynamiclib -std=gnu99 ../common/mac/SimpleClass.m  -current_version 1.0 -compatibility_version 1.0 -fvisibility=hidden -framework Cocoa -o bin/SimpleClass.dylib

bin/SimpleClassStatic.a: bin/
	$(Q) clang -c ../common/mac/SimpleClass.m -o bin/SimpleClass.o -std=gnu99 -mmacosx-version-min=10.7
	$(Q) xcrun libtool -static bin/SimpleClass.o -o bin/SimpleClassStatic.a

regression::
ifneq ($(SKIP_REGRESSION), 1)
	$(MAKE) -C regression/
endif

bin/x86/Debug/mmptest.app:
	xbuild mmptest.csproj

native-deps:: bin/SimpleClass.dylib bin/SimpleClassStatic.a

run:: bin/ bin/x86/Debug/mmptest.app native-deps 
	bin/x86/Debug/mmptest.app/Contents/MacOS/mmptest

clean-local::
	@rm -rf ./obj
	@rm -rf ./bin
