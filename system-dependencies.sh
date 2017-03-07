#!/bin/bash -e

set -o pipefail

FAIL=
PROVISION_DOWNLOAD_DIR=/tmp/x-provisioning

# parse command-line arguments
while ! test -z $1; do
	case $1 in
		--provision-xcode)
			PROVISION_XCODE=1
			shift
			;;
		--provision)
			# historical reasons :(
			PROVISION_XCODE=1
			PROVISION_XS=1
			shift
			;;
		--provision-xamarin-studio)
			PROVISION_XS=1
			shift
			;;
		--provision-mono)
			PROVISION_MONO=1
			shift
			;;
		--provision-cmake)
			PROVISION_CMAKE=1
			shift
			;;
		--provision-autotools)
			PROVISION_AUTOTOOLS=1
			shift
			;;
		--provision-all)
			PROVISION_MONO=1
			PROVISION_XS=1
			PROVISION_XCODE=1
			PROVISION_CMAKE=1
			PROVISION_AUTOTOOLS=1
			PROVISION_HOMEBREW=1
			shift
			;;
		--ignore-osx)
			IGNORE_OSX=1
			shift
			;;
		--ignore-xcode)
			IGNORE_XCODE=1
			shift
			;;
		--ignore-xamarin-studio)
			IGNORE_XAMARIN_STUDIO=1
			shift
			;;
		--ignore-mono)
			IGNORE_MONO=1
			shift
			;;
		--ignore-autotools)
			IGNORE_AUTOTOOLS=1
			shift
			;;
		--ignore-cmake)
			IGNORE_CMAKE=1
			shift
			;;
		*)
			echo "Unknown argument: $1"
			exit 1
			;;
	esac
done

# reporting functions
function fail () {
	tput setaf 1 2>/dev/null || true
	echo "    $1"
	tput sgr0 2>/dev/null || true
	FAIL=1
}

function warn () {
	tput setaf 3 2>/dev/null || true
	echo "    $1"
	tput sgr0 2>/dev/null || true
}

function ok () {
	echo "    $1"
}

function log () {
	echo "        $1"
}

