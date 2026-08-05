using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Administracja;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface ISerwerService
    {
        Task<IEnumerable<SerwerViewModel>> Select(string srvStatus);
        Task<SerwerDto> Get(int srvID);
        Task<ResponseMessage> Insert(SerwerDto serwer);
        Task<ResponseMessage> Update(int srvID, SerwerDto serwer);
        Task<ResponseMessage> Delete(int srvID);
        Task<SerwerInfoViewModel?> GetInfo();
        Task<IEnumerable<MonitorSerwerStatusViewModel>> SelectStatus();
        Task<MonitorSerwerStatusViewModel?> Check(int srvID);
        Task<IEnumerable<SqlAgentJobViewModel>> SelectJoby(int srvID);
    }
}
