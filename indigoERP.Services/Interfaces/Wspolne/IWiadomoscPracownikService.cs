using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IWiadomoscPracownikService
    {
        Task<IEnumerable<WiadomoscZakresEnumerator>> SelectZakresy(int appID);
        Task<IEnumerable<WiadomoscZakresPracownikViewModel>> SelectPracownicyZakres(int wzaID);
        Task<IEnumerable<WiadomoscWzorzecEnumerator>> SelectWzorce(int appID);
        Task<(ResponseMessage Response, WiadomoscWysylkaZakresWynikModel? Wynik)> InsertWysylka(
            WiadomoscWysylkaZakresModel model, IReadOnlyList<WiadomoscPlikWysylki> pliki);
        Task<(ResponseMessage Response, WiadomoscDto? Wiadomosc)> InsertRobocza(
            WiadomoscRoboczaModel model, IReadOnlyList<WiadomoscPlikWysylki> pliki);
        Task<IEnumerable<WiadomoscDto>> SelectRobocze(int appID);
        Task<(ResponseMessage Response, WiadomoscWysylkaZakresWynikModel? Wynik)> WyslijRobocza(
            int wiaID, WiadomoscWyslijRoboczaModel model);
        Task<IEnumerable<WiadomoscHarmonogramViewModel>> SelectHarmonogram(int appID);
        Task<ResponseMessage> AnulujHarmonogram(int wiaID);
        Task<ResponseMessage> PoprawRobocza(int wiaID, WiadomoscPoprawaModel model);
        Task<ResponseMessage> UsunWiadomosc(int wiaID);
        Task<IEnumerable<WiadomoscPracownikViewModel>> SelectWyslane(int appID);
        Task<IEnumerable<WiadomoscPodsumowanieDto>> SelectPodsumowanie(int appID);
    }
}
