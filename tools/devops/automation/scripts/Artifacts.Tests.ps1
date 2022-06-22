using module ".\\Artifacts.psm1"

Describe "Artifacts tests" {

    BeforeAll {
        $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
        $dataPath = Join-Path -Path $dataPath -ChildPath "artifacts.json"
        $data = Get-Content -Path $dataPath | ConvertFrom-Json
    }

    It "is correctly create" {
        $artifacts = [Artifacts]::new($data)
        $artifacts.Content | Should -Not -Be $null
    }

    It "is correctly create via a function" {
        $artifacts = New-Artifacts -Content $data
        $artifacts.Content | Should -Not -Be $null
    }

    It "writes comments with null artifacts" {
        $artifacts = [Artifacts]::new($null)
        $sb = [System.Text.StringBuilder]::new()
        $artifacts.WriteComment($sb)

        $content = $sb.ToString()
        $content.Contains("Artifacts were not provided.") | Should -Be $true
    }

    It "writes comments without content" {
        $artifacts = [Artifacts]::new(@{})
        $sb = [System.Text.StringBuilder]::new()
        $artifacts.WriteComment($sb)

        $content = $sb.ToString()
        $content.Contains("No packages found.") | Should -Be $true
    }

    It "writes comments with content" {
        $artifacts = [Artifacts]::new($data)
        $expectedContent = @(
         "delete-xma-build-host.pkg",
         "Microsoft.iOS.Bundle.15.2.303-ci.casa-blanca.58.pkg",
         "xamarin.ios-15.7.0.589.pkg",
         "xamarin.mac-8.7.0.589.pkg",
         "Microsoft.iOS.Bundle.15.2.303-ci.casa-blanca.58.pkg"
         )

        $sb = [System.Text.StringBuilder]::new()
        $artifacts.WriteComment($sb)

        $content = $sb.ToString()

        # assert title
        $content.Contains("# Packages generated") | Should -Be $true
        
        # assert the detail tags
        $content.Contains("<details>") | Should -Be $true
        $content.Contains("</details>") | Should -Be $true
        # assert that all the expected artifacts are there
        foreach($a in $expectedContent) {
            $content.Contains("$a") | Should -Be $true
        }

        # assert we do not show .zip files
        $content.Contains("bundle.zip") | Should -Be $false
    }
}

