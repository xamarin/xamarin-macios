# big, dirty hack - but a nice time saver to produce an API diff between any builds
# ./diff-bundles.sh base.zip target.zip

# anything alreaady built is wrong
make clean

# unzip the base/origin bundle zip
rm -Rf origin
unzip -d origin $1

# copy files over (dirty!) the current install directories
cp -cr origin/2.1/ ../../_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/2.1/
cp -cr origin/Xamarin.iOS/ ../../_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.iOS/
cp -cr origin/Xamarin.TVOS/ ../../_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.TVOS/
cp -cr origin/Xamarin.WatchOS/ ../../_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.WatchOS/

cp origin/Xam*.dll* ../../_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/mono/ 
cp -cr origin/4.5/ ../../_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/mono/4.5/
cp -cr origin/Xamarin.Mac/ ../../_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/mono/Xamarin.Mac/

# cleanup after ourselves
rm -Rf origin

# run Makefile using the target/current bundle
BUNDLE_ZIP=$2 make jenkins-api-diff

echo Custom API diff complete. You need to clean this directory and do another "make install"
