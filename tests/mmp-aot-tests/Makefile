TOP=../..
include $(TOP)/Make.config

PACKAGE_PATH = $(TOP)/packages
NUGET_CONSOLE = $(PACKAGE_PATH)/NUnit.Runners.2.6.4/tools/nunit-console.exe
NUGET_LIB = $(PACKAGE_PATH)/NUnit.2.6.4/lib/nunit.framework.dll
CECIL_LIB = $(PACKAGE_PATH)/Mono.Cecil.0.10.3/lib/net40/Mono.Cecil.dll

test_sources = \
	aot.cs \
	$(TOP)/tools/common/StringUtils.cs

ifdef INCLUDE_MAC
all-local:: run
endif

run:: tests.dll $(NUGET_CONSOLE)
	$(Q_NUNIT) mono --runtime=v4.0 $(NUGET_CONSOLE) -noshadow -nologo tests.dll

$(TOP)/tools/mmp/bin/Release/mmp.exe:
	$(Q) $(MAKE) -C $(TOP)/tools/mmp

mmp.exe: $(TOP)/tools/mmp/bin/Release/mmp.exe
	$(Q) $(CP) $< $@
 
nunit.framework.dll nunit-console.exe Mono.Cecil.dll:
	$(SYSTEM_XIBUILD) -t -- /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore packages.config
	$(CP) $(NUGET_LIB) nunit.framework.dll
	$(CP) $(CECIL_LIB) Mono.Cecil.dll

tests.dll: $(test_sources) mmp.exe nunit.framework.dll Mono.Cecil.dll
	$(Q_CSC) $(SYSTEM_CSC) $(test_sources) -r:mmp.exe -t:library -out:tests.dll -r:nunit.framework.dll -r:Mono.Cecil.dll -debug

clean-local::
	rm -f tests.dll
	rm -f tests.dll.mdb
	rm -f nunit.framework.dll
	rm -f TestResult.xml
