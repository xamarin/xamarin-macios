TOP=../../..
include $(TOP)/Make.config

PACKAGE_PATH = ../../../packages
NUGET_CONSOLE = $(PACKAGE_PATH)/NUnit.Runners.2.6.4/tools/nunit-console.exe
NUGET_LIB = $(PACKAGE_PATH)/NUnit.2.6.4/lib/nunit.framework.dll
CECIL_LIB = ../../../external/mono/external/cecil/bin/net_4_0_Release/Mono.Cecil.dll

test_sources = \
	aot.cs \
	../../common/StringUtils.cs

ifdef INCLUDE_MAC
all-local:: run
endif

run:: tests.dll $(NUGET_CONSOLE)
	$(Q_NUNIT) mono --runtime=v4.0 $(NUGET_CONSOLE) -noshadow -nologo tests.dll

mmp.exe: ../mmp.exe
	$(Q) cp ../mmp.exe mmp.exe
 
nunit.framework.dll nunit-console.exe:
	/Library/Frameworks/Mono.framework/Commands/nuget restore packages.config
	cp $(NUGET_LIB) nunit.framework.dll

Mono.Cecil.dll:
	cp $(CECIL_LIB) Mono.Cecil.dll

tests.dll: $(test_sources) mmp.exe nunit.framework.dll Mono.Cecil.dll
	$(Q_MCS) $(SYSTEM_MCS) $(test_sources) -r:mmp.exe -t:library -out:tests.dll -r:nunit.framework.dll -r:Mono.Cecil.dll -debug

clean-local::
	rm -f tests.dll
	rm -f tests.dll.mdb
	rm -f nunit.framework.dll
	rm -f TestResult.xml
