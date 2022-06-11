using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XF.APP.Droid.Renderer;

[assembly: ExportRenderer(typeof(ListView), typeof(ListRenderer))]
namespace XF.APP.Droid.Renderer
{
    public class ListRenderer : ListViewRenderer
    {
        private readonly Context _context;
        public ListRenderer(Context context) : base(context)
        {
            _context = context;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.VerticalScrollBarEnabled = false;
            }
        }
    }
}
