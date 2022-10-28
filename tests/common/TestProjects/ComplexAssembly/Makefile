TOP=../../../..

include $(TOP)/Make.config

.build-stamp.binlog: Makefile $(wildcard *.cs) $(wildcard *.csproj) $(wildcard *.resx) $(wildcard *.config)
	$(Q) $(DOTNET) build *.csproj $(MSBUILD_VERBOSITY) /bl:$@

all-local:: .build-stamp.binlog
