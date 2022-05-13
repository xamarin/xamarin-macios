TOP=../../../..
include $(TOP)/Make.config

global.json: $(TOP)/global6.json
	$(Q) $(CP) $< $@

run-ios-sim:: global.json
	$(DOTNET) run --project ios/ios.csproj
	
run-mac:: global.json
	$(DOTNET) run --project macOS/macOS.csproj
