using System;
using System.Collections.Generic;
using System.Text;

namespace WFX.Entities
{
    public class tbl_Translation
    {
        public int TranslationID { get; set; }
        public string ObjectName { get; set; }
        public string ObjectKey { get; set; }
        public string TranslatedString { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
