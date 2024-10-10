define TemplateScript
$(1)=$(TOP)/scripts/$(2)/bin/Debug/$(2).dll
$(1)_EXEC=$(DOTNET) exec $$($(1))

$$($(1)): $$(wildcard $$(TOP)/scripts/$(2)/*.cs) $$(wildcard $$(TOP)/scripts/$(2)/*.csproj)
	$$(Q) $$(DOTNET) build $(TOP)/scripts/$(2)/*.csproj /bl:$$(TOP)/scripts/$(2)/msbuild.binlog $$(DOTNET_BUILD_VERBOSITY)
endef
