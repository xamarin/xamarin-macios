<#
Example of a pester test.
#>
Import-Module ./Example

Describe TestExample {
    Context 'The weather outside is frightful' {
        $testdata = 'But the fire is so delightful'

        It 'Should be delightful' {
            LetItSnow -Delightful | Should -Be $testdata
        }
    }

    Context "Seems there's no place to go" {
        $testdata = 'Let it snow, let it snow, let it snow'

        It 'Should snow' {
            LetItSnow | Should -Be $testdata
        }
    }
}