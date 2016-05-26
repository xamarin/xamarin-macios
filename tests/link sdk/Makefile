
PLATFORM=ios
PROFILE=devunified

all:
	make -C ../ build-$(PLATFORM)-$(PROFILE)-link\ sdk

clean:
	rm -rf obj bin

install:
	make -C ../ install-$(PLATFORM)-$(PROFILE)-link\ sdk

exec:
	make -C ../ exec-$(PLATFORM)-$(PROFILE)-link\ sdk

debug-dev:
	fruitstrap debug --bundle bin/iPhone/Debug/linksdk.app --args "-app-arg:-autostart"

