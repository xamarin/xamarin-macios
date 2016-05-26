
PLATFORM=ios
ifdef COMPAT
PROFILE=devcompat
else
PROFILE=devunified
endif

all:
	make -C ../ build-$(PLATFORM)-$(PROFILE)-monotouch-test

clean:
	make -C ../ clean-$(PLATFORM)-$(PROFILE)-monotouch-test

install:
	make -C ../ install-$(PLATFORM)-$(PROFILE)-monotouch-test

exec:
	make -C ../ exec-$(PLATFORM)-$(PROFILE)-monotouch-test

# check that all files
check-files:
	@grep "Compile Include" monotouch-test.csproj | sed 's/.*Compile Include="//' | sed 's/".*//' | grep -v "^[.][.]" | sed 's_\\_/_' | sort > csproj-files
	@ls -1 *.cs */*.cs | sort > cs-files
	@if ! diff -u csproj-files cs-files; then \
		echo "There are test files not included in monotouch-test.csproj"; \
		rm -f csproj-files cs-files; \
		exit 1; \
	fi
	@rm -f csproj-files cs-files
