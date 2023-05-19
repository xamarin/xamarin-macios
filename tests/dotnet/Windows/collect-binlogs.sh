#!/bin/bash -eux

set -o pipefail
#IFS=$'\n\t'

cd "$(dirname "${BASH_SOURCE[0]}")"

DIRS=("$1" "$2")
rm -f files.txt
for dir in "$@"; do
	echo $dir
	for file in $(git --git-dir $dir/.git ls-files -o '*.binlog'); do
		echo "$dir/$file" >> files.txt
	done
done

if test -f files.txt; then
	mkdir -p ~/remote_build_testing/
	rm -f ~/remote_build_testing/windows-remote-logs.zip
	zip -9r ~/remote_build_testing/windows-remote-logs.zip -@ < files.txt
else
	echo "No binlogs found."
fi

if ls ~/Library/Logs/Xamarin.Messaging-* >& /dev/null ; then
	zip -9r ~/remote_build_testing/windows-remote-logs.zip ~/Library/Logs/Xamarin.Messaging-*
else
	echo "No logs in ~/Library/Logs/Xamarin.Messaging"
fi

if test -d ~/Library/Caches/Xamarin; then
	find ~/Library/Caches/Xamarin > filelist.txt
	find ~/Library/Caches/Xamarin -print0 | xargs -0 ls -lad >> filelist.txt
	find ~/Library/Caches/Xamarin/XMA/Agents -type f -print0 | xargs -0 shasum >> filelist.txt
else
	echo "No files in ~/Library/Caches/Xamarin" > filelist.txt
fi
zip -9r ~/remote_build_testing/windows-remote-logs.zip filelist.txt
rm -f filelist.txt
