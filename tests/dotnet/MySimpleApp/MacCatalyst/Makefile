TOP=../../../..

include $(TOP)/Make.config

build:
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY)

run:
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY) -t:Run
