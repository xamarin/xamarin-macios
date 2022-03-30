TOP=../../..

include $(TOP)/Make.config

build:
	$(DOTNET) build /bl *.csproj $(MSBUILD_VERBOSITY)

run:
	$(DOTNET) build /bl *.csproj $(MSBUILD_VERBOSITY) -t:Run
