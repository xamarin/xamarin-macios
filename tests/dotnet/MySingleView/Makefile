TOP=../../..

include $(TOP)/Make.config

build:
	$(DOTNET6) build /bl $(MSBULD_VERBOSITY)

run:
	$(DOTNET6) build /bl $(MSBUILD_VERBOSITY) /t:Run
