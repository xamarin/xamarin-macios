param ($SourcesDirectory, $LocalizeDirectory, $LocProjectPath)

$jsonFiles = @()
$jsonTemplateFiles = Get-ChildItem -Recurse -Path "$SourcesDirectory" | Where-Object { $_.FullName -Match "\.template\.config\\localize\\.+\.en\.json" } # .NET templating pattern
$jsonTemplateFiles | ForEach-Object {
    $null = $_.Name -Match "(.+)\.[\w-]+\.json" # matches '[filename].[langcode].json
    
    $destinationFile = "$($_.Directory.FullName)\$($Matches.1).json"
    $jsonFiles += Copy-Item "$($_.FullName)" -Destination $destinationFile -PassThru
    Write-Host "Template loc file generated: $destinationFile"
}

Push-Location "$SourcesDirectory"
$projectObject = Get-Content $LocProjectPath | ConvertFrom-Json
$jsonFiles | ForEach-Object {
    $sourceFile = $_.FullName
    $outputPath = "$($_.DirectoryName + "\")"
    $fullNameString = Convert-Path -Path $_.FullName
    $maciosPathArray = $fullNameString -split "xamarin-macios", 2
    $maciosPath = $maciosPathArray[1]
    if ($null -eq $maciosPath) {
        Write-Host "'fullNameString' could not be split at 'xamarin-macios'!"
        exit 1
    }
    $lclFile = Join-Path -Path $LocalizeDirectory -ChildPath "$($maciosPath).lcl"
    $projectObject.Projects[0].LocItems += (@{
        SourceFile = $sourceFile
        CopyOption = "LangIDOnName"
        OutputPath = $outputPath
        LclFile = $lclFile
    })
}
Pop-Location

$locProjectJson = ConvertTo-Json $projectObject -Depth 5
Set-Content $LocProjectPath $locProjectJson
Write-Host "LocProject.json was updated to contain template localizations:`n`n$locProjectJson`n`n"
