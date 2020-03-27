all:
	rm -Rf ~/.nuget/packages/xamarin.ios.*
	git clean -xfd
	make all install -j -C ../../..
	make -C ../.. pack-simple
	dotnet publish -r ios-x64 --self-contained true -p:PublishTrimmed=true -p:MonoAOT=true /v:diag /bl:MySingleView.binlog

