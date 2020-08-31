#!/bin/bash -ex

# Do some simple validation
if [[ "$BUILD_REVISION" != "jenkins" ]] ; then
	echo "This script should only be run on Jenkins bots."
	exit 1
fi

# Print disk status before cleaning
df -h

# We don't care about errors in this section, we just want to clean as much as possible
set +e

# Delete all the simulator devices. These can take up a lot of space over time (I've seen 100+GB on the bots)
/Applications/Xcode.app/Contents/Developer/usr/bin/simctl delete all

# Delete old Xcodes.
ls -lad /Applications/Xcode*.app
sudo rm -Rf /Applications/Xcode44.app
sudo rm -Rf /Applications/Xcode5.app
sudo rm -Rf /Applications/Xcode502.app
sudo rm -Rf /Applications/Xcode511.app
sudo rm -Rf /Applications/Xcode6.0.1.app
sudo rm -Rf /Applications/Xcode6.app
sudo rm -Rf /Applications/Xcode601.app
sudo rm -Rf /Applications/Xcode61.app
sudo rm -Rf /Applications/Xcode611.app
sudo rm -Rf /Applications/Xcode62.app
sudo rm -Rf /Applications/Xcode63.app
sudo rm -Rf /Applications/Xcode64.app
sudo rm -Rf /Applications/Xcode7.app
sudo rm -Rf /Applications/Xcode701.app
sudo rm -Rf /Applications/Xcode71.app
sudo rm -Rf /Applications/Xcode711.app
sudo rm -Rf /Applications/Xcode72.app
sudo rm -Rf /Applications/Xcode731.app
sudo rm -Rf /Applications/Xcode8-GM.app
sudo rm -Rf /Applications/Xcode8.app
sudo rm -Rf /Applications/Xcode81-GM.app
sudo rm -Rf /Applications/Xcode81.app
sudo rm -Rf /Applications/Xcode82.app
sudo rm -Rf /Applications/Xcode821.app
sudo rm -Rf /Applications/Xcode83.app
sudo rm -Rf /Applications/Xcode833.app
sudo rm -Rf /Applications/Xcode9-GM.app
sudo rm -Rf /Applications/Xcode9.app
sudo rm -Rf /Applications/Xcode91.app
sudo rm -Rf /Applications/Xcode92.app
sudo rm -Rf /Applications/Xcode93.app
sudo rm -Rf /Applications/Xcode94.app
sudo rm -Rf /Applications/Xcode941.app
sudo rm -Rf /Applications/Xcode10.app
sudo rm -Rf /Applications/Xcode101-beta2.app
sudo rm -Rf /Applications/Xcode101-beta3.app
sudo rm -Rf /Applications/Xcode101.app
sudo rm -Rf /Applications/Xcode102-beta1.app
sudo rm -Rf /Applications/Xcode102.app
sudo rm -Rf /Applications/Xcode1021.app
sudo rm -Rf /Applications/Xcode103.app
sudo rm -Rf /Applications/Xcode10GM.app
sudo rm -Rf /Applications/Xcode11-beta3.app
sudo rm -Rf /Applications/Xcode11-GM.app
sudo rm -Rf /Applications/Xcode11.app
sudo rm -Rf /Applications/Xcode111.app
sudo rm -Rf /Applications/Xcode112.app
sudo rm -Rf /Applications/Xcode1121.app
sudo rm -Rf /Applications/Xcode113.app
sudo rm -Rf /Applications/Xcode1131.app
sudo rm -Rf /Applications/Xcode114-beta1.app
sudo rm -Rf /Applications/Xcode114-beta2.app
sudo rm -Rf /Applications/Xcode114-beta3.app
sudo rm -Rf /Applications/Xcode114.app
sudo rm -Rf /Applications/Xcode1141.app
sudo rm -Rf /Applications/Xcode115-beta1.app
sudo rm -Rf /Applications/Xcode115-beta2.app
sudo rm -Rf /Applications/Xcode115-GM.app
sudo rm -Rf /Applications/Xcode_8.0.app
sudo rm -Rf /Applications/Xcode_8.1.app
sudo rm -Rf /Applications/Xcode_8.2.1.app
sudo rm -Rf /Applications/Xcode_8.3.3.app
sudo rm -Rf /Applications/Xcode_9.0.app
sudo rm -Rf /Applications/Xcode_9.1.0.app
sudo rm -Rf /Applications/Xcode_9.2.0.app
sudo rm -Rf /Applications/Xcode_9.2.app
sudo rm -Rf /Applications/Xcode_9.4.1.app
# Xcode 10.2.1 is currently used by Binding Tools for Swift # sudo rm -Rf /Applications/Xcode_10.2.1.app
sudo rm -Rf /Applications/Xcode_11.3.0.app
sudo rm -Rf /Applications/Xcode_11.5.0.app
sudo rm -Rf /Applications/Xcode_11.6.0-beta1.app
sudo rm -Rf /Applications/Xcode_12.0.0-beta1.app
sudo rm -Rf /Applications/Xcode_12.0.0-beta2.app
sudo rm -Rf /Applications/Xcode_12.0.0-beta3.app
sudo rm -Rf /Applications/Xcode_12.0.0-beta4.app
sudo rm -Rf /Applications/Xcode_12.0.0-beta5.app


# Print disk status after cleaning
df -h
