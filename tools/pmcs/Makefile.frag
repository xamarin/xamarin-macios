
$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/pmcs: $(TOP)/tools/pmcs/pmcs
	$(Q) install -d $(dir $@)
	$(Q) install -m 755 $^ $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/pmcs/pmcs.exe: $(PMCS_EXE)
	$(Q) install -d $(dir $@)
	$(Q) install -m 644 $^ $^.mdb $(dir $@)

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/pmcs/profiles:
	$(Q) mkdir -p $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/pmcs/profiles/%: $(TOP)/tools/pmcs/profiles/% | $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/pmcs/profiles
	$(Q) install -m 644 $^ $@

PMCS_MONOTOUCH_INSTALL_TARGETS = \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/pmcs \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/pmcs/pmcs.exe \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/pmcs/profiles/classic \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/pmcs/profiles/unified

PMCS_SOURCES =                                                  \
	$(wildcard $(TOP)/tools/pmcs/CSharp/*.cs)                   \
	$(wildcard $(TOP)/tools/pmcs/CSharp/Ast/*.cs)               \
	$(TOP)/tools/pmcs/Driver.cs                                 \
	$(TOP)/tools/pmcs/Settings.cs                               \
	$(TOP)/tools/pmcs/Preprocessor.cs                           \
	$(TOP)/tools/pmcs/XamarinPreprocessorVisitor.cs             \
	$(TOP)/tools/pmcs/Profile.cs                                \
	$(TOP)/tools/pmcs/BuiltinProfiles.cs                        \
	$(TOP)/tools/pmcs/Replacement.cs                            \
	$(TOP)/tools/pmcs/Timer.cs                                  \
	$(MONO_PATH)/mcs/class/Mono.Options/Mono.Options/Options.cs \
