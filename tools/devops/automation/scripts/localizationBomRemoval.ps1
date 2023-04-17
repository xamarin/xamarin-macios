# This function is used to remove the BOM from the checked-in Localization files
function RemoveUtf8Bom
{
    param(
        [string]$FilePath
    )

    $encoding = New-Object -TypeName System.Text.UTF8Encoding
    $content = Get-Content -Path $FilePath -Raw
    [System.IO.File]::WriteAllLines($FilePath, $content, $encoding)
}