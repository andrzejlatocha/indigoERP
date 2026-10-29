<#
.SYNOPSIS
Przygotowuje migrację historycznego „Głosu Hutnika” z WENUS do indigoERP.

.DESCRIPTION
1. Czyta oryginalny skrypt (GlosHutnika_Migracja_z_intranet_ORYGINAL.sql, 160 numerów).
2. Dla każdego załącznika PDF rozwiązuje faktyczny folder na WENUS. Trzy warianty:
   - ścieżka dokładnie z wdoKatalog,
   - sklejony znacznik czasu (…HEX20230113113831 -> …HEX\13012023113831),
   - nadmiarowe zera na początku (0000000\ -> 000000\).
3. (opcjonalnie -Kopiuj) kopiuje PDF do DocumentsRoot, zachowując rozwiązany folder.
   Pliki o tym samym rozmiarze są pomijane, więc skrypt można uruchomić ponownie.
4. Generuje idempotentny skrypt SQL (wiadomości po temacie i dacie, dokumenty po nazwie pliku)
   z wdoKatalog ustawionym na rozwiązany folder, czyli zgodnie z tym, gdzie leży plik.
5. Zapisuje raport brakujących PDF.

Uruchamiać na DEIMOS (C:\_documents) albo z mapowaniem Y: (Y:\_documents = C:\_documents).
Skrypt SQL uruchamia użytkownik w SSMS na JAPET (produkcja).
#>
param(
    [string]$Zrodlo = (Join-Path $PSScriptRoot 'GlosHutnika_Migracja_z_intranet_ORYGINAL.sql'),
    [string]$SqlWyjscie = (Join-Path $PSScriptRoot '..\..\_SQL Queries\GlosHutnika_Migracja_z_intranet_v2.sql'),
    [string]$RaportWyjscie = (Join-Path $PSScriptRoot 'Raport-GlosHutnika.txt'),
    [string]$WenusRoot = '\\10.0.45.10\d$\Shared Folders\dokumenty',
    [string]$DocumentsRoot = 'C:\_documents',
    [switch]$Kopiuj
)

$ErrorActionPreference = 'Stop'
$utf8Bom = New-Object System.Text.UTF8Encoding($true)

$sql = Get-Content -LiteralPath $Zrodlo -Raw -Encoding UTF8

$rxWiadomosc = "INSERT INTO dbo\.Wiadomosc \(.*?\) VALUES \(900, NULL, N'(?<temat>(?:[^']|'')*)', N'(?<tresc>(?:[^']|'')*)', '(?<data>[^']+)', '2', 1, '[^']+', '[^']+', 0\);"
$rxDokument = "INSERT INTO dbo\.WiadomoscDokument \(.*?\) VALUES \(SCOPE_IDENTITY\(\), N'(?<typ>[^']*)', N'(?<nazwa>(?:[^']|'')*)', N'(?<plik>(?:[^']|'')*)', N'(?<katalog>(?:[^']|'')*)', '2'\);"

$wiadomosci = [regex]::Matches($sql, $rxWiadomosc, 'Singleline')
$dokumenty = [regex]::Matches($sql, $rxDokument, 'Singleline')

if ($wiadomosci.Count -eq 0 -or $wiadomosci.Count -ne $dokumenty.Count) {
    throw "Niezgodne liczby wierszy: wiadomości=$($wiadomosci.Count), dokumenty=$($dokumenty.Count)."
}

function Q([string]$tekst) { "N'" + ($tekst -replace "'", "''") + "'" }

function Rozwiaz([string]$katalog, [string]$plik) {
    $sklejony = { param($k) $k -replace '^(.*[0-9A-F]{32})(\d{8})(\d{6})$', '$1\$2$3' }
    $zera = $katalog -replace '^0+\\', '000000\'
    $kandydaci = @($katalog, (& $sklejony $katalog), $zera, (& $sklejony $zera)) | Select-Object -Unique
    foreach ($k in $kandydaci) {
        if (Test-Path -LiteralPath (Join-Path (Join-Path $WenusRoot $k) $plik)) { return $k }
    }
    return $null
}

$wiersze = for ($i = 0; $i -lt $wiadomosci.Count; $i++) {
    $w = $wiadomosci[$i]
    $d = $dokumenty[$i]
    $plik = $d.Groups['plik'].Value -replace "''", "'"
    $katalogZrodlo = $d.Groups['katalog'].Value -replace "''", "'"
    [pscustomobject]@{
        lp       = $i + 1
        temat    = $w.Groups['temat'].Value
        tresc    = $w.Groups['tresc'].Value
        data     = $w.Groups['data'].Value
        nazwa    = $d.Groups['nazwa'].Value
        plik     = $plik
        katalog  = Rozwiaz $katalogZrodlo $plik
        zrodlo   = $katalogZrodlo
    }
}

$brakujace = @($wiersze | Where-Object { -not $_.katalog })
$znalezione = @($wiersze | Where-Object { $_.katalog })

if ($Kopiuj) {
    $skopiowane = 0; $pominiete = 0; $bledy = @()
    foreach ($r in $znalezione) {
        $src = Join-Path (Join-Path $WenusRoot $r.katalog) $r.plik
        $dstDir = Join-Path $DocumentsRoot $r.katalog
        $dst = Join-Path $dstDir $r.plik
        if ((Test-Path -LiteralPath $dst) -and ((Get-Item -LiteralPath $dst).Length -eq (Get-Item -LiteralPath $src).Length)) {
            $pominiete++; continue
        }
        New-Item -ItemType Directory -Force -Path $dstDir | Out-Null
        for ($proba = 1; $proba -le 5; $proba++) {
            try {
                Copy-Item -LiteralPath $src -Destination $dst -Force -ErrorAction Stop
                $skopiowane++
                break
            } catch {
                if ($proba -eq 5) { $bledy += "$($r.plik): $($_.Exception.Message)" } else { Start-Sleep -Seconds 3 }
            }
        }
    }
    "Kopiowanie: skopiowano $skopiowane, pominięto (istnieją) $pominiete, błędy $($bledy.Count)"
    $bledy | ForEach-Object { "  BŁĄD $_" }
}

