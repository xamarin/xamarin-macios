#!/bin/bash -e

# if you edit these version numbers, remember to edit the error message
# in Distrubution.in too to refer to the new version.
MONO_MIN_REQUIRED_VERSION=3.2.0

VERSION_FILE=/Library/Frameworks/Mono.framework/Versions/Current/lib/pkgconfig/mono.pc

if test -f $VERSION_FILE; then
        # Grab the version from the pkgconfig file
        installed_version=`grep Version $VERSION_FILE | cut -d ' ' -f 2`

        # If the installed version exactly matches the minimum, success!
        if `test "$installed_version" = "$MONO_MIN_REQUIRED_VERSION"`; then
                exit 2 # the .pkg installer really wants 2 to mean "OK"
        fi

        # Use 'sort', splitting on the '.' character, to compare the installed version and our minimum acceptable version
        # The newest version will be sorted to the end, so use tail to select the last item
        newest_version=`echo -e $MONO_MIN_REQUIRED_VERSION'\n'$installed_version | sort -t'.' -g | tail -n 1`

        if `test "$newest_version" = "$installed_version"`; then
                exit 2 # the .pkg installer really wants 2 to mean "OK"
        fi
fi

exit 1
