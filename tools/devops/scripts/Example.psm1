function LetItSnow {
    param ([switch]$Delightful)
    
    if ($Delightful) {
        'But the fire is so delightful'
    } else {
        'Let it snow, let it snow, let it snow'
    }
}
Export-ModuleMember -Function LetItSnow 