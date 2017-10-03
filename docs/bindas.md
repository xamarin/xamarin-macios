# How to add BindAs support for new types

Currently the BindAs code only supports converting between a certain set of
types and NSValue, NSNumber and smart enums.

If your BindAs support does not involve NSValue or NSNumber, then the required
changes are a bit more extensive than I explain here (but this is still a good
starting point).

[Sample code][8]

The sample code is to support a new type for NSValue, the exact code locations will differ slightly for NSNumber (different switches, etc).

1. Add a test (or three)

    * Add an entry to [tests/test-libraries/testgenerator.cs][1] for the new type.
      testgenerator.cs will generate the code required to test your new BindAs
      support for all known scenarios.

    * Any other manual tests should go in monotouch-test.

2. Add native conversions functions to runtime/trampolines.m|h. In the sample
   code this is the two functions to convert between NSValue and
   NSDirectionalEdgeInsets:

   `xamarin_nsdirectionaledgeinsets_to_nsvalue`: [trampolines.h#151][2], [trampolines.m#889][3]
   `xamarin_nsvalue_to_nsdirectionaledgeinsets`: [trampolines.h#116][4], [trampolines.m#799][5]

3. Add a switch entry to [trampolines.m#1007][6] to use the two new conversion functions.

4. The registrar also needs to know ([Registrar.cs#687][7]).

5. And the static registrar needs to know too, so that it can call the right native conversion function ([StaticRegistrar.cs#3796][9], [StaticRegistrar.cs#3830][10]).

6. Now there's just the generator support left ([generator.cs#1223][11], [generator.cs#1369][12]).

7. Finally run the following tests (at least)

* All variations of monotouch-test (iOS/watchOS/tvOS) on both simulator and device.
* link all on both simulator and device.


[1]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/tests/test-libraries/testgenerator.cs#L100
[2]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/runtime/xamarin/trampolines.h#L151
[3]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/runtime/trampolines.m#L889
[4]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/runtime/xamarin/trampolines.h#L116
[5]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/runtime/trampolines.m#L799
[6]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/runtime/trampolines.m#L1007-L1008
[7]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/src/ObjCRuntime/Registrar.cs#L687
[8]: https://github.com/xamarin/xamarin-macios/pull/2288/commits/b38c114fbe8c9d229ec41a312dc36802cb4f027e
[9]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/tools/common/StaticRegistrar.cs#L3796
[10]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/tools/common/StaticRegistrar.cs#L3830
[11]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/src/generator.cs#L1223
[12]: https://github.com/rolfbjarne/xamarin-macios/blob/b38c114fbe8c9d229ec41a312dc36802cb4f027e/src/generator.cs#L1369
