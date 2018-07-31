#!/bin/bash -e

DEPENDENCY_NAME="$1"

# Calculate the remote from the format: git@github.com:<remote>/<repo>.git
DEPENDENCY_REMOTE=${DEPENDENCY_MODULE/git@github.com:/}
DEPENDENCY_REMOTE=${DEPENDENCY_REMOTE%%/*}

if test -d "$DEPENDENCY_PATH"; then
	cd "$DEPENDENCY_PATH"
	# Check if we have the remote we need
	if ! git config "remote.$DEPENDENCY_REMOTE.url" > /dev/null; then
		echo "*** [$DEPENDENCY_NAME] git remote add -f '$DEPENDENCY_REMOTE' '$DEPENDENCY_MODULE'"
		git remote add -f "$DEPENDENCY_REMOTE" "$DEPENDENCY_MODULE"
	fi

	# Check if we have the hash we need
	if ! git log -1 --pretty=%H "$DEPENDENCY_HASH" > /dev/null 2>&1; then
		echo "*** [$DEPENDENCY_NAME] git fetch $DEPENDENCY_REMOTE"
		git fetch "$DEPENDENCY_REMOTE"
	fi
else
	echo "*** [$DEPENDENCY_NAME] git clone $DEPENDENCY_MODULE --recursive $DEPENDENCY_DIRECTORY -b $DEPENDENCY_BRANCH --origin $DEPENDENCY_REMOTE"
	mkdir -p "$(dirname "$DEPENDENCY_PATH")"
	cd "$(dirname "$DEPENDENCY_PATH")"
	git clone "$DEPENDENCY_MODULE" --recursive "$DEPENDENCY_DIRECTORY" -b "$DEPENDENCY_BRANCH" --origin "$DEPENDENCY_REMOTE"
	cd "$DEPENDENCY_DIRECTORY"
fi

if ! git log -1 --pretty=%H "$DEPENDENCY_HASH" > /dev/null 2>&1; then
	echo "The hash $DEPENDENCY_HASH does not exist in $DEPENDENCY_MODULE. Please verify that you pushed your changes."
	exit 1
fi

if test -z "$DEPENDENCY_IGNORE_VERSION"; then
	if git rev-parse --verify "$DEPENDENCY_BRANCH" >/dev/null 2>&1; then
		# branch already exists
		echo "*** [$DEPENDENCY_NAME] git checkout -f $DEPENDENCY_BRANCH"
		git checkout -f "$DEPENDENCY_BRANCH"
	else
		# branch does not exist
		echo "*** [$DEPENDENCY_NAME] git checkout -f -b $DEPENDENCY_BRANCH $DEPENDENCY_REMOTE/$DEPENDENCY_BRANCH"
		git checkout -f -b "$DEPENDENCY_BRANCH" "$DEPENDENCY_REMOTE/$DEPENDENCY_BRANCH"
	fi
	echo "*** [$DEPENDENCY_NAME] git reset --hard $DEPENDENCY_HASH"
	git reset --hard "$DEPENDENCY_HASH"
fi

echo "*** [$DEPENDENCY_NAME] git submodule update --init --recursive"
git submodule update --init --recursive

