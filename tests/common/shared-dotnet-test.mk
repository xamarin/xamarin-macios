include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

# this file is meant to be included from tests/<test suite>/dotnet/Makefile

build-%:
	@echo "Building for $*"
	$(Q) $(MAKE) -C $* build

build-all: $(foreach platform,$(DOTNET_PLATFORMS),build-$(platform))
	@echo "Build completed"
