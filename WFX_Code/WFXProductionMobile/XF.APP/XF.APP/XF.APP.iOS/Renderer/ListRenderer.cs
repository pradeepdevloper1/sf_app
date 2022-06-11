using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XF.APP.iOS.Renderer;

[assembly: ExportRenderer(typeof(ListView), typeof(ListRenderer))]
namespace XF.APP.iOS.Renderer
{
    public class ListRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.ShowsVerticalScrollIndicator = false;
            }
        }
    }
}
