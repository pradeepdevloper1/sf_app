using System.Collections.Generic;
using System.Threading.Tasks;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IDefectSelectionViewModel : IBaseViewModel
    {
        object SelectedItem { get; set; }
        int SelectedListCellId { get; set; }
        string SelectedClothType { get; set; }
        PostQCInputDto PostQCInput { get; set; }
        void OnScreenAppearing();
        Task<List<PoImageDto>> FetchListData();
        void ShowLoader();
        void HideLoader();
    }
}