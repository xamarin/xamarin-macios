
LIB=mini

ifneq ($(RELEASE),)
CONFIG=Release
else
CONFIG=Debug
endif

ifeq ($(TV),1)
CSPROJ_CONFIG=tvos
TARGET_SUFFIX=tvos
else
CSPROJ_CONFIG=dual
TARGET_SUFFIX=dual
endif

all: build-dev

clean: clean-dev

install: install-dev

exec: exec-dev

debug: debug-dev

build-dev:
	make -C .. build-ios-devunified-mini

clean-dev:
	make -C .. clean-ios-devunified-mini
	$(RM) -r obj

install-dev:
	make -C .. install-ios-devunified-mini

exec-dev:
	make -C .. exec-ios-devunified-mini

debug-dev:
	fruitstrap debug --bundle bin/iPhone/$(CONFIG)X/mini.app --args "-app-arg:-autostart"

build-%:
		$(MAKE) -C .. $@-$(LIB)
run-%:
		$(MAKE) -C .. $@-$(LIB)
exec-%:
		$(MAKE) -C .. $@-$(LIB)
install-%:
		$(MAKE) -C .. $@-$(LIB)
clean-%:
		$(MAKE) -C .. $@-$(LIB)
