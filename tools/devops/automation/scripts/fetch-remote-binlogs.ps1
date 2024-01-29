Import-Module "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY/xamarin-macios/tools/devops/automation/scripts/MaciosCI.psd1"

# Zip up all the binlogs into one file
Invoke-SshCommand `
  -RemoteHost "$Env:MAC_AGENT_IP" `
  -RemoteUserName "$Env:MAC_AGENT_USER" `
  -- `
  "$Env:MAC_AGENT_BUILD_SOURCESDIRECTORY/xamarin-macios/tests/dotnet/Windows/collect-binlogs.sh"

New-Item -Path "$Env:BUILD_ARTIFACTSTAGINGDIRECTORY" -Name "windows-binlogs" -ItemType "directory"
New-Item -Path "$Env:BUILD_ARTIFACTSTAGINGDIRECTORY\windows-binlogs" -Name "windows-remote-logs.zip" -ItemType "file" -Value "zip"

# Check file existence
Invoke-SshCommand `
  -RemoteHost "$Env:MAC_AGENT_IP" `
  -RemoteUserName "$Env:MAC_AGENT_USER" `
  -- `
  ls -la "/Users/$Env:MAC_AGENT_USER/remote_build_testing/windows-remote-logs.zip"

# Copy the zip from the remote Mac to this machine
Invoke-SshDownload `
  -RemoteHost "$Env:MAC_AGENT_IP" `
  -RemoteUserName "$Env:MAC_AGENT_USER" `
  -Source "/Users/$Env:MAC_AGENT_USER/remote_build_testing/windows-remote-logs.zip" `
  -Target "$Env:BUILD_ARTIFACTSTAGINGDIRECTORY\windows-binlogs\windows-remote-logs.zip"
