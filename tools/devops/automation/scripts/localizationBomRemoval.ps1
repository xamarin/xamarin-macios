function RemoveUtf8Bom
{
    param(
        [string]$FilePath
    )

    $encoding = New-Object -TypeName System.Text.UTF8Encoding -ArgumentList $false
    $content = Get-Content -Path $FilePath -Encoding UTF8
    [System.IO.File]::WriteAllText($FilePath, $content -join "`n", $encoding)
}

foreach ($locFile in (Get-Content -Path $env:XLocFileList))
{
    RemoveUtf8Bom -FilePath $locFile
}
