using System;
using System.Collections.Generic;
using System.Text;
using XF.APP.DTO;

namespace XF.APP.ABSTRACTION
{
    public interface IRoleSelectionPageViewModel : INavigationSearchViewModel
    {
        UserRole SelectedRole { get; set; }
        Line SelectedLine { get; set; }

        void OnScreenAppearing();
        void fetchLineList();
    }
}
