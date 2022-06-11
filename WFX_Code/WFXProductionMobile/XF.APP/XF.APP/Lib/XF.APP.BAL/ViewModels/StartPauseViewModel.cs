using Xamarin.Forms;

namespace XF.APP.BAL
{
    public class StartPauseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ImageSource Image { get; set; }
        public bool IsSelected { get; set; }
        public Color BorderColor { get; set; }
        public double Opacity { get; set; }
    }
}
