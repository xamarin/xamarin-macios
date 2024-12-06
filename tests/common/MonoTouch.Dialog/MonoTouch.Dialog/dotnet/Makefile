TOP=../../../..

all: iOS tvOS MacCatalyst macOS

NuGet.config: $(TOP)/tests/dotnet/NuGet.config
	cp $< $@

global.json: $(TOP)/tests/dotnet/global.json
	cp $< $@

define BuildTemplate
build-$(1): NuGet.config global.json
	donut build $(1)/MonoTouch.Dialog.csproj
endef
$(foreach platform,iOS tvOS MacCatalyst macOS,$(eval $(call BuildTemplate,$(platform))))
