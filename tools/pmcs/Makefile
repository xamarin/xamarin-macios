ifeq ($(strip $(WINDOWS)),)
MCS_FLAGS = -r:Mono.CompilerServices.SymbolWriter
else
MCS_FLAGS = -d:WINDOWS_BUILD
endif

pmcs.exe: $(wildcard */*.cs *.cs)
	xbuild pmcs.csproj

clean:
	rm -rf pmcs.exe pmcs.exe.mdb obj
