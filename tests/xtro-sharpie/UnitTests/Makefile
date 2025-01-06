TOP=../../..

include $(TOP)/Make.config


build:
	$(DOTNET) build UnitTests.csproj

publish:
	$(DOTNET) publish UnitTests.csproj

# Example TEST_FILTER:
#    TEST_FILTER="--filter FullyQualifiedName~Xtro"
# Docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details
run-tests run-unit-tests:
	$(DOTNET) test UnitTests.csproj $(TEST_FILTER)
