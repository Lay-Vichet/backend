param(
    [int]$Bytes = 32,
    [ValidateSet("base64","hex")][string]$Format = "base64",
    [int]$Count = 1
)

$project = Join-Path $PSScriptRoot 'JwtKeyGen.csproj'
if (-not (Test-Path $project)) { $project = Join-Path $PSScriptRoot '..\JwtKeyGen.csproj' }

dotnet run --project $project -- --bytes $Bytes --format $Format --count $Count
