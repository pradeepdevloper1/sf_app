using System.Drawing;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IBulkOutputViewModel : IBaseViewModel
    {
        string BulkOutputType { get; set; }
        string SmileType { get; set; }
        Color RoundedBtnBgColor { get; set; }
        PostQCInputDto PostQCInput { get; set; }
        void OnScreenAppearing();
    }
}
