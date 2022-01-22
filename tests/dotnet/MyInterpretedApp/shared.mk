TOP=../../../..
include $(TOP)/Make.config

reload:
	rm -Rf $(TOP)/tests/dotnet/packages
	$(MAKE) -C $(TOP) -j8 all
	$(MAKE) -C $(TOP) -j8 install
	git clean -xfdq

reload-and-build: reload
	$(MAKE) build

reload-and-run: reload
	$(MAKE) run

build:
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY)

run:
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY) -t:Run

diag:
	$(DOTNET6) build /v:diag msbuild.binlog
