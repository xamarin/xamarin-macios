using module ".\\StaticPages.psm1"  # windows path separators work on unix and windows

Describe "Static pages tests" {

    BeforeAll {
        $urlPrefix = "http://test.com"
        $prId = "PRID"
        $buildId = "BUILDID"
    }

    It "creates a valid static page" {
        $staticPagesComment = [StaticPages]::new($urlPrefix, $prId, $buildId)
        $staticPagesComment.UrlPrefix | Should -Be $urlPrefix
        $staticPagesComment.PrID | Should -Be $prId
        $staticPagesComment.BuildId  | Should -Be $buildId

    }

    It "writes the correct lines" {
        $sb = [System.Text.StringBuilder]::new()
        $staticPagesComment = [StaticPages]::new($urlPrefix, $prId, $buildId)
        $staticPagesComment.WriteComment($sb)
        $content = $sb.ToString()
        $expectedUrl = "$urlPrefix/pr/PR$prId/$buildId"

        # validate it contains all the links
        $content.Contains("$expectedUrl/HtmlReport-sim/tests/vsdrops_index.html") | Should -Be $true
        $content.Contains("$expectedUrl/HtmlReport-sim/api-diff/api-diff.html)") | Should -Be $true
        $content.Contains("$expectedUrl/apicomparison/api-diff.html)") | Should -Be $true
    }
}
