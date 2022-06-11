using System.Threading.Tasks;

namespace XF.APP.ABSTRACTION
{
    public interface ILoginPageViewModel : IBaseViewModel
    {
        string Version { get; set; }
        void OnScreenAppearing(string deviceName);
    }
}
