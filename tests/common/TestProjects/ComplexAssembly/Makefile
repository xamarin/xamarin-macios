TOP=../../../..

include $(TOP)/Make.config

NuGet%config global%json:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet all
	$(Q) $(CP) $(TOP)/tests/dotnet/global.json $(TOP)/tests/dotnet/NuGet.config .

.build-stamp.binlog: Makefile $(wildcard *.cs) $(wildcard *.csproj) $(wildcard *.resx) $(wildcard *.config) NuGet.config global.json
	$(Q) $(DOTNET) build *.csproj $(MSBUILD_VERBOSITY) /bl:$@

all-local:: .build-stamp.binlog
