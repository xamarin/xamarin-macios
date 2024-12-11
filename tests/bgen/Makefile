TOP=../..
include $(TOP)/Make.config

run-tests run-unit-tests:
	$(Q) $(DOTNET) test $(TEST_FILTER) bgen-tests.csproj

build:
	$(Q) $(DOTNET) build bgen-tests.csproj
