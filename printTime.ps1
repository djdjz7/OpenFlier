$cst = [System.TimeZoneInfo]::FindSystemTimeZoneById("China Standard Time")
$dateTime = [System.DateTime]::UtcNow
$cstCurrentTime = [System.TimeZoneInfo]::ConvertTimeFromUtc($dateTime, $cst)
$cstCurrentTime.ToString([System.Globalization.CultureInfo]::new('zh-CN')) > .\BuildArchive\build-time