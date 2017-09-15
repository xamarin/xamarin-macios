<img src="banner.png" alt="Xamarin.iOS + Xamarin.Mac logo" height="145" >

# Xamarin.iOS & Xamarin.Mac #

|              | Status                                    |
|--------------|-------------------------------------------|
| master       | [![xamarin-macios-builds-master][1]][2]   |

[1]: https://jenkins.mono-project.com/view/Xamarin.MaciOS/job/xamarin-macios-builds-master/badge/icon
[2]: https://jenkins.mono-project.com/view/Xamarin.MaciOS/job/xamarin-macios-builds-master

**Welcome!**

This module is the main repository for both **Xamarin.iOS** and **Xamarin.Mac**.

These frameworks allow us to create native iOS, tvOS, watchOS and Mac applications using the same UI controls we would in Objective-C and Xcode, except with the flexibility and elegance of a modern language (C#), the power of the .NET Base Class Library (BCL), and two first-class IDEs&mdash;Xamarin Studio and Visual Studio&mdash;at our fingertips.

### Continuous Builds ###

You can download continuous builds of our main development branches from [our wiki page](https://github.com/xamarin/xamarin-macios/wiki#continuous-builds).

## Build requirements ##

* Autoconf, automake and libtool.

  You can use brew, or [this script](https://gist.github.com/rolfbjarne/3a979187ddd0855da073) to get
  it directly from gnu.org (you'll have to edit your PATH to include /opt/bin if you use the script)
	
  To install brew and all the tool dependencies:

      $ ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
      $ brew update
      $ brew install libtool autoconf automake bison flex

* CMake

  You can use brew, or download manually from [cmake.org](https://cmake.org/download/).

  CMake must be in PATH, so if you install it somewhere else, you'll have to
  fix up your PATH accordingly (not necessary if installed using brew).

  To install using brew:

      $ brew install cmake

* Xcode

  To build the Xamarin.iOS and Xamarin.Mac SDKs you need a certain version of Xcode.
  The build will tell you exactly which version you need.

  You can download the Xcode version you need from [Apple's Developer Center](https://developer.apple.com/downloads/index.action?name=Xcode)
  (requires an Apple Developer account).

  To ease development with different versions of the SDK that require different versions
  of Xcode, we require Xcode to be in a non-standard location (based on the Xcode version).

  For example Xcode 7.0 must be installed in /Applications/Xcode7.app.

  The recommended procedure is to download the corresponding Xcode dmg from Apple's
  Developer Center, extract Xcode.app to your system, _and rename it before
  launching it the first time_. Renaming Xcode.app after having launched it
  once may confuse Xcode, and strange errors start occuring.

* Mono MDK.

  The build will tell you if you need to update, and where to get it.

* Xamarin Studio.

  The build will tell you if you need to update, and where to get it.

* You can also provision some of the dependencies with an included script:

        $ ./system-dependencies.sh --provision-[xcode|xamarin-studio|mono|all]

## Quick build & install ##

Follow the following steps to build and install Xamarin.iOS and Xamarin.Mac:

1. Clone this repository and its submodules

        $ git clone --recursive git@github.com:xamarin/xamarin-macios.git
        $ cd xamarin-macios

2. Fetch dependencies and build everything

        $ make world

3. Make sure permissions are OK to install into system directories (this will ask for your password)

        $ make fix-install-permissions

4. Install into the system

        $ make install-system

5. Build again after any local changes

    Don't use `make world` again to rebuild, because it resets dependencies
    and causes unnecessary rebuilds. Instead use the standard `make all install`
    (our Makefiles are parallel safe, which greatly speeds up the build):

        $ make all -j8 && make install -j8

## Configure ##

There is a configure script that can optionally be used to configure the build.
By default, everything required for both Xamarin.iOS and Xamarin.Mac will be built.

* --disable-mac: Disable Mac-related parts.
* --disable-ios: Disable iOS-related parts.

    In both cases the resulting build will contain both iOS and Mac bits because:

    * Parts of the iOS build depends on Mac parts (in particular mtouch uses
      Xamarin.Mac).

    * The class libraries builds can not be disabled because a very common error
      is to end up with code that only works/builds in either iOS or Mac.

* --enable-ccache: Enables cached builds with `ccache` (default if `ccache` is found in the path).
* --disable-ccache: Disables cached builds with `ccache`, even if it is present.
* --disable-strip: If executables should be stripped or not. This makes it easier
  to debug native executables using lldb.
* --help: Show the help.

## Contributing ##

### Mailing Lists

To discuss this project, and participate in the design, we use the [macios-devel@lists.xamarin.com](http://lists.xamarin.com/mailman/listinfo/macios-devel) mailing list.   

### Chat

There is also a gitter chat room that can be used to discuss this project, and participate in the design: 
[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/xamarin/xamarin-macios?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

### Coding Guidelines

We use [Mono's Coding Guidelines](http://www.mono-project.com/community/contributing/coding-guidelines/).

### Reporting Bugs

We use [Bugzilla](https://bugzilla.xamarin.com/newbug) to track issues.

