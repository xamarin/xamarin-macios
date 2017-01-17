TOP=../../..
include $(TOP)/Make.config

test_sources = \
	aot.cs \

ifdef INCLUDE_MAC
all-local:: run
endif

run:: tests.dll
	$(Q_NUNIT) /Library/Frameworks/Mono.framework/Commands/nunit-console4 -noshadow -nologo tests.dll

mmp.exe: ../mmp.exe
	$(Q) cp ../mmp.exe mmp.exe

tests.dll: $(test_sources) mmp.exe
	$(Q_MCS) $(SYSTEM_MCS) $(test_sources) -r:mmp.exe -t:library -out:tests.dll -r:nunit.framework -debug

clean-local::
	rm -f tests.dll
