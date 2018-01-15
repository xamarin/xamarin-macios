# Extrospection Tests based on ObjectiveSharpie #


## Goal

* Compare our bindings with the information available Apple's C/ObjC header files;

* Document, using annotations, the rational why some things differs;


### Notes

* Apple's headers are **not** perfect and discrepencies between them and other tests (e.g. intro) must be investigated;

* Sharpie or xtro are not perfect either - whenever in doubt, double check with headers and file bugs as needed;


## Design

* The runner visit the provided (managed) assembly first, then it visit the precompiled headers (pch file) for an SDK (e.g. iOS or OSX);

* Rules can be called at any steps to gather data and or report issues. Rules are also called at the end of the visits;

* Rules should be kept simple and the external files, e.g. `*.ignore`, should be used to track special cases, along with comments with our decisions, i.e. why we tolarate them. That will ease code sharing across existing and new platforms;

* The reporting tool tells if current issues can be ignored, are being worked on (part of a milestone) or needs immediate attention (fails the build);


## Policy

* The report tool **must** always report a success. Pull request can only be merged with an `xtro` green check;


## Report

The `xtro-report` tool creates an html report that describe the results, per platforms and per frameworks (as defined by the header files).

The bots produce the report on every commit/PR. It can also be produced locally with:

````
cd tests/xtro-sharpie
make
open report/index.html
```

### How to read the report

Links under **FIXME (unclassified)** points to the issue that **must** be fixed (to commit).

Links under **TODO (milestone)** indicate the progress for the issue/work under way. E.g. if PR 9999 claims to complete Xcode 99 support for UIKit then **nothing** should be reported on that line, otherwise the work is incomplete.

The links on the frameworks shows the issues that are presently ignored, either for historical reasons (not yet fixed) or because it cannot be fixed immediately (e.g. rdar) or ever. In the later case there are comments why.

#### Things to watch for

Different numbers between platforms can mean:

* extra, platform specific, API; or

* some existing bindings should be enabled for that platform;


## Working Files

### Ignore files

They come in different flavors

* common-{framework}.ignore
* {platform}-{framework}.ignore

They can include _markdown-like_ comments as they will never be sorted.


### Todo files

Format: {platform}-{framework}.todo

They are meant as _short_ term work, e.g. `xcode9.3`. This makes it easy to track progress against the current milestone.

Issues inside those files won't cause the bots to _fail_ a build.


### Unclassified files

Those files are **not** committed to git, they are produced locally (or on bots) based on the current code.

Anything that shows up in the **unclassified** requires immediate action, i.e. the bots will be angry and report a failure - so PR should **not** be merged.

Why ?

* it means it's not something that happened before (0 xtro issue policy);


How to fix ?

* If it's a short term issue, e.g. a new xcode beta, then the entries should be moved to the corresponding `*.todo` file;
* If this is not something we can fix (e.g. requires a rdar) then the entry should be moved to the corresponding `*.ignore` file along with a comment why;

Existing issues can be ignored (already shipped).


## Rules

### Existing

Those should be _good enough_ to be execute on the bots on each build. They must have zero (unreviewed) defect so the bots can detect any new breakage.

### Work In Progress

E.g. rules might be too noisy and require refinement, either in code or in external files. Until they have zero defect they must be commented;

### Ideas

Anything we do not check but for which data is available, e.g.

* NullAllowed;
* Enum member values;
* Generic updates to existing API (need to find a way to avoid braking changes first)


## Notes

* To develop you need to install ObjectiveSharpie. You can install the required version of ObjectiveSharpie by executing `./system-dependencies.sh --provision-sharpie` in this repository's root directory.

* You can use the `gen-[platform]` or `gen-all` target of the `Makefile` to generate C# code for all the API from the headers. You can then copy/paste from the (large) files to create the missing bindings;
