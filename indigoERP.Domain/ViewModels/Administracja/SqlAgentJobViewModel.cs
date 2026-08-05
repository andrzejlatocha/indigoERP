using System;

namespace indigoERP.Domain.ViewModels._Administracja
{
    public class SqlAgentJobViewModel
    {
        public string JobName { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string OstatniStatus { get; set; } = "Brak historii";
        public DateTime? OstatnieUruchomienie { get; set; }
    }
}
