$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$apiProject = Join-Path $repoRoot "src\Tijori.API"
$infraProject = Join-Path $repoRoot "src\Tijori.Infrastructure\Tijori.Infrastructure.csproj"

Set-Location $repoRoot
dotnet tool restore | Out-Null

Set-Location $apiProject

$globalDotnetEf = Join-Path $env:USERPROFILE ".dotnet\tools\dotnet-ef.exe"

if (Test-Path $globalDotnetEf) {
    & $globalDotnetEf database update --project $infraProject --startup-project .
    exit $LASTEXITCODE
}

dotnet dotnet-ef database update --project $infraProject --startup-project .
