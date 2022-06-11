using System.Drawing;

namespace XF.APP.ABSTRACTION
{
    public interface IResultMsgViewModel : IBaseViewModel
    {
        string MessageText { get; set; }
        string SmileType { get; set; }
    }
}
