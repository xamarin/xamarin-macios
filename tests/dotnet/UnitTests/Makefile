TOP=../../..

include $(TOP)/Make.config


build:
	$(DOTNET) build DotNetUnitTests.csproj

publish:
	$(DOTNET) publish DotNetUnitTests.csproj

# Example TEST_FILTER:
#    TEST_FILTER="--filter FullyQualifiedName~BuildMyCocoaApp"
# Docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details
run-unit-tests:
	$(DOTNET) test DotNetUnitTests.csproj $(TEST_FILTER)

run-published:
	$(DOTNET) test bin/Debug/net5.0/publish/DotNetUnitTests.dll $(TEST_FILTER)
