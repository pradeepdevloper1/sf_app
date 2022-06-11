using Xamarin.Forms;

namespace XF.APP.BAL
{
    public class DefectCardListViewModel
    {
        public int Id { get; set; }
        public bool IsAddCell { get; set; }
        public string TypeText { get; set; }
        public string LocOrOperText { get; set; }
        public string DefectListText { get; set; }
        public bool IsSelected { get; set; }
        public double Opacity { get; set; }
        public bool HasShadow { get; set; }
        public Color BackgroundColor { get; set; }
    }
}
