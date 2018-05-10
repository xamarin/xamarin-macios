ifdef ENABLE_XAMARIN
NEEDED_MACCORE_VERSION := f274834136c7b11dd4753f3d1337b4c42e747c55
NEEDED_MACCORE_BRANCH := xcode10

MACCORE_DIRECTORY := maccore
MACCORE_MODULE    := git@github.com:xamarin/maccore.git
MACCORE_VERSION   := $(shell cd $(MACCORE_PATH) 2> /dev/null && git rev-parse HEAD 2> /dev/null)
MACCORE_BRANCH    := $(shell cd $(MACCORE_PATH) 2> /dev/null && git symbolic-ref --short HEAD 2> /dev/null)

define CheckVersionTemplate
check-$(1)::
	@rm -f $(THISDIR)/.stamp-reset-$(1)
	@if test x$$(IGNORE_$(2)_VERSION) = "x"; then \
		if test ! -d $($(2)_PATH); then \
			if test x$$(RESET_VERSIONS) != "x"; then \
				make reset-$(1) || exit 1; \
			else \
				echo "Your $(1) checkout is $(COLOR_RED)missing$(COLOR_CLEAR), please run 'make reset-$(1)'"; \
				touch .check-versions-failure; \
			fi; \
		else \
			if test "x$($(2)_VERSION)" != "x$(NEEDED_$(2)_VERSION)" ; then \
				if test x$$(RESET_VERSIONS) != "x"; then \
					make reset-$(1) || exit 1; \
				else \
					echo "Your $(1) version is $(COLOR_RED)out of date$(COLOR_CLEAR), please run 'make reset-$(1)' (found $($(2)_VERSION), expected $(NEEDED_$(2)_VERSION)). Alternatively export IGNORE_$(2)_VERSION=1 to skip this check."; \
					test -z "$(BUILD_REVISION)" || $(MAKE) test-$(1); \
					touch .check-versions-failure; \
				fi; \
			elif test "x$($(2)_BRANCH)" != "x$(NEEDED_$(2)_BRANCH)" ; then \
				if test x$$(RESET_VERSIONS) != "x"; then \
					test -z "$(BUILD_REVISION)" || $(MAKE) test-$(1); \
					make reset-$(1) || exit 1; \
				else \
					echo "Your $(1) branch is $(COLOR_RED)out of date$(COLOR_CLEAR), please run 'make reset-$(1)' (found $($(2)_BRANCH), expected $(NEEDED_$(2)_BRANCH)). Alternatively export IGNORE_$(2)_VERSION=1 to skip this check."; \
					touch .check-versions-failure; \
				fi; \
			else \
				echo "$(1) is $(COLOR_GREEN)up-to-date$(COLOR_CLEAR)."; \
			fi; \
		fi; \
	else \
		echo "$(1) is $(COLOR_GRAY)ignored$(COLOR_CLEAR)."; \
	fi

test-$(1)::
	@echo $(1)
	@echo "   $(2)_DIRECTORY=$($(2)_DIRECTORY)"
	@echo "   $(2)_MODULE=$($(2)_MODULE)"
	@echo "   NEEDED_$(2)_VERSION=$(NEEDED_$(2)_VERSION)"
	@echo "   $(2)_VERSION=$($(2)_VERSION)"
	@echo "   NEEDED_$(2)_BRANCH=$(NEEDED_$(2)_BRANCH)"
	@echo "   $(2)_BRANCH=$($(2)_BRANCH)"
	@echo "   $(2)_PATH=$($(2)_PATH) => $(abspath $($(2)_PATH))"

reset-$(1)::
	$(Q) \
	DEPENDENCY_PATH=$($(2)_PATH) \
	DEPENDENCY_MODULE=$($(2)_MODULE) \
	DEPENDENCY_HASH=$(NEEDED_$(2)_VERSION) \
	DEPENDENCY_BRANCH=$(NEEDED_$(2)_BRANCH) \
	DEPENDENCY_DIRECTORY=$($(2)_DIRECTORY) \
	DEPENDENCY_IGNORE_VERSION=$(IGNORE_$(2)_VERSION) \
		$(TOP)/mk/xamarin-reset.sh $(1)
	@touch $(THISDIR)/.stamp-reset-$(1)

print-$(1)::
	@printf "*** %-16s %-45s %s (%s)\n" "$(DIRECTORY_$(2))" "$(MODULE_$(2))" "$(NEEDED_$(2)_VERSION)" "$(NEEDED_$(2)_BRANCH)"

.PHONY: check-$(1) reset-$(1) print-$(1)

reset-versions-impl:: reset-$(1)
check-versions:: check-$(1)
print-versions:: print-$(1)

DEPENDENCY_DIRECTORIES += $($(2)_PATH)

endef

$(MACCORE_PATH):
	$(Q) git clone --recursive $(MACCORE_MODULE) $(MACCORE_PATH)
	$(Q) $(MAKE) reset-maccore

$(eval $(call CheckVersionTemplate,maccore,MACCORE))
-include $(MACCORE_PATH)/mk/versions.mk
$(MACCORE_PATH)/mk/versions.mk: | $(MACCORE_PATH)
endif
