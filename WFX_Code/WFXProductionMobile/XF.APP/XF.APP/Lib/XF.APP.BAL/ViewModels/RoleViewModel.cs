using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace XF.APP.BAL
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        public string HeaderText { get; set; }
        public string SubText { get; set; }
        public bool IsSelected { get; set; }
        public bool HasShadow { get; set; }
        public double Opacity { get; set; }
        public double BorderWidth { get; set; }
        public Color BoxColor { get; set; }
        public Color BorderColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color HeaderTextColor { get; set; }
        public Color SubTextColor { get; set; }
    }
}
