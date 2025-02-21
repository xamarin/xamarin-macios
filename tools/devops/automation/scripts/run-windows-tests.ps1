$Env:PATH = "$(Build.SourcesDirectory)\xamarin-macios\tests\dotnet\Windows\bin\dotnet;$env:PATH"
$Env:DOTNET = "$(Build.SourcesDirectory)\xamarin-macios\tests\dotnet\Windows\bin\dotnet\dotnet.exe"
& $(Build.SourcesDirectory)\xamarin-macios\tests\dotnet\Windows\bin\dotnet\dotnet.exe `
    test `
    "$(Build.SourcesDirectory)/xamarin-macios/tests/dotnet/UnitTests/DotNetUnitTests.csproj" `
    --filter Category=Windows `
    --verbosity quiet `
    --settings $(Build.SourcesDirectory)/xamarin-macios/tests/dotnet/Windows/config.runsettings `
    "--results-directory:$(Build.SourcesDirectory)/xamarin-macios/jenkins-results/" `
    "--logger:console;verbosity=detailed" `
    "--logger:trx;LogFileName=$(Build.SourcesDirectory)/xamarin-macios/jenkins-results/windows-dotnet-tests.trx" `
    "--logger:html;LogFileName=$(Build.SourcesDirectory)/xamarin-macios/jenkins-results/windows-dotnet-tests.html" `
    "-bl:$(Build.SourcesDirectory)/xamarin-macios/tests/dotnet/Windows/run-dotnet-tests.binlog"