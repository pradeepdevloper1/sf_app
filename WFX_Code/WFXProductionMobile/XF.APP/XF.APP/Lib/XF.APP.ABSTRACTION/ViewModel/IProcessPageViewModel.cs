using System;
using System.Collections.Generic;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IProcessPageViewModel: IBaseViewModel 
    {
        ProcessDataViewModel ProcessModel { get; set; }
        string SelectedProductionOrderNo { get; set; }
        string SelectedColors { get; set; }
        string SelectedWorkOrderNo { get; set; }
        string SelectedParts { get; set; }
        string SelectedIssueToWorkOrderNo { get; set; }
        string SelectedIssueToLineNo { get; set; }
        string SelectedColorsCode { get; set; }
        string SelectedSizes { get; set; }
        ProcessDataDto ProcessDatadto  { get; set; }

        void OnScreenAppearing();
       
    }
}
