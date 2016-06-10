History
====================

Historically we've attempted to convert Objective-C exceptions to managed exceptions
by adding a handler to be notified of uncaught Objective-C exceptions [1], but that one
major problem: it did not handle *caught* Objective-C exceptions, where the exception
handler would just abort.

This is common in iOS apps, where the main runloop adds an Objective-C @try/@catch
handler that just aborts, preventing us from being notified of the Objectice-C exception.

This approach also has another problem: the Objective-C exception handling mechanism
could unwind through managed code, which would result in all kinds of inexplicable
behavior (if there was an Objective-C exception handler that _didn't_ abort the app):

    try {
        doSomethingThatCausesAnObjectiveCException ();
    } finally {
        Console.WriteLine ("Done!");
    }

If there happened to be an Objective-C exception handler in a stack frame further
up, the Console.WriteLine would never be executed.

Another frequent problem would occur in an IDE debugger if it tried to evaluate
every property for an object. If there were properties not supported on the
executing platform, it would throw an Objective-C exception (selector not found)
that would terminate the app.

The reverse problem also existed: throwing a managed exception that the native-to-managed
boundary didn't catch, would make the mono runtime unwind through native frames,
causing a similar set of inexplicable behavior:

    [obj selectorThatManagedCodeHasOverriddenAndThrowAManagedException];
    NSLog (@"done!");

The NSLog statement would never be executed.

[1] By using [NSSetUncaughtExceptionHandler](https://developer.apple.com/library/mac/documentation/Cocoa/Reference/Foundation/Miscellaneous/Foundation_Functions/#//apple_ref/c/func/NSSetUncaughtExceptionHandler).

Exception marshaling
====================

There are two parts to exception marshaling: Objective-C exceptions
and managed exceptions.

In both cases we offer a managed event the app can subscribe to to
be notified, and then the app can choose what to do:

1. Convert the managed/Objective-C exception to its counterpart
   (i.e. managed exceptions are converted to an Objective-C exception,
   and Objective-C exceptions are converted to a managed exception).

2. Abort.

3. Rethrow the original exception (this is not available when
   using the Coop GC, which is the only option on watchOS).

Managed exception marshaling
----------------------------

This is the simplest case, we just make sure to catch any managed
exceptions that reaches native code. There are a few sources:

1. When we call mono_runtime_invoke in our trampolines.

   In this case mono_runtime_invoke will catch any exceptions
   and return them to us (without unwinding anything else).

2. When managed code gives a delegate to native code, and 
   native code calls that delegate.

   This is still a TODO, we need to use this new API to be
   notified when the mono runtime detects this case:
   https://github.com/mono/mono/pull/2948

3. When we call any managed code from our runtime. This occurs
   quite frequently when Objective-C calls a selector with a
   managed information.

   In all cases we must handle exceptions properly - this is
   done by having the managed delegates in our runtime catch
   all exceptions, and returning a GCHandle with the caught
   exception. Then the caller must check if any exceptions
   were thrown, and decide how to handle it appropriately.

Objective-C exception marshaling
--------------------------------

This is more complicated, because there must be an Objective-C
exception handler on the managed-to-native boundary frame, 
catching the Objective-C exception.

We have two approaches for this:

1. Custom wrappers of the objc_msgSend functions with an
   Objective-C exception handler. These have to be written
   in assembly code, since it's not possible to do it in C.
   At runtime we inject a dllmap into the process, which
   redirects any P/invoke to the objc_msgSend functions to
   these custom wrappers.

   Since we can't write assembly code for bitcode targets
   (watchOS), another approach is still required though.

   This is the used for iOS/tvOS/watchOS simulator builds
   (both 32-bit and 64-bit) and Mac/64 bits. It does not
   work on Mac/32 bits because that platform does not use
   0-cost exceptions like the other platforms (it uses
   setjmp/longjmp instead, and would require a very
   different implementation in assembly code).

   The assembly code for these wrappers is in the 
   trampolines-[arch]-objc_msgSend* files.

   The documentation about 0-cost exceptions is scarce,
   but here are a few documents which were helpful:

	https://developer.apple.com/library/mac/documentation/Cocoa/Conceptual/Exceptions/Articles/Exceptions64Bit.html  
	https://sourceware.org/binutils/docs/as/CFI-directives.html  
	http://mentorembedded.github.io/cxx-abi/abi-eh.html  
	http://llvm.org/docs/ExceptionHandling.html  
	http://www.opensource.apple.com/source/llvmCore/llvmCore-3425.0.33/docs/ExceptionHandling.rst  
	http://stackoverflow.com/a/7535848  
	http://www.darlinghq.org/for-developers/exception-handling-on-os-x  

    The wrappers themselves were first created by writing
    something similar in C, and then telling clang to
    dump the corresponding assembly code (by passing
    `--save-temps -fverbose-asm` to clang).

2. At build time generate custom wrappers of every P/Invoke
   to the objc_msgSend functions, and modify the P/Invoke to
   call these generated wrappers.

   This is used for iOS/tvOS/watchOS device builds, and
   Mac/32 bits.
