TOP=../../..

include $(TOP)/Make.config

build:
	$(DOTNET) build /bl $(MSBULD_VERBOSITY)

run:
	$(DOTNET) build /bl $(MSBUILD_VERBOSITY) /t:Run
