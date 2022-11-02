$summaryPath = "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY/xamarin-macios/tests/TestSummary.md"
try {
    # Notarize
    & "$Env:BUILD_SOURCESDIRECTORY/release-scripts/notarize.ps1" -FolderForApps $Env:BUILD_SOURCESDIRECTORY/package/notarized

    # Verify that the notarized output is valid
    $notarizedRoot = Join-Path $Env:BUILD_SOURCESDIRECTORY package notarized
    Get-ChildItem -Path $notarizedRoot -Filter *.pkg -Recurse -File | ForEach-Object {
        Write-Debug "pkgutil --check-signature $($_.FullName)"
        pkgutil --check-signature "$($_.FullName)"
        if ($LastExitCode -eq 0) {
            Write-Debug "Signature check of $($_.FullName) succeeded."
        } else {
            Write-Debug "Signature check of $($_.FullName) failed (exit code: $LastExitCode)."
            throw "Signature check of $($_.FullName) failed (exit code: $LastExitCode)."
        }
    }
} catch {
    Add-Content -Path $summaryPath -Value "# :x: Notarization (ESRP) failed :x:`n`n```````n$PSItem`n```````n"
    throw
}
