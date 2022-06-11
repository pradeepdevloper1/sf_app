using System;
using System.ComponentModel;

namespace XF.APP.ABSTRACTION
{
    public interface IBaseViewModel : INotifyPropertyChanged,IDisposable
    {
        bool IsBusy { get; set; }
        string Title { get; set; }
        string LoadingText { get; set; }
    }
}
