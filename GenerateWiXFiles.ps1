$files = Get-ChildItem -Path '../OpenFlierRelease'
# $files = [System.IO.Directory]::GetFiles("../OpenFlierRelease")
$originalContent = Get-Content -Path '.\Installer.MSI\Files.wxs.Template'
$builder = [System.Text.StringBuilder]::new()
foreach($file in $files)
{
    if(!$file.PSIsContainer)
    {
        [void]$builder.Append("<Component><File Source=""$($file.FullName)"" /></Component>`r`n")
    }
}
$toReplace = $builder.ToString();
$newContent = $originalContent -replace '%FILES_REPLACE_SECTION%', $toReplace

$newContent | Set-Content -Path ".\Installer.MSI\Files.wxs"