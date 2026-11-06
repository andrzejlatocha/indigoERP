using System.Security.Cryptography.X509Certificates;
using DevExpress.Office.DigitalSignatures;
using DevExpress.Pdf;

namespace indigoERP.Agent.Signing;

/// <summary>Miejsce podpisu na stronie PDF (numer strony od 1, prostokąt w punktach PDF).</summary>
public sealed record SignaturePlacement(int PageNumber, PdfRectangle Bounds);

/// <summary>Nakłada wizualny podpis z grafiką i podpis PKCS#7 certyfikatem z magazynu bieżącego użytkownika.</summary>
public sealed class PdfSignatureService
{
    /// <param name="password">Hasło otwarcia PDF; null lub puste = plik bez zabezpieczenia hasłem.</param>
    public void Sign(
        string inputPath,
        string outputPath,
        string certificateSerial,
        byte[] signatureImage,
        SignaturePlacement placement,
        string reason,
        string location,
        string? password)
    {
        var certificate = FindCertificate(certificateSerial);

        var fieldInfo = new PdfSignatureFieldInfo(placement.PageNumber)
        {
            Name = $"Signature_{DateTime.Now:yyyyMMddHHmmssfff}",
            SignatureBounds = placement.Bounds
        };

        var pkcs7 = new Pkcs7Signer(certificate, HashAlgorithmType.SHA256);
        var builder = new PdfSignatureBuilder(pkcs7, fieldInfo);
        builder.SetImageData(signatureImage);
        builder.Reason = reason;
        builder.Location = location;
        builder.Name = certificate.GetNameInfo(X509NameType.SimpleName, false);

        using var inputStream = File.OpenRead(inputPath);
        using var signer = string.IsNullOrEmpty(password)
            ? new PdfDocumentSigner(inputStream)
            : new PdfDocumentSigner(inputStream, password);
        signer.SaveDocument(outputPath, builder);
    }

    private static X509Certificate2 FindCertificate(string serial)
    {
        using var store = new X509Store(StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

        var found = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, validOnly: true);
        if (found.Count == 0)
            throw new InvalidOperationException($"Nie znaleziono certyfikatu o numerze {serial}.");

        return found[0];
    }
}
