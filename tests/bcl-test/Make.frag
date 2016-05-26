
ifneq ($(RELEASE),)
CONFIG=Release
else
CONFIG=Debug
endif

all: build-dev

build-dev:
	$(MAKE) -C ../.. build-ios-devunified-$(LIB)

clean-dev:
	$(MAKE) -C ../.. clean-ios-devunified-$(LIB)

install-dev:
	$(MAKE) -C ../.. install-ios-devunified-$(LIB)

exec-dev:
	$(MAKE) -C ../.. exec-ios-devunified-$(LIB)

debug-dev:
	fruitstrap debug --bundle bin/iPhone/$(CONFIG)/$(shell echo $(LIB) | sed 's/-//g' | sed 's/\.//g').app --args "-app-arg:-autostart"

build: build-dev
clean: clean-dev
install: install-dev
exec: exec-dev
run: debug-dev

logdev:
		$(MAKE) -C ../.. logdev

build-%:
		$(MAKE) -C ../.. $@-$(LIB)
run-%:
		$(MAKE) -C ../.. $@-$(LIB)
exec-%:
		$(MAKE) -C ../.. $@-$(LIB)
install-%:
		$(MAKE) -C ../.. $@-$(LIB)
clean-%:
		$(MAKE) -C ../.. $@-$(LIB)
