$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\JustObjectsPrototype.Universal\bin\Release\JustObjectsPrototype.Universal.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\.nuget\JustObjectsPrototype.Universal.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\.nuget\JustObjectsPrototype.Universal.compiled.nuspec

& $root\.nuget\NuGet.exe pack $root\.nuget\JustObjectsPrototype.Universal.compiled.nuspec