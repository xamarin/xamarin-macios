TOP=../../..

include $(TOP)/Make.config

build:
	$(DOTNET) build /bl $(MSBULD_VERBOSITY)

run:
	$(DOTNET) build /bl $(MSBUILD_VERBOSITY) /t:Run

run-device: clean
	$(DOTNET) build -c Release /bl $(MSBUILD_VERBOSITY) /t:Run -r ios-arm64

run-device-no-strip: clean
	$(DOTNET) build -c Release /bl $(MSBUILD_VERBOSITY) /t:Run -r ios-arm64 -p:MtouchNoSymbolStrip=true

run-device-export-syms: clean
	$(DOTNET) build -c Release /bl $(MSBUILD_VERBOSITY) /t:Run -r ios-arm64 -p:_ExportSymbolsExplicitly=true

clean:
	rm -rf bin obj *.binlog
