#!/bin/bash -ex

XI_PKG=$XI_PACKAGE

XI_NAME=$(basename $XI_PKG)

if test -z "$XI_NAME"; then
   echo "No XI_NAME. XI_PACKAGE: $XI_PACKAGE"
   exit 1
fi

RETRIES=10
while true; do
   RV=0
   curl -LO --fail $XI_PKG || RV=$?

   if [[ $RV == 0 ]]; then
       # Yay, download succeeded
       break;
   fi

   # Sometimes this happens:
   #
   # [...]
   # 2017-08-15T07:29:26.7901630Z  59  454M   59  270M    0     0  5919k      0  0:01:18  0:00:46  0:00:32 2462k
   # 2017-08-15T07:29:26.7923300Z curl: (56) SSLRead() return error -36
   # 2017-08-15T07:29:26.8258480Z /bin/bash failed with return code: 56
   #
   # This is random, and if trying again it works.
   # So if curl exits with exit code 56, we try again a few times.
   #
   if [[ ( $RV == 56 ) && ( $RETRIES > 0 )]]; then
       echo "A hopefully transient error occurred. Trying again."

       # Wait a little bit before trying again. Wait a little bit longer each time.
       sleep 0.$(echo "10 - $RETRIES" | bc)

       # Make sure nothing is left to interfere with the next download attempt
       rm -f $XI_PKG

       # Make sure we don't loop forever if the internet goes down
       let RETRIES--;
       continue
   fi

   # All hope is lost.
   echo "Failed to download $XI_NAME from $XI_PACKAGE."
   exit $RV
done

sudo /usr/sbin/installer -pkg "$XI_NAME"   -target / -verbose -dumplog

# Print versions
/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mtouch --version
/Library/Frameworks/Mono.framework/Versions/Current/bin/mono --version
cat /Library/Frameworks/Mono.framework/Versions/Current/updateinfo
