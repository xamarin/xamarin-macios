TOP=../../..

include $(TOP)/Make.config

prepare:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet copy-dotnet-config
	rm -Rf */bin */obj

clean:
	rm -Rf */bin */obj

diag: prepare
	$(DOTNET6) build /v:diag *binlog
