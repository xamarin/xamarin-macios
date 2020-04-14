TOP=../../../..
include $(TOP)/Make.config

all: publish

publish: build
	dotnet publish -r osx-x64 --self-contained true -p:PublishTrimmed=true -p:MonoAOT=true /v:diag /bl:MySingleView-x64.binlog /p:UseAppHost=false

clean:
	rm -Rf ~/.nuget/packages/xamarin.ios.* ~/.nuget/packages/microsoft.ios.*
	cd ../.. && git clean -xfd
	rm -Rf $(TOP)/_build

build: clean
	make all install -j -C ../../..
	make -C ../.. pack-macOS test/global.json test/NuGet.config -j
	make -C ../.. install-nugets

run: publish
	dotnet run

