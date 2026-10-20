# MSBuild Localization

Messages for new MSBuild error codes live in `MSBStrings.resx`.

* You can now make changes to `MSBStrings.resx` in the Visual Studio for Mac IDE or from any text editor.

* If you make changes in the IDE, rebuild `Xamarin.MacDev.Tasks.csproj` to regenerate `MSBStrings.Designer.cs`.

* If you make changes from a text editor, build `msbuild/Xamarin.MacDev.Tasks/Xamarin.MacDev.Tasks.csproj`. Its satellite assemblies contain both the `MSBStrings` and `Errors` translations.

See [Localization Wiki][Localization-wiki] for more details on our localization process

or the [OneLocBuild Wiki][OneLocBuild-wiki] for information on OneLocBuild.

[Localization-wiki]: https://github.com/xamarin/maccore/wiki/Localization
[OneLocBuild-wiki]: https://ceapex.visualstudio.com/CEINTL/_wiki/wikis/CEINTL.wiki/107/Localization-with-OneLocBuild-Task
