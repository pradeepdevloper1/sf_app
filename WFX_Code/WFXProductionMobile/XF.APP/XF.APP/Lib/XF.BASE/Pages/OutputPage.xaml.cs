using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using XF.BASE.Assets.Localization;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutputPage : NavigationSearchPage
    {
        public IOutputViewModel Context { get; set; }
        private InspDataViewModel InspData { get; set; }

        public OutputPage(InspDataViewModel inspData)
        {
            InitializeComponent();
            string TabSize = Preferences.Get("TabSize", string.Empty);
            if (TabSize == "10inch" || TabSize == "")
            {
                gridMainView.IsVisible = true;
            }
            if (TabSize == "7inch")
            {
                gridMainView7i.IsVisible = true;
            }
            BindingContext = null;
            InspData = inspData;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
            {
                Context = DependencyService.Get<IOutputViewModel>();
                Context.InitilizeData();
                Context.LineID = Preferences.Get("LINE_ID", string.Empty);
                Context.LineMan = Preferences.Get("USER_NAME", string.Empty);
                Context.UserID = Preferences.Get("USER_ID", 0);
                Context.InspData = InspData;
                Context.SelectedPo = InspData.SelectedPo;
                BindingContext = Context;
            }

            // Prepare points
            string matchPoints = string.Empty;
            int changesQcStatus = Context.IsDefectOrRejectChanged();

            if (changesQcStatus == 2 || changesQcStatus == 3)
            {
                if (DefectSelectionPage.touchPoints.Count > 0)
                {
                    if (DefectSelectionPage.touchPoints.Values != null && DefectSelectionPage.touchPoints.Values.Count > 0)
                    {
                        foreach (var values in DefectSelectionPage.touchPoints.Values)
                        {
                            foreach (var point in values)
                            {
                                float x = point.Key.X;
                                float y = point.Key.Y;
                                int imgId = point.Value;

                                // QC_STATUS##IMAGE_ID##'X'##'Y'
                                matchPoints += $"{(string.IsNullOrEmpty(matchPoints) ? "" : "\\") }{changesQcStatus}##{imgId}##{x}##{y}";
                            }
                        }
                    }
                }
            }

            Context.OnScreenAppearing(AppResources.PauseButton, AppResources.ResumeButton,
                new System.Collections.Generic.Dictionary<string, string>
                {
                    {AppResources.WashroomText, "ic_washroom"},
                    {AppResources.NoFeedText, "ic_no_feed"},
                    {AppResources.LunchBreakText, "ic_lunch_break"},
                    {AppResources.MeetingText, "ic_meeting"},
                    {AppResources.OtherText, "ic_question"}
                }, matchPoints);
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Supresses highlighting of ListItems
            if (e.CurrentSelection == null) return;

            CollectionView list = sender as CollectionView;

            if (e.CurrentSelection.Count > 0)
                Context.SelectedPauseReason = e.CurrentSelection[0];

            list.SelectedItem = null;
        }

    }
}
