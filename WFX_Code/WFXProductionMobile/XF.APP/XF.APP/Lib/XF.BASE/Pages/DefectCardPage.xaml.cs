using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefectCardPage : BasePage
    {
        public IDefectCardViewModel context { get; set; }
        private PostQCInputDto PostQCInput { get; set; }
        private Dictionary<string, byte[]> imageData;
        private List<KeyValuePair<SKPoint, int>> lstDefectPoints;
        private List<KeyValuePair<SKPoint, int>> lstRejectPoints;

        public DefectCardPage(PostQCInputDto postQCInput)
        {
            InitializeComponent();
            PostQCInput = postQCInput;

            imageData = new Dictionary<string, byte[]>();

            lstDefectPoints = null;
            lstRejectPoints = null;

            Separator.BackgroundColor = postQCInput.QCStatus == 2 ? (Color)Application.Current.Resources["RoundedBtnDefectBgColor"] : Color.White;
            BtnCheck.TextColor = (Color)Application.Current.Resources[postQCInput.QCStatus == 2 ? "DefectBackgroundColor" : "RejectBackgroundColor"];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetTheme(PostQCInput.QCStatus == 2 ? 3 : 2);

            if (BindingContext == null)
            {
                context = DependencyService.Get<IDefectCardViewModel>();
                context.PostQCInput = PostQCInput;
                context.OnScreenAppearing();
                BindingContext = context;
            }

            CanvasViewVisibility();
        }

        private void CanvasViewVisibility()
        {
            corousalView.IsVisible = DefectSelectionPage.urlBitmaps.Count > 2;

            canvas1.IsVisible = DefectSelectionPage.urlBitmaps.Count > 0 && DefectSelectionPage.urlBitmaps[0].Value != null;
            canvas2.IsVisible = DefectSelectionPage.urlBitmaps.Count > 1 && DefectSelectionPage.urlBitmaps[1].Value != null;
            canvas3.IsVisible = DefectSelectionPage.urlBitmaps.Count > 2 && DefectSelectionPage.urlBitmaps[2].Value != null;
            canvas4.IsVisible = DefectSelectionPage.urlBitmaps.Count > 3 && DefectSelectionPage.urlBitmaps[3].Value != null;

            if (DefectSelectionPage.urlBitmaps.Count > 0 && DefectSelectionPage.urlBitmaps[0].Value != null)
                canvas1.InvalidateSurface();
            if (DefectSelectionPage.urlBitmaps.Count > 1 && DefectSelectionPage.urlBitmaps[1].Value != null)
                canvas2.InvalidateSurface();
            if (DefectSelectionPage.urlBitmaps.Count > 2 && DefectSelectionPage.urlBitmaps[2].Value != null)
                canvas3.InvalidateSurface();
            if (DefectSelectionPage.urlBitmaps.Count > 3 && DefectSelectionPage.urlBitmaps[3].Value != null)
                canvas4.InvalidateSurface();

           canvas3.IsVisible = false;
            canvas4.IsVisible = false;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Supresses highlighting of ListItems
            if (e.SelectedItem == null) return;

            ListView list = sender as ListView;
            if (list.SelectedItem != null)
            {
                bool isSelectionChanged = context.ModifyCellItem(list.SelectedItem);

                // Refresh canvas mode
                if (isSelectionChanged)
                    CanvasViewVisibility();
            }
            list.SelectedItem = null;
        }

        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // Set mark icon
                    if (DefectSelectionPage.touchPoints.Count > 0 && DefectSelectionPage.touchPoints.ContainsKey(context.SelectedClothType))
                        DefectSelectionPage.touchPoints.Single(t => t.Key.Equals(context.SelectedClothType, StringComparison.InvariantCultureIgnoreCase)).Value.Add(new KeyValuePair<SKPoint, int>(e.Location, int.Parse(((SKCanvasView)sender).ClassId)));
                    else
                        DefectSelectionPage.touchPoints.Add(context.SelectedClothType, new List<KeyValuePair<SKPoint, int>> { new KeyValuePair<SKPoint, int>(e.Location, int.Parse(((SKCanvasView)sender).ClassId)) });

                    // update the UI
                    ((SKCanvasView)sender).InvalidateSurface();
                    break;
            }
            // we have handled these events
            e.Handled = true;
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var metrics = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            int classId = int.Parse(((SKCanvasView)sender).ClassId);
           
            // Original bitmap
            canvas.Clear();
            SKBitmap originalFrontBitmap = DefectSelectionPage.urlBitmaps[classId].Value;
           //if (originalFrontBitmap.Pixels.Count() > 0)
           //      originalFrontBitmap = originalFrontBitmap.Resize(info, SKFilterQuality.High); //Resize to the canvas

            double metricWidth = metrics.Width - (metrics.Width / 100 * 30);

            double resolution = metrics.Height > metricWidth ? metricWidth : metrics.Height;

            double screenResolutionDivideRatio = (int)resolution / 2.0;
            double imageWidth = 0;
            double imageHeight = 0;

            double imageWidthRatio = 0;
            double imageHeightRatio = 0;

            if (classId == 0)
            {
                canvas1.WidthRequest = screenResolutionDivideRatio;
                canvas1.HeightRequest = screenResolutionDivideRatio;
            }
            else if (classId == 1)
            {
                canvas2.WidthRequest = screenResolutionDivideRatio;
                canvas2.HeightRequest = screenResolutionDivideRatio;
            }
            else if (classId == 2)
            {
                canvas3.WidthRequest = screenResolutionDivideRatio;
                canvas3.HeightRequest = screenResolutionDivideRatio;
            }
            else if (classId == 3)
            {
                canvas4.WidthRequest = screenResolutionDivideRatio;
                canvas4.HeightRequest = screenResolutionDivideRatio;
            }
            imageWidthRatio = screenResolutionDivideRatio / originalFrontBitmap.Width;
            imageHeightRatio = screenResolutionDivideRatio / originalFrontBitmap.Height;
            if (screenResolutionDivideRatio < originalFrontBitmap.Width || screenResolutionDivideRatio < originalFrontBitmap.Height)
            {
                imageWidth = (double)originalFrontBitmap.Width * (imageWidthRatio < imageHeightRatio ? imageWidthRatio : imageHeightRatio);
                imageHeight = (double)originalFrontBitmap.Height * (imageWidthRatio < imageHeightRatio ? imageWidthRatio : imageHeightRatio);
            }
            else
            {
                imageWidth = originalFrontBitmap.Width;
                imageHeight = originalFrontBitmap.Height;
            }
            
            double leftRatio = screenResolutionDivideRatio - imageWidth;
            double topRatio = screenResolutionDivideRatio - imageHeight;

            double leftmargin = leftRatio / 2;
            double topmargin = topRatio / 2;

            canvas.DrawBitmap(originalFrontBitmap, new SKRect((int)leftmargin, (int) topmargin, (int) (imageWidth+ leftmargin),
                    (int)(imageHeight + topmargin)));

            //canvas.DrawBitmap(originalFrontBitmap, info.Rect);



            SKBitmap markBitmap = GetMarkIcon(PostQCInput.QCStatus);

            // Draw mark bitmap
            if (DefectSelectionPage.touchPoints.Count > 0)
            {
                try
                {
                    var touches = DefectSelectionPage.touchPoints.Single(t => t.Key.Equals(context.SelectedClothType, StringComparison.InvariantCultureIgnoreCase));
                    if (touches.Value != null && touches.Value.Count > 0)
                    {
                        foreach (var touchPoint in touches.Value.Where(t => t.Value == classId))
                        {
                            float x = touchPoint.Key.X;
                            float y = touchPoint.Key.Y;
                            canvas.DrawBitmap(markBitmap, new SKRect(x - markBitmap.Width / 2, y, x + markBitmap.Width / 2, y + markBitmap.Height));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            // Plot existing marks
            markBitmap = GetMarkIcon(2);
            // Draw mark bitmap
            if (lstDefectPoints != null && lstDefectPoints.Count > 0 && lstDefectPoints.Any(p => p.Value == classId))
            {
                try
                {
                    var touches = lstDefectPoints.Where(d => d.Value == classId).Select(d => d.Key);
                    if (touches != null && touches.Count() > 0)
                    {
                        foreach (var touchPoint in touches)
                        {
                            float x = touchPoint.X;
                            float y = touchPoint.Y;
                            canvas.DrawBitmap(markBitmap, new SKRect(x - markBitmap.Width / 2, y, x + markBitmap.Width / 2, y + markBitmap.Height));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            // Plot existing marks
            markBitmap = GetMarkIcon(3);
            // Draw mark bitmap
            if (lstRejectPoints != null && lstRejectPoints.Count > 0 && lstRejectPoints.Any(p => p.Value == classId))
            {
                try
                {
                    var touches = lstRejectPoints.Where(d => d.Value == classId).Select(d => d.Key);
                    if (touches != null && touches.Count() > 0)
                    {
                        foreach (var touchPoint in touches)
                        {
                            float x = touchPoint.X;
                            float y = touchPoint.Y;
                            canvas.DrawBitmap(markBitmap, new SKRect(x - markBitmap.Width / 2, y, x + markBitmap.Width / 2, y + markBitmap.Height));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            {
                // save the data to a byte[]
                if (imageData.ContainsKey($"{context.SelectedClothType}_{classId}##{DefectSelectionPage.urlBitmaps[classId].Key}"))
                    imageData.Remove($"{context.SelectedClothType}_{classId}##{DefectSelectionPage.urlBitmaps[classId].Key}");
                imageData.Add($"{context.SelectedClothType}_{classId}##{DefectSelectionPage.urlBitmaps[classId].Key}", data.ToArray());
            }
        }

        private SKBitmap GetMarkIcon(int qcStatus)
        {
            SKBitmap markBitmap;

            // Create mark bitmap for pixelized image
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            if (qcStatus == 2)
            {
                using (Stream stream = assembly.GetManifestResourceStream("XF.BASE.ic_defect_mark.png"))
                {
                    markBitmap = SKBitmap.Decode(stream);
                }
            }
            else
            {
                using (Stream stream = assembly.GetManifestResourceStream("XF.BASE.ic_reject_mark.png"))
                {
                    markBitmap = SKBitmap.Decode(stream);
                }
            }

            return markBitmap;
        }

        private async void SubmitBtn_Clicked(object sender, EventArgs e)
        {
            context.ShowLoader();
            BtnCheck.IsEnabled = false;
            ReplotPoints();
            await Task.Delay(1500);
            lstDefectPoints = null;
            lstRejectPoints = null;

            context.SubmitDefect(imageData);
            context.HideLoader();
        }

        private void CarouselViewBtn_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            context = DependencyService.Get<IDefectCardViewModel>();

            switch (btn.ClassId)
            {
                case "0":
                    if (DefectSelectionPage.urlBitmaps.Count > 0 && DefectSelectionPage.urlBitmaps[0].Value != null)
                        canvas1.IsVisible = true;
                    if (DefectSelectionPage.urlBitmaps.Count > 1 && DefectSelectionPage.urlBitmaps[1].Value != null)
                        canvas2.IsVisible = true;
                    if (DefectSelectionPage.urlBitmaps.Count > 2 && DefectSelectionPage.urlBitmaps[2].Value != null)
                        canvas3.IsVisible = false;
                    if (DefectSelectionPage.urlBitmaps.Count > 3 && DefectSelectionPage.urlBitmaps[3].Value != null)
                        canvas4.IsVisible = false;
                    CarouselViewBtn1.Background = new SolidColorBrush(context.PostQCInput.QCStatus == 2 ? Color.FromHex("#FFB300") : Color.FromHex("#D24D57"));
                    CarouselViewBtn2.Background = new SolidColorBrush(Color.FromHex("#EDEDED"));
                    break;
                case "1":
                    if (DefectSelectionPage.urlBitmaps.Count > 0 && DefectSelectionPage.urlBitmaps[0].Value != null)
                        canvas1.IsVisible = false;
                    if (DefectSelectionPage.urlBitmaps.Count > 1 && DefectSelectionPage.urlBitmaps[1].Value != null)
                        canvas2.IsVisible = false;
                    if (DefectSelectionPage.urlBitmaps.Count > 2 && DefectSelectionPage.urlBitmaps[2].Value != null)
                        canvas3.IsVisible = true;
                    if (DefectSelectionPage.urlBitmaps.Count > 3 && DefectSelectionPage.urlBitmaps[3].Value != null)
                        canvas4.IsVisible = true;
                    CarouselViewBtn1.Background = new SolidColorBrush(Color.FromHex("#EDEDED"));
                    CarouselViewBtn2.Background = new SolidColorBrush(context.PostQCInput.QCStatus == 2 ? Color.FromHex("#FFB300") : Color.FromHex("#D24D57"));
                    break;
            }
        }

        private void ReplotPoints()
        {
            string markPoints = context.GetMarkPoints();

            if (string.IsNullOrEmpty(markPoints)) return;

            lstDefectPoints = new List<KeyValuePair<SKPoint, int>>();
            lstRejectPoints = new List<KeyValuePair<SKPoint, int>>();

            // {QC_STATUS##IMAGE_ID##'X'##'Y'}\\{QC_STATUS##IMAGE_ID##'X'##'Y'}
            string[] markPointArray = markPoints.Split(new string[] { "$$" }, StringSplitOptions.None);
            foreach (var pointData in markPointArray)
            {
                string[] markPointArray1 = pointData.Split(new string[] { "\\" }, StringSplitOptions.None);
                foreach (var pointData1 in markPointArray1)
                {
                    // QC_STATUS##IMAGE_ID##'X'##'Y'
                    string[] pointArray = pointData1.Split(new string[] { "##" }, StringSplitOptions.None);
                    if (int.Parse(pointArray[0]) == 2)
                        lstDefectPoints.Add(new KeyValuePair<SKPoint, int>(new SKPoint(float.Parse(pointArray[2]), float.Parse(pointArray[3])), int.Parse(pointArray[1])));
                    else
                        lstRejectPoints.Add(new KeyValuePair<SKPoint, int>(new SKPoint(float.Parse(pointArray[2]), float.Parse(pointArray[3])), int.Parse(pointArray[1])));
                }
            
            }

            if (lstDefectPoints.Count > 0 || lstRejectPoints.Count > 0)
                CanvasViewVisibility();
        }
    }
}
