#!/bin/bash -eu

set -o pipefail


# Get the list of runtimes using 'simctl runtime list -v'
# Remove lines that are not important (headers, etc)
# Replace newlines with "|""
# Replace "|    " with @
# Replace "|" with newline
# This will result in one simulator runtime per line, with each field separated by @
#
# The format is:
#
# iOS 17.2 (21C62) - 9213341D-E553-4D07-963F-69A4A1868257
#     State: Ready
#     Image Kind: Disk Image
#     Signature State: Verified
#     Deletable: YES
#     Last Used At: 2024-07-12 11:19:30 +0000
#     Mount Policy: Automatic
#     Mount Path: /Library/Developer/CoreSimulator/Volumes/iOS_21C62
#     Image Path: /Library/Developer/CoreSimulator/Images/9213341D-E553-4D07-963F-69A4A1868257.dmg
#     Size: 6.8G
#     Bundle Path: /Library/Developer/CoreSimulator/Volumes/iOS_21C62/Library/Developer/CoreSimulator/Profiles/Runtimes/iOS 17.2.simruntime
# iOS 18.0 (22A3351) - DA026F0C-2B28-4CB6-BCCC-9604B3A31AE2
#     State: Ready
#     Image Kind: Disk Image
#     Signature State: Verified
#     Deletable: YES
#     Last Used At: 2024-09-13 16:44:13 +0000
#     Mount Policy: Automatic
#     Mount Path: /Library/Developer/CoreSimulator/Volumes/iOS_22A3351
#     Image Path: /Library/Developer/CoreSimulator/Cryptex/Images/bundle/SimRuntimeBundle-D923E969-236B-4F3C-BCA7-15D410828009/Restore/096-69245-692.dmg
#     Size: 7.8G
#     Bundle Path: /Library/Developer/CoreSimulator/Volumes/iOS_22A3351/Library/Developer/CoreSimulator/Profiles/Runtimes/iOS 18.0.simruntime
#     Parent ID: D923E969-236B-4F3C-BCA7-15D410828009
#     Parent Image Path: /System/Library/AssetsV2/com_apple_MobileAsset_iOSSimulatorRuntime/cc1f035290d244fca4f74d9d243fcd02d2876c27.asset/AssetData/096-69246-684.dmg
#     Parent Mount Path: /Library/Developer/CoreSimulator/Cryptex/Images/bundle/SimRuntimeBundle-D923E969-236B-4F3C-BCA7-15D410828009
#
# And we end up with this, which is easier to parse in bash:
#
# iOS 17.2 (21C62) - 9213341D-E553-4D07-963F-69A4A1868257@State: Ready@Image Kind: Disk Image@Signature State: Verified@Deletable: YES@Last Used At: 2024-07-12 11:19:30 +0000@Mount Policy: Automatic@Mount Path: /Library/Developer/CoreSimulator/Volumes/iOS_21C62@Image Path: /Library/Developer/CoreSimulator/Images/9213341D-E553-4D07-963F-69A4A1868257.dmg@Size: 6.8G@Bundle Path: /Library/Developer/CoreSimulator/Volumes/iOS_21C62/Library/Developer/CoreSimulator/Profiles/Runtimes/iOS 17.2.simruntime
# iOS 18.0 (22A3351) - DA026F0C-2B28-4CB6-BCCC-9604B3A31AE2@State: Ready@Image Kind: Disk Image@Signature State: Verified@Deletable: YES@Last Used At: 2024-09-13 16:44:13 +0000@Mount Policy: Automatic@Mount Path: /Library/Developer/CoreSimulator/Volumes/iOS_22A3351@Image Path: /Library/Developer/CoreSimulator/Cryptex/Images/bundle/SimRuntimeBundle-D923E969-236B-4F3C-BCA7-15D410828009/Restore/096-69245-692.dmg@Size: 7.8G@Bundle Path: /Library/Developer/CoreSimulator/Volumes/iOS_22A3351/Library/Developer/CoreSimulator/Profiles/Runtimes/iOS 18.0.simruntime@Parent ID: D923E969-236B-4F3C-BCA7-15D410828009@Parent Image Path: /System/Library/AssetsV2/com_apple_MobileAsset_iOSSimulatorRuntime/cc1f035290d244fca4f74d9d243fcd02d2876c27.asset/AssetData/096-69246-684.dmg@Parent Mount Path: /Library/Developer/CoreSimulator/Cryptex/Images/bundle/SimRuntimeBundle-D923E969-236B-4F3C-BCA7-15D410828009

