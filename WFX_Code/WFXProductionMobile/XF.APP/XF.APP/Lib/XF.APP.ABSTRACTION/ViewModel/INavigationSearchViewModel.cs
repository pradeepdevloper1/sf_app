namespace XF.APP.ABSTRACTION
{
    public interface INavigationSearchViewModel : IBaseViewModel
    {
        string LineID { get; set; }
        string LineMan { get; set; }
        int UserID { get; set; }
    }
}
