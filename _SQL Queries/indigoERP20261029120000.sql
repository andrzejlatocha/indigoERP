/*
  Wzorzec wiadomości epicaHR (dbo.WiadomoscWzorzec, msgAplikacjaID = 100).
  Szablon HTML z podanego przykładu, dostosowany do Workera (WiadomoscTrescFactory):
    {{Title}}           -> {{subject}}   (temat wiadomości)
    {{Message}}         -> {{message}}   (treść wiadomości)
    {{ApplicationName}} -> epicaHR
    {{CurrentYear}}     -> 2026
    {{UserName}}        -> blok usunięty (Worker nie zna nazwy odbiorcy)
  Idempotentny: drugie uruchomienie nic nie dodaje.
  Uruchamia użytkownik (SSMS / sqlcmd -f 65001).
*/
SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM dbo.WiadomoscWzorzec WHERE msgAplikacjaID = 100 AND msgTemat = N'epicaHR - powiadomienie')
BEGIN
    PRINT N'Wzorzec epicaHR już istnieje - pominięto.';
END
ELSE
BEGIN
    DECLARE @tresc nvarchar(max) = N'';

    SET @tresc += N'<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body style="margin:0; padding:0; background:#F0F2F5; font-family:Helvetica,Arial,sans-serif;">
  <table border="0" cellpadding="0" cellspacing="0" width="100%" style="background:#F0F2F5;">
    <tr>
      <td align="center" style="padding:32px 16px;">
        <table border="0" cellpadding="0" cellspacing="0"
               style="width:620px; max-width:100%; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 12px rgba(0,0,0,0.08);">
          <tr>
            <td style="background:#58595B; padding:20px 32px;">
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td style="vertical-align:middle;">
                    <a href="https://hutaczestochowa.com.pl" target="_blank" style="text-decoration:none;">
                      <img src="https://portal.hutaczestochowa.com.pl/Images/logo/logo_white.svg" alt="Logo" border="0"
                           style="display:block; height:45px; border:none;" />
                    </a>
                  </td>
                  <td align="right" style="vertical-align:middle;">
                    <img src="https://portal.hutaczestochowa.com.pl/Images/logo/atacamaDMS_white.png" alt="Logo" border="0"
                         style="display:block; height:45px; border:none;" />
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td style="padding:0; background:#F47920;">
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td style="padding:40px 32px; vertical-align:middle;">
                    <table border="0" cellpadding="0" cellspacing="0">
                      <tr>
                        <td style="padding-right:20px; vertical-align:middle;">
                          <div style="width:56px; height:56px; background:rgba(255,255,255,0.2); border-radius:50%; text-align:center;">
                            <svg width="28" height="28" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" style="vertical-align:middle; margin-top:14px;">
                              <path d="M17 11H7a2 2 0 00-2 2v6a2 2 0 002 2h10a2 2 0 002-2v-6a2 2 0 00-2-2z" stroke="#ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                              <path d="M12 16a1 1 0 100-2 1 1 0 000 2z" fill="#ffffff"/>
                              <path d="M8 11V7a4 4 0 018 0v4" stroke="#ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                            </svg>
                          </div>
                        </td>
                        <td style="vertical-align:middle;">
                          <p style="margin:0 0 4px 0; font-size:22px; font-weight:700; color:#ffffff; letter-spacing:0.3px;">
                            {{subject}}
                          </p>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td align="right" style="padding-right:32px; vertical-align:middle; width:100px;">
                    <svg width="80" height="80" viewBox="0 0 80 80" fill="none" xmlns="http://www.w3.org/2000/svg" style="display:block; opacity:0.2;">
                      <polygon points="40,4 74,22 74,58 40,76 6,58 6,22" stroke="#ffffff" stroke-width="3" fill="none"/>
                    </svg>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td style="background:#58595B; height:3px; padding:0; font-size:0; line-height:0;">&nbsp;</td>
          </tr>
';

    SET @tresc += N'          <tr>
            <td style="padding:40px 32px 32px 32px;">
              <p style="margin:0 0 28px 0; font-size:15px; color:#444444; line-height:1.7;">
                {{message}}
              </p>
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td align="center" style="padding:8px 0 32px 0;">
                    <a href="https://intranet.hcz.com.pl:8210"
                       style="display:inline-block; padding:14px 40px; background:#F47920; color:#ffffff; font-size:15px; font-weight:600; text-decoration:none; border-radius:8px; letter-spacing:0.3px;">
                      Uruchom system
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td style="padding:0; background:#F7F8FA; border-top:1px solid #EEEEEE;">
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td style="padding:20px 32px; vertical-align:middle;">
                    <table border="0" cellpadding="0" cellspacing="0">
                      <tr>
                        <td style="padding-right:12px; vertical-align:middle;">
                          <img src="https://portal.hutaczestochowa.com.pl/Images/logo/SuperHero.png" alt="SoftCell" border="0"
                               style="display:block; width:28px; height:28px; border:none;" />
                        </td>
                        <td style="vertical-align:middle;">
                          <p style="margin:0; font-size:13px; color:#999999;">
                            IT Team &nbsp;&middot;&nbsp;
                            <a href="http://intranet.hcz.com.pl" style="color:#F47920; text-decoration:none;">intranet.hcz.com.pl</a>
                          </p>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td align="right" style="padding:20px 32px 20px 0;">
                    <p style="margin:0; font-size:11px; color:#BBBBBB; text-align:right;">
                      Wiadomość wygenerowana automatycznie.<br/>
                      Prosimy na nią nie odpowiadać.
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td style="padding:0; height:4px;">
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td style="background:#F47920; width:60%; height:4px;"></td>
                  <td style="background:#58595B; width:40%; height:4px;"></td>
                </tr>
              </table>
            </td>
          </tr>
        </table>
        <table border="0" cellpadding="0" cellspacing="0" style="width:620px; max-width:100%;">
          <tr>
            <td align="center" style="padding:16px 0 0 0;">
              <p style="margin:0; font-size:11px; color:#AAAAAA;">
                epicaHR&copy; 2026 Huta Częstochowa &nbsp;&middot;&nbsp;
              </p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>';

    INSERT INTO dbo.WiadomoscWzorzec (msgAplikacjaID, msgTyp, msgJezyk, msgTemat, msgTresc, msgStatus)
    VALUES (100, NULL, N'pl', N'epicaHR - powiadomienie', @tresc, N'2');

    PRINT N'Dodano wzorzec epicaHR.';
END;

-- Kontrola: długość i obecność polskich znaków (po zapisie, bez zmian danych)
SELECT msgID, msgAplikacjaID, msgTemat, msgStatus, LEN(msgTresc) AS DlugoscTresci,
       CASE WHEN CHARINDEX(N'Częstochowa', msgTresc) > 0 THEN N'OK' ELSE N'BŁĄD KODOWANIA' END AS PolskieZnaki,
       CASE WHEN CHARINDEX(N'{{message}}', msgTresc) > 0 AND CHARINDEX(N'{{subject}}', msgTresc) > 0 THEN N'OK' ELSE N'BRAK ZNACZNIKÓW' END AS Znaczniki
FROM dbo.WiadomoscWzorzec
WHERE msgAplikacjaID = 100 AND msgTemat = N'epicaHR - powiadomienie';
