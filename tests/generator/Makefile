TOP=../..

include $(TOP)/Make.config

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

#
# * Use [IOS|MAC]_TESTS for simple tests that just verify the code can compile / generate.
#   To add such a test, just add the file, and add the name of the file (without extension) to [IOS|MAC]_TESTS.
#
# * Use [IOS|MAC]_CUSTOM_TESTS for more advanced tests that needs more validation.
#   In this case you additionally need to add a target for the name of the test, with the 
#   your test logic.
#

IOS_CURRENT_DIR=$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current
ifdef IKVM
IOS_GENERATOR = $(IOS_CURRENT_DIR)/bin/bgen /unsafe --target-framework=Xamarin.iOS,v1.0
else
IOS_GENERATOR = $(IOS_CURRENT_DIR)/bin/btouch-native /baselib:$(IOS_CURRENT_DIR)/lib/mono/Xamarin.iOS/Xamarin.iOS.dll /unsafe
endif
IOS_TESTS = bug15283 bug15307 bug15799 bug16036 sof20696157 bug23041 bug27430 bug27428 bug34042 btouch-with-hyphen-in-name property-redefination-ios arrayfromhandlebug bug36457 bug39614 bug40282 bug17232 bug24078-ignore-methods-events strong-dict-support-templated-dicts bug43579 bindastests
IOS_CUSTOM_TESTS = forum54078 desk63279 desk79124 multiple-api-definitions1 multiple-api-definitions2 bug29493 classNameCollision bi1036 bug37527 bug27986 bug35176 bi1046 bindas1048error bindas1049error bindas1050modelerror bindas1050protocolerror virtualwrap

MAC_CURRENT_DIR=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
ifdef IKVM
MAC_GENERATOR = $(MAC_CURRENT_DIR)/bin/bgen
else
MAC_GENERATOR = $(MAC_CURRENT_DIR)/bin/bmac
endif
MAC_TESTS = bmac_smoke bmac-with-hyphen-in-name property-redefination-mac NSApplicationPublicEnsureMethods protocol-duplicate-abstract
MAC_CUSTOM_TESTS = bug31788 protocol-duplicate-abstract-error protocol-duplicate-method-diff-length protocol-duplicate-method-diff-out protocol-duplicate-method-diff-type protocol-duplicate-method-diff-return

ALL_TESTS = 
ifdef INCLUDE_IOS
ALL_TESTS += $(IOS_TESTS) $(IOS_CUSTOM_TESTS)
endif
ifdef INCLUDE_MAC
ALL_TESTS += $(MAC_TESTS) $(MAC_CUSTOM_TESTS)
endif

all-local:: $(ALL_TESTS)

$(MAC_TESTS):
ifdef IKVM
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) -d=MONOMAC --target-framework=XamMac,v1.0                                            $@.cs
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) -d=MONOMAC --target-framework=Xamarin.Mac,Version=v4.5,Profile=Full   -d=XAMCORE_2_0 $@.cs
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) -d=MONOMAC --target-framework=Xamarin.Mac,Version=v2.0,Profile=Mobile -d=XAMCORE_2_0 $@.cs
else
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) -d=MONOMAC $@.cs
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) -d=MONOMAC -d=XAMCORE_2_0 --unified-full-profile $@.cs
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) -d=MONOMAC -d=XAMCORE_2_0 --unified-mobile-profile $@.cs
endif

$(IOS_TESTS):
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $@.cs

define ProtocolTestTemplate
$(1):
ifdef IKVM
	$(if $(V),,@echo "$$@";) $(MAC_GENERATOR) -d=MONOMAC --target-framework=XamMac,v1.0                                            $(1).cs 2>&1 | grep BI$(2) > /dev/null
	$(if $(V),,@echo "$$@";) $(MAC_GENERATOR) -d=MONOMAC --target-framework=Xamarin.Mac,Version=v4.5,Profile=Full   -d=XAMCORE_2_0 $(1).cs 2>&1 | grep BI$(2) > /dev/null
	$(if $(V),,@echo "$$@";) $(MAC_GENERATOR) -d=MONOMAC --target-framework=Xamarin.Mac,Version=v2.0,Profile=Mobile -d=XAMCORE_2_0 $(1).cs 2>&1 | grep BI$(2) > /dev/null
else
	$(if $(V),,@echo "$$@";) $(MAC_GENERATOR) -d=MONOMAC $(1).cs 2>&1 | grep BI$(2) > /dev/null
	$(if $(V),,@echo "$$@";) $(MAC_GENERATOR) -d=MONOMAC -d=XAMCORE_2_0 --unified-full-profile $(1).cs 2>&1 | grep BI$(2) > /dev/null
	$(if $(V),,@echo "$$@";) $(MAC_GENERATOR) -d=MONOMAC -d=XAMCORE_2_0 --unified-mobile-profile $(1).cs 2>&1 | grep BI$(2) > /dev/null
endif
endef

