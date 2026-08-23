using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using Microsoft.AspNetCore.Components.Forms;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IWiadomoscDokumentService
    {
        Task<IEnumerable<WiadomoscDokumentDto>> SelectByWiadomosc(int wiaID);
        Task<ResponseMessage> Upload(int wiaID, IBrowserFile file);
        Task<WiadomoscDokumentFile?> Download(int wdoID);
        Task<ResponseMessage> Delete(int wdoID);
    }

    public record WiadomoscDokumentFile(Stream Content, string FileName, string ContentType);
}
