﻿using System;
using System.Threading.Tasks;

namespace XF.APP.ABSTRACTION
{
    public interface INavigationService
    {
        string CurrentPageKey { get; }
        void SetRootPage(string pageKey);
        void Configure(string pageKey, Type pageType);
        Task GoBack();
        Task RemoveNPagesFromStack(int count);
        Task NavigateModalAsync(string pageKey, bool animated = true);
        Task NavigateModalAsync(string pageKey, object parameter, bool animated = true);
        Task NavigateAsync(string pageKey, bool animated = true);
        Task NavigateAsync(string pageKey, object parameter, bool animated = true);
    }
}
