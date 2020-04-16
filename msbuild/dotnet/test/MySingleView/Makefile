TOP=../../../..
include $(TOP)/Make.config

all: publish

publish: build
	dotnet publish -r ios-x64 --self-contained true -p:PublishTrimmed=true -p:MonoAOT=true /v:diag /bl:MySingleView-x64.binlog /p:UseAppHost=false

publish-dev: build
	dotnet publish -r ios-arm64 --self-contained true -p:PublishTrimmed=false -p:MonoAOT=true /v:diag /bl:MySingleView-arm64.binlog /p:UseAppHost=false

clean:
	rm -Rf ~/.nuget/packages/xamarin.ios.* ~/.nuget/packages/microsoft.ios.*
	cd ../.. && git clean -xfd
	rm -Rf $(TOP)/_build

build: clean
	make all install -C $(TOP)/tools/dotnet-linker
	make all install -j -C ../../..
	make -C ../.. packsimple test/global.json test/NuGet.config -j
	make -C ../.. install-nugets

run: publish
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mlaunch --launchsim $(abspath $(CURDIR)/bin/Debug/netcoreapp5.0/ios-x64/MySingleView.app) --device=:v2:runtime=com.apple.CoreSimulator.SimRuntime.iOS-12-0,devicetype=com.apple.CoreSimulator.SimDeviceType.iPhone-6 --wait-for-exit --stderr="$(shell tty)" --stdout="$(shell tty)" "--setenv=MONO_TRACE=-E:all"

run-dev: publish-dev
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mlaunch --installdev $(abspath $(CURDIR)/bin/Debug/netcoreapp5.0/ios-arm64/MySingleView.app) $(DEVNAME) --wait-for-exit --stderr="$(shell tty)" --stdout="$(shell tty)"
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mlaunch --launchdev $(abspath $(CURDIR)/bin/Debug/netcoreapp5.0/ios-arm64/MySingleView.app) $(DEVNAME) --wait-for-exit --stderr="$(shell tty)" --stdout="$(shell tty)"