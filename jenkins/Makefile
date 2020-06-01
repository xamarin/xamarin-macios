TOP=..
include $(TOP)/Make.config

all check:
	shellcheck *.sh

print-abspath-variable:
	@echo $(VARIABLE)=$(abspath $($(VARIABLE)))
