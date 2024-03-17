dotnet publish OpenFlier.Desktop -p:PublishProfile=FolderProfile;
dotnet publish OpenFlier.Utils -p:PublishProfile=FolderProfile;
dotnet publish OpenFlier.Updater -p:PublishProfile=FolderProfile;
dotnet publish OpenFlier.DevUtils -p:PublishProfile=FolderProfile;

iscc.exe .\setup-script.iss 
$Exist = Test-Path ".\BuildArchive\Setup.exe";
if ($Exist -eq "True") {
    Remove-Item ".\BuildArchive\Setup.exe";
}
Move-Item -Path ".\Output\mysetup.exe" -Destination ".\BuildArchive\Setup.exe";

7z a DevUtils.7z devutils-dist/* -r -ssw
$Exist = Test-Path ".\BuildArchive\DevUtils.7z";
if ($Exist -eq "True") {
    Remove-Item ".\BuildArchive\DevUtils.7z";
}
Move-Item -Path "DevUtils.7z" -Destination ".\BuildArchive\DevUtils.7z";


7z a -tzip OpenFlier.zip dist/* -r -ssw
$Exist = Test-Path ".\BuildArchive\OpenFlier.zip";
if ($Exist -eq "True") {
    Remove-Item ".\BuildArchive\OpenFlier.zip";
}
Move-Item -Path "OpenFlier.zip" -Destination ".\BuildArchive\OpenFlier.zip";

$cst = [System.TimeZoneInfo]::FindSystemTimeZoneById('China Standard Time')
$dateTime = [System.DateTime]::UtcNow
$cstCurrentTime = [System.TimeZoneInfo]::ConvertTimeFromUtc($dateTime, $cst)
$cstCurrentTime.ToString('yyyy-MM-dd HH:mm:ss') > .\BuildArchive\build-time