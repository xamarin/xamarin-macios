$modulePath = "$PSScriptRoot\\APIDiff.psm1"  # windows path separators work on unix and windows
$scriptBody = "using module $modulePath"
$script = [ScriptBlock]::Create($scriptBody)
. $script

Describe "Platform diff tests" {

    BeforeAll {
        $platform = "ios"
        $html = "http://test.com"
        $gist = "httP://gist.github.com"
    }

    It "creates with no links" {
        $diff = [PlatformDiff]::new($platform)
        $diff.Platform | Should -Be $platform
        $diff.Html | Should -Be $null
        $diff.Gist | Should -Be $null
    }

    It "creates with links" {
        $diff = [PlatformDiff]::new($platform, $html, $gist)
        $diff.Platform | Should -Be $platform
        $diff.Html | Should -Be $html
        $diff.Gist | Should -Be $gist
    }

}

Describe "API diff tests" {

    BeforeAll {
        $header = "API diff"
        $result = "Success"
        $message = "Review diff."

        $gists = @{
          "iOS" = "https://gist.github.com/ios.md$";
          "macOS" = "https://gist.github.com/macos.md$";
          "tvOS" = "https://gist.github.com/tvos.md$";
          "watchOS" = "https://gist.github.com/watchos.md$";
          "dotnet-iOS" = "https://gist.github.com/dotnet-ios.md$";
          "dotnet-tvOS" = "https://gist.github.com/dotnet-tvos.md$";
          "dotnet-MacCatalyst" = "https://gist.github.com/dotnet-maccatalyst.md$";
          "dotnet-macOS" = "https://gist.github.com/dotnet-macos.md$";
          "dotnet-legacy-iOS" = "https://gist.github.com/dotnet-legacy-ios.md$";
          "dotnet-legacy-tvOS" = "https://gist.github.com/dotnet-legacy-tvos.md$";
          "dotnet-legacy-macOS" = "https://gist.github.com/dotnet-legacy-macos.md$";
          "dotnet-macCatiOS" = "https://gist.github.com/dotnet-maccatios.md$";
        }

        $html =  @{
          "iOS" = "http://example.com/ios-api-diff.html"; 
          "macOS" = "http://example.com/mac-api-diff.html";
          "tvOS" = "http://example.com/tvos-api-diff.html";
          "watchOS" = "http://example.com/watchos-api-diff.html";
          "index"= "http://example.com/api-diff.html";
          "dotnet-iOS" = "http://example.com/dotnet/Microsoft.iOS.Ref/ref/net6.0/Xamarin.iOS-api-diff.html";
          "dotnet-tvOS" = "http://example.com/dotnet/Microsoft.tvOS.Ref/ref/net6.0/Xamarin.TVOS-api-diff.html";
          "dotnet-MacCatalyst" = "http://example.com/dotnet/Microsoft.MacCatalyst.Ref/ref/net6.0/Xamarin.MacCatalyst-api-diff.html";
          "dotnet-macOS" = "http://example.com/dotnet/Microsoft.macOS.Ref/ref/net6.0/Xamarin.Mac-api-diff.html";
          "dotnet-legacy-iOS" = "http://example.com/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/net6.0/Xamarin.iOS-api-diff.html";
          "dotnet-legacy-tvOS" = "http://example.com/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/net6.0/Xamarin.TVOS-api-diff.html";
          "dotnet-legacy-macOS" = "http://example.com/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/net6.0/Xamarin.Mac-api-diff.html";
          "dotnet-macCatiOS" = "http://example.com/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/net6.0/Xamarin.iOS.MacCatalyst-api-diff.html";
        }

        $apiDiffData = @{
          "gist" = $gists;
          "html" = $html;
          "result" = $result;
          "message" = $message;
        }
    }

    It "creates with all data" {
        $diff = [APIDiff]::new($header, $apiDiffData)

        $diff.Header | Should -Be $header 

        # assert that the links are correct
        foreach($platform in $gists.Keys) {
            $diff.Platforms.ContainsKey($platform) | Should -Be $true
            $diff.Platforms[$platform].Gist | Should -Be $gists[$platform]
        }

        foreach($platform in $html.Keys) {
            $diff.Platforms.ContainsKey($platform) | Should -Be $true
            $diff.Platforms[$platform].Html | Should -Be $html[$platform]
        }
    }

    It "creates with only html links" {
        $apiDiffData = @{
          "gist" = @{};
          "html" = $html;
          "result" = $result;
          "message" = $message;
        }

        $diff = [APIDiff]::new($header, $apiDiffData)

        foreach($platform in $html.Keys) {
            $diff.Platforms.ContainsKey($platform) | Should -Be $true
            $diff.Platforms[$platform].Html | Should -Be $html[$platform]
            $diff.Platforms[$platform].Gist | Should -Be $null
        }

        $apiDiffData = @{
          "gist" = $null 
          "html" = $html;
          "result" = $result;
          "message" = $message;
        }

        $diff = [APIDiff]::new($header, $apiDiffData)

        foreach($platform in $html.Keys) {
            $diff.Platforms.ContainsKey($platform) | Should -Be $true
            $diff.Platforms[$platform].Html | Should -Be $html[$platform]
            $diff.Platforms[$platform].Gist | Should -Be $null
        }
    }
    
    It "creates with only gists links" {

        $apiDiffData = @{
          "gist" = $gists;
          "html" =  @{};
          "result" = $result;
          "message" = $message;
        }

        $diff = [APIDiff]::new($header, $apiDiffData)

        # assert that the links are correct
        foreach($platform in $gists.Keys) {
            $diff.Platforms.ContainsKey($platform) | Should -Be $true
            $diff.Platforms[$platform].Gist | Should -Be $gists[$platform]
            $diff.Platforms[$platform].Html | Should -Be $null
        }

        $apiDiffData = @{
          "gist" = $gists;
          "html" = $null 
          "result" = $result;
          "message" = $message;
        }

        $diff = [APIDiff]::new($header, $apiDiffData)

        # assert that the links are correct
        foreach($platform in $gists.Keys) {
            $diff.Platforms.ContainsKey($platform) | Should -Be $true
            $diff.Platforms[$platform].Gist | Should -Be $gists[$platform]
            $diff.Platforms[$platform].Html | Should -Be $null
        }
    }
}

