# New Releases

Note: Don't forget to :warning: **build** :warning: the XI/XM assemblies before trying to regenerate the XML files.

Doing a `make update-refs` will update the XML files where we keep the current **public** API.

The result should be added/committed to git so every future revisions can be compared to the reference files.

= Update reference sources using the dlls from the System version of XI/XM =

    make update-ios-refs     -j IOS_DESTDIR= IOS_INSTALL_VERSION=Current
    make update-watchos-refs -j IOS_DESTDIR= IOS_INSTALL_VERSION=Current
    make update-tvos-refs    -j IOS_DESTDIR= IOS_INSTALL_VERSION=Current
    make update-mac-refs     -j MAC_DESTDIR= MAC_INSTALL_VERSION=Current

You can change *_INSTALL_VERSION to other than Current if you have a different version
installed (say you have XI 9.4.0.0 installed in /Library/Frameworks/Xamarin.iOS.framework/Versions/9.4.0.0,
in which case you can do IOS_INSTALL_VERSION=9.4.0.0


# New Revisions

On the bots each revision rebuilds every assemblies. Each of them will be compared to the XML reference files. Any changes (addition/removal) to the public API will be reported in HTML files.

This can be done manually with `make`. The `.\diff\` directory will contain the diffs in HTML format.

The helper `make merge` target creates a single `api-diff.html` file (from all the `diff\*.html` files) that be used for the documentation web site.
