include ../shared.mk

publish:
	$(Q) $(DOTNET) publish /p:PublishAot=true /bl:$@.binlog /p:RuntimeIdentifier=maccatalyst-arm64 /p:SuppressTrimAnalysisWarnings=false /p:TrimmerSingleWarn=false
