$vsInstallPath = "/Applications/Visual Studio.app"
$version = /usr/libexec/PlistBuddy -c 'Print :CFBundleShortVersionString' "${vsInstallPath}/Contents/Info.plist" 2> $null
Write-Host $version
