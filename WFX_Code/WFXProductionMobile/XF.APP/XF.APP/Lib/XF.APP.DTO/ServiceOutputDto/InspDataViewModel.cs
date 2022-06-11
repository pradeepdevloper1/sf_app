using System.Collections.Generic;

namespace XF.APP.DTO
{
    public class InspDataViewModel
    {
        public string SelectedPart { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedHexCode { get; set; }
        public string SelectedSize { get; set; }
        public IEnumerable<string> Parts { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public Dictionary<string, string> Colors { get; set; }
        public PurchaseOrder1 SelectedPo { get; set; }
    }
}