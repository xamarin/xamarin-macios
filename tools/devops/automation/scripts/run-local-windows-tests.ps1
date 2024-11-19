if ("$Env:DOTNET" -eq "") {
    $Env:DOTNET = "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet\dotnet.exe"
    $Env:PATH = "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet;$Env:PATH"
}

$Env:MAC_AGENT_IP = ""
$Env:MAC_AGENT_USER = ""
$Env:XMA_PASSWORD = ""

& $Env:DOTNET `
    test `
    "$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/tests/dotnet/UnitTests/DotNetUnitTests.csproj" `
    --filter Category=Windows `
    --verbosity quiet `
    --settings $Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/tests/dotnet/Windows/config.runsettings `
    "--results-directory:$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/jenkins-results/windows-remote-tests/" `
    "--logger:console;verbosity=detailed" `
    "--logger:trx;LogFileName=$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/jenkins-results/windows-local-dotnet-tests.trx" `
    "--logger:html;LogFileName=$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/jenkins-results/windows-local-dotnet-tests.html" `
    "-bl:$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/tests/dotnet/Windows/windows-local-dotnet-tests.binlog"
