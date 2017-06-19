
ifneq ($(RELEASE),)
CONFIG=Release64
else
CONFIG=Debug64
endif

all: build-dev

build-dev:
	$(MAKE) -C ../.. build-ios-dev64-$(LIB)

clean-dev:
	$(MAKE) -C ../.. clean-ios-dev32-$(LIB) clean-ios-dev64-$(LIB)

install-dev:
	$(MAKE) -C ../.. install-ios-dev64-$(LIB)

exec-dev:
	$(MAKE) -C ../.. exec-ios-dev64-$(LIB)

debug-dev:
	fruitstrap debug --bundle bin/iPhone/$(CONFIG)-unified/$(shell echo $(LIB) | sed 's/-//g' | sed 's/\.//g')tests.app --args "-app-arg:-autostart"

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
