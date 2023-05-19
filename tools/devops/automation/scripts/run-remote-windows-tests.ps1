$Env:DOTNET = "$Env:BUILD_SOURCESDIRECTORY\xamarin-macios\tests\dotnet\Windows\bin\dotnet\dotnet.exe"
$Env:ServerAddress = $Env:MAC_AGENT_IP
$Env:ServerUser = $Env:MAC_AGENT_USER
$Env:ServerPassword = $Env:XMA_PASSWORD
$Env:_DotNetRootRemoteDirectory = "/Users/$Env:MAC_AGENT_USER/Library/Caches/Xamarin/XMA/SDKs/dotnet/"

& $Env:DOTNET `
    test `
    "$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/tests/dotnet/UnitTests/DotNetUnitTests.csproj" `
    --filter Category=RemoteWindows `
    --verbosity quiet `
    --settings $Env:BUILD_SOURCESDIRECTORY/xamarin-macios/tests/dotnet/Windows/config.runsettings `
    "--results-directory:$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows-remote-tests/" `
    "--logger:console;verbosity=detailed" `
    "--logger:trx;LogFileName=$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows-remote-dotnet-tests.trx" `
    "--logger:html;LogFileName=$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows-remote-dotnet-tests.html" `
    "-bl:$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/tests/dotnet/Windows/windows-remote-dotnet-tests.binlog"
