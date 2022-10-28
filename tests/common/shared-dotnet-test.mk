include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

# this file is meant to be included from tests/<test suite>/dotnet/Makefile

build-%:
	@echo "Building for $*"
	$(Q) $(MAKE) -C $* build

build-all: $(foreach platform,$(DOTNET_PLATFORMS),build-$(platform))
	@echo "Build completed"

build-desktop: $(foreach platform,$(DOTNET_DESKTOP_PLATFORMS),build-$(platform))
	@echo "Build completed"

run-%:
	@echo "Running for $*"
	$(Q) $(MAKE) -C $* run

run-all: $(foreach platform,$(DOTNET_DESKTOP_PLATFORMS),run-$(platform))
	@echo "Run complete"

remote-%:
	@echo "Running remotely for $*"
	$(Q) $(MAKE) -C $* run-remote

run-remote-all: $(foreach platform,$(DOTNET_DESKTOP_PLATFORMS),remote-$(platform))
	@echo "Run complete"

reload:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet reload