# $1: the version to check
# $2: the minimum version to check against
function is_at_least_version () {
	ACT_V=$1
	MIN_V=$2

	if [[ "$ACT_V" == "$MIN_V" ]]; then
		return 0
	fi

	IFS=. read -a V_ACT <<< "$ACT_V"
	IFS=. read -a V_MIN <<< "$MIN_V"
	
	# get the minimum # of elements
	AC=${#V_ACT[@]}
	MC=${#V_MIN[@]}
	COUNT=$(($AC>$MC?$MC:$AC))

	C=0
	while (( $C < $COUNT )); do
		ACT=${V_ACT[$C]}
		MIN=${V_MIN[$C]}
		if (( $ACT > $MIN )); then
			return 0
		elif (( "$MIN" > "$ACT" )); then
			return 1
		fi
		let C++
	done

	if (( $AC == $MC )); then
		# identical?
		return 0
	fi

	if (( $AC > $MC )); then
		# more version fields in actual than min: OK
		return 0
	elif (( $AC == $MC )); then
		# entire strings aren't equal (first check in function), but each individual field is?
		return 0
	else
		# more version fields in min than actual (1.0 vs 1.0.1 for instance): not OK
		return 1
	fi
}

function install_mono () {
	local MONO_URL=`grep MIN_MONO_URL= Make.config | sed 's/.*=//'`
	local MIN_MONO_VERSION=`grep MIN_MONO_VERSION= Make.config | sed 's/.*=//'`

	if test -z $MONO_URL; then
		fail "No MIN_MONO_URL set in Make.config, cannot provision"
		return
	fi

	mkdir -p $PROVISION_DOWNLOAD_DIR
	log "Downloading Mono $MIN_MONO_VERSION from $MONO_URL to $PROVISION_DOWNLOAD_DIR..."
	local MONO_NAME=`basename $MONO_URL`
	local MONO_PKG=$PROVISION_DOWNLOAD_DIR/$MONO_NAME
	curl -L $MONO_URL > $MONO_PKG

	log "Installing Mono $MIN_MONO_VERSION from $MONO_URL..."
	sudo installer -pkg $MONO_PKG -target /

	rm -f $MONO_PKG
}

function install_xamarin_studio () {
	local XS="/Applications/Xamarin Studio.app"
	local XS_URL=`grep MIN_XAMARIN_STUDIO_URL= Make.config | sed 's/.*=//'`
	local MIN_XAMARIN_STUDIO_VERSION=`grep MIN_XAMARIN_STUDIO_VERSION= Make.config | sed 's/.*=//'`

	if test -z $XS_URL; then
		fail "No MIN_XAMARIN_STUDIO_URL set in Make.config, cannot provision"
		return
	fi

	mkdir -p $PROVISION_DOWNLOAD_DIR
	log "Downloading Xamarin Studio $MIN_XAMARIN_STUDIO_VERSION from $XS_URL to $PROVISION_DOWNLOAD_DIR..."
	local XS_NAME=`basename $XS_URL`
	local XS_DMG=$PROVISION_DOWNLOAD_DIR/$XS_NAME
	curl -L $XS_URL > $XS_DMG

	local XS_MOUNTPOINT=$PROVISION_DOWNLOAD_DIR/$XS_NAME-mount
	log "Mounting $XS_DMG into $XS_MOUNTPOINT..."
	hdiutil attach $XS_DMG -mountpoint $XS_MOUNTPOINT -quiet -nobrowse
	log "Removing previous Xamarin Studio from $XS"
	sudo rm -Rf "$XS"
	log "Installing Xamarin Studio $MIN_XAMARIN_STUDIO_VERSION to $XS..."
	sudo cp -R "$XS_MOUNTPOINT/Xamarin Studio.app" /Applications
	log "Unmounting $XS_DMG..."
	hdiutil detach $XS_MOUNTPOINT -quiet

	XS_ACTUAL_VERSION=`/usr/libexec/PlistBuddy -c 'Print :CFBundleShortVersionString' "$XS/Contents/Info.plist"`
	ok "Xamarin Studio $XS_ACTUAL_VERSION provisioned"

	rm -f $XS_DMG
}

function install_specific_xcode () {
	local XCODE_URL=`grep XCODE$1_URL= Make.config | sed 's/.*=//'`
	local XCODE_VERSION=`grep XCODE$1_VERSION= Make.config | sed 's/.*=//'`
	local XCODE_ROOT=$(dirname `dirname $XCODE_DEVELOPER_ROOT`)

	if test -z $XCODE_URL; then
		fail "No XCODE$1_URL set in Make.config, cannot provision"
		return
	fi

	mkdir -p $PROVISION_DOWNLOAD_DIR
	log "Downloading Xcode $XCODE_VERSION from $XCODE_URL to $PROVISION_DOWNLOAD_DIR..."
	local XCODE_NAME=`basename $XCODE_URL`
	local XCODE_DMG=$PROVISION_DOWNLOAD_DIR/$XCODE_NAME

	# To test this script with new Xcode versions, copy the downloaded file to $XCODE_DMG,
	# uncomment the following curl line, and run ./system-dependencies.sh --provision-xcode
	if test -f "~/Downloads/$XCODE_NAME"; then
		log "Found XCode $XCODE_VERSION in your ~/Downloads folder, copying that version instead."
		cp "~/Downloads/$XCODE_NAME" "$XCODE_DMG"
	else
		curl -L $XCODE_URL > $XCODE_DMG
	fi

	if [[ ${XCODE_DMG: -4} == ".dmg" ]]; then
		local XCODE_MOUNTPOINT=$PROVISION_DOWNLOAD_DIR/$XCODE_NAME-mount
		log "Mounting $XCODE_DMG into $XCODE_MOUNTPOINT..."
		hdiutil attach $XCODE_DMG -mountpoint $XCODE_MOUNTPOINT -quiet -nobrowse
		log "Removing previous Xcode from $XCODE_ROOT"
		rm -Rf $XCODE_ROOT
		log "Installing Xcode $XCODE_VERSION to $XCODE_ROOT..."
		cp -R $XCODE_MOUNTPOINT/*.app $XCODE_ROOT
		log "Unmounting $XCODE_DMG..."
		hdiutil detach $XCODE_MOUNTPOINT -quiet
	elif [[ ${XCODE_DMG: -4} == ".xip" ]]; then
		log "Extracting $XCODE_DMG..."
		pushd . > /dev/null
		cd $PROVISION_DOWNLOAD_DIR
		# make sure there's nothing interfering
		rm -Rf *.app
		# extract
		/System/Library/CoreServices/Applications/Archive\ Utility.app/Contents/MacOS/Archive\ Utility "$XCODE_DMG"
		log "Installing Xcode $XCODE_VERSION to $XCODE_ROOT..."
		mv *.app $XCODE_ROOT
		popd > /dev/null
	else
		fail "Don't know how to install $XCODE_DMG"
	fi
	rm -f $XCODE_DMG

	log "Removing any com.apple.quarantine attributes from the installed Xcode"
	sudo xattr -d -r com.apple.quarantine $XCODE_ROOT

	if is_at_least_version $XCODE_VERSION 5.0; then
		log "Accepting Xcode license"
		sudo $XCODE_DEVELOPER_ROOT/usr/bin/xcodebuild -license accept
	fi

	if is_at_least_version $XCODE_VERSION 8.0; then
		PKGS="MobileDevice.pkg MobileDeviceDevelopment.pkg XcodeSystemResources.pkg"
		for pkg in $PKGS; do
			if test -f "$XCODE_DEVELOPER_ROOT/../Resources/Packages/$pkg"; then
				log "Installing $pkg"
				sudo /usr/sbin/installer -dumplog -verbose -pkg "$XCODE_DEVELOPER_ROOT/../Resources/Packages/$pkg" -target /
				log "Installed $pkg"
			else
				log "Not installing $pkg because it doesn't exist."
			fi
		done
	fi

	log "Executing 'sudo xcode-select -s $XCODE_DEVELOPER_ROOT'"
	sudo xcode-select -s $XCODE_DEVELOPER_ROOT

	ok "Xcode $XCODE_VERSION provisioned"
}

function check_specific_xcode () {
	local XCODE_DEVELOPER_ROOT=`grep XCODE$1_DEVELOPER_ROOT= Make.config | sed 's/.*=//'`
	local XCODE_VERSION=`grep XCODE$1_VERSION= Make.config | sed 's/.*=//'`
	local XCODE_ROOT=$(dirname `dirname $XCODE_DEVELOPER_ROOT`)
	local ENABLE_XAMARIN=$(grep -s ^ENABLE_XAMARIN= Make.config.local configure.inc | sed 's/.*=//')
	
	if ! test -d $XCODE_DEVELOPER_ROOT; then
		if ! test -z $PROVISION_XCODE; then
			if ! test -z $ENABLE_XAMARIN; then
				install_specific_xcode $1
			else
				fail "Automatic provisioning of Xcode is only supported for provisioning internal build bots."
				fail "Please download and install Xcode $XCODE_VERSION here: https://developer.apple.com/downloads/index.action?name=Xcode"
			fi
		else
			fail "You must install Xcode ($XCODE_VERSION) in $XCODE_ROOT. You can download Xcode $XCODE_VERSION here: https://developer.apple.com/downloads/index.action?name=Xcode"
		fi
		return
	else
		if is_at_least_version $XCODE_VERSION 5.0; then
			if ! $XCODE_DEVELOPER_ROOT/usr/bin/xcodebuild -license check >/dev/null 2>&1; then
				if ! test -z $PROVISION_XCODE; then
					sudo $XCODE_DEVELOPER_ROOT/usr/bin/xcodebuild -license accept
				else
					fail "The license for Xcode $XCODE_VERSION has not been accepted. Execute 'sudo $XCODE_DEVELOPER_ROOT/usr/bin/xcodebuild' to review the license and accept it."
				fi
				return
			fi
		fi
	fi

	local XCODE_ACTUAL_VERSION=`/usr/libexec/PlistBuddy -c 'Print :CFBundleShortVersionString' "$XCODE_DEVELOPER_ROOT/../version.plist"`
	# this is a hard match, having 4.5 when requesting 4.4 is not OK (but 4.4.1 is OK)
	if [[ ! "x$XCODE_ACTUAL_VERSION" =~ "x$XCODE_VERSION" ]]; then
		fail "You must install Xcode $XCODE_VERSION in $XCODE_ROOT (found $XCODE_ACTUAL_VERSION).  You can download Xcode $XCODE_VERSION here: https://developer.apple.com/downloads/index.action?name=Xcode";
		return
	fi

	local XCODE_SELECT=$(xcode-select -p)
	if [[ "x$XCODE_SELECT" != "x$XCODE_DEVELOPER_ROOT" ]]; then
		if ! test -z $PROVISION_XCODE; then
			log "Executing 'sudo xcode-select -s $XCODE_DEVELOPER_ROOT'"
			sudo xcode-select -s $XCODE_DEVELOPER_ROOT
		else
			fail "'xcode-select -p' does not point to $XCODE_DEVELOPER_ROOT, it points to $XCODE_SELECT. Execute 'sudo xcode-select -s $XCODE_DEVELOPER_ROOT' to fix."
		fi
	fi

	ok "Found Xcode $XCODE_ACTUAL_VERSION in $XCODE_ROOT"
}

function check_xcode () {
	if ! test -z $IGNORE_XCODE; then return; fi

	# must have latest Xcode in /Applications/Xcode<version>.app
	check_specific_xcode

	local XCODE_DEVELOPER_ROOT=`grep ^XCODE_DEVELOPER_ROOT= Make.config | sed 's/.*=//'`
	local IOS_SDK_VERSION=`grep ^IOS_SDK_VERSION= Make.config | sed 's/.*=//'`
	local OSX_SDK_VERSION=`grep ^OSX_SDK_VERSION= Make.config | sed 's/.*=//'`
	local WATCH_SDK_VERSION=`grep ^WATCH_SDK_VERSION= Make.config | sed 's/.*=//'`
	local TVOS_SDK_VERSION=`grep ^TVOS_SDK_VERSION= Make.config | sed 's/.*=//'`

	local D=$XCODE_DEVELOPER_ROOT/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator${IOS_SDK_VERSION}.sdk
	if test ! -d $D -a -z "$FAIL"; then
		fail "The directory $D does not exist. If you've updated the Xcode location it means you also need to update IOS_SDK_VERSION in Make.config."
	fi

	local D=$XCODE_DEVELOPER_ROOT/Platforms/MacOSX.platform/Developer/SDKs/MacOSX${OSX_SDK_VERSION}.sdk
	if test ! -d $D -a -z "$FAIL"; then
		fail "The directory $D does not exist. If you've updated the Xcode location it means you also need to update OSX_SDK_VERSION in Make.config."
	fi

	local D=$XCODE_DEVELOPER_ROOT/Platforms/AppleTVOS.platform/Developer/SDKs/AppleTVOS${TVOS_SDK_VERSION}.sdk
	if test ! -d $D -a -z "$FAIL"; then
		fail "The directory $D does not exist. If you've updated the Xcode location it means you also need to update TVOS_SDK_VERSION in Make.config."
	fi

	local D=$XCODE_DEVELOPER_ROOT/Platforms/WatchOS.platform/Developer/SDKs/WatchOS${WATCH_SDK_VERSION}.sdk
	if test ! -d $D -a -z "$FAIL"; then
		fail "The directory $D does not exist. If you've updated the Xcode location it means you also need to update WATCH_SDK_VERSION in Make.config."
	fi
}

function check_mono () {
	if ! test -z $IGNORE_MONO; then return; fi

	PKG_CONFIG_PATH=/Library/Frameworks/Mono.framework/Versions/Current/bin/pkg-config
	if ! /Library/Frameworks/Mono.framework/Commands/mono --version 2>/dev/null >/dev/null; then
		if ! test -z $PROVISION_MONO; then
			install_mono
		else
			fail "You must install the Mono MDK (http://www.mono-project.com/download/)"
			return
		fi
	elif ! test -e $PKG_CONFIG_PATH; then
		if ! test -z $PROVISION_MONO; then
			install_mono
		else
			fail "Could not find pkg-config, you must install the Mono MDK (http://www.mono-project.com/download/)"
			return
		fi
	fi

	MIN_MONO_VERSION=`grep MIN_MONO_VERSION= Make.config | sed 's/.*=//'`
	MAX_MONO_VERSION=`grep MAX_MONO_VERSION= Make.config | sed 's/.*=//'`

	ACTUAL_MONO_VERSION=`$PKG_CONFIG_PATH --modversion mono`.`cat /Library/Frameworks/Mono.framework/Home/updateinfo | cut -d' ' -f2 | cut -c6- | awk '{print(int($0))}'`
	if ! is_at_least_version $ACTUAL_MONO_VERSION $MIN_MONO_VERSION; then
		if ! test -z $PROVISION_MONO; then
			install_mono
			ACTUAL_MONO_VERSION=`$PKG_CONFIG_PATH --modversion mono`
		else
			fail "You must have at least Mono $MIN_MONO_VERSION, found $ACTUAL_MONO_VERSION"
			return
		fi
	elif [[ "$ACTUAL_MONO_VERSION" == "$MAX_MONO_VERSION" ]]; then
		: # this is ok
	elif is_at_least_version $ACTUAL_MONO_VERSION $MAX_MONO_VERSION; then
		if ! test -z $PROVISION_MONO; then
			install_mono
			ACTUAL_MONO_VERSION=`$PKG_CONFIG_PATH --modversion mono`.`cat /Library/Frameworks/Mono.framework/Home/updateinfo | cut -d' ' -f2 | cut -c6- | awk '{print(int($0))}'`
		else
			fail "Your mono version is too new, max version is $MAX_MONO_VERSION, found $ACTUAL_MONO_VERSION."
			fail "You may edit Make.config and change MAX_MONO_VERSION to your actual version to continue the"
			fail "build (unless you're on a release branch). Once the build completes successfully, please"
			fail "commit the new MAX_MONO_VERSION value."
			return
		fi
	fi

	ok "Found Mono $ACTUAL_MONO_VERSION (at least $MIN_MONO_VERSION and not more than $MAX_MONO_VERSION is required)"
}

function install_autoconf () {
	if ! brew --version >& /dev/null; then
		fail "Asked to install autoconf, but brew is not installed."
		return
	fi

	brew install autoconf
}

function install_libtool () {
	if ! brew --version >& /dev/null; then
		fail "Asked to install libtool, but brew is not installed."
		return
	fi

	brew install libtool
}

function install_automake () {
	if ! brew --version >& /dev/null; then
		fail "Asked to install automake, but brew is not installed."
		return
	fi

	brew install automake
}


function check_autotools () {
	if ! test -z $IGNORE_AUTOTOOLS; then return; fi

IFStmp=$IFS
IFS='
'
	if AUTOCONF_VERSION=($(autoconf --version 2>/dev/null)); then
		ok "Found ${AUTOCONF_VERSION[0]} (no specific version is required)"
	elif ! test -z $PROVISION_AUTOTOOLS; then
		install_autoconf
	else
		fail "You must install autoconf, read the README.md for instructions"
	fi

	if ! LIBTOOL=$(which glibtool 2>/dev/null); then
		LIBTOOL=$(which libtool)
	fi

	if LIBTOOL_VERSION=($($LIBTOOL --version 2>/dev/null )); then
		ok "Found ${LIBTOOL_VERSION[0]} (no specific version is required)"
	elif ! test -z $PROVISION_AUTOTOOLS; then
		install_libtool
	else
		fail "You must install libtool, read the README.md for instructions"
	fi

	if AUTOMAKE_VERSION=($(automake --version 2>/dev/null)); then
		ok "Found ${AUTOMAKE_VERSION[0]} (no specific version is required)"
	elif ! test -z $PROVISION_AUTOTOOLS; then
		install_automake
	else
		fail "You must install automake, read the README.md for instructions"
	fi
IFS=$IFS_tmp
}

function check_xamarin_studio () {
	if ! test -z $IGNORE_XAMARIN_STUDIO; then return; fi

	XS="/Applications/Xamarin Studio.app"
	local XS_URL=`grep MIN_XAMARIN_STUDIO_URL= Make.config | sed 's/.*=//'`
	if ! test -d "$XS"; then
		if ! test -z $PROVISION_XS; then
			install_xamarin_studio
		else
			fail "You must install Xamarin Studio, from http://www.monodevelop.com/download/"
		fi
		return
	fi

	MIN_XAMARIN_STUDIO_VERSION=`grep MIN_XAMARIN_STUDIO_VERSION= Make.config | sed 's/.*=//'`
	MAX_XAMARIN_STUDIO_VERSION=`grep MAX_XAMARIN_STUDIO_VERSION= Make.config | sed 's/.*=//'`
	XS_ACTUAL_VERSION=`/usr/libexec/PlistBuddy -c 'Print :CFBundleShortVersionString' "$XS/Contents/Info.plist"`
	if ! is_at_least_version $XS_ACTUAL_VERSION $MIN_XAMARIN_STUDIO_VERSION; then
		if ! test -z $PROVISION_XS; then
			install_xamarin_studio
			XS_ACTUAL_VERSION=`/usr/libexec/PlistBuddy -c 'Print :CFBundleShortVersionString' "$XS/Contents/Info.plist"`
		else
			fail "You must have at least Xamarin Studio $MIN_XAMARIN_STUDIO_VERSION (found $XS_ACTUAL_VERSION). Download URL: $XS_URL"
		fi
		return
	elif [[ "$XS_ACTUAL_VERSION" == "$MAX_XAMARIN_STUDIO_VERSION" ]]; then
		: # this is ok
	elif is_at_least_version $XS_ACTUAL_VERSION $MAX_XAMARIN_STUDIO_VERSION; then
		if ! test -z $PROVISION_XS; then
			install_xamarin_studio
			XS_ACTUAL_VERSION=`/usr/libexec/PlistBuddy -c 'Print :CFBundleShortVersionString' "$XS/Contents/Info.plist"`
		else
			fail "Your Xamarin Studio version is too new, max version is $MAX_XAMARIN_STUDIO_VERSION, found $XS_ACTUAL_VERSION."
			fail "You may edit Make.config and change MAX_XAMARIN_STUDIO_VERSION to your actual version to continue the"
			fail "build (unless you're on a release branch). Once the build completes successfully, please"
			fail "commit the new MAX_XAMARIN_STUDIO_VERSION value."
			fail "Alternatively you can download an older version from $XS_URL."
		fi
		return
	fi

	ok "Found Xamarin Studio $XS_ACTUAL_VERSION (at least $MIN_XAMARIN_STUDIO_VERSION and not more than $MAX_XAMARIN_STUDIO_VERSION is required)"
}

function check_osx_version () {
	if ! test -z $IGNORE_OSX; then return; fi

	MIN_OSX_BUILD_VERSION=`grep MIN_OSX_BUILD_VERSION= Make.config | sed 's/.*=//'`

	ACTUAL_OSX_VERSION=$(sw_vers -productVersion)
	if ! is_at_least_version $ACTUAL_OSX_VERSION $MIN_OSX_BUILD_VERSION; then
		fail "You must have at least OSX $MIN_OSX_BUILD_VERSION (found $ACTUAL_OSX_VERSION)"
		return
	fi

	ok "Found OSX $ACTUAL_OSX_VERSION (at least $MIN_OSX_BUILD_VERSION is required)"
}

function install_cmake () {
	if ! brew --version >& /dev/null; then
		fail "Asked to install cmake, but brew is not installed."
		return
	fi

	brew install cmake
}

function check_cmake () {
	if ! test -z $IGNORE_CMAKE; then return; fi

	local MIN_CMAKE_VERSION=`grep MIN_CMAKE_VERSION= Make.config | sed 's/.*=//'`
	local CMAKE_URL=`grep CMAKE_URL= Make.config | sed 's/.*=//'`

	if ! cmake --version &> /dev/null; then
		if ! test -z $PROVISION_CMAKE; then
			install_cmake
		else
			fail "You must install CMake ($CMAKE_URL)"
		fi
		return
	fi

	ACTUAL_CMAKE_VERSION=$(cmake --version | grep "cmake version" | sed 's/cmake version //')
	if ! is_at_least_version $ACTUAL_CMAKE_VERSION $MIN_CMAKE_VERSION; then
		fail "You must have at least CMake $MIN_CMAKE_VERSION (found $ACTUAL_CMAKE_VERSION)"
		return
	fi

	ok "Found CMake $ACTUAL_CMAKE_VERSION (at least $MIN_CMAKE_VERSION is required)"
}

function check_homebrew ()
{
IFStmp=$IFS
IFS='
'
	if HOMEBREW_VERSION=($(brew --version 2>/dev/null)); then
		ok "Found Homebrew ($HOMEBREW_VERSION)"
	elif ! test -z $PROVISION_HOMEBREW; then
		log "Installing Homebrew..."
		/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
		HOMEBREW_VERSION=($(brew --version 2>/dev/null))
		log "Installed Homebrew ($HOMEBREW_VERSION)"
	else
		warn "Could not find Homebrew. Homebrew is required to auto-provision some dependencies (autotools, cmake), but not required otherwise."
	fi
IFS=$IFS_tmp
}

echo "Checking system..."

check_osx_version
check_xcode
check_homebrew
check_autotools
check_mono
check_xamarin_studio
check_cmake

if [[ "x$(basename $(pwd))" != xxamarin-macios ]]; then
	fail "xamarin-macios must be checked out in a directory named 'xamarin-macios'."
fi

if test -z $FAIL; then
	echo "System check succeeded"
else
	echo "System check failed"
	exit 1
fi
