using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface ISalesOrdersViewModel : INavigationSearchViewModel
    {
        SalesOrder SelectedSo { get; set; }
        PurchaseOrder1 SelectedPo { get; set; }

        void OnScreenAppearing();
    }
}
