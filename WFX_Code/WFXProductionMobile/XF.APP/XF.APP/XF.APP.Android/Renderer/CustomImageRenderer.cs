
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XF.APP.Droid.Renderer;
using static Android.Views.View;

[assembly: ExportRenderer(typeof(Image), typeof(CustomImageRenderer))]
namespace XF.APP.Droid.Renderer
{
    public class CustomImageRenderer : ImageRenderer, IOnTouchListener
    {
        public CustomImageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            SetOnTouchListener(this);
        }

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            ABSTRACTION.Constants.TouchCoOrdinates.Enqueue(new KeyValuePair<float, float>(e.GetX(), e.GetY()));

            return false;
        }
    }
}
