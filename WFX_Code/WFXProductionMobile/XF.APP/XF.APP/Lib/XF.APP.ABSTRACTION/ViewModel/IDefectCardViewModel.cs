using System.Collections.Generic;
using System.IO;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IDefectCardViewModel : IBaseViewModel
    {
        PostQCInputDto PostQCInput { get; set; }
        string SelectedClothType { get; set; }
        void OnScreenAppearing();
        string GetMarkPoints();
        bool ModifyCellItem(object selectedItem);
        void SubmitDefect(Dictionary<string, byte[]> imageData);
        void ShowLoader();
        void HideLoader();
    }
}
