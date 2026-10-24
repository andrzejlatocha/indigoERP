[CmdletBinding()]
param(
    [string]$PublishDirectory = $PSScriptRoot,
    [string]$ServiceName = "indigoERP.Worker",
    [string]$ServiceDisplayName = "indigoERP Worker",
    [string]$ServiceAccount,
    [SecureString]$ServicePassword,
    [switch]$UseLocalSystem,
    [switch]$StartService
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Test-Administrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = [Security.Principal.WindowsPrincipal]::new($identity)

    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (-not (Test-Administrator)) {
    throw "Uruchom PowerShell jako administrator."
}

if ($UseLocalSystem -and -not [string]::IsNullOrWhiteSpace($ServiceAccount)) {
    throw "Użyj albo -UseLocalSystem, albo -ServiceAccount."
}

if (-not $UseLocalSystem -and [string]::IsNullOrWhiteSpace($ServiceAccount)) {
    $ServiceAccount = Read-Host "Konto usługi, np. HCZ\\svc_indigoERP_worker"
}

$PublishDirectory = [IO.Path]::GetFullPath($PublishDirectory)
$executable = Join-Path $PublishDirectory "indigoERP.Worker.exe"
$configurationPath = Join-Path $PublishDirectory "appsettings.json"
$logDirectory = Join-Path $PublishDirectory "logs"

if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "Nie znaleziono pliku $executable. Skopiuj skrypt do katalogu publish albo podaj -PublishDirectory."
}

if (-not (Test-Path -LiteralPath $configurationPath -PathType Leaf)) {
    throw "Nie znaleziono pliku $configurationPath."
}

$configuration = Get-Content -LiteralPath $configurationPath -Raw | ConvertFrom-Json
$connectionString = $configuration.ConnectionStrings.indigoERPConnection

if ([string]::IsNullOrWhiteSpace($connectionString)) {
    throw "Brak ConnectionStrings:indigoERPConnection w appsettings.json."
}

$graphVariables = @("MicrosoftGraph_TenantId", "MicrosoftGraph_ClientId", "MicrosoftGraph_ClientSecret")
$missingVariables = @($graphVariables | Where-Object {
    [string]::IsNullOrWhiteSpace([Environment]::GetEnvironmentVariable($_, [EnvironmentVariableTarget]::Machine))
})

if ($missingVariables.Count -gt 0) {
    throw "Brak zmiennych systemowych: $($missingVariables -join ', ')."
}

New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null

if (-not $UseLocalSystem) {
    & icacls.exe $logDirectory "/grant" "$ServiceAccount`:(OI)(CI)M" | Out-Null

    if ($LASTEXITCODE -ne 0) {
        throw "Nie udało się nadać kontu $ServiceAccount prawa zapisu do katalogu logs."
    }
}

$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if ($null -ne $existingService) {
    if ($existingService.Status -ne "Stopped") {
        Stop-Service -Name $ServiceName -Force
        $existingService.WaitForStatus("Stopped", [TimeSpan]::FromSeconds(30))
    }

    $binaryPath = '"{0}"' -f $executable
    & sc.exe config $ServiceName "binPath= $binaryPath" "start= auto" "DisplayName= $ServiceDisplayName" | Out-Null

    if ($LASTEXITCODE -ne 0) {
        throw "Nie udało się zaktualizować usługi $ServiceName."
    }

    Write-Host "Usługa już istnieje. Zachowano jej obecne konto logowania."
}
else {
    if ($UseLocalSystem) {
        New-Service -Name $ServiceName -BinaryPathName ('"{0}"' -f $executable) -DisplayName $ServiceDisplayName `
            -Description "Wysyłka wiadomości indigoERP przez Microsoft Graph." -StartupType Automatic
    }
    else {
        if ($null -eq $ServicePassword) {
            $ServicePassword = Read-Host "Hasło konta usługi" -AsSecureString
        }

        $credential = [PSCredential]::new($ServiceAccount, $ServicePassword)
        New-Service -Name $ServiceName -BinaryPathName ('"{0}"' -f $executable) -DisplayName $ServiceDisplayName `
            -Description "Wysyłka wiadomości indigoERP przez Microsoft Graph." -StartupType Automatic -Credential $credential
    }
}

& sc.exe failure $ServiceName "reset= 86400" "actions= restart/60000/restart/60000/restart/60000" | Out-Null

if ($LASTEXITCODE -ne 0) {
    throw "Nie udało się ustawić automatycznego ponawiania usługi $ServiceName."
}

if ($StartService) {
    Start-Service -Name $ServiceName
    Start-Sleep -Seconds 3
}

$service = Get-Service -Name $ServiceName
Write-Host "Usługa $($service.Name) została zainstalowana. Stan: $($service.Status)."
Write-Host "Logi: $logDirectory"
