dotnet publish OpenFlier.Desktop -p:PublishProfile=FolderProfile;
dotnet publish OpenFlier.Utils -p:PublishProfile=FolderProfile;
dotnet publish OpenFlier.Updater -p:PublishProfile=FolderProfile;

7z a OpenFlier.7z dist/* -r && Copy-Item "OpenFlier.7z" -Destination ".\BuildArchive\OpenFlier.7z";

$cst = [System.TimeZoneInfo]::FindSystemTimeZoneById('China Standard Time')
$dateTime = [System.DateTime]::UtcNow
$cstCurrentTime = [System.TimeZoneInfo]::ConvertTimeFromUtc($dateTime, $cst)
$cstCurrentTime.ToString('yyyy-MM-dd HH:mm:ss') > .\BuildArchive\build-time