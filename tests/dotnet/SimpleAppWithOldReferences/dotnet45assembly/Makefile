TOP=../../../..

include $(TOP)/Make.config

dotnet45assembly.dll: Library.cs
	$(Q) $(DOTNET) build *.csproj $(DOTNET_BUILD_VERBOSITY)

all-local:: dotnet45assembly.dll
