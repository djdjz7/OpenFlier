$cst = [System.TimeZoneInfo]::FindSystemTimeZoneById("China Standard Time")
$dateTime = [System.DateTime]::UtcNow
$cstCurrentTime = [System.TimeZoneInfo]::ConvertTimeFromUtc($dateTime, $cst)
$cstCurrentTime.ToString() > .\BuildArchive\build-time