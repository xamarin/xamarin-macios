# MSBuild Localization

Messages for new MSBuild error codes live in `MSBStrings.resx`.

If changes are made to `MBStrings.resx`, you will hit:

    XliffTasks.targets(91,5): error : 'xlf\MSBStrings.cs.xlf' is out-of-date with 'MSBStrings.resx'.
    Run `msbuild /t:UpdateXlf` to update .xlf files or set UpdateXlfOnBuild=true to update them on every build,
    but note that it is strongly discouraged to set UpdateXlfOnBuild=true in official/CI build environments
    as they should not modify source code during the build.

To regenerate the `.xlf` files run:

    $ msbuild Xamarin.Localization.MSBuild.csproj -restore -t:UpdateXlf

See [dotnet/xliff-tasks][xliff-tasks] or [Xamarin.Android's
documentation][xamarin-android] for details.

[xliff-tasks]: https://github.com/dotnet/xliff-tasks
[xamarin-android]: https://github.com/xamarin/xamarin-android/blob/master/Documentation/workflow/Localization.md
