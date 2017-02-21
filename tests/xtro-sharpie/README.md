# Extrospection Tests based on ObjectiveSharpie #


## Goals

* Compare our bindings with the information available Apple's C/ObjC header files



## Design

* The runner visit the provided (managed) assembly first, then it visit the precompiled headers (pch file) for an SDK (e.g. iOS or OSX);

* Rules can be called at any steps to gather data and or report issues. Rules are also called at the end of the visits;

* Rules should be kept simple and the external files, e.g. `known-issues`, should be used to track special cases, along with comments with our decisions, i.e. why we tolarate them. That will ease code sharing across existing and new platforms;


## Rules

### Existing

Those should be _good enough_ to be execute on the bots on each build.

	1) classify: takes the output from either 'sharpie' or 'all' (ios.results and osx.results files) classifies them in [ios|osx|common].[ignore|pending|unclassified] files
		NOTE: 	to add an entry to the ignore and pending files, just copy the entire line from the unclassified file into them and add your own comments 
			(why we are not binding/fixing that? who is going to bind this? etc) 

### Work In Progress

E.g. rules might be too noisy and require refinement, either in code or in external files.

### Ideas

Anything we do not check but for which data is available, e.g.

* NullAllowed;
* Enum member values;
* Generic updates to existing API (need to find a way to avoid braking changes first)


## Notes

* To develop you need a checkout of ObjectiveSharpie

* `clang` is only built for 64bits so you need a 64bits mono to execute the tool. The latest mono versions (required to build `xamarin-macios` supply a `mono64` binary);

* You can use the `gen-[platform]` or `gen-all` target of the `Makefile` to generate C# code for all the API from the headers. You can then copy/paste from the (large) files to create the missing bindings;
