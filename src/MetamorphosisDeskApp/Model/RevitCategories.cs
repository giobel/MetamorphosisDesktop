using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetamorphosisDeskApp.Model
{
    public class RevitCategories : RevitBase
    {
        public string DBFileName { get; set; }
        public int CategoryCount { get; set; }
        public int VariationOnPrevious { get; set; }
        
    }
}
