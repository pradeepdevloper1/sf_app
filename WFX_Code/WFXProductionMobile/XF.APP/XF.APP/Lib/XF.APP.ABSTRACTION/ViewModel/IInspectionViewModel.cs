using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IInspectionViewModel : INavigationSearchViewModel
    {
        PurchaseOrder1 SelectedPo { get; set; }
        int SelectedValue { get; set; }
        object SelectedItem { get; set; }
        int SelectedListCellId { get; set; }
        void OnScreenAppearing();
    }
}
