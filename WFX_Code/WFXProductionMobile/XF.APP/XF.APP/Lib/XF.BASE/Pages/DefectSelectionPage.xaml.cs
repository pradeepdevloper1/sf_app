using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.BASE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefectSelectionPage : BasePage
    {
        public IDefectSelectionViewModel context { get; set; }
        private PostQCInputDto PostQCInput { get; set; }

        public static Dictionary<string, List<KeyValuePair<SKPoint, int>>> touchPoints;
        public static List<KeyValuePair<string,SKBitmap>> urlBitmaps;

        private List<PoImageDto> poImages;

        public DefectSelectionPage(PostQCInputDto postQCInput)
        {
            InitializeComponent();

            touchPoints = new Dictionary<string, List<KeyValuePair<SKPoint, int>>>(StringComparer.InvariantCultureIgnoreCase);

            PostQCInput = postQCInput;
            PostQCInput.tbl_QCDefectDetails = new List<TblQCDefectDetail>();
            Separator.BackgroundColor = postQCInput.QCStatus == 2 ? (Color)Application.Current.Resources["RoundedBtnDefectBgColor"] : Color.White;
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();
            SetTheme(PostQCInput.QCStatus == 2 ? 3 : 2);

            if (BindingContext == null)
            {
                context = DependencyService.Get<IDefectSelectionViewModel>();
                context.PostQCInput = PostQCInput;
                BindingContext = context;
            }
            context.OnScreenAppearing();

            context.ShowLoader();

            poImages = await context.FetchListData();
            if (poImages != null)
            {
                poImages = poImages.ToList();
                try
                {
                    productImg1.Source = productImg2.Source = "";
                    productImg1.Source = poImages[0].imagePath;
                    productImg2.Source = poImages[1].imagePath;
                }
                catch (Exception)
                {
                }
                urlBitmaps = new List<KeyValuePair<string, SKBitmap>>();

                foreach (var image in poImages)
                {
                    string imgName = Path.GetFileName(image.imagePath);

                    urlBitmaps.Add(new KeyValuePair<string, SKBitmap>(imgName??"", LoadImage(image.imagePath)));
                }
                CanvasViewVisibility();
            }
            context.HideLoader();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Supresses highlighting of ListItems
            if (e.SelectedItem == null) return;

            ListView list = sender as ListView;

            context.SelectedListCellId = int.Parse(list.ClassId ?? "0");
            context.SelectedItem = e.SelectedItem;

            list.SelectedItem = null;
        }

        private void CarouselViewBtn_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.ClassId)
            {
                case "2":
                    if (urlBitmaps.Count > 0 && urlBitmaps[0].Value != null)
                        canvasCopy1.IsVisible = true;
                    if (urlBitmaps.Count > 1 && urlBitmaps[1].Value != null)
                        canvasCopy2.IsVisible = true;
                    if (urlBitmaps.Count > 2 && urlBitmaps[2].Value != null)
                        canvasCopy3.IsVisible = false;
                    if (urlBitmaps.Count > 3 && urlBitmaps[3].Value != null)
                        canvasCopy4.IsVisible = false;
                    CarouselViewBtn1.Background = new SolidColorBrush(context.PostQCInput.QCStatus == 2 ? Color.FromHex("#FFB300") : Color.FromHex("#D24D57"));
                    CarouselViewBtn2.Background = new SolidColorBrush(Color.FromHex("#EDEDED"));
                    break;
                case "3":
                    if (urlBitmaps.Count > 0 && urlBitmaps[0].Value != null)
                        canvasCopy1.IsVisible = false;
                    if (urlBitmaps.Count > 1 && urlBitmaps[1].Value != null)
                        canvasCopy2.IsVisible = false;
                    if (urlBitmaps.Count > 2 && urlBitmaps[2].Value != null)
                        canvasCopy3.IsVisible = true;
                    if (urlBitmaps.Count > 3 && urlBitmaps[3].Value != null)
                        canvasCopy4.IsVisible = true;
                    CarouselViewBtn1.Background = new SolidColorBrush(Color.FromHex("#EDEDED"));
                    CarouselViewBtn2.Background = new SolidColorBrush(context.PostQCInput.QCStatus == 2 ? Color.FromHex("#FFB300") : Color.FromHex("#D24D57"));
                    break;
            }
        }

        private void CanvasViewVisibility()
        {
            corousalViewCopy.IsVisible = urlBitmaps.Count > 2;
            canvasCopy1.IsVisible = urlBitmaps.Count > 0 && urlBitmaps[0].Value != null;
            canvasCopy2.IsVisible = urlBitmaps.Count > 1 && urlBitmaps[1].Value != null;
            canvasCopy3.IsVisible = urlBitmaps.Count > 2 && urlBitmaps[2].Value != null;
            canvasCopy4.IsVisible = urlBitmaps.Count > 3 && urlBitmaps[2].Value != null;

            if (urlBitmaps.Count > 0 && urlBitmaps[0].Value != null)
            {
                canvasCopy1.InvalidateSurface();
            }
            if (urlBitmaps.Count > 1 && urlBitmaps[1].Value != null)
            {
                canvasCopy2.InvalidateSurface();
            }
            if (urlBitmaps.Count > 2 && urlBitmaps[2].Value != null)
            {
                canvasCopy3.InvalidateSurface();
            }
            if (urlBitmaps.Count > 3 && urlBitmaps[3].Value != null)
            {
                canvasCopy4.InvalidateSurface();
            }
            canvasCopy3.IsVisible = false;
            canvasCopy4.IsVisible = false;
        }

        #region CanvasView

        private SKBitmap LoadImage(string url)
        {
            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.Timeout = 2000; // miliseconds
            SKBitmap resourceBitmap = new SKBitmap();

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // download the bytes
                    byte[] stream = null;
                    using (var webClient = new WebClient())
                    {
                        stream = webClient.DownloadData(url);
                    }

                    // decode the bitmap stream
                    resourceBitmap = SKBitmap.Decode(stream);
                }
                return resourceBitmap;
            }
            catch (Exception)
            {
                return resourceBitmap;
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        private void UpdateCanvas(SKCanvas canvas, SKImageInfo info, string classId)
        {
            var metrics = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
          
            int heightWidth;

            // Original bitmap
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            canvas.Clear();
            heightWidth = GramentView.IsVisible ? 500 : 300;

            SKBitmap urlBitmap = urlBitmaps[int.Parse(classId)].Value;
            double metricWidth = metrics.Width - (metrics.Width / 100 * 30);

            double resolution = metrics.Height > metricWidth ? metricWidth : metrics.Height;

            double screenResolutionDivideRatio = (int)resolution / 2.0;
            double imageWidth = 0;
            double imageHeight = 0;

            double imageWidthRatio = 0;
            double imageHeightRatio = 0;

            if (int.Parse(classId) == 0)
            {
                canvasCopy1.WidthRequest = screenResolutionDivideRatio;
                canvasCopy1.HeightRequest = screenResolutionDivideRatio;
            }
            else if (int.Parse(classId) == 1)
            {
                canvasCopy2.WidthRequest = screenResolutionDivideRatio;
                canvasCopy2.HeightRequest = screenResolutionDivideRatio;
            }
            else if (int.Parse(classId) == 2)
            {
                canvasCopy3.WidthRequest = screenResolutionDivideRatio;
                canvasCopy3.HeightRequest = screenResolutionDivideRatio;
            }
            else if (int.Parse(classId) == 3)
            {
                canvasCopy4.WidthRequest = screenResolutionDivideRatio;
                canvasCopy4.HeightRequest = screenResolutionDivideRatio;
            }

            imageWidthRatio = screenResolutionDivideRatio / urlBitmap.Width;
            imageHeightRatio = screenResolutionDivideRatio / urlBitmap.Height;

            if (screenResolutionDivideRatio < urlBitmap.Width || screenResolutionDivideRatio < urlBitmap.Height)
            {
                imageWidth = (double)urlBitmap.Width * (imageWidthRatio < imageHeightRatio ? imageWidthRatio : imageHeightRatio);
                imageHeight = (double)urlBitmap.Height * (imageWidthRatio < imageHeightRatio ? imageWidthRatio : imageHeightRatio);
            }
            else {
                imageWidth = urlBitmap.Width;
                imageHeight = urlBitmap.Height;
            }
            double leftRatio = screenResolutionDivideRatio - imageWidth;
            double topRatio = screenResolutionDivideRatio - imageHeight;

            double leftmargin = leftRatio / 2;
            double topmargin = topRatio / 2;

            canvas.DrawBitmap(urlBitmap, new SKRect((int) leftmargin, (int)topmargin, (int) (imageWidth + leftmargin), (int)(imageHeight + topmargin)));

            SKBitmap markBitmap;

            // Create mark bitmap for pixelized image
            if (PostQCInput.QCStatus == 2)
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

            // Draw mark bitmap
            if (touchPoints.Count > 0)
            {
                try
                {
                    var touches = touchPoints.Single(t => t.Key.Equals(context.SelectedClothType, StringComparison.InvariantCultureIgnoreCase));
                    if (touches.Value != null && touches.Value.Count > 0)
                    {
                        foreach (var touchPoint in touches.Value.Where(t => t.Value == int.Parse(classId)))
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
        }

        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // Set mark icon
                    if (touchPoints.Count > 0 && touchPoints.ContainsKey(context.SelectedClothType))
                        touchPoints.Single(t => t.Key.Equals(context.SelectedClothType, StringComparison.InvariantCultureIgnoreCase)).Value.Add(new KeyValuePair<SKPoint, int>(e.Location, int.Parse(((SKCanvasView)sender).ClassId)));
                    else
                        touchPoints.Add(context.SelectedClothType, new List<KeyValuePair<SKPoint, int>> { new KeyValuePair<SKPoint, int>(e.Location, int.Parse(((SKCanvasView)sender).ClassId)) });

                    // update the UI
                    ((SKCanvasView)sender).InvalidateSurface();
                    break;
            }
            // we have handled these events
            e.Handled = true;
        }

        public void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            UpdateCanvas(canvas, info, ((SKCanvasView)sender).ClassId);
        }

        public void UndoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var current = touchPoints.Single(t => t.Key.Equals(context.SelectedClothType, StringComparison.InvariantCultureIgnoreCase));

                if (current.Value == null || current.Value.Count == 0)
                    return;

                var touch = current.Value.Last();
                current.Value.RemoveAt(current.Value.Count - 1);

                switch (touch.Value)
                {
                    case 0:
                        if (urlBitmaps.Count > 0 && urlBitmaps[0].Value != null)
                        {
                            canvasCopy1.InvalidateSurface();
                        }
                        break;
                    case 1:
                        if (urlBitmaps.Count > 1 && urlBitmaps[1].Value != null)
                        {
                            canvasCopy2.InvalidateSurface();
                        }
                        break;
                    case 2:
                        if (urlBitmaps.Count > 2 && urlBitmaps[2].Value != null)
                        {
                            canvasCopy3.InvalidateSurface();
                        }
                        break;
                    case 3:
                        if (urlBitmaps.Count > 3 && urlBitmaps[3].Value != null)
                        {
                            canvasCopy4.InvalidateSurface();
                        }
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}