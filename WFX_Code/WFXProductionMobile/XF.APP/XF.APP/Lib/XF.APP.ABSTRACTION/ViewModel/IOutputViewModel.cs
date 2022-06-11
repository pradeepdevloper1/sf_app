using System.Collections.Generic;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IOutputViewModel : INavigationSearchViewModel
    {
        PurchaseOrder1 SelectedPo { get; set; }
        InspDataViewModel InspData { get; set; }
        object SelectedPauseReason { get; set; }
        void OnScreenAppearing(string arg1, string arg2, Dictionary<string,string> pauseReasonList, string matchPoints);
        int IsDefectOrRejectChanged();
        void InitilizeData();
    }
}
