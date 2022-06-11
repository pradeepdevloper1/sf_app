using System.Collections.Generic;
using System.Windows.Input;
using XF.APP.ABSTRACTION;

namespace XF.APP.BAL
{
    public class ResultMsgPageViewModel : BaseViewModel, IResultMsgViewModel
    {
        public ICommand SubmitCommand { set; get; }

        string _messageText;
        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }

        string _smileType;
        public string SmileType
        {
            get { return _smileType; }
            set { SetProperty(ref _smileType, value); }
        }

        public ResultMsgPageViewModel()
        {
            SubmitCommand = new BaseCommand((object obj) => SubmitCommandClicked());
        }

        private async void SubmitCommandClicked()
        {
            DefectCardPageViewModel.SelectedDefects = new List<DefectCardListViewModel>();

            // Redirect to OutputPage
            await NativeService.NavigationService.RemoveNPagesFromStack(3);
        }
    }
}
