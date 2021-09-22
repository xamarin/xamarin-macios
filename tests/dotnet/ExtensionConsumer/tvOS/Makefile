TOP=../../../..

include $(TOP)/Make.config

build:
	$(DOTNET6) build /bl ExtensionConsumer.sln $(MSBUILD_VERBOSITY)

run:
	$(DOTNET6) build /bl ExtensionConsumer.sln $(MSBUILD_VERBOSITY) -t:Run
