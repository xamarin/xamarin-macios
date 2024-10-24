if ("$Env:DOTNET" -eq "") {
    $Env:DOTNET = "$Env:BUILD_SOURCESDIRECTORY\xamarin-macios\tests\dotnet\Windows\bin\dotnet\dotnet.exe"
    $Env:PATH = "$Env:BUILD_SOURCESDIRECTORY\xamarin-macios\tests\dotnet\Windows\bin\dotnet;$Env:PATH"
}

$Env:ServerAddress = ""
$Env:ServerUser = ""
$Env:ServerPassword = ""

& $Env:DOTNET `
    test `
    "$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/tests/dotnet/UnitTests/DotNetUnitTests.csproj" `
    --filter Category=Windows `
    --verbosity quiet `
    --settings $Env:BUILD_SOURCESDIRECTORY/xamarin-macios/tests/dotnet/Windows/config.runsettings `
    "--results-directory:$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows-remote-tests/" `
    "--logger:console;verbosity=detailed" `
    "--logger:trx;LogFileName=$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows-dotnet-tests.trx" `
    "--logger:html;LogFileName=$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows-dotnet-tests.html" `
    "-bl:$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/tests/dotnet/Windows/run-dotnet-tests.binlog"
