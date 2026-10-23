include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

# this file is meant to be included from tests/<test suite>/dotnet/Makefile

TESTNAME:=$(notdir $(shell dirname "$(CURDIR)"))
DOTNET_TEST_PLATFORMS?=$(DOTNET_PLATFORMS)
DOTNET_TEST_DESKTOP_PLATFORMS?=$(DOTNET_DESKTOP_PLATFORMS)

build-%:
	@echo "Building '$(TESTNAME)' for $*"
	$(Q) $(MAKE) -C $* build

build-all: $(foreach platform,$(DOTNET_TEST_PLATFORMS),build-$(platform))
	@echo "Build of '$(TESTNAME)' completed"

build-desktop: $(foreach platform,$(DOTNET_TEST_DESKTOP_PLATFORMS),build-$(platform))
	@echo "Build of '$(TESTNAME)' for desktop platforms completed"

run-%:
	@echo "Running '$(TESTNAME)' for $*"
	$(Q) $(MAKE) -C $* run

run-all: $(foreach platform,$(DOTNET_TEST_DESKTOP_PLATFORMS),run-$(platform))
	@echo "Run of '$(TESTNAME)' complete"

remote-%:
	@echo "Running '$(TESTNAME)' remotely for $*"
	$(Q) $(MAKE) -C $* run-remote

run-remote-all: $(foreach platform,$(DOTNET_TEST_DESKTOP_PLATFORMS),remote-$(platform))
	@echo "Run of '$(TESTNAME)' complete"

reload:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet reload
