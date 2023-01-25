TOP=../../../..

TESTNAME=monotouchtest
include $(TOP)/tests/common/shared-dotnet.mk

export XAMARIN_LOG_MARSHALLED_EXCEPTIONS=1

managed-static-registrar msr:
	$(Q) $(MAKE) build-msr
	$(Q) $(MAKE) run-msr

build-msr:
	$(Q) rm -rf bin/ManagedStaticRegistrar obj/ManagedStaticRegistrar *.binlog
	$(Q) $(MAKE) build CONFIG=ManagedStaticRegistrar

build-sr:
	$(Q) rm -rf bin/StaticRegistrar obj/StaticRegistrar *.binlog
	$(Q) $(MAKE) build CONFIG=StaticRegistrar

run-msr:
	$(Q) time $(MAKE) run-bare CONFIG=ManagedStaticRegistrar

run-sr:
	$(Q) time $(MAKE) run-bare CONFIG=StaticRegistrar

build-msr-aot:
	$(Q) rm -rf bin/ManagedStaticRegistrar obj/ManagedStaticRegistrar *.binlog
	$(Q) $(MAKE) build CONFIG=ManagedStaticRegistrar BUILD_ARGUMENTS=/p:RuntimeIdentifier=ios-arm64

build-sr-aot:
	$(Q) rm -rf bin/StaticRegistrar obj/StaticRegistrar *.binlog
	$(Q) $(MAKE) build CONFIG=StaticRegistrar BUILD_ARGUMENTS=/p:RuntimeIdentifier=ios-arm64

run-msr-aot:
	DEVICE=--devname=00008101-000650AE3450001E launch-on-device

msr-aot:
	$(Q) $(MAKE) build-msr-aot
	$(Q) $(MAKE) run-msr-aot

registrar-tests:
	$(EXECUTABLE) --autostart --autoexit --test MonoTouchFixtures.ObjCRuntime.RegistrarTest

lldb: CONFIG=ManagedStaticRegistrar
lldb:
	MONO_DEBUG=disable_omit_fp,explicit-null-checks lldb -- $(EXECUTABLE) --autostart --autoexit $(TEST)

msr-compare:
	$(Q) $(MAKE) build CONFIG=ManagedStaticRegistrar
	$(Q) $(MAKE) build CONFIG=Debug
	-time $(MAKE) run-bare CONFIG=ManagedStaticRegistrar >& msr.log
	-time $(MAKE) run-bare CONFIG=Debug                  >& debug.log
