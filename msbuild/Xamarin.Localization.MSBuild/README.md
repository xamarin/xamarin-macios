# MSBuild Localization

Messages for new MSBuild error codes live in `MSBStrings.resx`.

* You can now make changes to `MSBStrings.resx` in the Visual Studio for Mac IDE or from any text editor.

* If you make changes in the IDE, you should see changes automatically copy into MSBStrings.Designer.cs. Be sure to rebuild the project after making your changes.

* If you make changes from a text editor, be sure to run `make` inside the xamarin-macios/msbuild/Xamarin.Localization.MSBuild directory.

See [Localization Wiki][Localization-wiki] for more details on our localization process

or the [OneLocBuild Wiki][OneLocBuild-wiki] for information on OneLocBuild.

[Localization-wiki]: https://github.com/xamarin/maccore/wiki/Localization
[OneLocBuild-wiki]: https://ceapex.visualstudio.com/CEINTL/_wiki/wikis/CEINTL.wiki/107/Localization-with-OneLocBuild-Task
