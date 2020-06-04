# MSBuild Localization

Messages for new MSBuild error codes live in `MSBStrings.resx`.

If changes are made to `MBStrings.resx`, you will hit:

    XliffTasks.targets(91,5): error : 'xlf\MSBStrings.cs.xlf' is out-of-date with 'MSBStrings.resx'.
    Run `msbuild /t:UpdateXlf` to update .xlf files or set UpdateXlfOnBuild=true to update them on every build,
    but note that it is strongly discouraged to set UpdateXlfOnBuild=true in official/CI build environments
    as they should not modify source code during the build.

To regenerate the `.xlf` files run:

    $ msbuild msbuild/Xamarin.Localization.MSBuild/Xamarin.Localization.MSBuild.csproj -restore -t:UpdateXlf

For `mtouch`, `Errors.resx` contains the localizable strings. Use
these commands instead to update `.xlf` files:

    $ nuget restore tools
    $ msbuild tools/mtouch/mtouch.csproj -t:UpdateXlf

For `generator`, `src/Resources.resx`, contains the localizable
strings. To update the `.xlf` files:

    $ nuget restore src
    $ msbuild src/generator.csproj -t:UpdateXlf

*NOTE: `nuget restore` can be used instead of the MSBuild `-restore`
switch for projects using `packages.config`*

See [dotnet/xliff-tasks][xliff-tasks] or [Xamarin.Android's
documentation][xamarin-android] for details.

[xliff-tasks]: https://github.com/dotnet/xliff-tasks
[xamarin-android]: https://github.com/xamarin/xamarin-android/blob/master/Documentation/workflow/Localization.md