IFS=$'\n'
RUNTIMES=$(xcrun simctl runtime list -v | grep -v "^Total" | grep -v ^== | grep -v ^-- | tr '\n' '|' | sed 's/|    /@/g' | tr '|' '\n')

echo "Terminating a few simulator-related services:"
echo "$ sudo launchctl kill -9 system/com.apple.CoreSimulator.simdiskimaged"
sudo launchctl kill -9 system/com.apple.CoreSimulator.simdiskimaged
echo "$ sudo pkill -9 com.apple.CoreSimulator.CoreSimulatorService"
sudo pkill -9 com.apple.CoreSimulator.CoreSimulatorService

for runtimeInfo in ${RUNTIMES[@]}; do
	state=""
	mountPath=""
	imagePath=""
	parentImagePath=""
	bundlePath=""

	IFS='@'
	FIELDS=($runtimeInfo)
	#udid=${FIELDS[0]//* /}
	#echo "udid: $udid"
	echo "${FIELDS[0]}"
	for field in ${FIELDS[@]}; do
		if [[ $field =~ ^State:\ * ]]; then
			state=${field//State: /}
			echo "    State: $state"
		elif [[ $field =~ ^Mount\ Path:\ * ]]; then
			mountPath=${field//Mount Path: /}
			echo "    Mount Path: $mountPath"
		elif [[ $field =~ ^Image\ Path:\ * ]]; then
			imagePath=${field//Image Path: /}
			echo "    Image Path: $imagePath"
		elif [[ $field =~ ^Parent\ Image\ Path:\ * ]]; then
			parentImagePath=${field//Parent Image Path: /}
			echo "    Parent Image Path: $parentImagePath"
		fi
	done

	if [[ $state == "" || $mountPath == "" || $imagePath == "" || $parentImagePath == "" ]]; then
		echo "    - Incomplete info, can't do anything about this simulator"
		continue
	fi

	if [[ $state == "Deleting" || $state =~ ^Unusable*$ ]]; then
		echo "    + OK, let's do this"
	else
		echo "    - Only deleting runtimes with states 'Deleting' or 'Unusuable'"
		continue
	fi

	echo "    + About to unmount $mountPath, current mount status:"
	df -h 2>&1 | sed 's/^/        /'
	echo "    $ sudo diskutil umount \"$mountPath\""
	if ! sudo diskutil umount "$mountPath" 2>&1 | sed 's/^/        /'; then
		echo "    - Failed to unmount $mountPath, will still venture on"
	else
		echo "    + Successfully unmounted $mountPath"
	fi
	echo "    + Post unmount $mountPath status:"
	df -h 2>&1 | sed 's/^/        /'

	echo "    + About to delete $mountPath, current status:"
	ls -la "$mountPath" 2>&1 | sed 's/^/        /' || true
	echo "    $ sudo ls -la \"$mountPath\""
	if ! sudo rm -rf "$mountPath" 2>&1 | sed 's/^/        /'; then
		echo "    - Failed to delete $mountPath, will still venure on"
	else
		echo "    + Successfully deleted $mountPath (or it didn't exist)"
	fi
	echo "    + Post delete $mountPath status:"
	ls -la "$mountPath" 2>&1 | sed 's/^/        /' || true

	echo "    + About to delete $parentImagePath, current status:"
	ls -la "$parentImagePath" 2>&1 | sed 's/^/        /' || true
	echo "    $ sudo ls -la \"$parentImagePath\""
	if ! sudo rm -rf "$parentImagePath" 2>&1 | sed 's/^/        /'; then
		echo "    - Failed to delete $parentImagePath, will still venure on"
	else
		echo "    + Successfully deleted $parentImagePath (or it didn't exist)"
	fi
	echo "    + Post delete $parentImagePath status:"
	ls -la "$parentImagePath" 2>&1 | sed 's/^/        /' || true
done

echo ""
echo "All done, print current info (which *MIGHT NOT BE UPDATED YET*)"
echo ""
xcrun simctl runtime list -v || true
