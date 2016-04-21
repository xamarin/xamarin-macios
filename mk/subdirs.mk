# subdir handling ala automake, but without dealing with auto*

all: all-hook
clean: clean-hook
install: install-hook

all-local clean-local install-local::

all-hook:: all-recurse
clean-hook:: clean-recurse
install-hook:: install-recurse

all-recurse:: all-local
clean-recurse:: clean-local
install-recurse:: install-local

all-recurse clean-recurse install-recurse::
	@for dir in $(SUBDIRS); do \
		echo "Making $(subst -recurse,,$@) in $$dir"; \
		$(MAKE) -C $$dir $(subst -recurse,,$@) || exit 1; \
	done

.PHONY: all-local all-recurse all-hook
.PHONY: clean-local clean-recurse clean-hook
.PHONY: install-local install-recurse install-hook
