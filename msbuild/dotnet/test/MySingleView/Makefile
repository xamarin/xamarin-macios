TOP=../../../..
include $(TOP)/Make.config

all: build
	dotnet publish -r ios-x64 --self-contained true -p:PublishTrimmed=true -p:MonoAOT=true /v:diag /bl:MySingleView.binlog /p:UseAppHost=false

publish: build
	dotnet publish *.csproj /restore /p:PublishTrimmed=True /p:SelfContained=True $(XBUILD_VERBOSITY)

build:
	rm -Rf ~/.nuget/packages/xamarin.ios.*
	cd ../.. && git clean -xfd
	rm -Rf $(TOP)/_build
	make all install -j -C ../../..
	make -C ../.. packsimple test/global.json test/NuGet.config -j
	make -C ../.. install-nugets
