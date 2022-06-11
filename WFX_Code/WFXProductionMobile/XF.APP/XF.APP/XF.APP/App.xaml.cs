using Autofac;
using AutoMapper;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.APP.ABSTRACTION;
using XF.APP.ApiService;
using XF.APP.BAL;
using XF.APP.DAL;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using XF.BASE.Pages;

namespace XF.APP
{
    public partial class App : Xamarin.Forms.Application
    {
        public static bool DoBack { get; set; }

        public App()
        {
            Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            InitializeComponent();

            //initilize config json
            this.InitilizeAppSettings();

            //initilize languages
            this.InitilizeResources();

            //initilize viewmodels
            this.InitializeViewModels();

            //initilize service
            this.InitilizeServices();

            //set startup page
            this.StartPage();            
        }

        private void StartPage()
        {
            string userDetails = Preferences.Get("AUTH_KEY", string.Empty);

            if (string.IsNullOrEmpty(userDetails))
            {
                NativeService.NavigationService.SetRootPage("LoginPage");
            }
            else
            {
                NativeService.NavigationService.SetRootPage("RoleSelectionPage");
                //NativeService.NavigationService.SetRootPage("MasterPage");
            }
        }

        #region Database setup
        public static void SetDatabasePath(string databasePath)
        {
            DAL.Constants.DbPath = databasePath;
        }
        #endregion

        #region Initilize localization
        private void InitilizeResources()
        {
            string language = Preferences.Get("SEL_LANG", string.Empty);
            BASE.CommonMethods.InitilizeLocalization(language);
        }
        #endregion

        #region Initilize configurations
        private void InitilizeAppSettings()
        {
            ApiService.Constants.BaseUrl = AppSettingsManager.Settings["WebServiceAPI"];
            LogService.Constants.BaseUrl = AppSettingsManager.Settings["LoggerServiceAPI"];

            // Version tracking
            VersionTracking.Track();
        }
        #endregion

        #region Initilize ViewModels & Pages
        private void InitializeViewModels()
        {
            DependencyService.Register<ILoginPageViewModel, LoginPageViewModel>();
            DependencyService.Register<INavigationSearchViewModel, NavigationSearchPageViewModel>();
            DependencyService.Register<ISalesOrdersViewModel, SalesOrdersPageViewModel>();
            DependencyService.Register<IInspectionViewModel, InspectionPageViewModel>();
            DependencyService.Register<IBulkOutputViewModel, BulkOutputPageViewModel>();
            DependencyService.Register<IResultMsgViewModel, ResultMsgPageViewModel>();
            DependencyService.Register<IDefectSelectionViewModel, DefectSelectionPageViewModel>();
            DependencyService.Register<IDefectCardViewModel, DefectCardPageViewModel>();
            DependencyService.Register<IOutputViewModel, OutputPageViewModel>();
            DependencyService.Register<IRoleSelectionPageViewModel, RoleSelectionPageViewModel>();
            DependencyService.Register<IProcessPageViewModel, ProcessPageViewModel>();
            DependencyService.Register<IMasterPageViewModel, MasterPageViewModel>();

            var navigationService = new NavigationService();

            navigationService.Configure("BasePage", typeof(BasePage));
            navigationService.Configure("LoginPage", typeof(LoginPage));
            navigationService.Configure("NavigationSearchPage", typeof(NavigationSearchPage));
            navigationService.Configure("SalesOrdersPage", typeof(SalesOrdersPage));
            navigationService.Configure("InspectionPage", typeof(InspectionPage));
            navigationService.Configure("DefectSelectionPage", typeof(DefectSelectionPage));
            navigationService.Configure("DefectCardPage", typeof(DefectCardPage));
            navigationService.Configure("BulkOutputPage", typeof(BulkOutputPage));
            navigationService.Configure("ResultMsgPage", typeof(ResultMsgPage));
            navigationService.Configure("OutputPage", typeof(OutputPage));
            navigationService.Configure("RoleSelectionPage", typeof(RoleSelectionPage));
            navigationService.Configure("ProcessPage", typeof(ProcessPage));
            navigationService.Configure("MasterPage", typeof(MasterPage));

            NativeService.NavigationService = navigationService;
        }
        #endregion

        #region Initilize Services
        private void InitilizeServices()
        {
            var builder = new ContainerBuilder();

            //repositories
            builder.RegisterType<BaseRepository>().As<IBaseRepository>().SingleInstance();
            builder.RegisterType<PurchaseOrderRepository>().As<IPurchaseOrderRepository>().SingleInstance();
            builder.RegisterType<UserDetailsRepository>().As<IUserDetailsRepository>().SingleInstance();

            //service
            builder.RegisterType<BaseService>().As<IBaseService>().SingleInstance();
            builder.RegisterType<LookupService>().As<ILookupService>().SingleInstance();
            builder.RegisterType<TransactionService>().As<ITransactionService>().SingleInstance();

            //Auth service - XF.APP.AuthService
            builder.RegisterType<UserWebService>().As<IUserWebService>().SingleInstance();

            //Log service - XF.APP.LogService
            builder.RegisterType<LogService.LogService>().As<ILoggerService>().SingleInstance();

            //API service - XF.APP.ApiService

            //vm service
            builder.RegisterType<LookupVMService>().As<ILookupVMService>().SingleInstance();

            //mapper
            IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
            builder.RegisterInstance(mapper);

            ServiceLocator.Container = builder.Build();
        }
        #endregion

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