Describe "API diff comment tests" {
    BeforeAll {
        # this headers help to check the content is there
        $prHeader = "API Current PR diff"
        $stableHeader = "API diff"
        $result = "Success"
        $message = "Review diff."

        $prGists = @{
          "iOS" = "https://gist.github.com/ios.md$";
          "macOS" = "https://gist.github.com/macos.md$";
          "tvOS" = "https://gist.github.com/tvos.md$";
          "watchOS" = "https://gist.github.com/watchos.md$";
          "dotnet-iOS" = "https://gist.github.com/dotnet-ios.md$";
          "dotnet-tvOS" = "https://gist.github.com/dotnet-tvos.md$";
          "dotnet-MacCatalyst" = "https://gist.github.com/dotnet-maccatalyst.md$";
          "dotnet-macOS" = "https://gist.github.com/dotnet-macos.md$";
          "dotnet-legacy-iOS" = "https://gist.github.com/dotnet-legacy-ios.md$";
          "dotnet-legacy-tvOS" = "https://gist.github.com/dotnet-legacy-tvos.md$";
          "dotnet-legacy-macOS" = "https://gist.github.com/dotnet-legacy-macos.md$";
          "dotnet-macCatiOS" = "https://gist.github.com/dotnet-maccatios.md$";
        }
        $stableGists = @{}

        foreach($p in $prGists.Keys) {
            $stableGists[$p] = $prGists[$p] + ".stable"
        }

        $prHtml =  @{
          "iOS" = "http://example.com/ios-api-diff.html"; 
          "macOS" = "http://example.com/mac-api-diff.html";
          "tvOS" = "http://example.com/tvos-api-diff.html";
          "watchOS" = "http://example.com/watchos-api-diff.html";
          "index"= "http://example.com/api-diff.html";
          "dotnet-iOS" = "http://example.com/dotnet/Microsoft.iOS.Ref/ref/net6.0/Xamarin.iOS-api-diff.html";
          "dotnet-tvOS" = "http://example.com/dotnet/Microsoft.tvOS.Ref/ref/net6.0/Xamarin.TVOS-api-diff.html";
          "dotnet-MacCatalyst" = "http://example.com/dotnet/Microsoft.MacCatalyst.Ref/ref/net6.0/Xamarin.MacCatalyst-api-diff.html";
          "dotnet-macOS" = "http://example.com/dotnet/Microsoft.macOS.Ref/ref/net6.0/Xamarin.Mac-api-diff.html";
          "dotnet-legacy-iOS" = "http://example.com/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/net6.0/Xamarin.iOS-api-diff.html";
          "dotnet-legacy-tvOS" = "http://example.com/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/net6.0/Xamarin.TVOS-api-diff.html";
          "dotnet-legacy-macOS" = "http://example.com/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/net6.0/Xamarin.Mac-api-diff.html";
          "dotnet-macCatiOS" = "http://example.com/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/net6.0/Xamarin.iOS.MacCatalyst-api-diff.html";
        }

        $stableHtml = @{}

        foreach($p in $prHtml.Keys) {
            $stableHtml[$p] = $prHtml[$p] + ".stable"
        }

        $prDiff = @{
          "gist" = $prGists;
          "html" = $prHtml;
          "result" = $result;
          "message" = $message;
        }

        $stableDiff = @{
          "gist" = $stableGists;
          "html" = $stableHtml;
          "result" = $result;
          "message" = $message;
        }

        # write a temp file that we will use to contain as text
        $generatorDiff = [System.IO.Path]::GetTempFileName()
        $generatorContent = "This is the generator data\nWe do not really care about it."
        Set-Content -Path $generatorDiff $generatorContent 
    }

    It "creates with null stable" {
        $comment = [APIDiffComment]::new($prDiff, $null, $generatorDiff)

        $comment.FromPR.Header | Should -Be $prHeader
        $comment.FromStable | Should -Be $null
        $comment.Generator | Should -Be $generatorDiff
    }

    It "creates with null pr" {
        $comment = [APIDiffComment]::new($null, $stableDiff, $generatorDiff)

        $comment.FromPR | Should -Be $null
        $comment.FromStable.Header | Should -Be $stableHeader
        $comment.Generator | Should -Be $generatorDiff
    }

    It "create with null generator" {
        $comment = [APIDiffComment]::new($prDiff, $stableDiff, $null)

        $comment.FromPR.Header | Should -Be $prHeader
        $comment.FromStable.Header | Should -Be $stableHeader
        $comment.Generator | Should -Be ""
    }

    Context "from paths" {

      BeforeAll {
        $prDiffPath = New-TemporaryFile
        $stableDiffPath = New-TemporaryFile

        $prDiff | ConvertTo-Json | Out-File $prDiffPath 
        $stableDiff | ConvertTo-Json | Out-File $stableDiffPath 
      }

      AfterAll {
        rm -Rf $prDiffPath 
        rm -Rf $stableDiffPath 
      }

      It "creates comment with all the paths" {
          $comment = [APIDiffComment]::FromJsonFiles($prDiffPath, $stableDiffPath, $generatorDiff)
          $comment.FromPR.Header | Should -Be $prHeader
          $comment.FromStable.Header | Should -Be $stableHeader
          $comment.Generator | Should -Be $generatorDiff
      }

      It "creates comment with missing paths" {
          $missinPath = New-TemporaryFile 
          rm -Rf $missinPath 
          $comment = [APIDiffComment]::FromJsonFiles($prDiffPath, $missinPath, $generatorDiff)
          $comment.FromPR.Header | Should -Be $prHeader
          $comment.FromStable.Header | Should -Be $null
          $comment.Generator | Should -Be $generatorDiff
      }

      It "creates comment with null paths" {
          $comment = [APIDiffComment]::FromJsonFiles($prDiffPath, $null, $generatorDiff)
          $comment.FromPR.Header | Should -Be $prHeader
          $comment.FromStable.Header | Should -Be $null
          $comment.Generator | Should -Be $generatorDiff
      }

    }
}
