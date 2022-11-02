TOP=../../../..

include $(TOP)/Make.config

dotnet45assembly.dll: Library.cs
	$(SYSTEM_CSC) -target:library -out:$@ $<

all-local:: dotnet45assembly.dll
