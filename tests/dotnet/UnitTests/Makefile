TOP=../../..

include $(TOP)/Make.config


build:
	$(DOTNET6) build DotNetUnitTests.csproj

publish:
	$(DOTNET6) publish DotNetUnitTests.csproj

# Example TEST_FILTER:
#    TEST_FILTER="--filter FullyQualifiedName~BuildMyCocoaApp"
# Docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details
run-unit-tests:
	$(DOTNET6) test DotNetUnitTests.csproj $(TEST_FILTER)

run-published:
	$(DOTNET6) test bin/Debug/net5.0/publish/DotNetUnitTests.dll $(TEST_FILTER)
