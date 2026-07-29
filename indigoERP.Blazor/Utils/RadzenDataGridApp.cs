using Radzen.Blazor;

namespace indigoERP.Blazor.Utils
{
    public class RadzenDataGridApp<TItem> : RadzenDataGrid<TItem> where TItem : notnull
    {
        public RadzenDataGridApp() : base()
        {
            base.EqualsText = "Równy";
            base.NotEqualsText = "Nie równy";
            base.LessThanText = "Mniejszy od";
            base.LessThanOrEqualsText = "Mniejszy lub równy od";
            base.GreaterThanText = "Większy od";
            base.GreaterThanOrEqualsText = "Większy lub równy od";
            base.IsNullText = "Puste";
            base.IsNotNullText = "Nie puste";
            base.AndOperatorText = "I";
            base.OrOperatorText = "Lub";
            base.ContainsText = "Zawiera";
            base.DoesNotContainText = "Nie zawiera";
            base.StartsWithText = "Zaczyna się";
            base.EndsWithText = "Kończy się";
            base.ClearFilterText = "Wyczyść filtr";
            base.ApplyFilterText = "Zastosuj";
            base.FilterText = "Filtr";
            base.IsEmptyText = "puste";
            base.IsNotEmptyText = "nie puste";
            base.AllowRowSelectOnRowClick = true;
            base.AllowVirtualization = true;
            base.AllowPaging = true;
            base.AllowPickAllColumns = false;
            base.AllowColumnResize = true;
            base.AllowColumnReorder = true;
        }
    }
}
