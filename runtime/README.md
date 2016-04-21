Native runtime
==============

This directory contains the native runtime for Xamarin.iOS and Xamarin.Mac.

Generated code
==============

A few files are generated automatically:

delegates.h|inc, Delegates.cs
-----------------------------

These files contain the code required to glue the native
runtime and the managed runtime.

mono-runtime.m, xamarin/mono-runtime.h
--------------------------------------

These files contain code that makes other code, consumers of
the mono runtime, oblivious to whether the mono runtime is linked
statically, dynamically, or using dlopen at runtime.

If you need to use a new function from the Mono headers, add it
to exports.t4.

If you need a new enum / constant / typedef / etc, add it to
mono-runtime.h.t4.

Shipped headers
===============

These are the headers shipped with XI/XM (they're not for public consumption,
but we need them to build generated code on customers machine, in particular
main.m and registrar.m)

xamarin/runtime.h
xamarin/trampolines.h
xamarin/main.h
xamarin/xamarin.h