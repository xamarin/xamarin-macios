TOP=../../..
include $(TOP)/Make.config

build-all:
	for platform in $(DOTNET_PLATFORMS); do \
		echo "Building in $$platform"; \
		$(MAKE) -C "$$platform" build; \
	done