$(eval $(call ProtocolTestTemplate,protocol-duplicate-abstract-error,1037))
$(eval $(call ProtocolTestTemplate,protocol-duplicate-method-diff-length,1039))
$(eval $(call ProtocolTestTemplate,protocol-duplicate-method-diff-out,1040))
$(eval $(call ProtocolTestTemplate,protocol-duplicate-method-diff-type,1041))
$(eval $(call ProtocolTestTemplate,protocol-duplicate-method-diff-return,1038))

define iOSErrorCodeTestTemplate
$(1):
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $(1).cs 2>&1 | grep BI$(2) > /dev/null
endef

$(eval $(call iOSErrorCodeTestTemplate,bindas1048error,1048))
$(eval $(call iOSErrorCodeTestTemplate,bindas1049error,1049))
$(eval $(call iOSErrorCodeTestTemplate,bindas1050modelerror,1050))
$(eval $(call iOSErrorCodeTestTemplate,bindas1050protocolerror,1050))

classNameCollision:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $@.cs -s:classNameCollision-enum.cs -tmpdir=$@.tmpdir 

bug31788:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
ifdef IKVM
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) --target-framework=XamMac,v1.0 --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
else
	$(if $(V),,@echo "$@";) $(MAC_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
endif
	@grep -q xamarin_IntPtr_objc_msgSend bug31788.tmpdir/Test/MarshalOnProperty.g.cs
	@grep -q xamarin_IntPtr_objc_msgSend bug31788.tmpdir/Test/MarshalInProperty.g.cs

desk63279:
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) desk63279A.cs desk63279B.cs

desk79124:
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $@.cs >/dev/null 2>&1
	@if ! monodis $@.dll --typedef | grep WYPopoverBackgroundViewAppearance > /dev/null; then \
		echo "error: Could not find WYPopoverBackgroundViewAppearance in generated code."; \
	fi

forum54078:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
	@! grep "Type does not conform to NSCoding" forum54078.tmpdir/Test/CustomController.g.cs

multiple-api-definitions1:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs

multiple-api-definitions2:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir --api=$@-a.cs --api=$@-b.cs

bug29493:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
	@! grep -r "static readonly IntPtr class_ptr = Class.GetHandle (\"global::" bug29493.tmpdir/Bug29493

bug37527:
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $@-missing-property.cs | grep "BaseType.Delegates were set but no properties could be found. Do ensure that the WrapAttribute is used on the right properties." >/dev/null 2>&1
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $@-wrong-property.cs | grep "should be renamed to 'Delegate' for BaseType.Events and BaseType.Delegates to work." >/dev/null 2>&1

bug27986:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
	@if ! grep -r Preserve bug27986.tmpdir/bug27986 > /dev/null; then \
		echo "error: Could not find Preserve attribute in generated code."; exit 1; \
	fi
	@if [ `grep -r Preserve bug27986.tmpdir/bug27986 | wc -l` -ne 28 ]; then \
		echo "Error: Expected 28 Preserve attributes in generated code. If you modified code that generates PreserveAttributes please update the preserve count."; exit 1; \
	fi

bug35176:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
	@if ! grep -r Introduced bug35176.tmpdir/bug35176 > /dev/null; then \
		echo "error: Could not find Introduced attribute in generated code."; exit 1; \
	fi
	@if [ `grep -r Introduced bug35176.tmpdir/bug35176 | wc -l` -ne 8 ]; then \
		echo "Error: Expected 4 Introduced attributes in generated code."; exit 1; \
	fi

bug46292:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs --process-enums
	@if ! grep -r Obsolete bug46292.tmpdir/BindingTests > /dev/null; then \
		echo "error: Could not find Obsolete attribute in generated code."; exit 1; \
	fi
	@if [ `grep -r Obsolete bug46292.tmpdir/BindingTests | wc -l` -ne 2 ]; then \
		echo "Error: Expected 2 Obsolete attributes in generated code."; exit 1; \
	fi

forcedtype:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
	@if ! grep -r GetINativeObject forcedtype.tmpdir > /dev/null; then \
		echo "error: Could not find GetINativeObject usage in generated code."; exit 1; \
	fi
	@if [ `grep -r GetINativeObject forcedtype.tmpdir | wc -l` -ne 12 ]; then \
		echo "Error: Expected 12 GetINativeObject usages in generated code."; exit 1; \
	fi

bi1036:
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $@.cs | grep BI1036 >/dev/null 2>&1

bi1046:
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) $@.cs --process-enums | grep BI1046 >/dev/null 2>&1

virtualwrap:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs --process-enums
	@if ! grep -r "public virtual" virtualwrap.tmpdir/WrapTest > /dev/null; then \
		echo "error: Could not find public virtual keywords in generated code."; exit 1; \
	fi
	@if [ `grep -r "public virtual" virtualwrap.tmpdir/WrapTest | wc -l` -ne 4 ]; then \
		echo "Error: Expected 4 virtual keywords in generated code."; exit 1; \
	fi

clean-local::
	rm -f *.dll *.source
	rm -Rf *.tmpdir
