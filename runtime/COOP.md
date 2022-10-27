Coop GC
=======

WatchOS requires a cooperative GC, which means our runtime
needs to use the proper API for the cooperative GC to work
properly.

http://www.mono-project.com/docs/advanced/runtime/docs/coop-suspend/#gc-unsafe-mode
https://docs.google.com/presentation/d/1Sa361Ru8ccRRZCm4BN2NUXPf2k5VYYrjqpGKsV1cPnI/edit#slide=id.p

Rules
=====

1. When touching (reading or writing) managed memory, we must
   be in GC UNSAFE mode.

2. When calling code we don't have control over (that can take
   an unbound amount of time), we must switch to GC SAFE mode.
   This includes calling any ObjC selector on any object.

Advice from the runtime people (TODO: need to re-review with this in mind):

> Ludovic Henry [22:10] 
> also if I can give you some advices for the state transition:
> - do the GC safe transition only right around where it's actually blocking: for example for a pthread_mutex_lock, don't have the whole function as GC safe, as you don't really control in this function which function calls it, or even any other function that it could call, and for selectors, you only should have the selector witch to GC safe, and if in the calle you need to actually be in GC unsafe, then switch in the callee at the beginning of the function.
> - do the GC unsafe transition at the beginning of the trampoline/pinvoke/... : by having 99% of the code as unsafe, it's way easier to reason about it: "everything is in unsafe mode, except these tiny bits that are limited to very few things and that we really know what they are doing
>
> [22:11] 
> initially, we were going to have most of the runtime in GC safe mode, but it lead to so many "oh here we can be called in this or that mode", that it was just easier to have the hypothesis "everything in the runtime is run in GC unsafe mode, except these very delimited/predetermined parts" (edited)
> 
> [22:12] 
> and out of a sudden, the only problematic parts are: "where can we enter the runtime, so we can switch to GC unsafe", and that's a much smaller problem than knowing "where in the runtime we should be in which mode at any moment"

Review
======

* bindings-generated.m [DONE]
* bindings.m [DONE]
* extension-main.m [DONE]
* launcher.m [MAC ONLY; NOT DONE]
* mono-runtime.m [PASS-THROUGH ONLY]
* monotouch-debug.m [DONE]
* monotouch-main.m [COMPLETE-ISH]
* nsstring-localization.m [DONE]
* runtime.m [FIRST PASS]
* shared.m [DONE]
* trampolines-i386.m [DONE]
* trampolines-invoke.m [FIRST PASS DONE; SEE PENDING NOTE BELOW]
* trampolines-x86_64.m [DONE]
* trampolines.m [FIRST PASS DONE]
* xamarin-support.m [DONE]

StaticRegistrar.cs [FIRST PASS DONE; SEE PENDING NOTE BELOW]

Pending
=======

Exceptions
----------

Exceptions must be caught when returning from managed code,
we can't allow mono unwinding native frames.

We do this by requiring exception marshaling to be enabled,
and disallowing "ThrowManaged" as an option.

Also we can't allow Objective-C exceptions to traverse
managed code

This is done by disallowing "ThrowObjectiveC" as an
option when marshalling Objectivec-C exceptions.

TODO:

* Somehow handle exceptions when calling into our delegates
  (look at mono_method_get_unmanaged_thunk)

* See also mono_method_get_unmanaged_thunk for when invoking delegates.

xamarin_invoke_trampoline
-------------------------

Looks like we need a handle API to properly construct the
parameters to the managed function while at the same time
being able to switch to safe mode when we call code we
don't control (we have an array of data that can be pointers
to managed objects, and need a way to register those pointers
with managed code).

Static registrar
----------------

This has the same problem as xamarin_invoke_trampoline,
having to construct parameters to a managed function
while at the same time being able to switch to safe mode.

Debugging tips
==============

* Tell lldb to attach when the watch extension launches:

    process attach --waitfor --name "com.xamarin.monotouch-test.watchkitapp.watchkitextension"

* Something went wrong in a thread, and now that thread is doing something very different.

    1. Put a breakpoint on pthread_setname_np, and check the assigned name: `p (char *) *(void **) ($esp + 4)`
    2. Once you've located the thread you're debugging, set a thread-specific breakpoint:

        break set -n mono_threads_reset_blocking_start -t 0x42b005 
        break set -n mono_threads_reset_blocking_end   -t 0x42b005 

* Display the current thread state:

    display (void *) mono_thread_info_current ()->thread_state

Enable GC Assertions
====================

Apply this patch to the iOS SDK (here only done for the `arm64_32` runtime):
```patch
diff --git a/sdks/builds/ios.mk b/sdks/builds/ios.mk
index 571dbd797a8..5bbe30040d2 100644
--- a/sdks/builds/ios.mk
+++ b/sdks/builds/ios.mk
@@ -155,7 +155,7 @@ watchos64_32_sysroot = -isysroot $(watchos64_32_sysroot_path)
 # explicitly disable dtrace, since it requires inline assembly, which is disabled on AppleTV (and mono's configure.ac doesn't know that (yet at least))
 ios-targettv_CONFIGURE_FLAGS =         --enable-dtrace=no --enable-llvm-runtime --with-bitcode=yes
 ios-targetwatch_CONFIGURE_FLAGS = --enable-cooperative-suspend --enable-llvm-runtime --with-bitcode=yes
-ios-targetwatch64_32_CONFIGURE_FLAGS = --enable-cooperative-suspend --enable-llvm-runtime --with-bitcode=yes
+ios-targetwatch64_32_CONFIGURE_FLAGS = --enable-cooperative-suspend --enable-llvm-runtime --with-bitcode=yes --enable-checked-build=gc

 ios-target32_SYSROOT = $(ios_sysroot) -miphoneos-version-min=$(IOS_VERSION_MIN)
 ios-target32s_SYSROOT = $(ios_sysroot) -miphoneos-version-min=$(IOS_VERSION_MIN)
```

and make sure you build `xamarin-macios` with `MONO_BUILD_FROM_SOURCE=1`. Also modify `mtouch` such that it enables checked-build:

```patch
diff --git a/tools/mtouch/mtouch.cs b/tools/mtouch/mtouch.cs
index 1ef159b79..cb2caf426 100644
--- a/tools/mtouch/mtouch.cs
+++ b/tools/mtouch/mtouch.cs
@@ -649,6 +654,8 @@ namespace Xamarin.Bundler
                                                sw.WriteLine ("\tsetenv (\"MONO_GC_PARAMS\", \"{0}\", 1);", app.MonoGCParams);
                                        foreach (var kvp in app.EnvironmentVariables)
                                                sw.WriteLine ("\tsetenv (\"{0}\", \"{1}\", 1);", kvp.Key.Replace ("\"", "\\\""), kvp.Value.Replace ("\"", "\\\""));
+                                       sw.WriteLine ("\tsetenv (\"MONO_CHECK_MODE\", \"gc\", 1);");
+
                                        sw.WriteLine ("\txamarin_supports_dynamic_registration = {0};", app.DynamicRegistrationSupported ? "TRUE" : "FALSE");
                                        sw.WriteLine ("}");
                                        sw.WriteLine ();
```
