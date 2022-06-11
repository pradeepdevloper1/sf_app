using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace XF.APP.iOS.Renderer
{
    public class FrameRenderer_iOS : FrameRenderer
    {
        public override void Draw(CGRect rect)
        {
            try
            {
                base.Draw(rect);

                // Update shadow to match better material design standards of elevation
                Layer.ShadowRadius = 2.0f;
                Layer.ShadowColor = UIColor.Gray.CGColor;
                Layer.ShadowOffset = new CGSize(2, 2);
                Layer.ShadowOpacity = 0.80f;
                Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                Layer.MasksToBounds = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
