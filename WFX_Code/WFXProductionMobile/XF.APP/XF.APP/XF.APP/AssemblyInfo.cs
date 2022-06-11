using Xamarin.Forms.Xaml;
using System.Resources;
using Xamarin.Forms;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

// The resources from the neutral language .resx file are stored directly
// within the library assembly. For that reason, changing en-US to a different
// language in this line will not by itself change the language shown in the
// app. See the discussion of UltimateResourceFallbackLocation in the
// documentation for additional information:
// https://docs.microsoft.com/dotnet/api/system.resources.neutralresourceslanguageattribute
[assembly: NeutralResourcesLanguage("en-US")]

//font icons
[assembly: ExportFont("grialiconsfill.ttf", Alias = "GrialIconsFill")]
[assembly: ExportFont("grialiconsline.ttf", Alias = "GrialIconsLine")]
[assembly: ExportFont("FontAwesome5Brands-Regular-400.otf", Alias = "FontAwesome5BrandsRegular")]
[assembly: ExportFont("FontAwesome5Free-Regular-400.otf", Alias = "FontAwesome5FreeRegular")]
[assembly: ExportFont("FontAwesome5Free-Solid-900.otf", Alias = "FontAwesome5FreeSolid")]

//fonts
[assembly: ExportFont("OpenSans-Bold.ttf", Alias = "OpenSansBold")] 
[assembly: ExportFont("OpenSans-BoldItalic.ttf", Alias = "OpenSansBoldItalic")] 
[assembly: ExportFont("OpenSans-ExtraBold.ttf", Alias = "OpenSansExtraBold")] 
[assembly: ExportFont("OpenSans-ExtraBoldItalic.ttf", Alias = "OpenSansExtraBoldItalic")] 
[assembly: ExportFont("OpenSans-Italic.ttf", Alias = "OpenSansItalic")] 
[assembly: ExportFont("OpenSans-Light.ttf", Alias = "OpenSansLight")] 
[assembly: ExportFont("OpenSans-LightItalic.ttf", Alias = "OpenSansLightItalic")] 
[assembly: ExportFont("OpenSans-Regular.ttf", Alias = "OpenSansRegular")] 
[assembly: ExportFont("OpenSans-SemiBold.ttf", Alias = "OpenSansSemiBold")]
[assembly: ExportFont("OpenSans-SemiBoldItalic.ttf", Alias = "OpenSansSemiBoldItalic")]