$linie = New-Object System.Collections.Generic.List[string]
$linie.Add("-- Migracja historycznego Głosu Hutnika z WENUS do indigoERP (wygenerowane: Generuj-migracje.ps1).")
$linie.Add("-- Wiadomości: $($wiersze.Count), dokumenty PDF do przypięcia: $($znalezione.Count), bez pliku (pominięte w dokumentach): $($brakujace.Count).")
$linie.Add("-- Idempotentne: ponowne uruchomienie nie dubluje wiadomości (temat + data) ani dokumentów (wdoPlik).")
$linie.Add("USE indigoERP;")
$linie.Add("GO")
$linie.Add("SET NOCOUNT ON;")
$linie.Add("DECLARE @w TABLE (lp int PRIMARY KEY, temat nvarchar(300), tresc nvarchar(max), data datetime);")
$linie.Add("INSERT INTO @w (lp, temat, tresc, data) VALUES")
for ($i = 0; $i -lt $wiersze.Count; $i++) {
    $r = $wiersze[$i]
    $koniec = if ($i -eq $wiersze.Count - 1) { ';' } else { ',' }
    $linie.Add("($($r.lp), N'$($r.temat)', N'$($r.tresc)', '$($r.data)')$koniec")
}
$linie.Add("INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety)")
$linie.Add("SELECT 900, NULL, w.temat, w.tresc, w.data, '2', 1, w.data, w.data, 0 FROM @w w")
$linie.Add("WHERE NOT EXISTS (SELECT 1 FROM dbo.Wiadomosc x WHERE x.wiaAplikacjaID = 900 AND x.wiaTemat = w.temat AND x.wiaData = w.data);")
$linie.Add("GO")
if ($znalezione.Count -gt 0) {
    $linie.Add("DECLARE @d TABLE (lp int PRIMARY KEY, nazwa nvarchar(300), plik nvarchar(300), katalog nvarchar(400));")
    $linie.Add("INSERT INTO @d (lp, nazwa, plik, katalog) VALUES")
    for ($i = 0; $i -lt $znalezione.Count; $i++) {
        $r = $znalezione[$i]
        $koniec = if ($i -eq $znalezione.Count - 1) { ';' } else { ',' }
        $linie.Add("($($r.lp), $(Q $r.nazwa), $(Q $r.plik), $(Q $r.katalog))$koniec")
    }
    $linie.Add("INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus)")
    $linie.Add("SELECT x.wiaID, N'pdf', d.nazwa, d.plik, d.katalog, '2'")
    $linie.Add("FROM @d d")
    $linie.Add("JOIN @w w ON w.lp = d.lp")
    $linie.Add("JOIN dbo.Wiadomosc x ON x.wiaAplikacjaID = 900 AND x.wiaTemat = w.temat AND x.wiaData = w.data")
    $linie.Add("WHERE NOT EXISTS (SELECT 1 FROM dbo.WiadomoscDokument y WHERE y.wdoWiadomoscID = x.wiaID AND y.wdoPlik = d.plik);")
    $linie.Add("GO")
}
$linie.Add("-- Kontrola: liczba numerów GH (900) i dokumentów PDF po migracji")
$linie.Add("SELECT (SELECT COUNT(*) FROM dbo.Wiadomosc WHERE wiaAplikacjaID = 900 AND wiaPubliczna = 1 AND wiaStatus = '2') AS numery,")
$linie.Add("       (SELECT COUNT(*) FROM dbo.WiadomoscDokument d JOIN dbo.Wiadomosc x ON x.wiaID = d.wdoWiadomoscID WHERE x.wiaAplikacjaID = 900 AND d.wdoStatus = '2') AS pliki;")
$linie.Add("GO")

[IO.File]::WriteAllText((Resolve-Path -LiteralPath (Split-Path -Parent $SqlWyjscie)).Path + "\" + (Split-Path -Leaf $SqlWyjscie), ($linie -join "`r`n") + "`r`n", $utf8Bom)

$raport = New-Object System.Collections.Generic.List[string]
$raport.Add("Brakujące PDF (wiadomość zostanie zaimportowana bez załącznika): $($brakujace.Count)")
$brakujace | ForEach-Object { $raport.Add("  lp $($_.lp): $($_.plik)  <=  $($_.zrodlo)") }
$raport.Add("Rozwiązane warianty folderu (inne niż zapisane w źródle): " + @($znalezione | Where-Object { $_.katalog -ne $_.zrodlo }).Count)
$znalezione | Where-Object { $_.katalog -ne $_.zrodlo } | ForEach-Object { $raport.Add("  lp $($_.lp): $($_.zrodlo)  ->  $($_.katalog)") }
[IO.File]::WriteAllText((Resolve-Path -LiteralPath (Split-Path -Parent $RaportWyjscie)).Path + "\" + (Split-Path -Leaf $RaportWyjscie), ($raport -join "`r`n") + "`r`n", $utf8Bom)

"Wiadomości: $($wiersze.Count) | z PDF: $($znalezione.Count) | bez PDF: $($brakujace.Count)"
"SQL: $SqlWyjscie"
"Raport: $RaportWyjscie"
