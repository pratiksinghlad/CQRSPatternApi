param(
    [string]$Configuration = "Release",
    [switch]$SkipFormat
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "../..")
Set-Location $repoRoot

$solution = "CQRSPattern.slnx"

function Invoke-HarnessCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Tool,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    & $Tool @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code $LASTEXITCODE`: $Tool $($Arguments -join ' ')"
    }
}

Write-Host "==> Restoring $solution"
Invoke-HarnessCommand "dotnet" @("restore", $solution)

Write-Host "==> Building $solution ($Configuration, warnings as errors)"
Invoke-HarnessCommand "dotnet" @("build", $solution, "--configuration", $Configuration, "--no-restore", "-warnaserror")

Write-Host "==> Testing $solution ($Configuration)"
Invoke-HarnessCommand "dotnet" @("test", $solution, "--configuration", $Configuration, "--no-build", "--verbosity", "normal")

if (-not $SkipFormat) {
    Write-Host "==> Verifying formatting"
    Invoke-HarnessCommand "dotnet" @("format", $solution, "--verify-no-changes", "--verbosity", "minimal")
}

Write-Host "==> Harness validation passed"
